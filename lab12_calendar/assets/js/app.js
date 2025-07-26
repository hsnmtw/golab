(function () {
    function initCalendar(date) {
        if (!date)                   return console.error("ERROR [initCalendar]: null date");
        if (!(date instanceof Date)) return console.error("ERROR [initCalendar]: date is expected, got ", date);
        
        const [y, m, d] = date.toISOString().split('T')[0].split('-').map(x => +x);
        
        const numberOfDays = getNumberOfDays(y, m);

        const div = document.createElement('div');
        const weekdays = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
        const iso = formatDate(y, m, 1);
        const firstDayOfMonth = new Date(iso).getDay();
        div.innerHTML = `
            <div class="absolute drop-down-calendar">
                <div class="month-head">${m} / ${y}</div>
                <div class="grid-7">
                    ${weekdays.map(x => `<span class="weekday">${x}</span>`).join('')}
                    ${range(0, firstDayOfMonth - 1).map(_ => '<i></i>').join('')}
                    ${range(1, numberOfDays).map(x => `<span tabIndex="1" data-date="${formatDate(y,m,x)}">${x}</span>`).join('')}
                </div>
            </div>
        `;
        div.className = "relative";
        const onDateSelected = e => { 
            if (!e.target.dataset.date) return;
            const data = { "detail": e.target.dataset.date };
            const event = new CustomEvent("onDateSelected", data);
            div.dispatchEvent(event);
            e.stopPropagation();
        };
        div.querySelectorAll("[data-date]").forEach(x => x.addEventListener("click", onDateSelected));
        div.querySelectorAll(`[data-date="${date.toISOString().split('T')[0]}"]`).forEach(x => x.classList.add("selected"));
        return div;
    }

    function range(s, e) {
        if (typeof s !== "number") return console.error("ERROR [range]: bad start", s);
        if (typeof e !== "number") return console.error("ERROR [range]: bad end", e);
        if (e < s)                 return console.error("ERROR [range]: e must be greater than s in range", s, e);
        if (e - s > 31)            return console.error("ERROR [range]: too many elements in range", e - s);
        return s >= e ? [] : [s].concat(range(s + 1, e));
    }

    function formatDate(y, m, d) {
        if (y < 1900 || y > 2050)              return console.error("ERROR [formatDate]: wrong year");
        if (m < 1 || m > 12)                   return console.error("ERROR [formatDate]: wrong month");
        if (d < 1 || d > getNumberOfDays(y,m)) return console.error("ERROR [formatDate]: wrong day");
        return `${new String(y).padStart(4, 0)}-${new String(m).padStart(2, 0)}-${new String(d).padStart(2, 0)}`;
    }

    function getNumberOfDays(y, m) {
        if (typeof m !== "number") return console.error("ERROR [getNumberOfDays]: m must be a number from 1 to 12");
        if (typeof y !== "number") return console.error("ERROR [getNumberOfDays]: y must be a number from 1 to 12");
        if (m > 12 || m < 1)       return console.error("ERROR [getNumberOfDays]: month is wrong", m);
        if (y > 2050 || y < 1900)  return console.error("ERROR [getNumberOfDays]: year is wrong", y);
        return [
            31, //1
            y % 4 === 0 ? 29 : 28, //2
            31, //3
            30, //4
            31, //5
            30, //6
            31, //7
            31, //8
            30, //9
            31, //10
            30, //11
            31, //12
        ][m - 1];
    }   

    function addCalendarDropDown(selector) {
        if (!selector) return console.error("ERROR [addCalendarDropDown]: null selector");
        if (typeof selector === "string") return document.querySelectorAll(selector).forEach(x => addCalendarDropDown(x));
        if (selector instanceof NodeList || (typeof selector === "object" && Array.isArray(selector)))
            return selector.forEach(x => addCalendarDropDown(x));
        if (!(selector instanceof Element || selector instanceof HTMLElement))
            return console.error("ERROR [addCalendarDropDown]: BAD selector");
        
        const container = document.createElement("div");
        const calendarView = document.createElement("div");
        let calendar = initCalendar(new Date());
        calendarView.append(calendar)
        
        function onInputFocus() {
            if (selector.value !== "") {
                const [y, m, d] = selector.value.split('T')[0].split('-').map(Number);
                const dateFormatted = formatDate(y, m, d);
                if (dateFormatted == selector.value) {
                    calendar = initCalendar(new Date(dateFormatted));
                    calendarView.innerHTML = "";
                    calendarView.append(calendar);
                }
            }
        }
        selector.addEventListener("focus", onInputFocus);
        
        selector.parentNode.insertBefore(container, selector);
        container.className = "flex-col gap-1 calendar-container";
        container.append(selector);
        container.append(calendarView);

        calendar.addEventListener("onDateSelected", e => { 
            selector.value = e.detail;
            document.activeElement.blur()
        });
    }

    window.initCalendar = initCalendar;

    window.addCalendarDropDown = addCalendarDropDown;

})();


document.addEventListener("DOMContentLoaded", _ => { 
    addCalendarDropDown("input");
});