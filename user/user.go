package users

import (
	"encoding/json"
	"fmt"
	"hsnmtw/sessions"
	"hsnmtw/storage"
	"hsnmtw/utilities"
	"io"
	"net/http"
	"os"
	"time"
)

// const (
// 	WEEKDAY_SUNDAY = iota
// 	WEEKDAY_MONDAY
// 	WEEKDAY_TUESDAY
// 	WEEKDAY_WEDENSDAY
// 	WEEKDAY_THURSDAY
// 	WEEKDAY_FRIDAY
// 	WEEKDAY_SATURDAY
// )

const SESSION_ID = "SESSION_ID"

type LoginResponse struct {
	Status  string // success/failues
	Message string
}

type UserLogin struct {
	Username string
	Password string
}

// [GET] html
func logout(w http.ResponseWriter, r *http.Request) ([]byte,string,error) {
	destroySession(w,r)
	return []byte(`<meta http-equiv="refresh" content="0; url=/">`),"",nil
}

func login(w http.ResponseWriter, r *http.Request) ([]byte,string,error) {
	//	fmt.Fprintf(w, "Path is %s", r.URL.Path)

	buffer, err := os.ReadFile("./pages/user/login.html")

	if err != nil {
		w.Write([]byte(err.Error()))
		return []byte{},"error",err
	}

	// w.Write(buffer)
	return buffer,"Login",nil
}

// [POST] json
func submitLogin(w http.ResponseWriter, r *http.Request) ([]byte,string,error) {
	// fmt.Fprintf(w, "Path is %s", r.URL.Path)
	// u := r.FormValue("Username")
	// p := r.FormValue("Password")
	buffer, _ := io.ReadAll(r.Body)
	ul := UserLogin{}
	json.Unmarshal(buffer, &ul)
	fmt.Printf("\n\n\n")
	fmt.Printf("Username : %s\n", ul.Username)
	fmt.Printf("Password : %s\n", ul.Password)
	response := LoginResponse{
		Status: "failure",
		Message: "Login failed",
	}
	if ul.Username == "admin" && ul.Password == "123" {
		response.Status = "success"
		response.Message = "OK"
		createSession(w,  ul.Username)		
	}
	encoded,_ := json.Marshal(response)
	// w.Write(encoded)
	return encoded,":json:",nil
}




func RegistersRoutes(routes map[string]func(w http.ResponseWriter, r *http.Request) ([]byte,string,error)) {
	routes["/user/login"] = login
	routes["/user/logout"] = logout
	routes["/user/login/submit"] = submitLogin
}


func createSession(w http.ResponseWriter, username string) {
	guid, _ := utilities.Guid()
	sessions.Sessions[guid] = username
	http.SetCookie(w, &http.Cookie{
		Name:        SESSION_ID,
		Value:       guid,
		Path:        "/",
		Expires:     time.Now().Add(3 * time.Hour),
		HttpOnly:    true,
		Secure:      true,
		SameSite:    http.SameSiteDefaultMode,
		MaxAge:      1000 * 60 * 60 * 3,
		Partitioned: false,
		Quoted:      false,
		RawExpires:  time.Now().Add(3 * time.Hour).GoString(),
	})
	storage.Save("sessions", sessions.Sessions)
}

func destroySession(w http.ResponseWriter, r *http.Request) {
	lsessions := r.CookiesNamed(SESSION_ID)
	if len(lsessions) == 0 {
		return
	}
	http.SetCookie(w, &http.Cookie{
		Name:    SESSION_ID,
		Path:    "/",
		Expires: time.Now().Add(-1),
	})
	delete(sessions.Sessions, lsessions[0].Value)
	storage.Save("sessions", sessions.Sessions)
}