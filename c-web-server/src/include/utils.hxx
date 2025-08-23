#include <strings.h>
#include <cstdlib>
#include <cstring>
#include <cstdio>
#include <ctime>
#ifndef UTILS_H
#define UTILS_H

#define KNRM  "\x1B[0m"
#define KRED  "\x1B[31m"
#define KGRN  "\x1B[32m"
#define KYEL  "\x1B[33m"
#define KBLU  "\x1B[34m"
#define KMAG  "\x1B[35m"
#define KCYN  "\x1B[36m"
#define KWHT  "\x1B[37m"
#define LONG_DATE_FORMAT "%Y-%m-%d %H:%M:%S"

#define ARRAY_LEN(array) (sizeof(array)/sizeof(array[0]))

using namespace std;

static bool streq(char const* a, char const* b) {
	return 0 == strcmp(a,b);
}

static bool streq(char const* a, string b) {
	return streq(a,b.c_str());
}

static bool streq(string a, char const* b) {
	return streq(a.c_str(),b);
}

static bool streq(string a, string b) {
	return streq(a.c_str(),b.c_str());
}


string concat(string a, string b) {
	string s;
	s.assign(a);
	s.append(b);
	return s;
}

const char* get_filename_ext(string filename) {
	const char* chrs = filename.c_str();
    char* dot = strrchr((char*)chrs, '.');
    if(!dot || streq(dot, chrs)) { return ""; }
    return dot + 1;
}



string now(const char* format) {
	time_t timer;
	int max = 26;
    char buffer[26];
    struct tm* tm_info;
	
    timer = time(NULL);
    tm_info = localtime(&timer);

    strftime(buffer, max, format, tm_info);
	string s;
	return s.assign(buffer);
}

char* int_to_hex( int i ) {
  char* r = new char[10];
  bzero(r, 10);
  sprintf(r, "%x", i);
  return r;
}

void inf(const string msg) {
	cout << '[' << KBLU << now(LONG_DATE_FORMAT) << KNRM << ']' << KGRN << " [inf] " << KMAG << msg << KNRM << endl;
}

void dbg(const string msg) {
	cout << '[' << KMAG << now(LONG_DATE_FORMAT) << KNRM << ']' << KYEL << " [dbg] " << KWHT << msg << KNRM << endl;
}

#endif