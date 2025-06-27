package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"lab3/db"
	"log"

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

// type User struct {
// 	id int
// 	name string
// }

func ORM() {
	cn := db.DbConnection{}
	fmt.Println("connection state before open is : ", cn.State)
	defer cn.Close()

	if e := cn.Open("sqlite3", "./test.db"); e != nil {
		log.Fatal(e)
	}

	fmt.Println("connection state after open is : ", cn.State)
	
		_,e := cn.Exec(`
    CREATE TABLE IF NOT EXISTS users (
        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        name TEXT
    );
    `, []any{})

	if e!=nil {
		log.Fatal("error while creating : ",e)
	}
	
	
	affected, e := cn.Exec("INSERT INTO [users] ([name]) VALUES (@0)", []any{"Hussain"})
	if e != nil {
		log.Fatal("error when inserting: ", e)
	}
	fmt.Println("Affected rows : ", affected)
	data, e := cn.Query("SELECT * FROM [users]", []any{})
	if e != nil {
		log.Fatal("error when selecting: ", e)
	}
	b, _ := json.Marshal(data)
	fmt.Println(string(b))
	fmt.Println("Connection State before closing is : ", cn.State)
	cn.Close()
	fmt.Println("Connection State after closing is : ", cn.State)
}

func main() {
	ORM()
}
