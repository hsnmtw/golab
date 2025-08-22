package db

import (
	"database/sql"
	"encoding/json"
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
	Query(sql string, parameters ...any) ([]map[string]any, error)
	QueryUnmarshalled(target any, sql string, parameters ...any) error
	Execute(sql string, parameters ...any) (int64, error)
	GetDriver() string
	GetSource() string
	GetState() string
}

type DbConnection struct {
	db     *sql.DB
	driver string
	source string
	state  int
}

func (dbConnection *DbConnection) GetDriver() string {
	return dbConnection.driver
}

func (dbConnection *DbConnection) GetSource() string {
	return dbConnection.source
}

func (dbConnection *DbConnection) GetState() string {
	switch dbConnection.state {
	case OPEN:
		return "OPEN"
	case CLOSED:
		return "CLOSED"
	case ERROR:
		return "ERROR"
	case CONNECTING:
		return "CONNECTING"
	case BUSY:
		return "BUSY"
	default:
		return "UNKNOWN"
	}
}

func (dbConnection *DbConnection) Open(driver string, sourceName string) error {
	dbConnection.state = CONNECTING
	dbConnection.driver = driver
	dbConnection.source = sourceName
	db, e := sql.Open(driver, sourceName)
	if e != nil {
		dbConnection.state = ERROR
		return e
	}
	dbConnection.state = OPEN
	dbConnection.db = db
	return nil
}

func (dbConnection *DbConnection) Close() error {

	if dbConnection.state == CLOSED {
		return nil
	}

	if e := dbConnection.db.Close(); e != nil {
		return e
	}

	dbConnection.state = CLOSED

	return nil
}

func (dbConnection *DbConnection) Query(sql string, parameters ...any) ([]map[string]any, error) {

	if dbConnection.state != OPEN {
		return nil, errors.New("ERROR: db connection state is not OPEN, ::= " + dbConnection.GetState())
	}

	dbConnection.state = BUSY
	defer func() {
		dbConnection.state = OPEN
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
	dbConnection.state = OPEN
	return data, nil
}

func (dbConnection *DbConnection) QueryUnmarshalled(target any, sql string, parameters ...any) error{
	
	data,e := dbConnection.Query(sql,parameters...)
	
	if e != nil {
		return e
	}

	b,e := json.Marshal(data)
	
	if e != nil {
		return e
	}

	if e = json.Unmarshal(b,target); e != nil {
		return e
	}

	return nil
}


func (dbConnection *DbConnection) Exec(sql string, parameters ...any) (int64, error) {

	if dbConnection.state != OPEN {
		return -1, errors.New("ERROR: db connection state is not OPEN, ::= " + dbConnection.GetState())
	}

	defer func() {
		dbConnection.state = OPEN
	}()

	dbConnection.state = BUSY
	result, e := dbConnection.db.Exec(sql, parameters...)

	if e != nil {
		return -2, e
	}

	dbConnection.state = OPEN
	return result.RowsAffected()
}
