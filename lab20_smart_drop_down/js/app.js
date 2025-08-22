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
        el.value = "";
        const filtered = optionValues.filter(x => x.text.containsInvariant(e.target.value));
        build(filtered);
        if(filtered.length === 1){
            el.value = filtered[0].value;
            fakeText.value = filtered[0].text;
        }
        showCurrentSelection();
    });

    //TODO: add key up/down (cyclic) to select options
    


    fakeText.addEventListener("change", e => {
        el.value = "";
        const filtered = optionValues.filter(x => x.text.containsInvariant(e.target.value));
        if(filtered.length !== 1) {
            e.target.value = "";
            build(optionValues);
            return;
        }
        e.target.value = filtered[0].text;
        el.value = filtered[0].value;   
        el.dataset.selected = 1;
    });
    
    fakeText.addEventListener("focus",_=>{ 
        el.dataset.selected=0; 
        showCurrentSelection();
    });

    fakeText.addEventListener("blur",_=>{
        for(let i=0;i<optionValues.length;++i){
            if(optionValues[i].value === el.value) {
                fakeText.value = optionValues[i].text;
                break;
            }
        }
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
        showCurrentSelection();
    }

    function sort() { 
        //sorting by display text
        optionValues.sort((a,b)=>{
            return a.text.toLowerCase().localeCompare(b.text.toLowerCase());
        });
    }
    function addOption(v,t){
        optionValues.push(new OptionValue(v,t.trim(),formatText(t)));
        return this;
    }

    function setOptions(listOfOptions) {
        if(!listOfOptions || typeof listOfOptions !== "object" || !Array.isArray(listOfOptions)) return;
        for(let i=0;i<listOfOptions.length;++i){
            addOption(listOfOptions[i][0], listOfOptions[i][1]);
        }
    }

    //TODO: show current selected option highlighted
    function showCurrentSelection(){
        options.querySelectorAll(`.smart-option-value`).forEach((o,idx) => {
            if(o.value === el.value) o.classList.add("active");
            else o.classList.remove("active");
        })
    }

    

    return {addOption,setOptions,build,formatText,sort};
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

const listOfOptions = [
    ["Riyadh"  , "RUH | Riyadh   | الرياض "],
    ["Dammam"  , "DMM | Dammam   | الدمام "],
    ["Jeddah"  , "JED | Jeddah   | جدة    "],
    ["Hofuf"   , "HOF | Hofuf    | الهفوف "],
    ["Hail"    , "HAL | Hail     | حائل   "],
    ["Makka"   , "MAK | Makka    | مكة    "],
    ["Tabuk"   , "TBK | Tabuk    | تبوك   "],
    ["Arar"    , "ARR | Arar     | عرعر   "],
    ["Qatif"   , "QTF | Qatif    | القطيف "],
    ["Dhahran" , "DHR | Dhahran  | الظهران"],
    ["Jaizan"  , "JZN | Jaizan   | جيزان  "],
    ["Qasim"   , "QSM | Qasim    | القصيم "],
    ["Khobar"  , "KHB | Khobar   | الخبر  "],
    ["Bqaiq"   , "BAQ | Bqaiq    | بقيق   "],
];

document.querySelector('button[type="button"]').addEventListener("click",onButtonClick);
const input = document.querySelector('input.smart');
const sdd = new SmartDropDown(input, x => `<td>${x.split(/[|]/g).join('</td><td>')}</td>`);
sdd.setOptions(listOfOptions);
sdd.sort();
sdd.build();