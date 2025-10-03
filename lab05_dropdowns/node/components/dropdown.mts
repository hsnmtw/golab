import { query } from "../main/app.mjs"

let select2 = query('[name="select2"]',x=>x)[0]

// replace select with a button

const star = `
<svg viewBox="0 -0.5 33 33" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <!-- Uploaded to: SVG Repo, www.svgrepo.com, Generator: SVG Repo Mixer Tools --> <title>star</title> <desc>Created with Sketch.</desc> <defs> </defs> <g id="Vivid.JS" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"> <g id="Vivid-Icons" transform="translate(-903.000000, -411.000000)" fill="#f3937a"> <g id="Icons" transform="translate(37.000000, 169.000000)"> <g id="star" transform="translate(858.000000, 234.000000)"> <g transform="translate(7.000000, 8.000000)" id="Shape"> <polygon points="27.865 31.83 17.615 26.209 7.462 32.009 9.553 20.362 0.99 12.335 12.532 10.758 17.394 0 22.436 10.672 34 12.047 25.574 20.22"> </polygon> </g> </g> </g> </g> </g> </g></svg>
`
const armor = `
<svg viewBox="-2.5 0 34 34" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <!-- Uploaded to: SVG Repo, www.svgrepo.com, Generator: SVG Repo Mixer Tools --> <title>shield</title> <desc>Created with Sketch.</desc> <defs> </defs> <g id="Vivid.JS" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"> <g id="Vivid-Icons" transform="translate(-281.000000, -566.000000)"> <g id="Icons" transform="translate(37.000000, 169.000000)"> <g id="shield" transform="translate(234.000000, 390.000000)"> <g transform="translate(9.000000, 7.000000)" id="Shape"> <path d="M15.516,34 C-0.414,27.828 1.032,6.8 1.032,6.8 C10.675,8.578 15.516,0 15.516,0 C15.516,0 20.346,8.578 29.968,6.8 C29.968,6.8 31.411,27.828 15.516,34 Z" fill="#FF6E6E"> </path> <polygon fill="#0C0058" points="8 18 13 23 23 13 21 11 13 19 10 16"> </polygon> </g> </g> </g> </g> </g> </g></svg>
`
const thunder = `
<svg viewBox="-4 0 32 32" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <!-- Uploaded to: SVG Repo, www.svgrepo.com, Generator: SVG Repo Mixer Tools --> <title>thunder</title> <desc>Created with Sketch.</desc> <defs> </defs> <g id="Vivid.JS" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"> <g id="Vivid-Icons" transform="translate(-205.000000, -723.000000)"> <g id="Icons" transform="translate(37.000000, 169.000000)"> <g id="thunder" transform="translate(156.000000, 546.000000)"> <g transform="translate(12.000000, 8.000000)" id="Shape"> <polygon fill="#FF6E6E" points="21 0 8.842 0 2 14 8 14 0 32 24 13 15 13"> </polygon> <polygon fill="#0C0058" points="16.421 19 0 32 5.778 19"> </polygon> </g> </g> </g> </g> </g> </g></svg>
`

const icons = [star,armor,thunder]
const texts = [
    "twincle little star",
    "armor sheild",
    "thunder"
]

const template = `
    <div class="flex-v modified-drop-down abs w-100">
        <button class="flex-h ac spaced" type="button">
            <span class="selected-item">
                ${select2.options[select2.options.selectedIndex].innerHTML}
            </span>
            <span class="rel"><b class="abs fnt-diff"></b></span>
        </button>
        <div class="grid flex-g" data-list tabIndex="1">
            <div class="rb p-3 drop-down gap-1 flex-v" style="background-color:cornsilk">
                ${(Array.from(select2.options) as Array<HTMLElement>).map(x=>x.innerText).map((x,i)=>`
                    <span data-option-index="${i}" class="p-3 hv flex-h gap-1 a-c"> 
                        <div class="grid-n" style="width:20px">${icons[i%icons.length]}</div> 
                        <span>${x} : ${texts[i%texts.length]}</span>
                    </span>
                `).join('\n')}
            </div>
        </div>
    </div>
    <div data-focus-out tabIndex="2"></div>
`
select2.classList.add('hidden');

const div = document.createElement('div')
div.innerHTML = template
const text = div.querySelector<HTMLElement>('.selected-item')!;
const focusOut = div.querySelector<HTMLElement>('[data-focus-out]')!;
select2.parentNode.insertBefore(div, select2)
div.querySelectorAll<HTMLElement>('[data-option-index]').forEach(item => {
    item.addEventListener("click", _ => {
        // console.log(item)
        select2.selectedIndex = +item.dataset["optionIndex"]!
        text.innerText = select2.value
        // console.log(text.innerText)
        setTimeout(()=>focusOut.focus(),1)
    })
})