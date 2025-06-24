package data

import (
	"bytes"
	"html/template"
	"io"
	"net/http"
)

type DataList struct {
	Item string
}

type DataModel struct {
	Title string
	Body  any
	List  []DataList
}

func dataView(w http.ResponseWriter, r *http.Request) ([]byte,string,error) {
	tmpl, _ := template.ParseFiles("./pages/data/sample.html")
	buffer := bytes.Buffer{}
	var wr io.Writer = io.MultiWriter(&buffer)
	err := tmpl.Execute(wr, DataModel{
		Title: "my titile",
		Body:  ("You typed : ") + template.HTML("<b>bold</b>"),
		List:  []DataList{{Item: "Item1"}, {Item: "Item2"}, {Item: "Item3"}},
	})
	return buffer.Bytes(),"Data",err
}

const MY_CONST = "SOME value"

func RegistersRoutes(routes map[string]func(w http.ResponseWriter, r *http.Request) ([]byte,string,error)) {
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
