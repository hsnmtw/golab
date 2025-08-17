function modernizeSelect(selector){
    document.querySelectorAll(selector).forEach(select => {
        select.className = 'hidden'

            const options = function() {
                return Array.from(select.options)
            }

            const circles = `
            <svg viewBox="-3 0 36 36" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <!-- Uploaded to: SVG Repo, www.svgrepo.com, Generator: SVG Repo Mixer Tools --> <title>share</title> <desc>Created with Sketch.</desc> <defs> </defs> <g id="Vivid.JS" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"> <g id="Vivid-Icons" transform="translate(-202.000000, -565.000000)"> <g id="Icons" transform="translate(37.000000, 169.000000)"> <g id="share" transform="translate(156.000000, 390.000000)"> <g transform="translate(9.000000, 6.000000)" id="Shape"> <path d="M20.954,9.211 L7.818,16.76 L5.752,14.19 L18.889,6.641 L20.954,9.211 Z M20.86,26.772 L7.632,18.72 L5.551,21.528 L18.78,29.58 L20.86,26.772 Z" fill="#0C0058" fill-rule="nonzero"> </path> <path d="M6,12 C9.3137085,12 12,14.6862915 12,18 C12,21.3137085 9.3137085,24 6,24 C2.6862915,24 0,21.3137085 0,18 C0,14.6862915 2.6862915,12 6,12 Z M24,0 C27.3137085,0 30,2.6862915 30,6 C30,9.3137085 27.3137085,12 24,12 C20.6862915,12 18,9.3137085 18,6 C18,2.6862915 20.6862915,0 24,0 Z M24,24 C27.3137085,24 30,26.6862915 30,30 C30,33.3137085 27.3137085,36 24,36 C20.6862915,36 18,33.3137085 18,30 C18,26.6862915 20.6862915,24 24,24 Z" fill="#FF6E6E" fill-rule="nonzero"> </path> </g> </g> </g> </g> </g> </g></svg>
            `
            const magnify = `
            <svg viewBox="-0.5 0 32 32" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <!-- Uploaded to: SVG Repo, www.svgrepo.com, Generator: SVG Repo Mixer Tools --> <title>search-disabled</title> <desc>Created with Sketch.</desc> <defs> </defs> <g id="Vivid.JS" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"> <g id="Vivid-Icons" transform="translate(-670.000000, -489.000000)"> <g id="Icons" transform="translate(37.000000, 169.000000)"> <g id="search-disabled" transform="translate(624.000000, 312.000000)"> <g transform="translate(9.000000, 8.000000)" id="Shape"> <path d="M17.283,20.611 L27.889,31.218 L30.717,28.39 L20.111,17.783 L17.283,20.611 Z M14.829,16.243 L12,13.414 L9.172,16.242 L7.758,14.828 L10.586,12 L7.758,9.172 L9.172,7.758 L12,10.586 L14.828,7.758 L16.242,9.172 L13.414,12 L16.242,14.828 L14.829,16.243 Z" fill="#0C0058" fill-rule="nonzero"> </path> <path d="M12,24 C5.372583,24 3.94430453e-31,18.627417 -1.77635684e-15,12 C-3.55271368e-15,5.372583 5.372583,3.94430453e-31 12,-1.77635684e-15 C18.627417,-3.55271368e-15 24,5.372583 24,12 C24,15.1825979 22.7357179,18.2348448 20.4852814,20.4852814 C18.2348448,22.7357179 15.1825979,24 12,24 Z M12,4 C7.581722,4 4,7.581722 4,12 C4,16.418278 7.581722,20 12,20 C16.418278,20 20,16.418278 20,12 C20,9.87826808 19.1571453,7.84343678 17.6568542,6.34314575 C16.1565632,4.84285472 14.1217319,4 12,4 Z" fill="#FF6E6E" fill-rule="nonzero"> </path> </g> </g> </g> </g> </g> </g></svg>
            `

            const bin = `
                <svg viewBox="-4.5 0 35 35" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#000000"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <!-- Uploaded to: SVG Repo, www.svgrepo.com, Generator: SVG Repo Mixer Tools --> <title>trash</title> <desc>Created with Sketch.</desc> <defs> </defs> <g id="Vivid.JS" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"> <g id="Vivid-Icons" transform="translate(-48.000000, -722.000000)"> <g id="Icons" transform="translate(37.000000, 169.000000)"> <g id="trash" transform="translate(0.000000, 546.000000)"> <g transform="translate(11.000000, 7.000000)"> <rect id="Rectangle-path" fill="#FF6E6E" fill-rule="nonzero" x="2" y="9" width="22" height="26"> </rect> <path d="M7.969,0 L17.938,0 L22,4 L4,4 L7.969,0 Z M0,4 L26,4 L26,9 L0,9 L0,4 Z M7,13 L11,13 L11,30 L7,30 L7,13 Z M15,13 L19,13 L19,30 L15,30 L15,13 Z" id="Shape" fill="#0C0058"> </path> </g> </g> </g> </g> </g> </g></svg>
            `

            const icons = [circles,magnify,bin]


            function optionTemplate(x,index) {
                return `
                    <span tabIndex="2" class="child-option flex-h">
                        <div class="w-20 grid">${icons[index]}</div>
                        <span class="f-g">${x.innerText}</span>
                    </span>
                `
            }
            
            const selectContainer = document.createElement('div')
            let button = {}
            let childOptions = {}
            function init(){
                const template = `
                    <div class="modern-select flex-v gap-1">
                        <button type="button">
                            <span class="selectedText flex-h">
                                <div class="w-20">${icons[select.selectedIndex]}</div>
                                <span class="f-g">${select.value}</span>
                            </span>
                            <b class="chevron"></b>
                        </button>
                        <div class="grid rel">
                            <div class="drop-down abs flex-v">
                                ${options().map(optionTemplate).join('\n')}
                            </div>
                        </div>
                    </div>
                    <i class="focus-next" tabIndex="2"></i>

                `
                selectContainer.innerHTML = template
                select.parentNode.insertBefore(selectContainer,select)
                button = selectContainer.querySelector('button')
                childOptions = selectContainer.querySelectorAll('.child-option')
                const selectedText = selectContainer.querySelector('.selectedText')
                const focusNext = selectContainer.querySelector('.focus-next')
                childOptions.forEach((o,index) => o.addEventListener("click",_=>{
                    select.selectedIndex = index
                    selectedText.innerHTML = o.innerHTML
                    focusNext.focus()
                }))
            }

            var observer = new MutationObserver(function(mutations) {
                mutations.forEach(function(mutation) {
                    if (mutation.type === "attributes") {
                        console.log("attributes changed");

                        // Example of accessing the element for which 
                        // event was triggered
                        //mutation.target.textContent = "Attribute of the element changed";
                        button.disabled = mutation.target.disabled
                    }
                    if (mutation.type === "childList") {
                        if(select.options.length != childOptions.length){
                            console.log('?????????????')
                            init()
                        }
                    }
                        
                    console.log(mutation.target);
                });
            });

            observer.observe(select, {
                attributes: true //configure it to listen to attribute changes
                ,childList: true
            });
            
            init()
    })
}