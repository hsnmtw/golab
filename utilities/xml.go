package utilities

import (
	"encoding/json"

	// "hsnmtw/utilities"
	"regexp"
)

func XmlToJson(xml string) string {
	x := regexp.MustCompile(`\s(?P<key>[a-zA-Z0-9]+)[^=]*[=][^\"]*[\"](?P<value>[^\"]*)[\"]`)
	ms := Flatten(x.FindAllStringSubmatch(xml, 100))
	pvs := make(map[string]string)
	for i := 1; i < len(ms); i += 3 {
		pvs[ms[i]] = ms[i+1]
	}
	b, _ := json.Marshal(pvs)
	return string(b)
}
