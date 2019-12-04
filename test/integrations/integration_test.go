package main

import (
	"log"
	"net/http"
	"os"
	"testing"
	"time"

	"github.com/mozilla-services/guardian-vpn-windows/test/integrations/apimock/server"
	"github.com/mozilla-services/guardian-vpn-windows/test/integrations/apimock/server/models"
	"github.com/stretchr/testify/assert"
)

var BASEURL = "http://localhost:8000"

func TestMain(m *testing.M) {
	setup()
	code := m.Run()
	teardown()
	os.Exit(code)
}

func setup() {
	go func() {
		log.Printf("Server started")
		router, err := server.NewRouter()
		if err != nil {
			log.Fatal(err)
		}
		log.Fatal(http.ListenAndServe(":8080", router))
	}()

	for {
		if checkWCFServerUp(BASEURL) {
			break
		}
		time.Sleep(2 * time.Second)
	}
}

func teardown() {
	http.Post(BASEURL+"/CloseConnection", "none", nil)
}

func TestVPNConnection(t *testing.T) {
	t.Run("Login", LoginWithActiveSubscription)
	t.Run("Connect", VPNConnection)
	t.Run("Disconnect", VPNDisconnection)
	t.Run("Logout", Logout)
}

func TestDeviceManagement(t *testing.T) {
	t.Run("Login", LoginWithActiveSubscription)
	t.Run("ListDevices", ListDevices)
	t.Run("AddDevice", AddDevice)
	t.Run("RemoveDevice", RemoveDevice)
	t.Run("Logout", Logout)
}

func TestServerList(t *testing.T) {
	t.Run("Login", LoginWithActiveSubscription)
	t.Run("ListServers", ListServers)
	t.Run("Logout", Logout)
}

func TestVersionCheck(t *testing.T) {
	correctCertificateModel := models.BalrogCertificate{
		AuthorityKeyID:                           []byte{1, 3, 6, 1, 5, 5, 7, 3, 3},
		NotBefore:                                time.Now().Add(time.Hour * 24 * 10 * -1),
		Subject:                                  "aus.content-signature.mozilla.org",
		AdditionalIntermediate:                   false,
		AdditionalRoot:                           false,
		AdditionalRootTopOrBot:                   false,
		AdditionalIrrelevantIntermediate:         false,
		AdditionalIrrelevantIntermediateTopOrBot: false,
	}
	t.Run("Mandatory update", func(t *testing.T) {
		UpdateRootFingerprint(t)
		required, _ := VersionCheck("0.5.0.0")
		assert.Equal(t, true, required)
	})
	t.Run("Successfully download msi and update", func(t *testing.T) {
		res := DownloadMSIAndUpdate("0.5.0.0")
		status := res.Get("Status").MustInt()
		message := res.Get("Message").MustString()
		t.Log("DownloadMSIAndUpdate Response status: ", status)
		t.Log("DownloadMSIAndUpdate Response message: ", message)
		assert.Equal(t, 200, status)
		assert.Equal(t, "Success", message)
	})
	t.Run("Wrong expiration date of one of the intermediates", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.NotBefore = time.Now().Add(time.Hour * 24 * 10)
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		_, err := VersionCheck("0.5.1.0")
		assert.Errorf(t, err, "Not Found")
	})
	t.Run("Wrong certificate subject", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.Subject = "whatever"
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		_, err := VersionCheck("0.5.1.0")
		assert.Errorf(t, err, "Not Found")
	})
	t.Run("Missing both p384 and p256 signatures", func(t *testing.T) {
		certificateModel := correctCertificateModel
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		_, err := VersionCheck("0.0.0.0")
		assert.Errorf(t, err, "Not Found")
	})
	t.Run("JSON signature is wrong", func(t *testing.T) {
		certificateModel := correctCertificateModel
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		_, err := VersionCheck("0.0.0.1")
		assert.Errorf(t, err, "Not Found")
	})
	t.Run("MSI file downloaded has wrong hash", func(t *testing.T) {
		res := DownloadMSIAndUpdate("0.0.0.2")
		status := res.Get("Status").MustInt()
		message := res.Get("Message").MustString()
		t.Log("DownloadMSIAndUpdate Response status: ", status)
		t.Log("DownloadMSIAndUpdate Response message: ", message)
		assert.Equal(t, 500, status)
		assert.Equal(t, "Fail", message)
	})
	t.Run("MSI File downloaded is missing hash", func(t *testing.T) {
		res := DownloadMSIAndUpdate("0.0.0.3")
		status := res.Get("Status").MustInt()
		message := res.Get("Message").MustString()
		t.Log("DownloadMSIAndUpdate Response status: ", status)
		t.Log("DownloadMSIAndUpdate Response message: ", message)
		assert.Equal(t, 500, status)
		assert.Equal(t, "Fail", message)
	})
	t.Run("JSON cannot be parsed", func(t *testing.T) {
		_, err := VersionCheck("0.0.0.4")
		assert.Errorf(t, err, "Not Found")
	})
	t.Run("Root, intermediate, intermediate, leaf - configuration in chain file", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.AdditionalIntermediate = true
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		required, _ := VersionCheck("0.5.0.0")
		assert.Equal(t, true, required)
	})
	t.Run("Irrelevant root, root, intermediate, intermediate, leaf - configuration in chain file", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.AdditionalIntermediate = true
		certificateModel.AdditionalRoot = true
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		required, _ := VersionCheck("0.5.0.0")
		assert.Equal(t, false, required)
	})
	t.Run("Root, irrelevant root, intermediate, intermediate, leaf - configuration in chain file", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.AdditionalIntermediate = true
		certificateModel.AdditionalRoot = true
		certificateModel.AdditionalRootTopOrBot = true
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		required, _ := VersionCheck("0.5.0.0")
		assert.Equal(t, false, required)
	})
	t.Run("Root, irrelevant intermediate, intermediate, intermediate, leaf - configuration in chain file", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.AdditionalIntermediate = true
		certificateModel.AdditionalIrrelevantIntermediate = true
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		required, _ := VersionCheck("0.5.0.0")
		assert.Equal(t, false, required)
	})
	t.Run("Root, intermediate, irrelevant intermediate, intermediate, leaf - configuration in chain file", func(t *testing.T) {
		certificateModel := correctCertificateModel
		certificateModel.AdditionalIntermediate = true
		certificateModel.AdditionalIrrelevantIntermediate = true
		certificateModel.AdditionalIrrelevantIntermediateTopOrBot = true
		RegenerateCert(&certificateModel)
		UpdateRootFingerprint(t)
		required, _ := VersionCheck("0.5.0.0")
		assert.Equal(t, false, required)
	})
}

func TestSubscriptionCheck(t *testing.T) {
	t.Run("When user login with active subscription", func(t *testing.T) {
		t.Run("Login With Active Subscirption", LoginWithActiveSubscription)
		t.Run("Logout", Logout)
	})

	t.Run("When user login with inactive subscription", func(t *testing.T) {
		t.Run("Login With Inactive Subscirption", LoginWithInactiveSubscription)
		t.Run("Logout", Logout)
	})

	t.Run("The subscription expires after user login with active subscription", func(t *testing.T) {
		t.Run("Login with Active Subscription", LoginWithActiveSubscription)
		t.Run("Update Subscription Status to Inactive", UpdateSubscriptionStatus)
		t.Run("Subscription Expired", SubscriptionCheck)
	})
}
