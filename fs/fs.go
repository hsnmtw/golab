package fs

import "os"

func Exists(path string) bool {
	_,err := os.Stat(path)
	return err == nil
}

func ReadHTML(path string) []byte {
	return Read(path)
}

func Read(path string) []byte {
	buffer, err := os.ReadFile(path)
	if err != nil {
		return []byte(err.Error())
	}
	return buffer
}

func Write(path string, content []byte) error {
	return os.WriteFile(path, content, 0642)
}
