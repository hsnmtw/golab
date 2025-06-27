package db

import (
	"database/sql"
	"errors"
)

const (
	CLOSED = iota
	CONNECTING
	ERROR
	OPEN
	BUSY
)

type IDbConnection interface {
	Open(driver string, sourceName string) error
	Close() error
	Query(sql string, parameters []any) ([]map[string]any, error)
	Execute(sql string, parameters []any) (int64, error)
}

type DbConnection struct {
	db     *sql.DB
	Source string
	State  int
}

func (dbConnection *DbConnection) Open(driver string, sourceName string) error {
	dbConnection.State = CONNECTING
	dbConnection.Source = sourceName
	db, e := sql.Open(driver, sourceName)
	if e != nil {
		dbConnection.State = ERROR
		return e
	}
	dbConnection.State = OPEN
	dbConnection.db = db
	return nil
}

func (dbConnection *DbConnection) Close() error {

	if dbConnection.State == CLOSED {
		return nil
	}

	if e := dbConnection.db.Close(); e != nil {
		return e
	}

	dbConnection.State = CLOSED

	return nil
}

func (dbConnection *DbConnection) Query(sql string, parameters []any) ([]map[string]any, error) {

	if dbConnection.State != OPEN {
		return nil, errors.New("ERROR: db connection is not OPEN")
	}

	dbConnection.State = BUSY
	defer func() {
		dbConnection.State = OPEN
	}()

	rows, e := dbConnection.db.Query(sql, parameters...)

	if e != nil {
		return nil, e
	}

	data := make([]map[string]any, 0)
	cols, e := rows.Columns()

	if e != nil {
		return nil, e
	}

	numColumns := len(cols)
	values := make([]any, numColumns)

	for i := range values {
		values[i] = new(any)
	}

	for rows.Next() {
		if err := rows.Scan(values...); err != nil {
			return nil, err
		}
		record := make(map[string]any, numColumns)
		for i, col := range cols {
			record[col] = *(values[i].(*any))
		}
		data = append(data, record)
	}

	if err := rows.Err(); err != nil {
		return nil, err
	}
	dbConnection.State = OPEN
	return data, nil
}

func (dbConnection *DbConnection) Exec(sql string, parameters []any) (int64, error) {

	if dbConnection.State != OPEN {
		return -1, errors.New("ERROR: db connection is not OPEN")
	}

	defer func() {
		dbConnection.State = OPEN
	}()

	dbConnection.State = BUSY
	result, e := dbConnection.db.Exec(sql, parameters...)

	if e != nil {
		return -2, e
	}

	dbConnection.State = OPEN
	return result.RowsAffected()
}
