#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include "file.h"

void   write_to_file    (const char* filename, char* content)
{
    printf("write_to_file : %s : %s", filename, content);
}

bool   file_exists      (const char* filename)
{
    FILE *file;
    if ((file = fopen(filename, "r"))) {
        fclose(file);
        return true;
    }
    return false;
}



FileReadOption  read_entire_file (const char* filename)
{
    FileReadOption fro = {0};
    
    FILE* f = fopen(filename, "r");

    if(!f) {
        fro.error = "file cannot be read";
        return fro;
    }
    
    fseek(f, 0, SEEK_END);
    long fsize = ftell(f);
    fseek(f, 0, SEEK_SET);  /* same as rewind(f); */  

    char* buffer = malloc(fsize + 1);
    fread(buffer, fsize, 1, f);
    fclose(f);

    buffer[fsize] = 0;


    fro.contents = buffer;

    return fro;

    // use the string, then ...
    // free(buffer);  
}


char** list_files       (const char* directory)
{
    printf("list_file : %s", directory);
}
void   create_directory (const char* path, bool recursive)
{
    printf("create_directory : %s : %s", path, recursive);
}