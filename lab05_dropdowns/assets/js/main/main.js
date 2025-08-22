const http = {}

http.post = function(url,data,callback){
    const options = {
        method:  "post",
        headers: {"Accept":"text/json","Content-Type":"text/json"},
        body:    JSON.stringify(data)
    }
    fetch(url,options).then(x=>x.json()).then(callback).catch(callback)
}

http.get = function(url,data,callback){
    const args = Object.keys(data).map(x=>`${x}=${encodeURIComponent(data[x])}`).join('&')
    fetch(url+'?'+args).then(x=>x.text()).then(callback).catch(callback)
}