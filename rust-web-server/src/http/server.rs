use tokio::{io::AsyncReadExt, net::TcpListener};

use crate::http::{request::HttpRequest, response::HttpResponse};


pub struct HttpServer {
    pub address : &'static str,
    pub port    : u32
}

impl HttpServer {
    pub async fn main_loop(self : &HttpServer) {
        let port = self.port;
        println!("Application started");
        let listener = TcpListener::bind(format!("{}:{}", self.address, port)).await.unwrap();
        println!("Waiting to clients... [Port: http://{}:{}/]", self.address, port);

        loop {
            match listener.accept().await {
              Ok((mut stream, _)) => {
                tokio::spawn(async move {
                  let mut buf : [u8; 1024] = [0; 1024];
                  match stream.read(&mut buf).await {
                    Ok(_) => { 
                      let request = HttpRequest::from(&String::from_utf8_lossy(&buf)); 
                      let mut response = HttpResponse::build(stream,request);
                      response.handle().await;
                      // let _ = stream.write(&response).await;
                    }
                    Err(e) => println!("[ERROR ] while reading client request:  {}\n", e),
                  }
                });
              }
              Err(e) => println!("[ERROR] in accepting client connection: {}", e),
            }
        }
    }
}