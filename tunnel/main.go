/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

package main

import (
	"crypto/rand"
	"log"
	"path/filepath"
	"unsafe"

	"C"

	"golang.org/x/crypto/curve25519"
	"golang.org/x/sys/windows"

	"golang.zx2c4.com/wireguard/windows/conf"
	"golang.zx2c4.com/wireguard/windows/tunnel"
)

//export WireGuardTunnelService
func WireGuardTunnelService(confFile16 *uint16) bool {
	confFile := windows.UTF16ToString((*[(1 << 30) - 1]uint16)(unsafe.Pointer(confFile16))[:])
	tunnel.UseFixedGUIDInsteadOfDeterministic = true
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

func main() {}
