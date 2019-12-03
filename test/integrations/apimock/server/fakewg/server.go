/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

package fakewg

import (
	"bufio"
	"log"
	"math/rand"
	"net"
	"os"
	"runtime"
	"strings"
	"sync/atomic"

	"golang.org/x/net/icmp"
	"golang.org/x/net/ipv4"
	"golang.org/x/sys/windows"
	"golang.zx2c4.com/wireguard/device"
	"golang.zx2c4.com/wireguard/tun"
	"golang.zx2c4.com/wireguard/windows/conf"
	"golang.zx2c4.com/wireguard/windows/tunnel/winipcfg"
)

type dummyTun struct {
	close     chan bool
	events    chan tun.Event
	s         *Server
	pingQueue chan []byte
}

type Server struct {
	closed     int32
	device     *device.Device
	nextIp     int
	listenPort uint16
	pubkey     string
	gatewayA   uint8
	gatewayB   uint8
	log        *log.Logger
}

func newDummyTun(s *Server) *dummyTun {
	t := &dummyTun{
		s:         s,
		close:     make(chan bool),
		events:    make(chan tun.Event),
		pingQueue: make(chan []byte, 8),
	}
	return t
}

func (t *dummyTun) File() *os.File {
	return nil
}

func (t *dummyTun) Read(buf []byte, offset int) (int, error) {
	select {
	case <-t.close:
		return 0, os.ErrClosed
	case pong := <-t.pingQueue:
		copy(buf[offset:], pong)
		return len(pong), nil
	}
}

func (t *dummyTun) Write(buf []byte, offset int) (int, error) {
	buf = buf[offset:]
	header, err := ipv4.ParseHeader(buf)
	if err != nil {
		return len(buf) - offset, nil
	}
	if header.Version != 4 || len(header.Options) != 0 {
		return len(buf) - offset, nil
	}
	message, err := icmp.ParseMessage(header.Protocol, buf[ipv4.HeaderLen:])
	if err != nil || message.Type != ipv4.ICMPTypeEcho {
		return len(buf) - offset, nil
	}

	message.Type = ipv4.ICMPTypeEchoReply
	src := header.Src
	header.Src = header.Dst
	header.Dst = src

	headerBytes, err := header.Marshal()
	if err != nil {
		return len(buf) - offset, nil
	}
	icmpBytes, err := message.Marshal(nil)
	if err != nil {
		return len(buf) - offset, nil
	}
	t.s.log.Printf("Received ping to %v from %v, sending pong", header.Src, header.Dst)
	t.pingQueue <- append(headerBytes, icmpBytes...)
	return len(buf) - offset, nil
}

func (t *dummyTun) Flush() error {
	return nil
}

func (t *dummyTun) MTU() (int, error) {
	return 1420, nil
}

func (t *dummyTun) Name() (string, error) {
	return "dummy0", nil
}

func (t *dummyTun) Events() chan tun.Event {
	return t.events
}

func (t *dummyTun) Close() error {
	close(t.close)
	return nil
}

func defaultRouteAdapterIpAddress() (*net.IP, error) {
	r, err := winipcfg.GetIPForwardTable2(windows.AF_INET)
	if err != nil {
		return nil, err
	}
	lowestMetric := ^uint32(0)
	luid := winipcfg.LUID(0)
	for i := range r {
		if r[i].DestinationPrefix.PrefixLength != 0 {
			continue
		}
		ifrow, err := r[i].InterfaceLUID.Interface()
		if err != nil || ifrow.OperStatus != winipcfg.IfOperStatusUp {
			continue
		}
		if r[i].Metric < lowestMetric {
			lowestMetric = r[i].Metric
			luid = r[i].InterfaceLUID
		}
	}
	if luid == 0 {
		return nil, windows.ERROR_FILE_NOT_FOUND
	}
	addrs, err := winipcfg.GetUnicastIPAddressTable(windows.AF_INET)
	if err != nil {
		return nil, err
	}
	for _, addr := range addrs {
		if addr.InterfaceLUID == luid {
			ip := addr.Address.IP()
			return &ip, nil
		}
	}
	return nil, windows.ERROR_FILE_NOT_FOUND
}

func NewServer() (*Server, error) {
	key, err := conf.NewPrivateKey()
	if err != nil {
		return nil, err
	}
	s := &Server{
		listenPort: uint16((rand.Uint32() % 128) + 51820),
		gatewayA:   uint8(rand.Uint32() % 256),
		gatewayB:   uint8(rand.Uint32() % 256),
		pubkey:     key.Public().String(),
	}
	uapi, err := (&conf.Config{
		Interface: conf.Interface{
			PrivateKey: *key,
			ListenPort: s.listenPort,
		},
	}).ToUAPI()
	if err != nil {
		return nil, err
	}

	runtime.SetFinalizer(s, func(f *Server) { f.Close() })

	s.log = log.New(os.Stderr, "[FakeWG] ", 0)
	s.device = device.NewDevice(newDummyTun(s), &device.Logger{s.log, s.log, s.log})
	ipcErr := s.device.IpcSetOperation(bufio.NewReader(strings.NewReader(uapi)))
	if ipcErr != nil {
		return nil, ipcErr
	}
	s.device.Up()

	return s, nil
}

func (s *Server) AddClient(publicKeyBase64 string) (allocatedIP string, err error) {
	key, err := conf.NewPrivateKeyFromString(publicKeyBase64)
	if err != nil {
		return
	}
	ip := net.IPv4(10, s.gatewayA, s.gatewayB, byte(s.nextIp%252)+2)
	s.nextIp++

	uapi, err := (&conf.Config{
		Peers: []conf.Peer{{
			PublicKey:  *key,
			AllowedIPs: []conf.IPCidr{{IP: ip, Cidr: 32}}},
		},
	}).ToUAPI()
	if err != nil {
		return
	}
	uapi = strings.ReplaceAll(uapi, "replace_peers=true\n", "")
	uapi = uapi[strings.IndexByte(uapi, '\n')+1:]
	ipcErr := s.device.IpcSetOperation(bufio.NewReader(strings.NewReader(uapi)))
	if ipcErr != nil {
		return "", ipcErr
	}
	return ip.String(), nil
}

func (s *Server) Close() {
	if s == nil || !atomic.CompareAndSwapInt32(&s.closed, 0, 1) {
		return
	}
	if s.device != nil {
		s.Close()
	}
}

func (s *Server) Endpoint() (string, uint16, error) {
	ourIp, err := defaultRouteAdapterIpAddress()
	if err != nil {
		return "", 0, err
	}
	return ourIp.String(), s.listenPort, nil
}

func (s *Server) PublicKey() string {
	return s.pubkey
}

func (s *Server) Gateway() string {
	return net.IPv4(10, s.gatewayA, s.gatewayB, 1).String()
}
