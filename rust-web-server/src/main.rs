
mod http;

use crate::http::server::HttpServer;

fn main() {
  init();
}

#[tokio::main]
async fn init() {
  let myserver = HttpServer {
    address : "0.0.0.0",
    port : 80
  };
  return myserver.main_loop().await;
}


