const source = document.querySelector('[name="source"]');
const target = document.querySelector('[name="target"]');
const expected = document.querySelector('[name="expected"]');

const sampleSource = `
main(){
  print("hello world !");
}
`;
const expectedTarget = `
TOKEN_FUNC: main
TOKEN_OPPR: (
TOKEN_CLPR: )
TOKEN_OPCL: {
TOKEN_FCAL: print
TOKEN_OPPR: (
TOKEN_STRG: hello world !
TOKEN_CLPR: )
TOKEN_SMCL: ;
TOKEN_CLCL: }
`;


/*
read the source one character at a time
and build abstract syntax tree (AST)
*/

const TOKEN_FUNC = "TOKEN_FUNC";
const TOKEN_OPPR = "TOKEN_OPPR";
const TOKEN_CLPR = "TOKEN_CLPR";
const TOKEN_OPCL = "TOKEN_OPCL";
const TOKEN_FCAL = "TOKEN_FCAL";
const TOKEN_STRG = "TOKEN_STRG";
const TOKEN_SMCL = "TOKEN_SMCL";
const TOKEN_CLCL = "TOKEN_CLCL";
const EMPTY = "";

class Token {
    #type
    #row
    #col
    #value

    constructor(type, row, col, value) {
        this.#type = type;
        this.#row = row;
        this.#col = col;
        this.#value = value;
    }

    get type() { return this.#type; }
    get row() { return this.#row; }
    get col() { return this.#col; }
    get value() { return this.#value; }

    display() {
        return `${this.#type}: ${this.#value}`;
    }
}

const Characters = {
    isAlpha(chr) {
        if (!chr || typeof chr !== "string" || chr.trim().length !== 1) return false;
        const code = chr.toLowerCase().charCodeAt(0);
        return code >= 97 && code <= 122;
    },
    isNumber(chr) {
        if (!chr || typeof chr !== "string" || chr.trim().length !== 1) return false;
        const code = chr.toLowerCase().charCodeAt(0);
        return code >= 97 && code <= 122;
    },
    isAlphaNumeric(chr) {
        return this.isAlpha(chr) || this.isNumber(chr);
    },
    isSpace(chr) {
        if (!chr || typeof chr !== "string") return false;
        return chr.trim().length === 1;
    }
};

class Lexer { 
    #currentIndex
    #source
    #row
    #col
    
    constructor(source) {
        this.#source = source;
        this.#currentIndex = -1;
        this.#row = 1;
        this.#col = 1;
    }

    #next() {
        let chr = EMPTY;
        if (this.#done()) return chr;
        chr = this.#source[this.#currentIndex++];
        this.#col++;
        if (chr === "\n") {
            this.#row++;
            this.#col = 1;
        }
        return chr;
    }

    #trimLeft() {
        let retry = 1000;
        while (!this.#done() && Characters.isSpace(this.#next()) && --retry > 0);
    }

    #done() {
        return this.#currentIndex >= this.#source.length;
    }

    

    getTokens() {
        const tokens = [];
        // console.log("1/got here !!");
        if (!this.#source || this.#done()) return tokens;
        this.#trimLeft();
        // console.log("2/got here !!");
        let retry = 1000;
        while (!this.#done() && --retry > 0) {
            const index = this.#currentIndex;
            const first = this.#next();
            if (Characters.isAlpha(first)) {
                //reading the name of the function 
                let xretry = 1000;
                while (!this.#done() && Characters.isAlphaNumeric(this.#next()) && --xretry > 0);
                this.#currentIndex -= 1;
                const value = this.#source.substring(index, this.#currentIndex);
                const type = tokens.length === 0
                          || tokens[tokens.length - 1].type === TOKEN_CLCL
                            ? TOKEN_FUNC
                            : TOKEN_FCAL;
                tokens.push(new Token(type, this.#row, this.#col, value));
                continue;
            }
            if (first === "(") {
                tokens.push(new Token(TOKEN_OPPR, this.#row, this.#col, first));
                continue;
            }
            if (first === ")") {
                tokens.push(new Token(TOKEN_CLPR, this.#row, this.#col, first));
                continue;
            }
            if (first === "{") {
                tokens.push(new Token(TOKEN_OPCL, this.#row, this.#col, first));
                continue;
            }
            if (first === "}") {
                tokens.push(new Token(TOKEN_CLCL, this.#row, this.#col, first));
                continue;
            }
            if (first === ";") {
                tokens.push(new Token(TOKEN_SMCL, this.#row, this.#col, first));
                continue;
            }

            if (first === '"') {
                let xretry = 1000;
                while (!this.#done() && this.#next() !== '"' && --xretry>0);
                const value = this.#source.substring(index+1, this.#currentIndex - 1);
                tokens.push(new Token(TOKEN_STRG, this.#row, this.#col, value));
                continue;
            }

        }
        return tokens;
    }
}


document.addEventListener("DOMContentLoaded", _ => { 
    source.value = sampleSource.trim();
    expected.value = expectedTarget.trim();
    function onChange() {
        target.value = new Lexer(source.value).getTokens().map(x => x.display()).join("\n");
    }
    onChange();
    source.addEventListener("input", onChange);
});