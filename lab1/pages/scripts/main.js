const http = {}

/**
 * 
 * @param {string} url the path / route to the server resource
 * @param {any} data the model to be transmitted
 * @param {function} callback the function to be invoked when the request is sent to the server, one argument of type object
 */
http.post = function(url, data, callback){
    const options = {
        method: "POST", 
        headers: { 
            "Content-Type": "application/json", 
            "Accept": "application/json" 
        }, 
        body: JSON.stringify(data)
    }
    fetch(url, options)
    .then(x => x.json())
    .then(callback)
    .catch(callback)
}

/**
 * 
 * @param {string} url path to the request service
 * @param {object} data arguments to be passed
 * @param {function} callback a function that accepts an argument (string)
 */
http.get = function(url, data, callback) {
    if(!callback || typeof callback !== "function") return console.error("wrong use of http.get")
    if(!url || url.length === 0) return callback('empty url')
    if(!data) data = {}
    data.isPartial = true
    const args = !data ? "" : Object.keys(data).map(x => `${x}=${encodeURIComponent(data[x])}`).join('&')
    const path = url + (url.indexOf('?') > -1 ? '&' : '?') + args
    fetch(path.replace('&&','&'))
    .then(x=>x.text())
    .then(callback)
    .catch(callback)
}

/**
 * 
 * @param {string} selector xpath selector
 * @returns {string|null} the value in input/select/textarea field
 */
function getValue(selector) {
    const el = document.querySelector(selector)
    if(!el) return null
    return el.value
}

function form2json(selector) {
    return {}
}

export { http, getValue }