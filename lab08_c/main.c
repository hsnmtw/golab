#include<stdlib.h>
#include<stdio.h>

#include "collections.c"

int main() {

    int* arr = (int*)malloc(3 * sizeof(3));
    arr[0] = 1;
    arr[1] = 2;
    arr[2] = 3;

    printf("[%d,%d,%d]\n", arr[0], arr[1], arr[2]);

    printArray(arr);

    int arr2[] = {4,5,6};
    printArray(arr2);

    free(arr);
    // cannot use free because it was not allocated using malloc
    //free(arr2);

    return 0;
}