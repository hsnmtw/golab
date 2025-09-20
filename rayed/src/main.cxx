#include <iostream>
#include <fstream>
#include <string>
#include <raylib.h>
#include <cstring>
#include <vector>

using namespace std;
const auto DARK_BLUE = Color { .r = 0x00, .g = 0x31, .b = 0x6E, .a = 0x00 };

const auto W = 1024;
const auto H = 800;

typedef struct {
    int col;
    int row;
} Position;

typedef struct {
    vector<string> lines;
    Position cursor;
} Buffer;

Buffer buffer = {};
const auto BUFFER_CHUNK_SIZE = 1024;

void Redraw(void);
int ReadFile(vector<string> *lines,const char *fileName);
Font customFont;

int main(void) { //int argc, char **argv) {
    //cout << "Hello World !" << endl;
    buffer.lines = {};
    ReadFile(&buffer.lines,"./src/main.cxx");
    InitWindow(W,H,"Test Editor");
    customFont = LoadFontEx("c:/windows/fonts/consola.ttf",34,0,255); 
    while(!WindowShouldClose()){
       Redraw();
    }
    UnloadFont(customFont);
    CloseWindow();
    return 0;
}

int ReadFile(vector<string> *lines,const char *fileName) {
    // Create an ifstream object and open the file
    // Replace "example.txt" with the path to your file
    std::ifstream inputFile(fileName);

    // Check if the file was successfully opened
    if (!inputFile.is_open()) {
        std::cerr << "Error opening the file!" << std::endl;
        return 1; // Indicate an error
    }

    std::string line; // Variable to store each line read from the file

    // Read the file line by line using std::getline
    while (std::getline(inputFile, line)) {
        //std::cout << line << std::endl; // Print each line to the console
        lines->push_back(line);
    }

    // Close the file when done
    inputFile.close();

    return 0; // Indicate successful execution
}

void Redraw(void) {
    BeginDrawing();
    ClearBackground(DARK_BLUE);
    float x = 5;
    float y = 5;
    for (const std::string& line : buffer.lines) {
        //cout << line << endl;
        DrawTextEx(customFont,line.c_str(),Vector2{.x=x,.y=y},17,1,WHITE);
        y += 21;
    }
    EndDrawing();
}