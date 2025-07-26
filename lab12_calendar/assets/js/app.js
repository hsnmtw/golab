const calendar = (function () {
    function initCalendar(date) {
        if (!date) return console.error("ERROR [initCalendar]: null date");
        if (!(date instanceof Date) || isNaN(date)) return console.error("ERROR [initCalendar]: date is expected, got ", date);
        
        const [y, m, d] = getDateParts(date);
        
        const numberOfDays = getNumberOfDays(y, m);

        const div = document.createElement('div');
        const weekdays = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
        const firstDayOfMonth = buildDate(y, m, 1).getDay();
        div.innerHTML = `
            <div class="absolute drop-down-calendar">
                <div class="grid-3">
                    <a tabIndex="1" data-prev-month href="javascript:void(0)">&lt;&lt;</a>
                    <div class="month-head">${m} / ${y}</div>
                    <a tabIndex="1" data-next-month href="javascript:void(0)">&gt;&gt;</a>
                </div>
                <div class="grid-7">
                    ${weekdays.map(x => `<span class="weekday">${x}</span>`).join('')}
                    ${range(0, firstDayOfMonth).map(_ => '<i></i>').join('')}
                    ${range(1, numberOfDays+1).map(x => `<span tabIndex="1" data-date="${formatDate(y,m,x)}">${x}</span>`).join('')}
                </div>
            </div>
        `;
        div.className = "relative";
        const onDateSelected = e => { 
            if (!e.target.dataset.date) return;
            div.dateValue = e.target.dataset.date;
            const data = { "detail": div.dateValue };
            const event = new CustomEvent("onDateSelected", data);
            div.dispatchEvent(event);
            e.stopPropagation();
        };
        div.querySelectorAll("[data-date]").forEach(x => x.addEventListener("click", onDateSelected));
        div.querySelectorAll(`[data-date="${date.toISOString().split('T')[0]}"]`).forEach(x => x.classList.add("selected"));
        div.querySelectorAll("[data-prev-month]").forEach(x => x.addEventListener("click",_=>div.dispatchEvent(new Event("onPrevMonth"))));
        div.querySelectorAll("[data-next-month]").forEach(x => x.addEventListener("click",_=>div.dispatchEvent(new Event("onNextMonth"))));
        div.dateValue = date.toISOString().split('T')[0];
        return div;
    }

    function range(s, e) {
        if (typeof s !== "number") return console.error("ERROR [range]: bad start", s);
        if (typeof e !== "number") return console.error("ERROR [range]: bad end", e);
        if (e < s)                 return console.error("ERROR [range]: e must be greater than s in range", s, e);
        if (e - s > 31)            return console.error("ERROR [range]: too many elements in range", e - s);
        return s >= e ? [] : [s].concat(range(s + 1, e));
    }

    function getDateParts(date) {
        if (!date)                   return console.error("ERROR [getDateParts]: date is null");
        if (!(date instanceof Date) || isNaN(date)) return console.error("ERROR [getDateParts]: date is not of correct data type", date);
        return date.toISOString().split('T')[0].split('-').map(Number);
    }

    function buildDate(y, m, d) {
        if (!y || isNaN(y)) return console.error("ERROR [buildDate]: year is NaN or NULL",y);
        if (!m || isNaN(m)) return console.error("ERROR [buildDate]: month is NaN or NULL",m);
        if (!d || isNaN(d)) return console.error("ERROR [buildDate]: day is NaN or NULL",d);
        if (y < 1900 || y > 2050) return console.error("ERROR [buildDate]: wrong year",y);
        if (m < 1 || m > 12)                   return console.error("ERROR [buildDate]: wrong month",m);
        if (d < 1 || d > getNumberOfDays(y,m)) return console.error("ERROR [buildDate]: wrong day",d);
        return new Date(formatDate(y, m, d));
    }

    function formatDate(y, m, d) {
        if (!y || isNaN(y)) return console.error("ERROR [formatDate]: year is NaN or NULL",y);
        if (!m || isNaN(m)) return console.error("ERROR [formatDate]: month is NaN or NULL",m);
        if (!d || isNaN(d)) return console.error("ERROR [formatDate]: day is NaN or NULL",d);
        if (y < 1900 || y > 2050)              return console.error("ERROR [formatDate]: wrong year",y);
        if (m < 1 || m > 12)                   return console.error("ERROR [formatDate]: wrong month",m);
        if (d < 1 || d > getNumberOfDays(y,m)) return console.error("ERROR [formatDate]: wrong day",d);
        return `${new String(y).padStart(4, 0)}-${new String(m).padStart(2, 0)}-${new String(d).padStart(2, 0)}`;
    }

    function getNumberOfDays(y, m) {
        if (typeof m !== "number") return console.error("ERROR [getNumberOfDays]: m must be a number from 1 to 12",m);
        if (typeof y !== "number") return console.error("ERROR [getNumberOfDays]: y must be a number from 1 to 12",y);
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

    function addMonths(date, n) {
        if (!(date instanceof Date) || isNaN(date)) return console.error("ERROR [addMonths]: date is not of correct data type", date);
        if (!date)                                  return console.error("ERROR [addMonths]: date is null", date);
        if (!n || isNaN(n))                         return console.error("ERROR [addMonths]: n is null / NaN", n);
        if (typeof n !== "number")                  return console.error("ERROR [addMonths]: n must be numeric",n);
        n = parseInt(n);
        if (n === 0) return date; // dont bother
        let [y, m, d] = getDateParts(date);
        const delta = n > 0 ? 1 : -1;
        for (let i = 0; i < Math.abs(n); i++){
            m += delta;
            if (m > 12) {
                m = 1;
                y++;
            } else if (m < 1) {
                m = 12;
                y--;
            }
            while (d>28 && d > getNumberOfDays(y, m)) { --d; }
        }
        return buildDate(y, m, d);
    }

    function addCalendarDropDown(selector) {
        if (!selector) return console.error("ERROR [addCalendarDropDown]: null selector");
        if (typeof selector === "string") return document.querySelectorAll(selector).forEach(x => addCalendarDropDown(x));
        if (selector instanceof NodeList || (typeof selector === "object" && Array.isArray(selector)))
            return selector.forEach(x => addCalendarDropDown(x));
        if (!(selector instanceof Element || selector instanceof HTMLElement))
            return console.error("ERROR [addCalendarDropDown]: BAD selector", selector);
        
        const container = document.createElement("div");
        const calendarView = document.createElement("div");
        calendarView.tabIndex = 1;
        let calendar;

        const rebuildCalendarEvents = date => {
            calendar = initCalendar(date);
            calendar.addEventListener("onDateSelected", e => {
                selector.value = e.detail;
                document.activeElement.blur();
            });
            calendar.addEventListener("onPrevMonth", e => { 
                if (!calendar.dateValue) return;
                const date = addMonths(new Date(calendar.dateValue), -1);
                rebuildCalendarEvents(date);
                calendarView.focus();
            });
            calendar.addEventListener("onNextMonth", e => {
                if (!calendar.dateValue) return;                
                const date = addMonths(new Date(calendar.dateValue), +1);
                rebuildCalendarEvents(date);
                calendarView.focus();
            });
            calendarView.innerHTML = "";
            calendarView.append(calendar);
        };

        function onInputFocus() {
            if (selector.value !== "") {
                const [y, m, d] = selector.value.split('T')[0].split('-').map(Number);
                const dateFormatted = formatDate(y, m, d);
                const date = new Date(dateFormatted);
                if (dateFormatted == selector.value && date && date instanceof Date && !isNaN(date)) {
                    rebuildCalendarEvents(date);
                }
            }
        }

        selector.addEventListener("focus", onInputFocus);
        
        selector.parentNode.insertBefore(container, selector);
        container.className = "flex-col gap-1 calendar-container";
        container.append(selector);
        container.append(calendarView);
        rebuildCalendarEvents(new Date());
    }

    return { initCalendar, addCalendarDropDown, addMonths };

})();


document.addEventListener("DOMContentLoaded", _ => { 
    calendar.addCalendarDropDown("input");
});