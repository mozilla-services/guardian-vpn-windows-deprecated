package main

import (
	"bytes"
	"io"
	"io/ioutil"
	"log"
	"net/http"

	"github.com/bitly/go-simplejson"
)

func getTextContent(url string) string {
	res, err := http.Get(url)
	if err != nil {
		log.Fatal(err)
	}
	defer res.Body.Close()

	resData, err := ioutil.ReadAll(res.Body)
	if err != nil {
		log.Fatal(err)
	}

	resString := string(resData)

	return resString
}

func getJson(url string) *simplejson.Json {
	res, err := http.Get(url)
	if err != nil {
		log.Fatalln(err)
	}

	body, err := ioutil.ReadAll(res.Body)
	if err != nil {
		log.Fatalln(err)
	}

	js, err := simplejson.NewJson(body)
	if err != nil {
		log.Fatalln(err)
	}

	return js
}

func postJson(url string, reqBody []byte) *simplejson.Json {
	res, err := http.Post(url, "application/json", bytes.NewBuffer(reqBody))
	if err != nil {
		log.Fatalln(err)
	}

	body, err := ioutil.ReadAll(res.Body)
	if err != nil {
		log.Fatalln(err)
	}
	if len(body) <= 0 {
		return nil
	}
	js, err := simplejson.NewJson(body)
	if err != nil {
		log.Fatalln(err)
	}

	return js
}

func deleteJson(url string, payload io.Reader) *simplejson.Json {

	req, _ := http.NewRequest("DELETE", url, payload)

	req.Header.Add("Content-Type", "application/json")
	req.Header.Add("Accept", "*/*")
	req.Header.Add("Cache-Control", "no-cache")
	req.Header.Add("Accept-Encoding", "gzip, deflate")
	req.Header.Add("Connection", "keep-alive")
	req.Header.Add("cache-control", "no-cache")

	res, err := http.DefaultClient.Do(req)

	if err != nil {
		log.Fatalln(err)
	}

	defer res.Body.Close()
	body, err := ioutil.ReadAll(res.Body)
	if err != nil {
		log.Fatalln(err)
	}

	js, err := simplejson.NewJson(body)
	if err != nil {
		log.Fatalln(err)
	}

	return js
}

func checkWCFServerUp(url string) bool {
	res, err := http.Get(url)
	if err != nil {
		log.Println(err)
		return false
	}

	return res.StatusCode == http.StatusOK
}
