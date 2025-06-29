import { query } from "../main/app.js"

let select2 = query('[name="select2"]',x=>x)[0]

// replace select with a button

const template = `
    <div class="flex-v modified-drop-down abs w-100">
        <button class="flex-h ac spaced" type="button">
            <span class="selected-item">${select2.options[select2.options.selectedIndex].innerHTML}</span>
            <span class="rel"><b class="abs fnt-diff"></b></span>
        </button>
        <div class="grid">
            <div class="rb p-3 drop-down gap-1 flex-v" style="background-color:cornsilk">
                ${Array.from(select2.options).map(x=>x.innerText).map((x,i)=>`<span data-option-index="${i}" class="p-3 hv">${x}</span>`).join('\n')}
            </div>
        </div>
    </div>
`
select2.classList.add('hidden');

const div = document.createElement('div')
div.innerHTML = template
const text = div.querySelector('.selected-item')
select2.parentNode.insertBefore(div, select2)
div.querySelectorAll('[data-option-index]').forEach(item => {
    item.addEventListener("click", _ => {
        // console.log(item)
        select2.selectedIndex = +item.dataset.optionIndex
        text.innerText = select2.value
        // console.log(text.innerText)
    })
})