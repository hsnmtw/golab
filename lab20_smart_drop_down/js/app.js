function OptionValue(val,txt){
    this.value = val;
    this.text = txt ?? val;
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


    fakeText.addEventListener("change", e => {
        const filtered = optionValues.filter(x => x.text.containsInvariant(e.target.value));
        if(filtered.length !== 1) {
            e.target.value = "";
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

    
    function optionTemplate(v,t){
        return `
            <tr tabIndex="1" 
                 data-value="${v}" 
                 class="smart-option-value">${t}</tr>
        `;
    }

    function formatText(t){
        if(formatter && typeof formatter === "function") return formatter(t);
        return t;
    }

    function build(ov){
        ov ??= optionValues ?? [];
        options.innerHTML = "<table>"+ov.map(x=>optionTemplate(x.value,x.text)).join("")+"</table>";
        options.querySelectorAll('.smart-option-value').forEach(o => o.addEventListener("click", e => {
            fakeText.value = e.target.textContent.trim();
            el.value = e.target.dataset.value;
            el.dataset.selected=1;
        }));
    }

    //TODO: sort items 
    function addOption(v,t){
        optionValues.push(new OptionValue(v,formatText(t)));
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
const sdd = new SmartDropDown(input, x => `<td>${x.en}</td><td>${x.ar}</td>`)
        .addOption("Riyadh"  , { en: "Riyadh" , ar: "الرياض"  })
        .addOption("Dammam"  , { en: "Dammam" , ar: "الدمام"  })
        .addOption("Jeddah"  , { en: "Jeddah" , ar: "جدة"     })
        .addOption("Hofuf"   , { en: "Hofuf"  , ar: "الهفوف"  })
        .addOption("Hail"    , { en: "Hail"   , ar: "حائل"    })
        .addOption("Makka"   , { en: "Makka"  , ar: "مكة"     })
        .addOption("Tabuk"   , { en: "Tabuk"  , ar: "تبوك"    })
        .addOption("Arar"    , { en: "Arar"   , ar: "عرعر"    })
        .addOption("Qatif"   , { en: "Qatif"  , ar: "القطيف"  })
        .addOption("Dhahran" , { en: "Dhahran", ar: "الظهران" })
        .addOption("Jaizan"  , { en: "Jaizan" , ar: "جيزان"   })
        .addOption("Qasim"   , { en: "Qasim"  , ar: "القصيم"  })
        .addOption("Khobar"  , { en: "Khobar" , ar: "الخبر"   })
        .addOption("Bqaiq"   , { en: "Bqaiq"  , ar: "بقيق"    })
        .build();