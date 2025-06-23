package data

import "net/http"

// [POST] json
func dataView(w http.ResponseWriter, r *http.Request) {
	w.Write([]byte("in data"))
}

func RegistersRoutes(routes map[string]func(w http.ResponseWriter, r *http.Request)) {
	/*
		<table>
			@foreach(){
			}
		</table>
	*/
	routes["/data"] = dataView
}
