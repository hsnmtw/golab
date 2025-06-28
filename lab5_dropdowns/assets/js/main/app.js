

NodeList.prototype.toArray = function(){
    return Array.from(this)
}

function query(selector, callback) {
    if(!selector) return []
    const empty = _ => {}
    return document.querySelectorAll(selector).toArray().map(callback ?? empty)
}

// function main() {
//     query('main',m=>m.innerHTML = 'OO')
// }

// main()

export { query }
//document.querySelector('main')