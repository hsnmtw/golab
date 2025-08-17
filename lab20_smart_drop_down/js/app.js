

document.querySelectorAll('input.smart').forEach(makeItSmart);

function OptionValue(val,txt){
    this.value = val;
    this.text = txt ?? val;
}

String.prototype.contains = function(other){
    return this.indexOf(other) > -1;
}

function StringComparerContain(a,b){
    if(!a || !b) return 0;
    if(typeof a !== "string" || typeof b !== "string") return 0;
    return a.trim().toLowerCase().contains(b.trim().toLowerCase()) ? 0 : 1;
}

function makeItSmart(el){
    const optionValues = el.dataset.options.split(',').map(x=>new OptionValue(x));

    const fakeText = document.createElement('input');
    const container = document.createElement('div');
    const wrapper = document.createElement('div');
    const options = document.createElement('div');
    const tmp = document.createElement('i');
    tmp.tabIndex = 2;
    // tmp.style.display = "none";
    el.type = "hidden";
    el.parentNode.insertBefore(container, el);
    document.body.append(tmp);
    container.append(fakeText);
    container.append(el);
    container.className = "smart-container";
    wrapper.className = "smart-wrapper";
    options.className = "smart-options";

    container.append(wrapper);
    wrapper.append(options);

    function onOptionSelect(e){
        fakeText.value = e.target.dataset.value;
        el.value = e.target.textContent.trim();
        tmp.focus()
    }

    function buildDropDown(ov){
        options.innerHTML = `
            ${ov.map(x => `<div tabIndex="1" data-value="${x.value}" class="smart-option-value">${x.text}</div>`).join("")}
        `;
        options.querySelectorAll('.smart-option-value').forEach(o => o.addEventListener("click", onOptionSelect));
    }

    fakeText.addEventListener("input", onTextInput);
    fakeText.addEventListener("change",onTextChange)

    function onTextInput(e){
        const filtered = optionValues.filter(x => StringComparerContain(x.text, e.target.value) == 0 );
        buildDropDown(filtered);
    }

    function onTextChange(e){
        const filtered = optionValues.filter(x => StringComparerContain(x.text, e.target.value) == 0 );
        if(filtered.length !== 1) {
            e.target.value = "";
            return;
        }
        e.target.value = filtered[0].text;
        el.value = filtered[0].value;
    }


    buildDropDown(optionValues);
}

document.querySelector('button[type="button"]').addEventListener("click",onButtonClick);

function onButtonClick(e){
    document.querySelectorAll('pre').forEach(p => p.innerHTML = JSON.stringify(form2object(document.querySelector("form")),0,2))
}

function form2object(form){
    const data = {};
    form.querySelectorAll('input,textarea,select').forEach(x => {
        if(x.name)
            data[x.name] = x.value;
    });
    return data;
}