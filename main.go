package main

import (
	"fmt"
	"html/template"
	"log"
	"net/http"
	"os"
	"slices"
	"strings"

	"hsnmtw/data"
	"hsnmtw/routes"
	"hsnmtw/sessions"
	"hsnmtw/storage"

	// "hsnmtw/sessions"
	users "hsnmtw/user"
)

type Layout struct {
	Header any
	Title  any
	Main   any
	Footer any
}

var exmpted = []string{
	"/user/login",
	"/user/logout",
	"/user/login/submit",
}

func handler(w http.ResponseWriter, r *http.Request) {
	log.Default().Printf("[INF] requested Path is %s\n", r.URL.Path)
	r.URL.Path = strings.ToLower(r.URL.Path)
	var h, ok = routes.Routes[r.URL.Path]
	sessionId := ""
	cookies := r.CookiesNamed("SESSION_ID")
	
	if len(cookies) > 0 {
		sessionId = cookies[0].Value
	}

	var f string = "not found"
	if ok {
		f = "found"
	}

	fmt.Println("p='"+r.URL.Path+"' ok='"+f+"'")

	if !ok {
		fpath := "." + r.URL.Path
		_, err := os.Stat(fpath)
		if err == nil {
			buffer, err := os.ReadFile(fpath)
			if err == nil {
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
		}
		w.WriteHeader(http.StatusNotFound)
		fmt.Fprintf(w, "404 Page not found : %v", r.URL.Path)
		return
	}

	u, ok := sessions.Sessions[sessionId]

	if !slices.Contains(exmpted, r.URL.Path) && (!ok || u == "") {
		fmt.Println("not logged in - redirect")
		h = routes.Routes["/user/login"]
	}

	buffer, title, err := h(w, r)

	if err != nil {
		w.Write([]byte(err.Error()))
	}
	
	if title == ":json:" {
		w.Header().Set("Content-Type", "application/json")
		w.Write(buffer)
		return
	}
	
	tmpl, err := template.ParseFiles("./pages/html/layout.html")
	
	if err != nil {
		w.Write([]byte(err.Error()))
	}
	
	var header any = ""
	
	if u != "" {
		header = template.HTML(`<a href="/user/logout">Logout</a>`)
	}
	
	layout := Layout{
		Title:  title,
		Header: header,
		Main:   template.HTML(buffer),
		Footer: err,
	}
	tmpl.Execute(w, layout)
}

func home(w http.ResponseWriter, r *http.Request) ([]byte,string,error) {
	b,e := os.ReadFile("./pages/html/home.html")
	return b,"Home",e
}

func main() {
	// fmt.Println("Project([1,2,3],x=>x*x)", utilities.Project([]int{1, 2, 3}, func(x int) int { return x * x }))
	// fmt.Println("Filter([1,2,3],x=>x%2==0)", utilities.Filter([]int{1, 2, 3}, func(x int) bool { return x%2 == 0 }))

	// s := []float32{600, 470, 170, 430, 300}
	// fmt.Println("mean=", utilities.Mean(s))
	// fmt.Println("sdev=", utilities.StdDev(s))

	//load sessions from os
	var s map[string]string = map[string]string{}
	err := storage.Query("sessions", &s)
	if err != nil {
		sessions.Sessions = s
	}
	users.RegistersRoutes(routes.Routes)
	data.RegistersRoutes(routes.Routes)
	routes.Routes["/"] = home
	http.HandleFunc("/", handler)
	log.Fatal(http.ListenAndServe("localhost:80", nil))
}
