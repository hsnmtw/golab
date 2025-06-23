
/**
 * 
 * @param {string} url the path / route to the server resource
 * @param {any} data the model to be transmitted
 * @param {function} callback the function to be invoked when the request is sent to the server
 */
function POST(url, data, callback){
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

export { POST, getValue }