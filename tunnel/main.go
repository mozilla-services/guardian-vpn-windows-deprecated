/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

package main

import (
	"bytes"
	"crypto/rand"
	"errors"
	"fmt"
	"io/ioutil"
	"log"
	"net"
	"net/http"
	"path/filepath"
	"unsafe"

	"C"

	"golang.org/x/crypto/curve25519"
	"golang.org/x/sys/windows"

	"golang.zx2c4.com/wireguard/windows/conf"
	"golang.zx2c4.com/wireguard/windows/tunnel"
	"golang.zx2c4.com/wireguard/windows/tunnel/winipcfg"
	"golang.zx2c4.com/wireguard/windows/tunnel/firewall"
)

//export WireGuardTunnelService
func WireGuardTunnelService(confFile16 *uint16) bool {
	confFile := windows.UTF16ToString((*[(1 << 30) - 1]uint16)(unsafe.Pointer(confFile16))[:])
	tunnel.UseFixedGUIDInsteadOfDeterministic = true
	firewall.ExemptBuiltinAdministrators = true
	conf.PresetRootDirectory(filepath.Dir(confFile))
	err := tunnel.Run(confFile)
	if err != nil {
		log.Printf("Tunnel service error: %v", err)
	}
	return err == nil
}

//export WireGuardGenerateKeypair
func WireGuardGenerateKeypair(publicKey *byte, privateKey *byte) {
	publicKeyArray := (*[32]byte)(unsafe.Pointer(publicKey))
	privateKeyArray := (*[32]byte)(unsafe.Pointer(privateKey))
	n, err := rand.Read(privateKeyArray[:])
	if err != nil || n != len(privateKeyArray) {
		panic("Unable to generate random bytes")
	}
	privateKeyArray[0] &= 248
	privateKeyArray[31] = (privateKeyArray[31] & 127) | 64

	curve25519.ScalarBaseMult(publicKeyArray, privateKeyArray)
}

func findPhysicalDefaultRoute() (winipcfg.LUID, net.IP, error) {
	r, err := winipcfg.GetIPForwardTable2(windows.AF_INET)
	if err != nil {
		return 0, nil, err
	}
	lowestMetric := ^uint32(0)
	var nextHop net.IP
	var luid winipcfg.LUID
	for i := range r {
		if r[i].DestinationPrefix.PrefixLength != 0 {
			continue
		}
		ifrow, err := r[i].InterfaceLUID.Interface()
		if err != nil || ifrow.OperStatus != winipcfg.IfOperStatusUp || ifrow.MediaType == winipcfg.NdisMediumIP {
			continue
		}
		if r[i].Metric < lowestMetric {
			lowestMetric = r[i].Metric
			nextHop = r[i].NextHop.IP()
			luid = r[i].InterfaceLUID
		}
	}
	if len(nextHop) == 0 {
		return 0, nil, errors.New("Unable to find default route")
	}
	return luid, nextHop, nil
}

//export TestConnectivity
func TestConnectivity() int32 {
	const (
		exemptIP = "13.107.4.52"
		host     = "www.msftconnecttest.com"
		url      = "http://%s/connecttest.txt"
	)

	luid, nextHop, err := findPhysicalDefaultRoute()
	if err != nil {
		return -1
	}
	destination := net.IPNet{IP: net.ParseIP(exemptIP), Mask: net.IPv4Mask(255, 255, 255, 255)}
	err = luid.AddRoute(destination, nextHop, 0)
	if err != nil {
		return -1
	}
	defer luid.DeleteRoute(destination, nextHop)

	req, err := http.NewRequest("GET", fmt.Sprintf(url, exemptIP), nil)
	if err != nil {
		return -1
	}
	req.Host = host
	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		return -1
	}
	text, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return -1
	}
	if bytes.Equal(text, []byte("Microsoft Connect Test")) {
		return 1
	} else {
		return 0
	}
}

func main() {}
