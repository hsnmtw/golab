package storage

import (
	"encoding/json"
	"os"
)

func Query(name string, data any) error {
	buffer, err := os.ReadFile("./json/"+name+".json")
	
	if err != nil {
		return err
	}
	err = json.Unmarshal(buffer, data)
	return err
}

func Save(name string, data any) error {
	buffer,err := json.Marshal(data)
	if err != nil {
		return err
	}
	return os.WriteFile("./json/"+name+".json", buffer, 0642)
}