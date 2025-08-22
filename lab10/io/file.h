#include <stdbool.h>

#ifndef FILE_H
#define FILE_H

typedef struct {
    char *contents;
    char *error;
} FileReadOption;


void   write_to_file    (const char* filename, char* content);
FileReadOption  read_entire_file (const char* filename);
char** list_files       (const char* directory);
void   create_directory (const char* path, bool recursive);
bool   file_exists      (const char* filename);

#endif