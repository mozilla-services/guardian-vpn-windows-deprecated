package balrog

import (
	"bytes"
	"crypto"
	"crypto/ecdsa"
	"crypto/elliptic"
	"crypto/rand"
	"crypto/rsa"
	"crypto/x509"
	"crypto/x509/pkix"
	"encoding/asn1"
	"encoding/hex"
	"encoding/pem"
	"math/big"
	"strings"
	"time"

	"github.com/mozilla-services/guardian-vpn-windows/test/integrations/apimock/server/models"
)

type Chain struct {
	contents                 string
	leafPrivateKey           *ecdsa.PrivateKey
	RootCertificateSignature string
}

func NewChain() (*Chain, error) {
	newChain := new(Chain)
	certificateModel := &models.BalrogCertificate{
		AuthorityKeyID:                           []byte{1, 3, 6, 1, 5, 5, 7, 3, 3},
		NotBefore:                                time.Now().Add(time.Hour * 24 * 10 * -1),
		Subject:                                  "aus.content-signature.mozilla.org",
		AdditionalIntermediate:                   false,
		AdditionalRoot:                           false,
		AdditionalRootTopOrBot:                   false,
		AdditionalIrrelevantIntermediate:         false,
		AdditionalIrrelevantIntermediateTopOrBot: false,
	}
	err := newChain.Regenerate(certificateModel)
	if err != nil {
		return nil, err
	}
	return newChain, nil
}

func (c *Chain) String() string {
	return c.contents
}

func (c *Chain) Regenerate(certificateModel *models.BalrogCertificate) error {
	template := &x509.Certificate{
		Subject: pkix.Name{
			CommonName:         "test-root-ca-production-amo",
			Country:            []string{"Mozilia"},
			Organization:       []string{"Mozilla"},
			OrganizationalUnit: []string{"Mozilla AMO Test Signing Service"},
		},
		NotBefore:             certificateModel.NotBefore,
		NotAfter:              time.Now().Add(time.Hour * 24 * 365),
		KeyUsage:              x509.KeyUsageCertSign | x509.KeyUsageCRLSign,
		ExtKeyUsage:           []x509.ExtKeyUsage{x509.ExtKeyUsageCodeSigning},
		BasicConstraintsValid: true,
		SerialNumber:          big.NewInt(1),
		AuthorityKeyId:        certificateModel.AuthorityKeyID,
		SignatureAlgorithm:    x509.SignatureAlgorithm(x509.SHA384WithRSA),
		IsCA:                  true,
	}
	rootPrivateKey, err := rsa.GenerateKey(rand.Reader, 4096)
	if err != nil {
		return err
	}
	rootCertificateBytes, err := x509.CreateCertificate(rand.Reader, template, template, &rootPrivateKey.PublicKey, rootPrivateKey)
	if err != nil {
		return err
	}
	rootCertificate, err := x509.ParseCertificate(rootCertificateBytes)
	if err != nil {
		return err
	}
	// additional irrelevant root certificate
	var additionalRootCertificateBytes []byte
	if certificateModel.AdditionalRoot {
		additionalRootTemplate := *template
		additionalRootTemplate.Subject.CommonName = "irrelevant-root-template"

		additionalRootPrivateKey, err := rsa.GenerateKey(rand.Reader, 4096)
		if err != nil {
			return err
		}
		additionalRootCertificateBytes, err = x509.CreateCertificate(rand.Reader, &additionalRootTemplate, &additionalRootTemplate, &additionalRootPrivateKey.PublicKey, additionalRootPrivateKey)
		if err != nil {
			return err
		}
	}

	// Intermediate certificate
	template.Subject.CommonName = "Content Signing Intermediate/emailAddress=foxsec@mozilla.com"
	template.PermittedDNSDomains = []string{".content-signature.mozilla.org", "content-signature.mozilla.org"}
	intermediatePrivateKey, err := ecdsa.GenerateKey(elliptic.P384(), rand.Reader)
	if err != nil {
		return err
	}
	intermediateCertificateBytes, err := x509.CreateCertificate(rand.Reader, template, rootCertificate, intermediatePrivateKey.Public(), rootPrivateKey)
	if err != nil {
		return err
	}
	intermediateCertificate, err := x509.ParseCertificate(intermediateCertificateBytes)
	if err != nil {
		return err
	}

	// Optional second intermediate certificate
	var secondIntermediateCertificate *x509.Certificate
	var secondIntermediateCertificateBytes []byte
	var secondIntermediatePrivateKey *ecdsa.PrivateKey
	if certificateModel.AdditionalIntermediate {
		template.SignatureAlgorithm = x509.SignatureAlgorithm(x509.ECDSAWithSHA384)
		secondIntermediatePrivateKey, err = ecdsa.GenerateKey(elliptic.P384(), rand.Reader)
		if err != nil {
			return err
		}
		secondIntermediateCertificateBytes, err = x509.CreateCertificate(rand.Reader, template, intermediateCertificate, secondIntermediatePrivateKey.Public(), intermediatePrivateKey)
		if err != nil {
			return err
		}
		secondIntermediateCertificate, err = x509.ParseCertificate(secondIntermediateCertificateBytes)
		if err != nil {
			return err
		}
	}

	// additional irrelevant intermediate certificate
	var additionalIrrelevantIntermediateCertificateBytes []byte
	if certificateModel.AdditionalIntermediate {
		template.SignatureAlgorithm = x509.SignatureAlgorithm(x509.ECDSAWithSHA384)
		additionalIrrelevantIntermediatePrivateKey, err := ecdsa.GenerateKey(elliptic.P384(), rand.Reader)
		if err != nil {
			return err
		}
		additionalIrrelevantIntermediateCertificateBytes, err = x509.CreateCertificate(rand.Reader, template, template, additionalIrrelevantIntermediatePrivateKey.Public(), additionalIrrelevantIntermediatePrivateKey)
		if err != nil {
			return err
		}
	}

	template = &x509.Certificate{
		Subject: pkix.Name{
			CommonName:         certificateModel.Subject,
			Country:            []string{"Mozilia"},
			Organization:       []string{"Mozilla"},
			OrganizationalUnit: []string{"Cumulonimbus Services"},
			Locality:           []string{"Broadview"},
		},
		NotBefore:          time.Now().Add(time.Hour * 24 * 10 * -1),
		NotAfter:           time.Now().Add(time.Hour * 24 * 365),
		KeyUsage:           x509.KeyUsageDigitalSignature,
		ExtKeyUsage:        []x509.ExtKeyUsage{x509.ExtKeyUsageCodeSigning},
		SerialNumber:       big.NewInt(1988),
		AuthorityKeyId:     certificateModel.AuthorityKeyID,
		SignatureAlgorithm: x509.SignatureAlgorithm(x509.ECDSAWithSHA384),
	}
	leafPrivateKey, err := ecdsa.GenerateKey(elliptic.P384(), rand.Reader)
	if err != nil {
		return err
	}

	var leafCertificateBytes []byte
	if certificateModel.AdditionalIntermediate {
		leafCertificateBytes, err = x509.CreateCertificate(rand.Reader, template, secondIntermediateCertificate, leafPrivateKey.Public(), secondIntermediatePrivateKey)
		if err != nil {
			return err
		}
	} else {
		leafCertificateBytes, err = x509.CreateCertificate(rand.Reader, template, intermediateCertificate, leafPrivateKey.Public(), intermediatePrivateKey)
		if err != nil {
			return err
		}
	}

	buffer := &bytes.Buffer{}

	// Export chain cert
	err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: leafCertificateBytes})
	if err != nil {
		return err
	}
	err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: intermediateCertificateBytes})
	if err != nil {
		return err
	}

	// additional irrelevant root certificate before intermediate certificate
	if certificateModel.AdditionalIrrelevantIntermediate && certificateModel.AdditionalIrrelevantIntermediateTopOrBot {
		err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: additionalIrrelevantIntermediateCertificateBytes})
		if err != nil {
			return err
		}
	}

	// Additional intermediate certificate
	if certificateModel.AdditionalIntermediate {
		err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: secondIntermediateCertificateBytes})
		if err != nil {
			return err
		}
	}

	// additional irrelevant root certificate before intermediate certificate
	if certificateModel.AdditionalIrrelevantIntermediate && !certificateModel.AdditionalIrrelevantIntermediateTopOrBot {
		err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: additionalIrrelevantIntermediateCertificateBytes})
		if err != nil {
			return err
		}
	}

	// additional irrelevant root certificate before root certificate
	if certificateModel.AdditionalRoot && certificateModel.AdditionalRootTopOrBot {
		err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: additionalRootCertificateBytes})
		if err != nil {
			return err
		}
	}

	err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: rootCertificateBytes})
	if err != nil {
		return err
	}

	// additional irrelevant root certificate after root certificate
	if certificateModel.AdditionalRoot && !certificateModel.AdditionalRootTopOrBot {
		err = pem.Encode(buffer, &pem.Block{Type: "CERTIFICATE", Bytes: additionalRootCertificateBytes})
		if err != nil {
			return err
		}
	}

	c.contents = string(buffer.Bytes())
	c.leafPrivateKey = leafPrivateKey
	c.RootCertificateSignature = c.getRootCertSignature(rootCertificateBytes)
	return nil
}

func (*Chain) getRootCertSignature(certificate []byte) string {
	hasher := crypto.SHA256.New()
	hasher.Write(certificate)
	rootCertHash := hasher.Sum(nil)

	ret := ""
	splitter := ""
	for i, rch := range rootCertHash {
		if i > 0 {
			splitter = ":"
		}
		ret += splitter + strings.ToUpper(hex.EncodeToString([]byte{rch}))
	}

	return ret
}

type ecdsaSignature struct {
	R, S *big.Int
}

func (c *Chain) Sign(stuffToSign []byte) ([]byte, error) {
	hasher := crypto.SHA384.New()
	hasher.Write([]byte("Content-Signature:\x00"))
	hasher.Write(stuffToSign)
	bytes, err := c.leafPrivateKey.Sign(rand.Reader, hasher.Sum(nil), crypto.SHA384)
	if err != nil {
		return nil, err
	}
	sig := ecdsaSignature{}
	_, err = asn1.Unmarshal(bytes, &sig)
	if err != nil {
		return nil, err
	}
	bytes = make([]byte, 96)
	copy(bytes[48-len(sig.R.Bytes()):], sig.R.Bytes())
	copy(bytes[96-len(sig.S.Bytes()):], sig.S.Bytes())
	return bytes, nil
}
