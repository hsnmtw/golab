interface Http {
    post(url: string,data : any,callback: () => void) : void;
    get(url: string,data : any,callback: () => void) : void;
}
const http : Http = {

    post(url: string,data : any,callback: () => void) : void{
        const options = {
            method:  "post",
            headers: {"Accept":"text/json","Content-Type":"text/json"},
            body:    JSON.stringify(data)
        }
        fetch(url,options).then(x=>x.json()).then(callback).catch(callback)
    },

    get(url: string,data : any,callback : () => void) : void {
        const args = Object.keys(data).map(x=>`${x}=${encodeURIComponent(data[x])}`).join('&')
        fetch(url+'?'+args).then(x=>x.text()).then(callback).catch(callback)
    }
}