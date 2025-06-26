package routes

import "net/http"

var Routes = make(map[string]func(w http.ResponseWriter, r *http.Request) ([]byte,string,error))
