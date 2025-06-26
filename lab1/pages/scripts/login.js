import { getValue, http } from "./main.js";

function submitLogin(){

    const model = {
        Username: getValue('[name="Username"]'),
        Password: getValue('[name="Password"]')
    }
    
    http.post('/user/login/submit', model, r => {
        console.log(r)
        //alert(JSON.stringify(r))
        if(r.Status === "success") top.location.href = "/data"
    })
}


document.querySelectorAll('button.primary').forEach(b => b.addEventListener("click",submitLogin))