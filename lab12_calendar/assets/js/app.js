(function () {
    function initCalendar(date) {
        if (!date) return console.error("null date");
        if (!(date instanceof Date)) return console.error("date is expected, got ", date);
        
        const [y, m, d] = date.toISOString().split('T')[0].split('-').map(x => +x);
        console.log({ y, m, d });
        const numberOfDays = getNumberOfDays(y, m);

        const div = document.createElement('div');
        const weekdays = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
        const iso = `${new String(y).padStart(4, 0)}-${new String(m).padStart(2, 0)}-01`;
        const firstDayOfMonth = new Date(iso).getDay();
        div.innerHTML = `
            <center><h1>${m} / ${y}</h1></center>
            <div class="grid-7">
                ${weekdays.map(x => `<span>${x}</span>`).join('')}
                ${range(0, firstDayOfMonth - 1).map(_ => '<i></i>').join('')}
                ${range(1, numberOfDays).map(x => `<span>${x}</span>`).join('')}
            </div>
        `;
        div.className = "drop-down-calendar absolute";
        return div;
    }

    function range(s, e) {
        return s > e ? [] : [s].concat(range(s + 1, e));
    }

    function getNumberOfDays(y, m) {
        if (typeof m !== "number") return console.error("m must be a number from 1 to 12");
        if (typeof y !== "number") return console.error("y must be a number from 1 to 12");
        if (m > 12 || m < 1) return console.error("month is wrong", m);
        if (y > 2050 || y < 1900) return console.error("year is wrong", y);
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
        if (!selector) return console.error("null selector");
        if (typeof selector === "string") return document.querySelectorAll(selector).forEach(x => addCalendarDropDown(x));
        if (selector instanceof NodeList || (typeof selector === "object" && Array.isArray(selector)))
            return selector.forEach(x => addCalendarDropDown(x));
        if (!(selector instanceof Element || selector instanceof HTMLElement))
            return console.error("BAD selector");
        
        function onInputFocus() {
            
        }

        selector.addEventListener("focus", onInputFocus);
        const container = document.createElement("div");
        selector.parentNode.insertBefore(container, selector);
        container.className = "flex-col calendar-container";
        container.append(selector);
        container.append(initCalendar(new Date()))

    }

    window.initCalendar = initCalendar;

    window.addCalendarDropDown = addCalendarDropDown;

})();


document.addEventListener("DOMContentLoaded", _ => { 
    addCalendarDropDown("input");
});