use std::collections::HashMap;

pub const GET     : &'static str = "GET";
pub const POST    : &'static str = "POST";
pub const DELETE  : &'static str = "DELETE";
pub const PATCH   : &'static str = "PATCH";
pub const PUT     : &'static str = "PUT";
pub const UNKNOWN : &'static str = "UNKNOWN";

pub struct HttpRequest {
  // pub stream: TcpStream,
  pub method : String,
  pub path   : String,
  pub body   : String,
  pub query  : HashMap<String,String>,
  pub cookie : HashMap<String,String>,
  pub headers: HashMap<String,String>,
}

impl HttpRequest {
  pub fn route(&self) -> String {
    return format!("{}:{}",self.method,self.path);
  }
  pub fn from(request: &str) -> HttpRequest {
    let mut _method : &str = UNKNOWN;
    let mut _path   : &str = "/";
    let mut _body   : &str = "";
    let mut _query  : HashMap<String,String> = HashMap::new();
    let mut _cookie : HashMap<String,String> = HashMap::new();
    let mut _headers: HashMap<String,String> = HashMap::new();

    let mut  lines = request.lines();
    let first = lines.next().unwrap();
    let parts : Vec<&str> = first.split(" ").collect::<Vec<&str>>();
    if parts.len() >= 1 {
      _method = match format!("{}", parts[0].trim().to_uppercase()).as_str() {
          GET    => GET,
          POST   => POST,
          DELETE => DELETE,
          PUT    => PUT,
          PATCH  => PATCH,
              _  => UNKNOWN
      };
      let _path_and_query = parts[1].split("?").collect::<Vec<_>>();
      _path = _path_and_query[0];
      if _path_and_query.len()>1 {
        let _query_items = _path_and_query[1].split("&");
        for item in _query_items {
          let kv = item.split("=").collect::<Vec<_>>();
          if kv.len() > 1 {
            _query.insert(String::from(kv[0]), kv[1..].join("="));
          }
        }
      }
    }
    let mut reached_empty_line = false;
    // once reaching an empty line, the lines afterward are the body
    for line in lines {
      if !reached_empty_line {
        if line.starts_with("Cookie:") {
          let cookies = line["Cookie:".len()..].trim().split("; ").collect::<Vec<_>>();
          for item in cookies {
            let kv = item.split("=").collect::<Vec<_>>();
            if kv.len() > 1 {
              _cookie.insert(String::from(kv[0]), kv[1..].join("="));
            }
          }
          continue;
        }
        else {
            //parse headers
            let kv = line.split(":").collect::<Vec<_>>();
            if kv.len() > 1 {
              _headers.insert(String::from(kv[0]), kv[1..].join(":"));
            }
        }
      }
      reached_empty_line = reached_empty_line || line.trim().len() == 0 ;
    }

    return HttpRequest {
      // stream : stream,
      method : String::from(_method),
      path   : String::from(_path),
      body   : String::from(_body),
      query  : _query  ,
      cookie : _cookie ,
      headers: _headers,
    };
  }
}
