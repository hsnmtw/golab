import { query } from "../main/app.js"

function dropdown(items) {
    const html = `
        <select>
            ${items.map(x=>`<option>${x}</option>`)}
        </select>
    `
    return html
}

query('DropDown', s => {
    s.innerHTML = dropdown(s.attributes["options"].value.split('|'))
})

export { dropdown }