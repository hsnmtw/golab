#include <iostream>
#include <raylib.h>

using namespace std;

int main(int argc, char **argv) {
    cout << "Hello World !" << endl;
    InitWindow(600,600,"Test");
    while(!WindowShouldClose()){
        BeginDrawing();
        ClearBackground(Color{.r=250,.g=89,.b=78,.a=233});
        EndDrawing();
    }
    return 0;
}