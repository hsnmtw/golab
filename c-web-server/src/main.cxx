#include <cerrno>
#include <cstdlib>
#include <cstring>
#include <cstdio>
#include <ctime>
#include <iostream>
#include <netinet/in.h>
#include <sys/socket.h>
#include <unistd.h>
#include <arpa/inet.h>


#include "include/thread_pools.hxx"
#include "include/pdf_handlers.hxx"
#include "include/http_request.hxx"
#include "include/utils.hxx"

#ifndef NULL
	#define NULL nullptr
#endif
#include <assert.h>

#ifndef INADDR_ANY
	#define	INADDR_ANY		((in_addr_t) 0x00000000)
#endif
#include <format>

#define BUFFER_CHUNK_SIZE 1024
#define LONG_DATE_FORMAT "%Y-%m-%d %H:%M:%S"
#define ARRAY_LEN(array) (sizeof(array)/sizeof(array[0]))
#define HTTP_PROTOCOL "HTTP/1.1"

#ifndef HTTP_REQUEST_BUFFER_SIZE
#define HTTP_REQUEST_BUFFER_SIZE 2048
#endif
#define HTTP_RORT_NUMBER 8080
#define HTTP_RESPONSE_STATUS_CODE_200 "OK"
#define HTTP_RESPONSE_STATUS_CODE_404 "Not Found"
#define HTTP_RESPONSE_STATUS_CODE_500 "Internal Server Error"
#define HTTP_RESPONSE_STATUS_CODE_306 "(Unused)"
#define HTTP_DEFAULT_HEADERS "Transfer-Encoding: chunked\n"                \
							 "Server: C Web Server By Hussain Al Mutawa\n"

/*
#define HTTP_DEFAULT_HEADERS "Access-Control-Allow-Origin: *\n"            \
							 "Transfer-Encoding: chunked\n"                \
							 "Server: C Web Server By Hussain Al Mutawa\n" \
							 "Vary: Cookie, Accept-Encoding\n"             \
							 "X-Powered-By: C99\n"                         \
							 "X-Content-Type-Options: nosniff\n"           \
							 "x-frame-options: SAMEORIGIN\n"
*/
#define NEW_LINE "\n"
#define HTTP_METHOD_GET  0
#define HTTP_METHOD_POST 1


static const char* error(char const* message) {
	char* e = (char*) malloc(1000);
	sprintf(e,"[ERROR]: %s [%d] %s", message, errno, strerror(errno));
	//exit(1);
	return e;
}

const char* get_filename_ext(char* filename) {
    char* dot = strrchr(filename, '.');
    if(!dot || dot == filename) { return ""; }
    return dot + 1;
}

static bool streq(char const* a, char const* b) {
	return 0 == strcmp(a,b);
}

 

const char* get_content_type(char* path) {
	if (path == NULL || strlen(path) == 0) { return ""; }
	const char* ext = get_filename_ext(path);

	if(streq(ext, "md"   )) { return "text/markdown"; }	
	if(streq(ext, "png"  )  
	|| streq(ext, "jpg"  )  
	|| streq(ext, "jpeg" ) 
	|| streq(ext, "gif"  )  
	|| streq(ext, "bmp"  )  
	|| streq(ext, "tif"  )  
	|| streq(ext, "tiff" )) { return concat("image/",ext); }
	if(streq(ext, "svg"  )) { return "image/svg+xml"; }
	if(streq(ext, "zip"  )  
	|| streq(ext, "rtf"  )  
	|| streq(ext, "pdf"  )  
	|| streq(ext, "xml"  )  
	|| streq(ext, "wasm" ) 
	|| streq(ext, "json" )) { return concat("application/",ext); }
	if(streq(ext, "txt"  )  
	|| streq(ext, "xslt" ) 
	|| streq(ext, "xhtml") 
	|| streq(ext, "htm"  )  
	|| streq(ext, "html" )  
	|| streq(ext, "csv"  )  
	|| streq(ext, "css"  )) { return concat("text/",ext); }
	if(streq(ext, "mjs"  ) 
	|| streq(ext, "js"   )) { return "text/javascript"; }
	if(streq(ext, "otf"  )  
	|| streq(ext, "ttf"  )  
	|| streq(ext, "eot"  )  
	|| streq(ext, "woff" ) 
	|| streq(ext, "woff2")) { return concat("font/",ext); }
	if(streq(ext, "dll"  )
	|| streq(ext, "msi"  )  
	|| streq(ext, "exe"  )) { return "application/octet-stream"; }
	if(streq(ext, "doc"  ) 
	|| streq(ext, "docx" )) { return "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; }
	if(streq(ext, "xls"  ) 
	|| streq(ext, "xlsx" )) { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
	if(streq(ext, "ppt"  ) 
	|| streq(ext, "pptx" )) { return "application/vnd.openxmlformats-officedocument.presentationml.presentation"; } 
	
	return "text/plain";
}

const char* get_status_code(int status) {
	switch (status)
	{
	case 200: return HTTP_RESPONSE_STATUS_CODE_200;
	case 404: return HTTP_RESPONSE_STATUS_CODE_404;
	case 500: return HTTP_RESPONSE_STATUS_CODE_500;
	default : return HTTP_RESPONSE_STATUS_CODE_306;
	}
}



int write_to_socket(int sockfd, char const* contents, bool is_chuncked = false) {
	int len = strlen(contents);
	if(!is_chuncked) {
		return write(sockfd, contents, len);
	}
	auto header = concat(int_to_hex(len), "\r\n");
	int n = write(sockfd, header,strlen(header));
	n += write(sockfd, contents, len);
	n += write(sockfd,"\r\n",2);
	return n;
}


int write_response_headers(int sockfd, int status, char* path) {
	int n = 0;
	printf("%s\n",path);
	n += write_to_socket(sockfd, HTTP_PROTOCOL);
	n += write_to_socket(sockfd, " 200 ");
	n += write_to_socket(sockfd, get_status_code(status));
	n += write_to_socket(sockfd, NEW_LINE);
	n += write_to_socket(sockfd, "Content-Type: ");
	n += write_to_socket(sockfd, get_content_type(path));
	n += write_to_socket(sockfd, NEW_LINE);	
	// n += write_to_socket(sockfd, "Content-Length: ");
	// n += write_to_socket(sockfd, "0");
	// n += write_to_socket(sockfd, NEW_LINE);
	n += write_to_socket(sockfd, "Date: ");
	n += write_to_socket(sockfd, now(LONG_DATE_FORMAT"\n"));
	n += write_to_socket(sockfd, "Last-Modified: ");
	n += write_to_socket(sockfd, now(LONG_DATE_FORMAT"\n"));	
	//write_to_socket(sockfd, "Content-Length: 445\n");	
	n += write_to_socket(sockfd, HTTP_DEFAULT_HEADERS );
	n += write_to_socket(sockfd, NEW_LINE);
	return n;
}



int serve_static_file(int sockfd, char* path) {
	errno = 0;
	printf("serving static file : [%s]\n", path);
	int n = 0;
	FILE *fptr = fopen(path, "rb");

	if(fptr == NULL || errno != 0) {

		write_to_socket(sockfd,error("failed to read file ... "));
		return errno;
	}

	{
		char buffer[BUFFER_CHUNK_SIZE];
        while (!feof(fptr))
        {
			bzero(buffer, BUFFER_CHUNK_SIZE);
			int bytes = fread(buffer,1,BUFFER_CHUNK_SIZE,fptr);
			if(bytes>0)
				bytes = write_to_socket(sockfd,buffer,true);
        }
	}
	fclose(fptr);
	return n;
}

typedef struct HttpConnection {
	int  sockfd;
	char* remote_address;
} HttpConnection;


void handle_connection(HttpConnection* cn) {
	int sockfd = cn->sockfd;
	char* buffer = (char*) malloc(HTTP_REQUEST_BUFFER_SIZE);
	//printf("got here ........ %d\n",sockfd);
	bzero(buffer,HTTP_REQUEST_BUFFER_SIZE);
	int n = read(sockfd,buffer,HTTP_REQUEST_BUFFER_SIZE);
	if (n < 0) {
		error("failed reading from socket");
	}

	HttpRequest* request = (HttpRequest*) malloc(sizeof(HttpRequest));
	fill_http_request(request, buffer);
	printf("Http Request [From = '%s'] [Path   = '%s']\n", cn->remote_address , request->path);
	// printf("Http Request [Query  = '%s']\n", request->query);
	// printf("Http Request [Method = '%d']\n", request->method);
	// printf("Http Request [Body   = '%s']\n", request->body);

	//printf("Here is the message: %s\n",buffer);
	printf("[%s] connected\n", cn->remote_address);

	if(streq(request->path,"./") || streq(request->path,"./home")) {
		strcpy(request->path, "./assets/html/home.html");
	}


	if(streq(request->path,"./favicon.ico") || streq(request->path, "./.well-known/appspecific/com.chrome.devtools.json")) {
		n += write_response_headers(sockfd, 404, request->path);

	} else if(streq(request->path,"./pdf.pdf")) {
		n += write_response_headers(sockfd, 200, request->path);
		generate_pdf([sockfd](char* _buffer, int _bytes){			
			(void)write_to_socket(sockfd,_buffer,true);
		});
	} else {
		n += write_response_headers(sockfd, 200, request->path);
		n += serve_static_file(sockfd,request->path);
	}

	if (n < 0) {
		error("failed writing to socket");
	}

	// close(sockfd);
	
	write_to_socket(sockfd,"0\r\n\r\n");
	shutdown(sockfd, SHUT_RDWR);
	close(sockfd);

	try{
		free(cn);
		free(request->path);
		free(request->query);
		free(request->body);
		free(request);
		free(buffer);
	}catch(int err){
		cout << "got some error while freeing memory" << err << endl;
	}
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


	if (sockfd < 0 || errno != 0) {
        error("failed opening socket");
	}

	bzero((char *) &serv_addr, sizeof(serv_addr));
	serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = INADDR_ANY;
    serv_addr.sin_port = htons(port);
	
	// const linger _linger = {
	// 	.l_onoff = 1,
	// 	.l_linger = 0,
	// };
	// setsockopt(sockfd, SOL_SOCKET, SO_LINGER, &_linger, sizeof(_linger));

	int on = 1;
	setsockopt(sockfd, SOL_SOCKET, SO_LINGER, &on, sizeof(on)); // or sizeof(on) in C


	if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0 || errno != 0) {
        error("ERROR on binding");
	}

	// Create a thread pool with 4 threads
    thread_pools::ThreadPool pool(10);
	
	while(true) {
		printf("--------------------------------------------------------[listening on port %d] \n", port);		
		listen(sockfd,5);
		clilen = sizeof(cli_addr);
		int newsockfd = accept(sockfd, (struct sockaddr *) &cli_addr, &clilen);
		if (newsockfd < 0) 
		  error("failed on accept");

		//pthread_t threadx;
		//pthread_create(&threadx, NULL,handle_connection, cn);	
		HttpConnection* cn = (HttpConnection*) malloc(sizeof *cn);
		cn->sockfd = newsockfd;
		cn->remote_address = get_remote_end_socket_ip(cli_addr);
		pool.enqueue([cn](){
			handle_connection(cn);
		});
	}
	
	shutdown(sockfd, SHUT_RDWR);
	close(sockfd);
}


int main() {
	cout << int_to_hex(10) << endl;
	// printf("%s\n", get_default_response_headers());
	// return 0;
    printf("Http/TCP Client in C++ : %s\n", concat("by : ", "Hussain Al Mutawa"));
	accept_client_connection();
	if(errno!=0) error("network error");	
    return 0;
}