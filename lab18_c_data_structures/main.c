
#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#define COUNT_OF(x) ((sizeof(x)/sizeof(0[x])) / ((size_t)(!(sizeof(x) % sizeof(0[x])))))



int main(int argc, char** argv)
{
    int* a = {};
    realloc(a, 1);
    printf("%lu\n",COUNT_OF(a));

    printf("finished\n\0");
    return 0;
}