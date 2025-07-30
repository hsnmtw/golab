const canvas = document.querySelector('canvas');
const ctx = canvas.getContext("2d");
ctx.fillStyle = "#181818ff";

const CANVAS_WIDTH = 220;
const CANVAS_HEIGHT = 120;
const WIDTH = 10;
const HEIGHT = 10;
const DELAY = 10;


const COLORS = [
    '#000080',
    '#808000',
    '#6B8E23',
    '#FFA500',
    '#FF4500',
    '#DA70D6',
    '#EEE8AA',
    '#98FB98',
    '#AFEEEE',
    '#DB7093',
    '#CD853F',
    '#FFC0CB',
    '#DDA0DD',
    '#B0E0E6',
    '#800080',
    '#663399',
    '#FF0000',
    '#BC8F8F',
    '#4169E1',
    '#8B4513',
    '#FA8072',
    '#F4A460',
    '#2E8B57',
    '#FFF5EE',
    '#A0522D',
    '#C0C0C0',
    '#87CEEB',
    '#6A5ACD',
    '#708090',
    '#708090',
    '#00FF7F',
    '#4682B4',
    '#D2B48C',
    '#008080',
    '#D8BFD8',
    '#FF6347',
    '#40E0D0',
    '#EE82EE',
    '#F5DEB3',
    '#FFFF00',
    '#9ACD32',
];


class Box { 
    x = 0
    y = 0
    dx = 1
    dy = 1
    w = WIDTH
    h = HEIGHT
    color = "red"
    constructor(x,y,w,h,color) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.color = color;
        if (x + w + 1 > CANVAS_WIDTH) this.x = 0;
        if (y + h + 1 > CANVAS_HEIGHT) this.y = 0;
    }
    #isOffBorder() {
        if (this.x + this.dx < 0 || this.x + this.dx + this.w > CANVAS_WIDTH) this.dx *= -1;
        if (this.y + this.dy < 0 || this.y + this.dy + this.h > CANVAS_HEIGHT) this.dy *= -1;        
    }
    move() {
        this.#isOffBorder();
        this.x += this.dx;
        this.y += this.dy;    
        drawRect(this.x, this.y, this.w, this.h, this.color);
    }
}

function drawRect(x, y, w, h, color) {
    ctx.fillStyle = color;
    ctx.fillRect(x, y, w, h);
}

function beginDrawing() {
    ctx.fillStyle = "#181818ff";
    ctx.fillRect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
}

function endDrawing() {
    
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function randomBox() {
    const x = getRandomInt(10,CANVAS_WIDTH/2);
    const y = getRandomInt(10,CANVAS_WIDTH/2);
    const w = getRandomInt(10,CANVAS_WIDTH/9);
    const color = COLORS[ getRandomInt(0,COLORS.length*2) % COLORS.length ];
    return new Box(x,y,w,w,color);
}

async function main() {
    let boxes = [0,0,0,0,0,0,0,0,0,0,0,1,2,3,4,5,6,7,8,9,10].map(randomBox);
    for (; ;) {
        beginDrawing();
        //drawRect(box.x, box.y, "red");
        boxes.forEach(x=>x.move());
        endDrawing();   
        await new Promise(r => setTimeout(r, DELAY));
    }
}

main();