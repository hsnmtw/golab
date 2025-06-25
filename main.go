package main

import (
	"bytes"
	"fmt"
	"html/template"
	"io"
	"log"
	"net/http"
	"regexp"
	"slices"
	"strings"

	"hsnmtw/data"
	"hsnmtw/fs"
	"hsnmtw/routes"
	"hsnmtw/sessions"
	"hsnmtw/storage"
	"hsnmtw/utilities"

	users "hsnmtw/user"
)

type Layout struct {
	Header any
	Title  any
	Main   any
	Footer any
	User   any
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
	isPartial := r.URL.Query().Get("isPartial") == "true"

	if len(cookies) > 0 {
		sessionId = cookies[0].Value
	}

	if !ok {
		fpath := "." + r.URL.Path
		if fs.Exists(fpath) {
			buffer := fs.Read(fpath)
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

	if title == ":json:" || isPartial {
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
		User:   u,
	}

	b := bytes.Buffer{}
	var myWriter = io.MultiWriter(&b)
	tmpl.Execute(myWriter, layout)

	//
	var html = b.String()
	patten := `(<Comp1[^\/]+\/>)`
	rx := regexp.MustCompile(patten)
	m := rx.FindAllStringSubmatch(html, 100)

	// pvs := strings.ReplaceAll(strings.Join(m, ","), "=", ":")

	// //k := pvs
	// patten = `(\w+)\s*=\s*"([^"]+)"`
	// rx = regexp.MustCompile(patten)
	// z := rx.FindAllStringSubmatch(pvs, 50)
	// d, _ := json.Marshal(utilities.SetOf(utilities.Flatten(m)))

	s := utilities.SetOf(utilities.Flatten(m))
	x := strings.Join(utilities.Project(s, utilities.XmlToJson), "|")

	html = strings.ReplaceAll(html, "<Comp1", "<pre>XML: "+template.HTMLEscapeString(s[0])+" \nJSON: "+x+"</pre><Comp1")
	//

	// w.Write(b.Bytes())
	fmt.Fprint(w, html)
}

func home(w http.ResponseWriter, r *http.Request) ([]byte, string, error) {
	b := fs.ReadHTML("./pages/html/home.html")
	return b, "Home", nil
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
	if err == nil {
		sessions.Sessions = s
	}
	users.RegistersRoutes(routes.Routes)
	data.RegistersRoutes(routes.Routes)
	routes.Routes["/"] = home
	http.HandleFunc("/", handler)
	log.Fatal(http.ListenAndServe("localhost:80", nil))
}
