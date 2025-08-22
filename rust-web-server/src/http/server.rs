use std::io::Read;
use std::thread;
use std::{net::{TcpListener, TcpStream}};
use std::collections::HashMap;

use crate::http::{request::HttpRequest, response::HttpResponse};


pub struct HttpServer {
    pub address  : &'static str,
    pub port     : u32,
    // pub router : fn(TcpStream,HttpRequest)->HttpResponse,
    pub handlers : HashMap<&'static str,fn(TcpStream,HttpRequest)->HttpResponse>
}

impl HttpServer {
    // fn handle_request(&'static self, request : HttpRequest) -> HttpResponse {
    //   let route = format!("{}:{}",request.method,request.path);
    //   return match self.handler(&route) {
    //     Some(v)=>v(request),
    //     None => panic!()
    //   };
    // }

    pub fn main_loop(srvr : HttpServer) { //(&'static self) {

        let port = srvr.port;
        println!("Application started");
        let listener = TcpListener::bind(format!("{}:{}", srvr.address, port)).unwrap();
        println!("Waiting to clients... [Port: http://{}:{}/]", srvr.address, port);


        loop {
            match listener.accept() {
              Ok((mut socket, _)) => {
                // let socket = s;

                let mut buf : [u8; 1024] = [0; 1024];
                match socket.read(&mut buf) {
                  Ok(_) => { 
                    let request = HttpRequest::from(&String::from_utf8_lossy(&buf)); 
                    // let mut response = (srvr.router)(socket,request);
                    let mut response = srvr.handlers[&request.route().as_str()](socket,request);
                    thread::spawn(move||response.handle());
                  }
                  Err(e) => println!("[ERROR ] while reading client request:  {}\n", e),
                }
                
              }
              Err(e) => println!("[ERROR] in accepting client connection: {}", e),
            }
        }
    }
}