#include <cstdio>

#include <functional>


#include "utils.hxx"

#ifndef PDF_HANDLERS_H
#define PDF_HANDLERS_H

#ifndef BUFFER_CHUNK_SIZE
#define BUFFER_CHUNK_SIZE 1024
#endif

int generate_pdf(function<void(const char*,int)> callback) {
    FILE *fptr = fopen("./assets/pdf.pdf", "rb");
    if(fptr == NULL || errno != 0) {
        return errno;
    }

    {
        char buffer[BUFFER_CHUNK_SIZE+1];
        while (!feof(fptr))
        {
            bzero(buffer, BUFFER_CHUNK_SIZE);
            int bytes = fread(buffer,1,BUFFER_CHUNK_SIZE,fptr);
            if(bytes>0)
                callback(buffer,bytes);
            
        }
    }
    fclose(fptr);
    return 0;
}


#endif