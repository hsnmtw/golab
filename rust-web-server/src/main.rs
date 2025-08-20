
mod http;


use std::{io::{Write}, net::TcpStream};

use crate::http::{request::HttpRequest, response::HttpResponse, server::HttpServer};

fn main() -> () {
  HttpServer {
    address : "0.0.0.0",
    port : 80,
    router: route_map
  }.main_loop();
}

fn route_map(mut stream :TcpStream,request:HttpRequest)->HttpResponse {
  println!("someone requsted '{}'", request.route());

    return match request.route().as_str() {
      "GET:/what" => {
        let mut r = HttpResponse::build(stream, request);
        _ = r.stream.write(b"test");
        r.is_handled = true;
        return r;
      },
      _ => HttpResponse::build(stream,request)    
    };
}


