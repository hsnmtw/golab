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
#include <signal.h> 


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
#define HTTP_PROTOCOL "HTTP/1.1"

#ifndef HTTP_REQUEST_BUFFER_SIZE
#define HTTP_REQUEST_BUFFER_SIZE 2048
#endif
#define HTTP_RORT_NUMBER 8080
#define HTTP_RESPONSE_STATUS_CODE_200 "OK"
#define HTTP_RESPONSE_STATUS_CODE_404 "Not Found"
#define HTTP_RESPONSE_STATUS_CODE_500 "Internal Server Error"
#define HTTP_RESPONSE_STATUS_CODE_306 "(Unused)"
#define HTTP_DEFAULT_HEADERS "Access-Control-Allow-Origin: *\n"            \
							 "Transfer-Encoding: chunked\n"                \
							 "Server: C/CXX Web Server By Hussain Al Mutawa\n" \
							 "Vary: Cookie, Accept-Encoding\n"             \
							 "X-Powered-By: C99\n"                         \
							 "X-Content-Type-Options: nosniff\n"           \
							 "x-frame-options: SAMEORIGIN\n"
#define NEW_LINE "\n"
#define HTTP_METHOD_GET  0
#define HTTP_METHOD_POST 1


static const char* error(char const* message) {
	char* e = new char[1000];
	sprintf(e,"[ERROR]: %s [%d] %s", message, errno, strerror(errno));
	//exit(1);
	return e;
}

const string get_content_type(string path) {
	if (path.length() == 0) { return ""; }
	string ext = get_filename_ext(path);

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

const string get_status_code(int status) {
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
		int n = write(sockfd, header.c_str(),header.length());
		n += write(sockfd, contents, len);
		n += write(sockfd,"\r\n",2);
		return n;

}


int write_response_headers(int sockfd, int status, string path) {

		int n = 0;

		n += write_to_socket(sockfd, HTTP_PROTOCOL);
		n += write_to_socket(sockfd, " 200 ");
		n += write_to_socket(sockfd, get_status_code(status).c_str());
		n += write_to_socket(sockfd, NEW_LINE);
		n += write_to_socket(sockfd, "Content-Type: ");
		n += write_to_socket(sockfd, get_content_type(path).c_str());
		n += write_to_socket(sockfd, NEW_LINE);	
		// n += write_to_socket(sockfd, "Content-Length: ");
		// n += write_to_socket(sockfd, "0");
		// n += write_to_socket(sockfd, NEW_LINE);
		n += write_to_socket(sockfd, "Date: ");
		n += write_to_socket(sockfd, now(LONG_DATE_FORMAT"\n").c_str());
		n += write_to_socket(sockfd, "Last-Modified: ");
		n += write_to_socket(sockfd, now(LONG_DATE_FORMAT"\n").c_str());	
		//write_to_socket(sockfd, "Content-Length: 445\n");	
		n += write_to_socket(sockfd, HTTP_DEFAULT_HEADERS );
		n += write_to_socket(sockfd, NEW_LINE);
		return n;
}



int serve_static_file(int sockfd, string path) {
		errno = 0;
		inf(concat("serving static file : ", path));
		int n = 0;
		FILE *fptr = fopen(path.c_str(), "rb");

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

void handle_connection(HttpConnection cn) {
	// try{
		int n = 0;
		int sockfd = cn.sockfd;

		HttpRequest request = {
			.method = -1,
			.path   = "",
			.query  = "",
			.body   = "",
		};

		// while(0 < (n = read(sockfd,buffer,1))) {
		dbg("receiving client request char by char");
		char buffer[HTTP_REQUEST_BUFFER_SIZE] = {0};
		if(0 > (n = read(sockfd,buffer,sizeof(buffer)-1))) {
			dbg("empty request");
			shutdown(sockfd, SHUT_RDWR);
			close(sockfd);
			inf("rejected empty request : ''");
			return;
		}

		dbg(buffer);
		fill_http_request(&request, buffer);
		dbg(concat("m: ",request.method == 0 ? "GET" : "POST"));
		dbg(concat("p: ",request.path));
		dbg(concat("q: ",request.query));
		dbg(concat("b: ",request.body));

		dbg("check if serving home page");
		if(streq(request.path,"./") || streq(request.path,"./home")) {
			request.path = "./assets/html/home.html";
		}

		dbg("check if favicon or chrome well-known");
		if(streq(request.path,"./favicon.ico") || streq(request.path, "./.well-known/appspecific/com.chrome.devtools.json")) {
			n += write_response_headers(sockfd, 404, request.path);

		} else if(streq(request.path,"./pdf.pdf")) {
			n += write_response_headers(sockfd, 200, request.path);
			generate_pdf([sockfd](char* _buffer, int _bytes){
				(void)write_to_socket(sockfd,_buffer,true);
			});
		} else {
			n += write_response_headers(sockfd, 200, request.path);
			n += serve_static_file(sockfd,request.path);
		}

		if (n < 0) {
			cout << KRED << error("failed writing to socket") << KNRM << endl;
		}

		// close(sockfd);
		dbg("sending zero length frame to denote terminating connection");
		write_to_socket(sockfd,"0\r\n\r\n");
		dbg("client socket shutdown");
		shutdown(sockfd, SHUT_RDWR);
		dbg("close client socket connection");
		close(sockfd);

	// } catch (int err) {
	// 	cout << KRED << "[ERROR] while handeling client connection : " << KCYN << err << KNRM << endl;
	// }
}

string get_remote_end_socket_ip(struct sockaddr_in cli_addr) {
	struct in_addr ip_address = ((struct sockaddr_in *) &cli_addr)->sin_addr;
	char* ip = new char[INET_ADDRSTRLEN];
	inet_ntop( AF_INET, &ip_address, ip, INET_ADDRSTRLEN );
	string s;
	s.assign(ip);
	return s;
}



void accept_client_connection(){

		printf("trying to connect ...\n");
		socklen_t clilen;
		struct sockaddr_in serv_addr, cli_addr;
		int port = HTTP_RORT_NUMBER;
		dbg("initiating server socket");
		int sockfd = socket(AF_INET, SOCK_STREAM, 0); //AF_INET: IPv4

		if (sockfd < 0 || errno != 0) {
			cout << KRED << error("failed opening socket") << KNRM << endl;
		}

		bzero((char *) &serv_addr, sizeof(serv_addr));
		serv_addr.sin_family = AF_INET;
		serv_addr.sin_addr.s_addr = INADDR_ANY;
		serv_addr.sin_port = htons(port);
		
		const linger _linger = {
			.l_onoff = 1,
			.l_linger = 0,
		};
		dbg("set server socket option linger to 1");
		setsockopt(sockfd, SOL_SOCKET, SO_LINGER, &_linger, sizeof(_linger));

		dbg("binding server socket to ip address and port");
		if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0 || errno != 0) {
			if(errno==98) {
				printf("\n\n%s[ERROR]%s address is already in use, %sterminating%s ...\n",KRED,KYEL,KCYN,KNRM);
				dbg("exit(1)");
				exit(1);
			}
			
			cout << KRED << error("ERROR on binding") << KNRM << endl;
		}

		// Create a thread pool with 4 threads
		//thread_pools::ThreadPool pool(10);
		
		while(true) {

			inf(" waiting for connection ");
			dbg("listening to clients");
			if(listen(sockfd,100) < 0 || errno != 0) {
				cout << KRED << error("failed to listen, ... \n") << KNRM << endl;
				break;
			}

			clilen = sizeof(cli_addr);
			dbg("accepting client connection");
			int newsockfd = accept(sockfd, (struct sockaddr *) &cli_addr, &clilen);
			
			if (newsockfd < 0 || errno != 0) {
				cout << KRED << error("failed on accept") << KNRM << endl;
				exit(1);
				break;
			}

			//pthread_t threadx;
			//pthread_create(&threadx, NULL,handle_connection, cn);	
			dbg("getting client ip address and assigning it an http connection with socket interface identifier");
			HttpConnection cn = {  
				.sockfd = newsockfd,
				.remote_address = get_remote_end_socket_ip(cli_addr),
			};
			// auto task = ([cn](){
				dbg(concat("handeling client connection : ", cn.remote_address));
				handle_connection(cn);
			// });
			// pool.enqueue(task);
		}
		
		dbg("shutdown server socket");
		shutdown(sockfd, SHUT_RDWR);
		dbg("close server socket");
		close(sockfd);
	
}


void sig_func(int sig)
{
	cerr << KRED << "SIG FAULT" << KNRM << endl;
    cerr.flush();
	exit(1);
}

int main() {
	signal(SIGSEGV, sig_func); // sets a new signal function for SIGSEGV
	// raise(SIGSEGV); // causes the signal function to be called
    cout << "Http/TCP Client in C++ : %s\n" << KGRN << concat("by : ", "Hussain Al Mutawa") << KNRM << endl;
	accept_client_connection();
	if(errno!=0) error("network error");	

	cout << "\nexiting ...\n" << endl;
    return 0;
}