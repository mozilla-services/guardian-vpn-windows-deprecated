package service

import (
	"log"
	"os"
	"os/exec"
	"strings"

	"golang.org/x/sys/windows"
	"golang.org/x/sys/windows/registry"
)

func checkApplicationInstalled() (bool, error) {
	applicationName := "Mozilla VPN"
	k, err := registry.OpenKey(registry.LOCAL_MACHINE, `SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall`, registry.QUERY_VALUE|registry.ENUMERATE_SUB_KEYS)
	if err != nil {
		log.Fatal(err)
	}
	defer k.Close()
	subKeyNames, err := k.ReadSubKeyNames(-1)
	if err != nil {
		log.Fatal(err)
	}
	for _, subKeyName := range subKeyNames {
		subkey, err := registry.OpenKey(registry.LOCAL_MACHINE, `SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\`+subKeyName, registry.QUERY_VALUE)
		if err != nil {
			log.Fatal(err)
		}
		defer subkey.Close()
		displayName, _, err := subkey.GetStringValue("DisplayName")
		if err != nil {
			continue
		}
		if displayName == applicationName {
			return true, nil
		}
	}
	return false, nil
}

func checkWintunDriverInstalled() (bool, error) {
	cmd := exec.Command("pnputil.exe", "/enum-drivers")
	out, _ := cmd.Output()
	return strings.Contains(string(out), "wintun.inf"), nil
}

func checkTunnelDllExist() (bool, error) {
	path, err := windows.KnownFolderPath(windows.FOLDERID_ProgramFiles, 0)
	if err != nil {
		return false, err
	}
	_, err = os.Stat(path + "\\Mozilla\\Mozilla VPN\\tunnel.dll")
	return !os.IsNotExist(err), nil
}
