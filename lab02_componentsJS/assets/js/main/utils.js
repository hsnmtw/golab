function guid(){
    const chars = 'abcdefghijklmnopqrstuvwxyz0123456789'

    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c => chars[(chars.length*Math.random()|0)%chars.length] )
}


export { guid }