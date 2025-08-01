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
    '#DB7093',
    '#CD853F',
    '#800080',
    '#663399',
    '#FF0000',
    '#BC8F8F',
    '#4169E1',
    '#8B4513',
    '#FA8072',
    '#008080',
    '#D8BFD8',
    '#FF6347',
    '#40E0D0',
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
    
    collide(box) {
        const _x = this.x + this.dx;
        const _y = this.y + this.dy;
        if ((_x > box.x && _x < box.x + box.w) && (_y > box.y && _y < box.y + box.h)) {
            this.dx *= -1;
            this.dy *= -1;
            box.dx *= -1;
            box.dy *= -1;
            return true;
        }
        return false;
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

function random(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function randomBox() {
    const w = random(10,CANVAS_WIDTH/9);
    const x = random(10,CANVAS_WIDTH - w);
    const y = random(10,CANVAS_WIDTH - w);
    const color = COLORS[ random(0,COLORS.length-1) ];
    return new Box(x,y,w,w,color);
}

async function main() {
    let boxes = [1,2,3,4,2,3,4,2,3,4,2,3,4,2,3,4].map(randomBox);
    for (; ;) {
        beginDrawing();
        boxes.forEach(box => { 
            box.move();
            for(let other of boxes){
                box !== other && box.collide(other);
            }
        });
        endDrawing();   
        await new Promise(r => setTimeout(r, DELAY));
    }
}

main();