#include <errno.h>
#include <netinet/in.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <sys/types.h> 
#include <unistd.h>
#include <arpa/inet.h>
#include <assert.h>
#include <time.h>
#include <stdbool.h>

#define ARRAY_LEN(array) (sizeof(array)/sizeof(array[0]))

#define HTTP_PROTOCOL "HTTP/1.0"
#define HTTP_REQUEST_BUFFER_SIZE 256
#define HTTP_RORT_NUMBER 8080
#define HTTP_RESPONSE_STATUS_CODE_200 "OK"
#define HTTP_RESPONSE_STATUS_CODE_404 "Not Found"
#define HTTP_RESPONSE_STATUS_CODE_500 "Internal Server Error"
#define HTTP_RESPONSE_STATUS_CODE_306 "(Unused)"
#define LANDING_PAGE "./assets/html/home.html"
#define NEW_LINE "\n"

void error(char* message) {
	printf("[ERROR]: %s [%d] %s", message, errno, strerror(errno));
	exit(1);
}

char* get_filename_ext(char *filename) {
    char* dot = strrchr(filename, '.');
    if(!dot || dot == filename) return "";
    return dot + 1;
}

bool streq(char* a, char* b) {
	return 0 == strcmp(a,b);
}

char* concat(char* a, char* b) {
	size_t al = strlen(a);
	size_t bl = strlen(b);
	char* r = (char*) malloc(al+bl);
	bzero(r,al+bl);
	size_t max=al;
	if(max<bl) max=bl;
	for(size_t i=0;i<max;++i){
		if(i<al)    r[i] = a[i];
		if(i<bl) r[al+i] = b[i];
	}
	return r;
} 

char* get_content_type(char* path) {
	char* contentType = "text/plain";
	if (path == NULL || strlen(path) == 0) return contentType;
	char* ext = get_filename_ext(path);

	if(streq(ext, "md"   )) return "text/markdown";	
	if(streq(ext, "png"  )  
	|| streq(ext, "jpg"  )  
	|| streq(ext, "jpeg" ) 
	|| streq(ext, "gif"  )  
	|| streq(ext, "bmp"  )  
	|| streq(ext, "tif"  )  
	|| streq(ext, "tiff" )) return concat("image/",ext);
	if(streq(ext, "svg"  )) return "image/svg+xml";
	if(streq(ext, "zip"  )  
	|| streq(ext, "rtf"  )  
	|| streq(ext, "pdf"  )  
	|| streq(ext, "xml"  )  
	|| streq(ext, "wasm" ) 
	|| streq(ext, "json" )) return concat("application/",ext); 
	if(streq(ext, "txt"  )  
	|| streq(ext, "xslt" ) 
	|| streq(ext, "xhtml") 
	|| streq(ext, "htm"  )  
	|| streq(ext, "html" )  
	|| streq(ext, "csv"  )  
	|| streq(ext, "css"  )) return concat("text/",ext);
	if(streq(ext, "mjs"  ) 
	|| streq(ext, "js"   )) return "text/javascript";
	if(streq(ext, "otf"  )  
	|| streq(ext, "ttf"  )  
	|| streq(ext, "eot"  )  
	|| streq(ext, "woff" ) 
	|| streq(ext, "woff2")) return concat("font/",ext);	
	if(streq(ext, "dll"  )
	|| streq(ext, "msi"  )  
	|| streq(ext, "exe"  )) return "application/octet-stream";	
	if(streq(ext, "doc"  ) 
	|| streq(ext, "docx" )) return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
	if(streq(ext, "xls"  ) 
	|| streq(ext, "xlsx" )) return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
	if(streq(ext, "ppt"  ) 
	|| streq(ext, "pptx" )) return "application/vnd.openxmlformats-officedocument.presentationml.presentation"; 

	return contentType;
}

char* get_status_code(int status) {
	switch (status)
	{
	case 200: return HTTP_RESPONSE_STATUS_CODE_200;
	case 404: return HTTP_RESPONSE_STATUS_CODE_404;
	case 500: return HTTP_RESPONSE_STATUS_CODE_500;
	default : return HTTP_RESPONSE_STATUS_CODE_306;
	}
}


char* now(char* format) {
	time_t timer;
	size_t max = 26;
    char buffer[26];
    struct tm* tm_info;
	
    timer = time(NULL);
    tm_info = localtime(&timer);

    strftime(buffer, max, format, tm_info);

	char* rs = NULL;
	rs = malloc(26);
	strcpy(rs,buffer);
	return rs;
}

int write_to_socket(int sockfd, char* contents) {
	return write(sockfd, contents, strlen(contents));
}


int write_response_headers(int sockfd, int status, char* path) {
	int n = 0;
	printf("%s\n",path);
	n += write_to_socket(sockfd, HTTP_PROTOCOL);
	n += write_to_socket(sockfd, " 200 ");
	n += write_to_socket(sockfd, get_status_code(status));
	n += write_to_socket(sockfd, NEW_LINE);
	n += write_to_socket(sockfd, "Content-Type: ");
	// n += write_to_socket(sockfd, "text/html\n"); //get_content_type(path));
	char* content_type = get_content_type(path);
	n += write_to_socket(sockfd, content_type);
	free(content_type);
	n += write_to_socket(sockfd, NEW_LINE);
	n += write_to_socket(sockfd, "Date: ");
	n += write_to_socket(sockfd, now("%Y-%m-%d %H:%M:%S\n"));
	n += write_to_socket(sockfd, "Last-Modified: ");
	n += write_to_socket(sockfd, now("%Y-%m-%d %H:%M:%S\n"));		
	n += write_to_socket(sockfd , 
	"Access-Control-Allow-Origin: *\n"
	"Connection: Keep-Alive\n"
	"Keep-Alive: timeout=5, max=997\n"
	"Server: C Web Server By Hussain Al Mutawa\n"
	"Vary: Cookie, Accept-Encoding\n"
	"X-Powered-By: C99\n"
	"X-Content-Type-Options: nosniff\n"
	"x-frame-options: SAMEORIGIN\n"
	);
	n += write_to_socket(sockfd, NEW_LINE);

	return n;
}



int serve_static_file(int sockfd, char* path) {
	printf("serving static file : [%s]", path);
	int n = 0;
	FILE *fptr = fopen(path, "r");
	//printf("\nfptr  = %d\n", fptr == NULL);
	//printf("\nerrno = %d\n", errno == 0);
	if(fptr == NULL || errno != 0) {
		error("failed to read file ... ");
		return errno;
	}
	{
		char c = fgetc(fptr);
        while (c != EOF)
        {
			char buffer[1] = {c};
			n += write_to_socket(sockfd,buffer);		
            c = fgetc(fptr);
        }

	}
	fclose(fptr);
	return n;
}

typedef struct {
	int  sockfd;
	const char* remote_address;
} HttpConnection;

void handle_connection(HttpConnection *cn) {
	int sockfd = cn->sockfd;
	char buffer[HTTP_REQUEST_BUFFER_SIZE];
	//printf("got here ........ %d\n",sockfd);
	bzero(buffer,HTTP_REQUEST_BUFFER_SIZE);
	int n = read(sockfd,buffer,HTTP_REQUEST_BUFFER_SIZE);
	if (n < 0) error("failed reading from socket");
	
	//printf("Here is the message: %s\n",buffer);
	printf("[%s] connected", cn->remote_address);
	n += write_response_headers(sockfd, 200, LANDING_PAGE);
	n += serve_static_file(sockfd,LANDING_PAGE);		

	if (n < 0) 
		error("failed writing to socket");

	close(sockfd);
	free(cn);
}

char* get_remote_end_socket_ip(struct sockaddr_in cli_addr) {
	struct in_addr ip_address = ((struct sockaddr_in *) &cli_addr)->sin_addr;
	char * ip = (char *) malloc(INET_ADDRSTRLEN);
	inet_ntop( AF_INET, &ip_address, ip, INET_ADDRSTRLEN );
	return ip;
}

void accept_client_connection(){
	printf("trying to connect ...\n");
	socklen_t clilen;
	struct sockaddr_in serv_addr, cli_addr;
	int port = HTTP_RORT_NUMBER;
	int sockfd = socket(AF_INET, SOCK_STREAM, 0); //AF_INET: IPv4
	
	if (sockfd < 0 || errno != 0) 
        error("failed opening socket");
	
	bzero((char *) &serv_addr, sizeof(serv_addr));
	serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = INADDR_ANY;
    serv_addr.sin_port = htons(port);
	
	if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0 || errno != 0) 
        error("ERROR on binding");
	
	while(1) {
		printf("--------------------------------------------------------[listening on port %d] \n", port);		
		listen(sockfd,5);
		clilen = sizeof(cli_addr);
		int newsockfd = accept(sockfd, (struct sockaddr *) &cli_addr, &clilen);

		if (newsockfd < 0) 
		  error("failed on accept");
		
		pthread_t threadx;
		HttpConnection *cn = malloc(sizeof *cn);
		cn->sockfd = newsockfd;
		cn->remote_address = get_remote_end_socket_ip(cli_addr);
		pthread_create(&threadx, NULL,(void*) handle_connection,(void*) cn);		
	}
	
	close(sockfd);
}


int main() {
	// printf("%s\n", get_default_response_headers());
	// return 0;
    printf("Http/TCP Client in C : %s\n", concat("by : ", "Hussain Al Mutawa"));
	accept_client_connection();
	if(errno!=0) error("network error");	
    return 0;
}



