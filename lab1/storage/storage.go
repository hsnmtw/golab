package storage

import (
	"encoding/json"
	"hsnmtw/fs"
)

func Query(name string, data any) error {
	buffer := fs.Read("./json/" + name + ".json")
	return json.Unmarshal(buffer, data)
}

func Save(name string, data any) error {
	buffer, err := json.Marshal(data)
	if err != nil {
		return err
	}
	return fs.Write("./json/"+name+".json", buffer)
}
