#include <cstdlib>
#include <strings.h>
#ifndef HTTP_REQUEST_H
#define HTTP_REQUEST_H

#ifndef HTTP_METHOD_GET
#define HTTP_METHOD_GET  0
#endif
#ifndef HTTP_METHOD_POST
#define HTTP_METHOD_POST 1
#endif

#ifndef HTTP_REQUEST_BUFFER_SIZE
#define HTTP_REQUEST_BUFFER_SIZE 2048
#endif

typedef struct HttpRequest {
	int method;
	char* path;
	char* query;
	char* body;
} HttpRequest;


void fill_http_request(HttpRequest* request, char* buffer) {
	
	request->method = HTTP_METHOD_GET;
	if(buffer[0]=='P' && buffer[1]=='O' && buffer[2]=='S' && buffer[3]=='T') 
		request->method = HTTP_METHOD_POST;

	int row=1;
	int col=1;
	int j=0;

	request->path  = (char*) malloc(518);
	request->query = (char*) malloc(518);
	request->body  = (char*) malloc(1024);

	bzero(request->path, 518);
	bzero(request->query, 518);
	bzero(request->body, 1024);

	for(int i=0;i<HTTP_REQUEST_BUFFER_SIZE || buffer[i]=='\0';++i) {
		if(row == 1 && col>1 && buffer[i]==' ') {
			j=1;
			i++;
			request->path[0] = '.';
			while(i<HTTP_REQUEST_BUFFER_SIZE && j<518 && buffer[i]!='\n' && buffer[i] !=' ' && buffer[i] !='?' && buffer[i] != '\0') {
				request->path[j++] = buffer[i++];
			}
			j=0;
			if(buffer[i]=='?') {
				i++;
				while(i<HTTP_REQUEST_BUFFER_SIZE && buffer[i]!='\n' && buffer[i] !=' ' && buffer[i] != '\0') {
					request->query[j++] = buffer[i++];
				}
			}
		}
		if(buffer[i]=='\n') {
			row++;
			col=1;
			if(i+1<HTTP_REQUEST_BUFFER_SIZE && buffer[i+1]=='\n') {
				//everything else is the body
				j=0;
				while(buffer[i] != '\0' && i<HTTP_REQUEST_BUFFER_SIZE) {
					request->body[j++] = buffer[i++];
				}
			}
		}
		col++;
	}
}

#endif