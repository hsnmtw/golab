package data

import (
	"html/template"
	"net/http"
)

// [POST] json
// func dataView(w http.ResponseWriter, r *http.Request) {
// 	htmlb, err := os.ReadFile("./pages/data/sample.html")
// 	if err != nil {
// 		w.Write([]byte(err.Error()))
// 		return
// 	}
// 	html := string(htmlb)
// 	html = strings.ReplaceAll(html, "@title", "my title")
// 	html = strings.ReplaceAll(html, "@body", "my body")
// 	w.Write([]byte(html))
// }

type DataList struct {
	Item string
}

type DataModel struct {
	Title string
	Body  any
	List  []DataList
}

func dataView(w http.ResponseWriter, r *http.Request) {
	tmpl, _ := template.ParseFiles("./pages/data/sample.html")
	tmpl.Execute(w, DataModel{
		Title: "my titile",
		Body:  ("You typed : ") + template.HTML("<b>bold</b>"),
		List:  []DataList{{Item: "Item1"}, {Item: "Item2"}, {Item: "Item3"}},
	})
}

const MY_CONST = "SOME value"

func RegistersRoutes(routes map[string]func(w http.ResponseWriter, r *http.Request)) {
	/*
		<table>
			@foreach(){
			}
		</table>
	*/

	var SomeConst = "some value"


	mymap := make(map[int]string)
	mymap[0] = SomeConst

	SomeConst = "yyu"


	routes["/data"] = dataView
}
