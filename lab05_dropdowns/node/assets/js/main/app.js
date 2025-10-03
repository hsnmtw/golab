"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.query = query;
NodeList.prototype.toArray = function () {
    return Array.from(this);
};
function query(selector, callback) {
    if (!selector)
        return [];
    const empty = _ => { };
    return document.querySelectorAll(selector).toArray().map(callback ?? empty);
}
//document.querySelector('main')
//# sourceMappingURL=app.js.map