
//@ts-ignore
NodeList.prototype.toArray = function(){
    return Array.from(this)
}

function query(selector : string, callback : (a: any) => any) : Array<any> {
    if(!selector) return []
    const empty = (a:any) => function (){};
    const nl = document.querySelectorAll<HTMLElement>(selector);
    const arr = Array.from(nl);
    return arr.map(callback ?? empty);
}

// function main() {
//     query('main',m=>m.innerHTML = 'OO')
// }

// main()

export { query }
//document.querySelector('main')