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
#include <stdarg.h>

#define ARRAY_LENGTH(arr) ((int) (sizeof (arr) / sizeof (arr)[0]))
#define HTTP_PROTOCOL "HTTP/2"
#define HTTP_REQUEST_BUFFER_SIZE 256
#define HTTP_RORT_NUMBER 8080
#define HTTP_RESPONSE_STATUS_CODE_200 "OK"
#define HTTP_RESPONSE_STATUS_CODE_404 "Not Found"
#define HTTP_RESPONSE_STATUS_CODE_500 "Internal Server Error"
#define HTTP_RESPONSE_STATUS_CODE_306 "(Unused)"
#define LANDING_PAGE "./assets/html/home.html"

char* concat(char* a, char* b) {
	size_t l = strlen(a)+strlen(b)+1;
	char fn[l];
	// int length = 0;
	// for(size_t i=0;i<count;++i){
	// 	length += strlen(arg[i]);
	// 	// printf("%s\n",arg[i]);
	// }
	// fn = malloc( length ); // Add 1 for null terminator.
	// strcpy( fn, arg[0] );
	// for(size_t i=1;i<count;++i){
	// 	strcat(fn, arg[i]);
	// }
	// return fn;
	sprintf(fn,"%s%s",a,b);
	// printf("------%s-----",fn);
	char* rs = malloc (l);
	strcpy(rs,fn);
	return rs;
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

char* get_default_response_headers() {
	char* headers =
	"Access-Control-Allow-Origin: *\n"
	"Connection: Keep-Alive\n"
	"Keep-Alive: timeout=5, max=997\n"
	"Server: C Web Server By Hussain Al Mutawa\n"
	"Vary: Cookie, Accept-Encoding\n"
	"X-Powered-By: C99\n"
	"X-Content-Type-Options: nosniff\n"
	"x-frame-options: SAMEORIGIN\n"
	;
	char* date = concat("Date: ", now("%Y-%m-%d %H:%M:%S\n"));
	char* modify = concat("Last-Modified", now("%Y-%m-%d %H:%M:%S\n"));

	return concat(concat(headers,date),modify);
}

typedef struct {
	char** array;
	size_t length;
} StringArray;

StringArray str_split(char* a_str, const char a_delim)
{
    char** result    = 0;
    size_t count     = 0;
    char* tmp        = a_str;
    char* last_comma = 0;
    char delim[2];
    delim[0] = a_delim;
    delim[1] = 0;

    /* Count how many elements will be extracted. */
    while (*tmp)
    {
        if (a_delim == *tmp)
        {
            count++;
            last_comma = tmp;
        }
        tmp++;
    }

    /* Add space for trailing token. */
    count += last_comma < (a_str + strlen(a_str) - 1);

    /* Add space for terminating null string so caller
       knows where the list of returned strings ends. */
    count++;

    result = malloc(sizeof(char*) * count);

    if (result)
    {
        size_t idx  = 0;
        char* token = strtok(a_str, delim);

        while (token)
        {
            assert(idx < count);
            *(result + idx++) = strdup(token);
            token = strtok(0, delim);
        }
        assert(idx == count - 1);
        *(result + idx) = 0;
    }

	StringArray string_array = {
		.array = result,
		.length = count
	};

    return string_array;
}

char* get_content_type(char* path) {
	char* contentType = "text/plain";
	if (path == NULL || sizeof(path) == 0) return contentType;
	StringArray ps = str_split(path,'.');
	char* ext = ps.array[ps.length-1];

	if(!strcmp(ext, "md")) return "text/markdown";
	
	if(!strcmp(ext, "png")  
	|| !strcmp(ext, "jpg")  
	|| !strcmp(ext, "jpeg") 
	|| !strcmp(ext, "gif")  
	|| !strcmp(ext, "bmp")  
	|| !strcmp(ext, "tif")  
	|| !strcmp(ext, "tiff") )  return strcat("image/",ext);

	if(!strcmp(ext, "svg")) return "image/svg+xml";
	if(!strcmp(ext, "zip")  
	|| !strcmp(ext, "rtf")  
	|| !strcmp(ext, "pdf")  
	|| !strcmp(ext, "xml")  
	|| !strcmp(ext, "wasm") 
	|| !strcmp(ext, "json")) return strcat("application/",ext); 

	if(!strcmp(ext, "txt")  
	|| !strcmp(ext, "xslt") 
	|| !strcmp(ext, "xhtml") 
	|| !strcmp(ext, "htm")  
	|| !strcmp(ext, "html")  
	|| !strcmp(ext, "csv")  
	|| !strcmp(ext, "css")  ) return strcat("text/",ext);

	if(!strcmp(ext,"mjs") 
	|| !strcmp(ext,"js" )) return "text/javascript";

	if(!strcmp(ext, "otf")  
	|| !strcmp(ext, "ttf")  
	|| !strcmp(ext, "eot")  
	|| !strcmp(ext, "woff") 
	|| !strcmp(ext, "woff2")) return strcat("font/",ext);	

	if(!strcmp(ext, "dll")
	|| !strcmp(ext, "msi")  
	|| !strcmp(ext, "exe") ) return "application/octet-stream";
	
	if(!strcmp(ext, "doc") 
	|| !strcmp(ext, "docx")) return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
	if(!strcmp(ext, "xls") 
	|| !strcmp(ext, "xlsx")) return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
	if(!strcmp(ext, "ppt") 
	|| !strcmp(ext, "pptx")) return "application/vnd.openxmlformats-officedocument.presentationml.presentation"; 

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

int write_to_socket(int sockfd, char* contents) {
	return write(sockfd, contents, sizeof(contents));
}

int serve_static_file(int sockfd, char* path) {
	int n = 0;
	FILE *fptr = fopen(path, "r");
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

void error(char* message) {
	printf("[ERROR]: %s [%d] %s", message, errno, strerror(errno));
	exit(1);
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
	char header[256];
	sprintf(header, "%s %d %s\n%s\n%s\n\n",HTTP_PROTOCOL,200,get_status_code(200),"Content-Type:", get_content_type(LANDING_PAGE));
	n += write_to_socket(sockfd, header);
	n += serve_static_file(sockfd,LANDING_PAGE);		

	if (n < 0) 
		error("failed writing to socket");

	close(sockfd);
	free(cn);
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
		
		struct in_addr ip = ((struct sockaddr_in *) &cli_addr)->sin_addr;
		char str[INET_ADDRSTRLEN];
		inet_ntop( AF_INET, &ip, str, INET_ADDRSTRLEN );

		pthread_t threadx;
		HttpConnection *cn = malloc(sizeof *cn);
		cn->sockfd = newsockfd;
		cn->remote_address = str;
		pthread_create(&threadx, NULL,(void*) handle_connection,(void*) cn);		
	}
	
	close(sockfd);
}


int main() {

	printf("%s\n", get_default_response_headers());
	return 0;
    // printf("Http/TCP Client in C\n");
	// accept_client_connection();
	// if(errno!=0) error("network error");	
    // return 0;
}



