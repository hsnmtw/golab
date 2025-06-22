import { getValue, POST } from "./main.js";

function submitLogin(){

    const model = {
        Username: getValue('[name="Username"]'),
        Password: getValue('[name="Password"]')
    }
    POST('/user/login/submit', model, r => {
        console.log(r)
        alert(JSON.stringify(r))
    })
}


document.querySelectorAll('button.primary').forEach(b => b.addEventListener("click",submitLogin))