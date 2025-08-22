#include <strings.h>
#include <cstdlib>
#include <cstring>
#include <cstdio>
#include <ctime>
#ifndef UTILS_H
#define UTILS_H

char* concat(char const* a, char const* b) {
	const int al = strlen(a);
	const int bl = strlen(b);
	const int nl = al+bl;
	char* r = (char*) malloc(nl+2);
	bzero(r,nl);

	strcpy(r,a);
	strcat(r,b);

	return r;
}


const char* now(const char* format) {
	time_t timer;
	int max = 26;
    char buffer[26];
    struct tm* tm_info;
	
    timer = time(NULL);
    tm_info = localtime(&timer);

    strftime(buffer, max, format, tm_info);

	char* rs = (char*) malloc(26);
	strcpy(rs,buffer);
	return rs;
}

char* int_to_hex( int i ) {
  char* r = (char*) malloc(10);
  bzero(r, 10);
  sprintf(r, "%x", i);
  return r;
}


#endif