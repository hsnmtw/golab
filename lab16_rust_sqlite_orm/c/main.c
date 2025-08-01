#include <signal.h>
#include <stdio.h>
#include "sqlite3.h"

void Segfault_Handler(int signo)
{
    fprintf(stderr,"\n[!] Oops! Segmentation fault...%d\n",signo);
}


int main(void) 
{
    signal(SIGSEGV,Segfault_Handler);

    sqlite3* db;
    int rc = sqlite3_open ("test.db", &db); 

    if (rc)                                        
    {
        fprintf(stderr, "error: %s\n", sqlite3_errmsg(db));
        return 0;
    }
    
    return 0;
}
