#include <fstream>
#include <iostream>
#include <cstring>
#include "./zlib-1.3.1/zlib.h"

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

void printc(string color, const char *message);
int swrite(fstream *fs, const char *message);

int main() {
    char s[20] = {0};
    fstream fs;
    fs.open("./test.pdf", ios::out);
    int cursor;
    string xref = "0000000000 65535 f";
    cursor += swrite(&fs, "%PDF-1.4\n");
    cursor += swrite(&fs, "%\u00A9\u00B5\u00AE\u00AA\n");
    sprintf(s,"\n%010d 00000 n", cursor); xref.append(s);
    cursor += swrite(&fs, "1 0 obj "
                          "<< /Type /Catalog /Outlines 2 0 R /Pages 3 0 R >> "
                          "endobj\n");

    sprintf(s,"\n%010d 00000 n", cursor); xref.append(s);
    cursor += swrite(&fs, "2 0 obj "
                          "<< /Type Outlines /Count 0 >> "
                          "endobj\n");

    sprintf(s,"\n%010d 00000 n", cursor); xref.append(s);
    cursor += swrite(&fs, "3 0 obj "
                          "<< /Type /Pages /Kids [ 4 0 R ] /Count 1 >> "
                          "endobj\n");

    sprintf(s,"\n%010d 00000 n", cursor); xref.append(s);
    cursor += swrite(&fs, "4 0 obj "
                          "<< /Type /Page /Parent 3 0 R /MediaBox [ 0 0 612 792 ] /Contents 5 0 R /Resources << /ProcSet 6 0 R /Font << /F1 7 0 R >> >> >> "
                          "endobj\n");


    sprintf(s,"\n%010d 00000 n", cursor); xref.append(s);
    cursor += swrite(&fs, "5 0 obj "
                          "<< >> "
                          "stream\n"
                          "   BT\n"
                          "   /F1 24 Tf\n"
                          "   20 700 Td\n"
                          "   (Hello World) Tj"
                          "   ET\n"
                          "endstream\n"
                          "endobj\n");


    sprintf(s,"\n%010d 00000 n", cursor); xref.append(s);
    cursor += swrite(&fs, "6 0 obj "
                          "<< [/PDF /Text] >> "
                          "endobj\n");

    cursor += swrite(&fs, "7 0 obj "
                          "<< /Type /Font /Subtype /Type1 /Name /F1 /BaseFont /Times-Roman /Encoding /WinAnsiEncoding >> "
                          "endobj\n");


    const int startxref = cursor;
    cursor += swrite(&fs, "xref\n"
                          "0 7\n");
    cursor += swrite(&fs, xref.c_str());

    swrite(&fs, "\ntrailer\n"
                "<< /Size 7 /Root 1 0 R >>\n"
                "startxref\n");
    sprintf(s,"%d\n",startxref);
    swrite(&fs, s);
    swrite(&fs, "%%EOF");    
    fs.close();
    return 0;
}


int swrite(fstream *fs, const char *message) {
    const int l = strlen(message);
    fs->write(message, l);
    return l;
}

void printc(string color, const char *message) {
    cout << color << message << RST;
}