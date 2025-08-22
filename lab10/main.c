#include <stdio.h>
// #include "io/file.h"
#include "io/file.c"

int main() {
    FileReadOption fro = read_entire_file("./test.txt");
    printf("content = %s", fro.contents);
    printf("  error = %s", fro.error   );

    free(fro.contents);

    printf("\n---------------------\nexit 0\n");
    return 0;
}

