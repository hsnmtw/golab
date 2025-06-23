package routes

import "net/http"

Routes := make(map[string]func(w http.ResponseWriter, r *http.Request))
