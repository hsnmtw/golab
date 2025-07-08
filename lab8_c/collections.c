
/** gets the length of array */
int len(int *arr) {
    return sizeof(arr)/sizeof(arr[0]);
}

/** prints an array */
void printArray(int *arr) {
    printf("[");
    int l = len(arr);
    for(int i=0;i<=l;i++){
        printf("%d",arr[i]);
        if(i<l) printf(",");
    }
    printf("]\n");
}