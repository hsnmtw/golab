use std::{collections::HashMap, fs::{self, File}, io::{Read, Write}, path::Path};
use std::{net::TcpStream};
use crate::http::request::HttpRequest;

const ERR_404   : &'static str = "404 : requested resource cannot be found !";
const HOME_PAGE : &'static str = "/assets/html/home.html";


pub struct HttpResponse {
  pub stream: TcpStream,
  pub request: HttpRequest,
  pub headers: HashMap<String,String>,
  pub cookie : HashMap<String,String>,
  pub is_handled: bool
}

impl HttpResponse {
    pub fn is_static_file(&mut self) -> bool {
        if &self.request.path == "/" {
            self.request.path = String::from(HOME_PAGE);
        }
        let path = format!("./{}",self.request.path).to_owned();
        if !Path::new(&path).exists() {
            return false;
        }

        let result = match File::open(&path) {
            Ok(_) => get_file_as_byte_vec(&path),
            Err(e) => Vec::from(format!("[ERROR]: unable to read file: {}",e).as_bytes()) 
        };

        let _ = self.stream.write(&result);
        return true;
    }

    pub fn handle(&mut self) -> bool {
        if self.is_handled {
            return true;
        }
        if self.is_static_file() {
            return true;
        }
        //let _ = self.stream.write(ERR_404.as_bytes());
        self.request.path = String::from("/assets/html/404.html");
        return self.is_static_file();
    }
    
    pub(crate) fn build(stream: TcpStream, request: HttpRequest) -> HttpResponse {
        let mut _cookie : HashMap<String,String> = HashMap::new();
        let mut _headers: HashMap<String,String> = HashMap::new();
        return HttpResponse{
            stream  : stream, 
            request : request,
            headers : _headers, 
            cookie  : _cookie,
            is_handled: false,
        };
    }
}
  

fn get_file_as_byte_vec(filename: &str) -> Vec<u8> {
  let mut f = File::open(&filename).expect("no file found");
  let metadata = fs::metadata(&filename).expect("unable to read metadata");
  let mut buffer = vec![0; metadata.len() as usize];
  f.read(&mut buffer).expect("buffer overflow");
  buffer
}

