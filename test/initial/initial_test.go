package service

import (
	"testing"

	"gotest.tools/assert"
)

func TestUninstallAndInstall(t *testing.T) {
	t.Run("Uninstall", Uninstall)
	t.Run("Install", Install)
}
func TestFirefoxPrivateNetworkVPNInstalled(t *testing.T) {
	existed, err := checkApplicationInstalled()
	if err != nil {
		t.Fatal(err)
	}
	assert.Equal(t, existed, true)
}

func TestGuardianTunnelDllExist(t *testing.T) {
	existed, err := checkTunnelDllExist()
	if err != nil {
		t.Fatal(err)
	}
	assert.Equal(t, existed, true)
}

func TestWintunDriverInstalled(t *testing.T) {
	installed, err := checkWintunDriverInstalled()
	if err != nil {
		t.Fatal(err)
	}
	assert.Equal(t, installed, true)
}
