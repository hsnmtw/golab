import { guid } from '../main/utils.js'
//import { http } from '../main/http.js'

const uuid = guid()

function render(el){
    const html = `
    <div data-uuid="${uuid}">
        <h2>تسجيل الدخول</h2>
        <hr>
        <br>
        <div class="grid-n v-center" style="grid-template-columns: auto 1fr;">
            <label>Username</label><input name="Username">
            <label>Password</label><input name="Password" type="password">
            <i></i><div> <button type="button">Go</button> </div>
        </div>
    </div>
    `
    el.innerHTML = html
    return {
        getUsername(){ return el.querySelector(`[data-uuid="${uuid}"] [name="Username"]`).value },
        getPassword(){ return el.querySelector(`[data-uuid="${uuid}"] [name="Password"]`).value },
        addEventListener(listener){
            el.querySelectorAll(`[data-uuid="${uuid}"] button`).forEach(b=>{
                b.addEventListener("click",listener)
            })
        }
    }
}

export { render }