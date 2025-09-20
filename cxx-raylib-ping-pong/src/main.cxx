#include <iostream>
#include <raylib.h>

using namespace std;

bool BouncingBall(void);

const auto W = 800;
const auto H = 600;
const auto SZ = 15;
const auto FPS = 60;
const auto DARK_BLUE = Color { .r = 0x00, .g = 0x31, .b = 0x6E, .a = 0x00 };
const auto rcolor = Color { .r = 0xFF, .g = 0x50, .b = 0x50, .a = 0xFF };

int x = 100;
int y = 100;
int dx = 5;
int dy = 5;

int rx = 100;
int ry = H - 100;

typedef struct { int x; int y; bool alive; Color color; } Cell;

const int BW = 50;
int iota = 0;
Cell cells[] = {
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * ++iota, .y = 100, .alive = true, .color = GREEN },
    
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },
    Cell{ .x = 50 + BW * iota--, .y = 130, .alive = true, .color = GREEN },    
};

int cellsLength = sizeof(cells)/sizeof(Cell);

int main () {

    InitWindow(W, H, "Raylib Game Dev !");
    SetTargetFPS(FPS);
    while (!WindowShouldClose() && BouncingBall());
    CloseWindow();

    return 0;
}

bool CollideWith(int nx, int ny, int rx, int ry, int w, int h) {
    return (nx>=rx && ny>=ry && nx <= rx+w && ny <= ry+h);
}

bool CollideWithCell(int nx, int ny) {
    for(size_t i=0;i<cellsLength;++i) {
        if(!cells[i].alive) continue;
        int rx = cells[i].x;
        int ry = cells[i].y;
        const auto collide = CollideWith(nx,ny,rx,ry,BW - 10, 20);
        if(collide) {
            cells[i].alive = false;
            return true;
        }
    }
    return false;
}

bool BouncingBall(void) {
    x += dx;
    y += dy;

    int nx = x+dx;
    int ny = y+dy;

    const auto collide = CollideWith(nx,ny,rx,ry,100,20) 
                      || CollideWithCell(nx,ny);

    if(x >= W - SZ || x <= SZ) dx *= -1;
    if(y >= H - SZ || y <= SZ || collide) dy *= -1;
    
    BeginDrawing();
        ClearBackground(DARK_BLUE);
        DrawCircle(x,y,SZ, WHITE);
        DrawRectangle(rx,ry,100,20, rcolor);
        if(IsKeyUp(KEY_LEFT)  && rx < W - 100) rx += 10;
        if(IsKeyUp(KEY_RIGHT) && rx - 10 >= 0) rx -= 10;
        for(size_t i=0;i<cellsLength;++i) {
            if(!cells[i].alive) continue;
            DrawRectangle(
                cells[i].x,
                cells[i].y,
                BW - 10,
                20,
                cells[i].color
            );        
        }
    EndDrawing();

    return true;
}