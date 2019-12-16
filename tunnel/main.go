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

func marshalCSharpStringPointerToString(str16 *uint16) string {
	return windows.UTF16ToString((*[(1 << 30) - 1]uint16)(unsafe.Pointer(str16))[:])
}

//export WireGuardTunnelService
func WireGuardTunnelService(confFile16 *uint16) bool {
	confFile := marshalCSharpStringPointerToString(confFile16)
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

//export TestOutsideConnectivity
func TestOutsideConnectivity(ip16 *uint16, host16 *uint16, url16 *uint16, expectedTestResult16 *uint16) int32 {
	ip := marshalCSharpStringPointerToString(ip16)
	host := marshalCSharpStringPointerToString(host16)
	url := marshalCSharpStringPointerToString(url16)
	expectedTestResult := marshalCSharpStringPointerToString(expectedTestResult16)

	// Attempt to locate a default route
	luid, nextHop, err := findPhysicalDefaultRoute()
	if err != nil {
		return -1
	}

	destination := net.IPNet{IP: net.ParseIP(ip), Mask: net.IPv4Mask(255, 255, 255, 255)}
	err = luid.AddRoute(destination, nextHop, 0)

	// Check for errors, and check if route already exists
	if err != nil && err != windows.ERROR_OBJECT_ALREADY_EXISTS {
		return -1
	}
	defer luid.DeleteRoute(destination, nextHop)

	// Attempt to setup a new GET request
	req, err := http.NewRequest("GET", fmt.Sprintf(url, ip), nil)
	if err != nil {
		return -1
	}

	// Redirects should be treated as an assumption that a captive portal is available
	redirected := false
	client := &http.Client{
		CheckRedirect: func(r *http.Request, via []*http.Request) error {
			redirected = true
			return errors.New("Redirect detected.")
		},
	}

	// Set the host header and try to retrieve the supplied URL
	req.Host = host
	resp, err := client.Do(req)

	if redirected {
		// We were redirected!
		return 0
	}

	if err != nil {
		return -1
	}

	// Read response from the body
	text, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return -1
	}

	// Compare retrieved body contents to expected contents
	if bytes.Equal(text, []byte(expectedTestResult)) {
		return 1
	} else {
		return 0
	}
}

func main() {}
