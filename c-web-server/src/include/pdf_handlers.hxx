#include <cstdio>

#include <functional>


#include "utils.hxx"

#ifndef PDF_HANDLERS_H
#define PDF_HANDLERS_H

#ifndef BUFFER_CHUNK_SIZE
#define BUFFER_CHUNK_SIZE 1024
#endif

int generate_pdf(function<void(char*,int)> callback) {

    return 0;
}


#endif