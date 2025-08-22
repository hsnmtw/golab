use std::collections::HashMap;

mod http;


use std::fs::metadata;
use std::io::BufWriter;
use std::{io::{Write}, net::TcpStream};

// use skia_safe::pdf::{self, Metadata};
// use skia_safe::utils::CustomTypefaceBuilder;
// use skia_safe::{canvas, typeface, Color, Color4f, ColorSpacePrimaries, ColorSpaceTransferFn, Document, Font, Handle, Paint, Point, RCHandle, Rect, Size, Typeface};

use skia_safe::{pdf, Paint, Rect, Size};

use crate::http::{request::HttpRequest, response::HttpResponse, server::HttpServer};

fn main() -> () {
  let mut srvr = HttpServer {
    address : "0.0.0.0",
    port : 8080,
    handlers: HashMap::new()
  };

  srvr.handlers.insert("GET:/", route_home);
  srvr.handlers.insert("GET:/favicon.ico", route_home);
  srvr.handlers.insert("GET:/what", route_what);
  srvr.handlers.insert("GET:/pdf", route_pdf);
  
  HttpServer::main_loop(srvr);
}

fn route_what(stream :TcpStream,request:HttpRequest)->HttpResponse {
  let mut response = HttpResponse::build(stream, request);
  _=response.stream.write(b"HTTP/1.1 200 OK\n");
  _=response.stream.write(b"Content-Type: text/plain\n\n");
  _ = response.stream.write(b"test");
  response.is_handled = true;
  return response;
}

fn route_home(stream :TcpStream,request:HttpRequest)->HttpResponse {
  return HttpResponse::build(stream, request);
}

fn route_pdf(stream :TcpStream,request:HttpRequest)->HttpResponse {
  let mut memory = Vec::new();
  let mut document =  pdf::new_document(&mut memory, None);

  let mut page = document.begin_page(Size::new(100.0,100.0), None);
  let canvas = page.canvas();
  let paint = Paint::default();//new(&Color4f::new(255.0,0.00,0.00,255.0),color_sp);
  canvas.draw_rect(Rect::new(10.0, 10.0, 40.0, 40.0), &paint);
  
  let mut response = HttpResponse::build(stream, request);
  _ = response.stream.write(b"HTTP/1.1 200 OK\n");
  _ = response.stream.write(b"Content-Type: application/pdf\n\n");
  _ = response.stream.write(&memory);

  response.is_handled = true;

  return response;
}

// fn set_default_headers() -> &[u8] {
//   return [
//     to_line_bytes("Access-Control-Allow-Origin","*"),
//     to_line_bytes("Connection"                 ,"Keep-Alive"),
//     to_line_bytes("Date"                       , format!("{:?}", chrono::offset::Utc::now()).as_str() ),
//     to_line_bytes("Last-Modified"              , format!("{:?}", chrono::offset::Utc::now()).as_str() ),
//     to_line_bytes("Keep-Alive"                 ,"timeout=5, max=997"),
//     to_line_bytes("Server"                     ,"Simple Web Server By Hussain Al Mutawa"),
//     to_line_bytes("Vary"                       ,"Cookie, Accept-Encoding"),
//     to_line_bytes("X-Powered-By"               ,"RST"),
//     to_line_bytes("X-Content-Type-Options"     ,"nosniff"),
//     to_line_bytes("x-frame-options"            ,"SAMEORIGIN"),
//   ]	
// }

// fn to_line_bytes(k:&str,v:&str) -> &[u8] {

// }



