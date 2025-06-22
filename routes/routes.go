package routes

import "net/http"

var Routes map[string]func(w http.ResponseWriter, r *http.Request) = make(map[string]func(w http.ResponseWriter, r *http.Request), 0)
