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

func Install(t *testing.T) {
	// make sure we didn't install the app
	existed, err := checkApplicationInstalled()
	if err != nil {
		t.Fatal(err)
	}

	t.Logf("Install - App Installed? %t \n", existed)
	assert.Equal(t, existed, false)

	// try to install the app
	t.Log("Install - Start to install")
	cmd := exec.Command("msiexec", "/log", "install.log", "/qn", "/i", os.Getenv("PRJ_DIR")+"\\installer\\x64\\FirefoxPrivateNetworkVPN.msi")
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	err = cmd.Run()
	log, _ := ioutil.ReadFile("install.log")
	log, _, _ = transform.Bytes(unicode.UTF16(unicode.LittleEndian, unicode.IgnoreBOM).NewDecoder(), log)
	os.Stderr.Write(log)
	if err != nil {
		t.Fatal(err)
	}

	// check the app installed
	existed, err = checkApplicationInstalled()
	if err != nil {
		t.Fatal(err)
	}
	t.Logf("Install - App installed? %t \n", existed)
	assert.Equal(t, existed, true)

	// check Wintun Driver installed
	installed, err := checkWintunDriverInstalled()
	if err != nil {
		t.Fatal(err)
	}
	t.Logf("Install - Wintun installed? %t \n", installed)
	assert.Equal(t, installed, true)
}
