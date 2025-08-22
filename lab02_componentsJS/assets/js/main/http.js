const http = {
    post(url,data,callback){
        const options = {
            method:"post",
            body: JSON.stringify(data),
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        }
        return fetch(url,options).then(x=>x.json()).then(callback).catch(callback)
    },
    get(url,data,callback){
        data ??= {}
        data.isPartial=true
        const args = (url.indexOf('?')>-1?'&':'?')+Object.keys(data).map(x=>`${x}=${encodeURIComponent(data[x])}`).join('&')     
        return fetch(url+args,{method:"get"}).then(x=>x.text()).then(callback).catch(callback)
    }
}

//export { http }