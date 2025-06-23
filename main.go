package main

import (
	"fmt"
	"log"
	"net/http"
	"os"
	"slices"
	"strings"

	"hsnmtw/data"
	"hsnmtw/routes"
	"hsnmtw/sessions"
	users "hsnmtw/user"
)

func handler(w http.ResponseWriter, r *http.Request) {
	log.Default().Printf("[INF] requested Path is %s\n", r.URL.Path)
	r.URL.Path = strings.ToLower(r.URL.Path)
	var h, ok = routes.Routes[r.URL.Path]
	sessionId := ""
	cookies := r.CookiesNamed("SESSION_ID")
	if len(cookies) > 0 {
		sessionId = cookies[0].Value
	}
	if !ok {
		fpath := "." + r.URL.Path
		_, err := os.Stat(fpath)
		if err == nil {
			buffer, _ := os.ReadFile(fpath)
			parts := strings.Split(fpath, ".")
			if len(parts) > 0 && parts[len(parts)-1] == "js" {
				w.Header().Set("Content-Type", "text/javascript")
			}
			if len(parts) > 0 && parts[len(parts)-1] == "css" {
				w.Header().Set("Content-Type", "text/css")
			}
			w.Write(buffer)
			return
		}
		w.Header().Set("Header", "404")
		fmt.Fprintf(w, "404 Page not found : %v", r.URL.Path)
		return
	}

	exmpted := []string{
		"/user/login",
		"/user/logout",
		"/user/login/submit",
	}

	u, ok := sessions.Sessions[sessionId]

	if !slices.Contains(exmpted, r.URL.Path) && !ok {
		w.Header().Set("Status", "401")
		w.Write([]byte("401 UNAUTHORIZED:" + u))
		return
	}
	h(w, r)
}

func main() {
	users.RegistersRoutes(routes.Routes)
	data.RegistersRoutes(routes.Routes)
	http.HandleFunc("/", handler)
	log.Fatal(http.ListenAndServe("localhost:80", nil))
}
