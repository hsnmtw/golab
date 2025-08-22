function onSubmitLogin(target){
    console.log("something...");
}

function login(target){
    if(!target) return
    const html = `
    <div class="grid-n" style="--columns:2">
        <label>Username</label><input>
        <label>Password</label><input type="password">
        <i></i><button class="primary" type="button">Go</button>
    </div>
    `
    target.innerHTML = html
    target.querySelector('button.primary').addEventListener("click",_=>onSubmitLogin(target))
}

