package main

import (
	"encoding/json"
	"errors"
	"fmt"
	"io/ioutil"
	"log"
	"os/exec"
	"strings"
	"testing"
	"time"

	"github.com/bitly/go-simplejson"
	"github.com/mozilla-services/guardian-vpn-windows/test/integrations/apimock/server/models"
	"github.com/stretchr/testify/assert"
)

func VersionCheck(currentVersion string) (bool, error) {
	body := simplejson.New()
	body.Set("CurrentVersion", currentVersion)

	command := BASEURL + "/VersionCheck"
	marshaled, err := body.MarshalJSON()
	if err != nil {
		log.Fatalln(err)
	}

	res := postJson(command, marshaled)
	if res == nil {
		return false, errors.New("Not Found")
	}
	required := res.Get("Required").MustBool()

	log.Print("VersionCheck Response - required: ", required)
	return required, nil
}

func UpdateRootFingerprint(t *testing.T) {
	// get latest root figure print
	rootFingerprint := getTextContent("http://localhost:8080/rootsig")
	body := simplejson.New()
	body.Set("RootFingerprint", rootFingerprint)

	command := BASEURL + "/UpdateRootFingerprint"
	marshaled, err := body.MarshalJSON()
	if err != nil {
		t.Fatal(err)
	}

	res := postJson(command, marshaled)
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("UpdateRootFingerprint Response status: ", status)
	t.Log("UpdateRootFingerprint Response message: ", message)
	assert.Equal(t, 200, status)
}

func SubscriptionCheck(t *testing.T) {
	count := 5
	command := BASEURL + "/SubscriptionCheck"
	res := postJson(command, []byte{})
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("SubscriptionCheck Response status: ", status)
	t.Log("SubscriptionCheck Response message: ", message)
	for {
		command := BASEURL + "/LoginState"
		res := getJson(command)
		status = res.Get("Status").MustInt()
		message = res.Get("Message").MustString()
		if message == "LoggedOut" || count < 0 {
			break
		}
		time.Sleep(1 * time.Second)
		count--
	}
	t.Log("Subscription Expired Response status: ", status)
	t.Log("Subscription Expired Response message: ", message)
	assert.Equal(t, "LoggedOut", message)
}

func ListServers(t *testing.T) {
	command := BASEURL + "/ServerCityList"
	res := getJson(command)
	servers := res.MustArray()
	assert.Equal(t, true, len(servers) >= 0)
}

func VPNConnection(t *testing.T) {
	command := BASEURL + "/Connect"
	res := postJson(command, []byte{})
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	stackTrace := res.Get("StackTrace").MustString()

	t.Log("VPN Connection Response status: ", status)
	t.Log("VPN Connection Response message: ", message)
	t.Log("VPN Connection Response stacktrace: ", stackTrace)

	assert.Equal(t, 200, status)

	// Verify the connection status
	VerifyConnectionStatus(t, "Protected")
	VerifyProcesses(t)
	time.Sleep(2 * time.Second)
	SimplePing(t)
}

func VPNDisconnection(t *testing.T) {
	command := BASEURL + "/Disconnect"
	res := postJson(command, []byte{})
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("VPN Connection Response status: ", status)
	t.Log("VPN Connection Response message: ", message)
	assert.Equal(t, 200, status)

	// Verify the connection status
	VerifyConnectionStatus(t, "Unprotected")
}

func VerifyConnectionStatus(t *testing.T, expectedStatus string) {
	count := 10
	var status = 0
	var message = ""
	for {
		command := BASEURL + "/ConnectionStatus"
		res := getJson(command)
		status = res.Get("Status").MustInt()
		message = res.Get("Message").MustString()
		if message == expectedStatus || count < 0 {
			break
		}
		time.Sleep(3 * time.Second)
		count--
	}
	t.Log("VPN ConnectionStatus Response status: ", status)
	t.Log("VPN ConnectionStatus Response message: ", message)
	assert.Equal(t, 200, status)
	assert.Equal(t, expectedStatus, message)
}

func ListDevices(t *testing.T) {
	command := BASEURL + "/DeviceList"
	res := getJson(command)
	devices := res.MustArray()
	assert.Equal(t, true, len(devices) >= 1)
}

func AddDevice(t *testing.T) {
	noOfDevicesBeforeAdd := getNumberOfCurrentDevices()
	assert.Equal(t, true, noOfDevicesBeforeAdd >= 0)

	body := simplejson.New()
	body.Set("deviceName", "TestAddDevice")
	body.Set("publicKey", "TestAddDevice")
	command := BASEURL + "/AddDevice"
	marshaled, err := body.MarshalJSON()
	if err != nil {
		t.Fatal(err)
	}
	res := postJson(command, marshaled)
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("Add Device Response status: ", status)
	t.Log("Add Device Response message: ", message)
	assert.Equal(t, 200, status)
	assert.Equal(t, "Success", message)

	noOfDevicesAfterAdd := getNumberOfCurrentDevices()
	assert.Equal(t, true, noOfDevicesAfterAdd >= 0)

	assert.Equal(t, noOfDevicesBeforeAdd+1, noOfDevicesAfterAdd)
}

func RemoveDevice(t *testing.T) {
	noOfDevicesBeforeRemove := getNumberOfCurrentDevices()
	assert.Equal(t, true, noOfDevicesBeforeRemove >= 0)

	url := BASEURL + "/RemoveDevice"

	payload := strings.NewReader("{\"publicKey\": \"TestAddDevice\"}")

	res := deleteJson(url, payload)

	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("Remove Device Response status: ", status)
	t.Log("Remove Device Response message: ", message)
	assert.Equal(t, 200, status)
	assert.Equal(t, "Success", message)

	noOfDevicesAfterRemove := getNumberOfCurrentDevices()
	assert.Equal(t, true, noOfDevicesAfterRemove >= 0)

	assert.Equal(t, noOfDevicesBeforeRemove-1, noOfDevicesAfterRemove)
}

func getNumberOfCurrentDevices() int {
	command := BASEURL + "/DeviceList"
	res := getJson(command)
	devices := res.MustArray()
	noOfDevicesAfterAdd := len(devices)
	return noOfDevicesAfterAdd
}

func getActiveSubscriptionPayload() string {
	expiresOn := time.Now().Add(time.Hour).UnixNano() / int64(time.Millisecond/time.Nanosecond)
	return fmt.Sprintf(
		`{
			"login_url": "http://localhost:8080/",
			"verification_url": "http://localhost:8080/v1/vpn/login/verify/active",
			"expires_on": "/Date(%d)/",
			"poll_interval": 20
		}`, expiresOn)
}

func getInactiveSubscriptionPayload() string {
	expiresOn := time.Now().Add(time.Hour).UnixNano() / int64(time.Millisecond/time.Nanosecond)
	return fmt.Sprintf(
		`{
			"login_url": "http://localhost:8080/",
			"verification_url": "http://localhost:8080/v1/vpn/login/verify/inactive",
			"expires_on": "/Date(%d)/",
			"poll_interval": 20
		}`, expiresOn)
}

func LoginWithActiveSubscription(t *testing.T) {
	Login(t, getActiveSubscriptionPayload())
	// Verify the loggedin state
	VerifyLoggedInState(t, "LoggedIn")
}

func LoginWithInactiveSubscription(t *testing.T) {
	Login(t, getInactiveSubscriptionPayload())
	// Verify the loggedin state
	VerifyLoggedInState(t, "LoggedOut")
}

func Login(t *testing.T, body string) {
	payload := strings.NewReader(body)
	command := BASEURL + "/Login"
	marshaled, err := ioutil.ReadAll(payload)
	if err != nil {
		t.Fatal(err)
	}

	res := postJson(command, marshaled)
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("Login Response status: ", status)
	t.Log("Login Response message: ", message)
	assert.Equal(t, 200, status)
	assert.Equal(t, "Success", message)
}

func Logout(t *testing.T) {
	command := BASEURL + "/Logout"
	res := postJson(command, []byte{})
	status := res.Get("Status").MustInt()
	message := res.Get("Message").MustString()
	t.Log("Logout Response status: ", status)
	t.Log("Logout Response message: ", message)
	assert.Equal(t, 200, status)
	assert.Equal(t, "Success", message)
	time.Sleep(2 * time.Second)
	// Verify the loggedin state
	VerifyLoggedInState(t, "LoggedOut")
}

func VerifyLoggedInState(t *testing.T, expectedState string) {
	count := 5
	var status = 0
	var message = ""
	for {
		command := BASEURL + "/LoginState"
		res := getJson(command)
		status = res.Get("Status").MustInt()
		message = res.Get("Message").MustString()
		if message == expectedState || count < 0 {
			break
		}
		time.Sleep(5 * time.Second)
		count--
	}
	t.Log("VPN LoggedInState Response status: ", status)
	t.Log("VPN LoggedInState Response message: ", message)
	assert.Equal(t, 200, status)
	assert.Equal(t, expectedState, message)
}

func VerifyProcesses(t *testing.T) {
	command := BASEURL + "/FirefoxPrivateNetworkProcessCheck"
	res := postJson(command, []byte{})
	brokerProcessStatus := res.Get("BrokerProcess").MustBool()
	uiProcessStatus := res.Get("UIProcess").MustBool()
	tunnelProcessStatus := res.Get("TunnelProcess").MustBool()

	t.Log("GuardianProcessCheck Response - BrokerProcessStatus: ", brokerProcessStatus)
	t.Log("GuardianProcessCheck Response - TunnelProcessStatus: ", tunnelProcessStatus)
	t.Log("GuardianProcessCheck Response - UIProcessStatus: ", uiProcessStatus)

	assert.Equal(t, true, brokerProcessStatus)
	assert.Equal(t, true, tunnelProcessStatus)
	assert.Equal(t, true, uiProcessStatus)
}

func SimplePing(t *testing.T) {
	out, _ := exec.Command("ping", "1.2.3.4").Output()
	output := string(out)
	t.Log(output)
	if !strings.Contains(output, "Packets: Sent = 4, Received = 4") {
		t.Fatal(output)
	}
}

func RegenerateCert(certificate *models.BalrogCertificate) {
	body, err := json.Marshal(certificate)
	command := "http://localhost:8080/regenerate"
	if err != nil {
		log.Fatal(err)
	}

	postJson(command, body)
}

func UpdateSubscriptionStatus(t *testing.T) {
	body := `{
		"active": false
	}`
	payload := strings.NewReader(body)
	command := "http://localhost:8080/update/subscription"
	marshaled, err := ioutil.ReadAll(payload)
	if err != nil {
		t.Fatal(err)
	}

	res := postJson(command, marshaled)
	active := res.Get("active").MustBool()

	t.Log("UpdateSubscriptionStatus Response active: ", active)
	assert.Equal(t, false, active)
}
func DownloadMSIAndUpdate(currentVersion string) *simplejson.Json {
	body := simplejson.New()
	body.Set("CurrentVersion", currentVersion)

	command := BASEURL + "/DownloadMSIAndUpdate"
	marshaled, err := body.MarshalJSON()
	if err != nil {
		log.Fatal(err)
	}

	res := postJson(command, marshaled)
	if res == nil {
		log.Fatal(err)
	}
	return res
}
