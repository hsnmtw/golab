package main

import (
	"log"
	"mime"
	"net/http"
	"os"
	"path/filepath"
	"strings"
)

func option(b []byte, e error) []byte {
	if e != nil {
		return []byte(e.Error())
	}
	return b
}

func mimeTypeForFile(file string) string {
	// We use a built in table of the common types since the system
	// TypeByExtension might be unreliable. But if we don't know, we delegate
	// to the system.
	ext := filepath.Ext(file)
	switch ext {
	case ".htm", ".html":
		return "text/html"
	case ".css":
		return "text/css"
	case ".js":
		return "application/javascript"

		// ...

	default:
		return mime.TypeByExtension(ext)
	}
}

func router(w http.ResponseWriter, r *http.Request) {
	p := strings.Trim(strings.ToLower(r.URL.Path), " ")
	if p == "/" {
		p = "/assets/html/home.html"
	}
	log.Default().Printf("[inf] requested %s \n", p)
	if strings.HasPrefix(p, "/assets") {
		w.Header().Set("Content-Type", mimeTypeForFile("."+p))
		w.Write(option(os.ReadFile("." + p)))
		return
	}
}

func main() {
	http.HandleFunc("/", router)
	log.Fatal(http.ListenAndServe("localhost:80", nil))
}
