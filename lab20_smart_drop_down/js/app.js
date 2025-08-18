function OptionValue(val,txt,dsply){
    this.value = val;
    this.text = txt ?? val;
    this.display = dsply ?? txt ?? val;
}

function SmartDropDown(el, formatter) {
    let optionValues = [];

    const container = document.createElement('div');
    const wrapper = document.createElement('div');
    const options = document.createElement('div');
    const fakeText = document.createElement('input');
    // tmp.style.display = "none";
    el.type = "hidden";
    el.parentNode.insertBefore(container, el);
    el.dataset.selected = 0;     

    container.append(fakeText);
    container.append(el);
    container.className = "smart-container";
    wrapper.className = "smart-wrapper";
    options.className = "smart-options";

    container.append(wrapper);
    wrapper.append(options);

    //TODO: hide vs rebuild
    fakeText.addEventListener("input", e => {
        const filtered = optionValues.filter(x => x.text.containsInvariant(e.target.value));
        build(filtered);
    });

    //TODO: add key up/down (cyclic) to select options
    //TODO: show current selected option highlighted


    fakeText.addEventListener("change", e => {
        const filtered = optionValues.filter(x => x.text.containsInvariant(e.target.value));
        if(filtered.length !== 1) {
            e.target.value = "";
            build(optionValues);
            return;
        }
        e.target.value = filtered[0].text;
        el.value = filtered[0].value;   
        el.dataset.selected = 1;
        console.log("change",el.dataset.selected)
    });
    
    fakeText.addEventListener("focus",_=>{ 
        el.dataset.selected=0; 
        console.log("focus",el.dataset.selected);
    });

    
    function optionTemplate(ov){
        return `
            <tr tabIndex="1" 
                 data-value="${ov.value}" 
                 data-text="${ov.text}"
                 class="smart-option-value">${ov.display}</tr>
        `;
    }

    function formatText(t){
        if(formatter && typeof formatter === "function") return formatter(t);
        return t;
    }

    function build(ov){
        ov ??= optionValues ?? [];
        options.innerHTML = "<table>"+ov.map(optionTemplate).join("")+"</table>";
        options.querySelectorAll('.smart-option-value').forEach(o => o.addEventListener("click", e => {
            fakeText.value = o.dataset.text.trim();
            el.value = o.dataset.value.trim();
            el.dataset.selected=1;
        }));
    }

    //TODO: sort items 
    function addOption(v,t){
        optionValues.push(new OptionValue(v,t,formatText(t)));
        return this;
    }

    return {addOption,build,formatText};
}

String.prototype.contains = function(other){
    return this.indexOf(other) > -1;
}

String.prototype.containsInvariant = function(other){
    return this,this.toLowerCase().indexOf(other.toLowerCase()) > -1;
}


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

document.querySelector('button[type="button"]').addEventListener("click",onButtonClick);
const input = document.querySelector('input.smart');
const sdd = new SmartDropDown(input, x => `<td>${x.split(/[|]/g).join('</td><td>')}</td>`)
        .addOption("Riyadh"  , "RUH | Riyadh   | الرياض ")
        .addOption("Dammam"  , "DMM | Dammam   | الدمام ")
        .addOption("Jeddah"  , "JED | Jeddah   | جدة    ")
        .addOption("Hofuf"   , "HOF | Hofuf    | الهفوف ")
        .addOption("Hail"    , "HAL | Hail     | حائل   ")
        .addOption("Makka"   , "MAK | Makka    | مكة    ")
        .addOption("Tabuk"   , "TBK | Tabuk    | تبوك   ")
        .addOption("Arar"    , "ARR | Arar     | عرعر   ")
        .addOption("Qatif"   , "QTF | Qatif    | القطيف ")
        .addOption("Dhahran" , "DHR | Dhahran  | الظهران")
        .addOption("Jaizan"  , "JZN | Jaizan   | جيزان  ")
        .addOption("Qasim"   , "QSM | Qasim    | القصيم ")
        .addOption("Khobar"  , "KHB | Khobar   | الخبر  ")
        .addOption("Bqaiq"   , "BAQ | Bqaiq    | بقيق   ")
        .build();