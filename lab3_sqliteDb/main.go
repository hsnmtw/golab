package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"lab3/db"
	"log"
	"strings"

	_ "github.com/mattn/go-sqlite3"
)

func Direct() {
	db, err := sql.Open("sqlite3", "./test.db")
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	sqlStmt := `
    CREATE TABLE IF NOT EXISTS users (
        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        name TEXT
    );
    `
	_, err = db.Exec(sqlStmt)
	if err != nil {
		log.Fatal(err)
	}

	log.Println("Table 'users' created successfully")
}

type User struct {
	Id int
	Name string
}

func ORM() {
	cn := db.DbConnection{}
	fmt.Println("connection state before open is : ", cn.GetState())
	defer cn.Close()

	if e := cn.Open("sqlite3", "./test.db"); e != nil {
		log.Fatal(e)
	}

	fmt.Println("connection state after open is : ", cn.GetState())
	
		_,e := cn.Exec(`
    CREATE TABLE IF NOT EXISTS users (
        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        name TEXT
    );
    `)

	if e!=nil {
		log.Fatal("error while creating : ",e)
	}
	
	
	affected, e := cn.Exec("INSERT INTO [users] ([name]) VALUES (@0)", "Hussain")
	
	if e != nil {
		log.Fatal("error when inserting: ", e)
	}

	fmt.Println("Affected rows : ", affected)
	data, e := cn.Query("SELECT * FROM [users]")
	if e != nil {
		log.Fatal("error when selecting: ", e)
	}
	b, _ := json.Marshal(data)
	fmt.Println(strings.Join( strings.Split(string(b),"},{") ,"}\n,{"))
	fmt.Println("Connection State before closing is : ", cn.GetState())

	var users []User
	_ = cn.QueryUnmarshalled(&users,"select [Id],[Name] from users")
	fmt.Println("Query unmarshalled")
	for i,user := range users {
		fmt.Printf("\n [%d] User : id=%d, name=%s", i+1, user.Id, user.Name)
	}

	cn.Close()
	fmt.Println("\n\nConnection State after closing is : ", cn.GetState())
}

func main() {
	ORM()
}
