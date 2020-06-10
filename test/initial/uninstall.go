package service

import (
	"io/ioutil"
	"os"
	"os/exec"
	"testing"

	"golang.org/x/text/encoding/unicode"
	"golang.org/x/text/transform"
	"gotest.tools/assert"
)

func Uninstall(t *testing.T) {
	// make sure we already installed the app
	existed, err := checkApplicationInstalled()
	if err != nil {
		t.Fatal(err)
	}
	t.Logf("Uninstall - App Installed? %t \n", existed)
	assert.Equal(t, existed, true)

	// try to uninstall the app
	t.Log("Uninstall - Start to uninstall")
	cmd := exec.Command("msiexec", "/log", "uninstall.log", "/qn", "/x", os.Getenv("PRJ_DIR")+"\\installer\\x64\\FirefoxPrivateNetworkVPN.msi")
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	err = cmd.Run()
	log, _ := ioutil.ReadFile("uninstall.log")
	log, _, _ = transform.Bytes(unicode.UTF16(unicode.LittleEndian, unicode.IgnoreBOM).NewDecoder(), log)
	os.Stderr.Write(log)
	if err != nil {
		t.Fatal(err)
	}

	// check the app uninstalled
	existed, err = checkApplicationInstalled()
	if err != nil {
		t.Fatal(err)
	}
	t.Logf("Uninstall - App uninstalled? %t \n", !existed)
	assert.Equal(t, existed, false)

	// check Firefox Guardian folder removed from Program Files
	existed, err = checkTunnelDllExist()
	assert.Equal(t, existed, false)
	// check Wintun Driver uninstalled
	installed, err := checkWintunDriverInstalled()
	if err != nil {
		t.Fatal(err)
	}
	t.Logf("Uninstall - Wintun uninstalled? %t \n", !installed)
	assert.Equal(t, installed, false)
}
