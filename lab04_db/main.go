package main

import (
	"database/sql"
	"fmt"

	_ "modernc.org/sqlite"  // !
)

func main(){
	/*
	Class.forName("jdbc::odbc::")
	*/
	db,e := sql.Open("sqlite","./database.sqlite")
	if e != nil {
		panic(e)
	}
	defer db.Close()
	_,e = db.Exec("create table [myTable] ([name] text, [age] int)")
	if e != nil {
		panic(e)
	}
	_,_ = db.Exec("insert into [myTable] ([name], [age]) values ('Mohammad',15),('Ali',10)")
	rows,e := db.Query("select name,age from [mytable]")
	if e != nil {
		panic(e)
	}
	for rows.Next() {
		var name string
		var age int32
		_ = rows.Scan(&name,&age)
		fmt.Printf("Name=%s \t Age=%d\n",name,age)
	}	
	db.Close()
}