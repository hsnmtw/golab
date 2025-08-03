const canvas = document.querySelector('canvas');
const ctx = canvas.getContext("2d");
ctx.fillStyle = "#181818ff";

const CANVAS_WIDTH = 220;
const CANVAS_HEIGHT = 120;
const WIDTH = 10;
const HEIGHT = 10;
const DELAY = 5;


const COLORS = [
    '#FFC0CB',
    '#DDA0DD',
    '#9932CC',
    '#483D8B',
    '#FA8072',
    '#FF0000',
    '#FF8C00',
    '#FFFF00',
    '#FFDAB9',
    '#00FF00',
	'#008000',
	'#008B8B',
	'#00FFFF',
	'#48D1CC',
	'#ADD8E6',
	'#5F9EA0',
	'#0000CD',
	'#FFEBCD',
	'#D2B48C',
	'#808000',
	'#A52A2A',
	'#C0C0C0',
	'#778899',
	'#2F4F4F',
	'#FFFFFF',
	'#FFccde',
	'#EDCF12',
];


class Box { 
    #id = 0
    x = 0
    y = 0
    dx = 1
    dy = 1
    w = WIDTH
    h = HEIGHT
    color = "red"
	get id() { return this.#id; } 
    constructor(x,y,w,h,color) {
		this.#id = Math.random() * 3786328764 | 0;
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.color = color;
        if (x + w + 1 > CANVAS_WIDTH) this.x = 0;
        if (y + h + 1 > CANVAS_HEIGHT) this.y = 0;
    }
    #isOffBorder() {
        if (this.x + this.dx < 0 || this.x + this.dx + this.w > CANVAS_WIDTH)  this.dx *= -1;
        if (this.y + this.dy < 0 || this.y + this.dy + this.h > CANVAS_HEIGHT) this.dy *= -1;        
    }
    move() {
        this.#isOffBorder();
        this.x += this.dx;
        this.y += this.dy;    
        drawRect(this.x, this.y, this.w, this.h, this.color);
    }
    
	collide(box){
		
		//if (this.x + this.dx > box.x && this.x + this.dx + this.w < box.w) this.dx *= -1;
        //if (this.y + this.dy > box.y && this.y + this.dy + this.h < box.h) this.dy *= -1;  
         
		//[ ] [ ]
         
		if(this.x + this.w  + this.dx > box.x 
		&& this.y + this.h  + this.dy > box.y
        && this.y                     < box.y + box.h
		&& this.x                     < box.x + box.w) {
			
			
			
			this.dx *= -1;
			this.dy *= -1;
			box.dx  *= -1;
			box.dy  *= -1;
			
			box.color = COLORS[(Math.random()*9999 |0) % COLORS.length]; 
			
		}
	}
	
    _collide(box) {
        const _x = this.dx + this.x;
        const _y = this.dy + this.y;
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
	const m = 1//((100 * Math.random()) | 0);
	if(m > 0 && m < 33)
	{
		ctx.fillRect(x, y, w, h);
	}
	else if(m > 32 && m < 66){
		ctx.beginPath();
		ctx.arc(x,y,h,0,Math.PI*2,true);
		ctx.closePath();
		ctx.fill();

	}else{
		var path=new Path2D();
		path.moveTo(x,y);
		path.lineTo(x,   y-5);
		path.lineTo(x-5,y);
		path.lineTo(x,y);
		ctx.fill(path);				
	}
	
}

function beginDrawing() {
	ctx.fillStyle = "#181818ff";
	//ctx.fillStyle = COLORS[((Math.random()*10000)|0)%COLORS.length];
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
    let boxes = [
		1,2,3,4,5,6,7,8,9,0,
	].map(randomBox);
	
    for (let i = 0; i< 1_000_000 ;++i) {
        beginDrawing();
		for(let box of boxes){
			for(let other of boxes){
				box.id !== other.id && box.collide(other);
			}
            box.move();
			
		}
        endDrawing();   
        await new Promise(r => setTimeout(r, DELAY));
    }
}

main();