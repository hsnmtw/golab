package comp

import "fmt"

type IComp interface{}

type Comp1 struct {
	FirstName string
	LastName  string
}

func (comp Comp1) Render() string { 
	return fmt.Sprintf(`
	
		<div class="grid-2 gap-1em">
			<div class="flex f-col gap-1em">
				<label>First Name</label>
				<b>%s</b>
			</div>
			<div class="flex f-col gap-1em">
				<label>Last Name</label>
				<b>%s</b>
			</div>
		</div>
	
	
	`,comp.FirstName,comp.LastName)
}
