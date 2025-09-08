#include <fstream>
#include <iostream>

using namespace std;

#define RED "\x1B[31m"
#define GRN "\x1B[32m"
#define YLW "\x1B[33m"
#define BLU "\x1B[34m"
#define CYN "\x1B[35m"
#define SKY "\x1B[35m"
#define WHT "\x1B[37m"
#define GLD "\x1B[93m"
#define BGR "\033[3;42;30m"
#define BYL "\033[3;43;30m"
#define BBL "\033[3;44;30m"
#define BSK "\033[3;104;30m"
#define BGY "\033[3;100;30m"
#define BWT "\033[1;47;35m"
#define RST "\033[0m"

void printc(string color, string message);

int main() {
    fstream fs;
    fs.open("./test.pdf", ios::out);
    return 0;
}

void printc(string color, string message) {
    cout << color << message << RST;
}