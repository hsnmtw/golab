struct User {
    name: String,
    age: i8
}

impl core::fmt::Debug for User {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        f.debug_struct("User").field("name", &self.name).field("age", &self.age).finish()
    }
}

fn main() {
    let connection = sqlite::open(":memory:").unwrap();

    let query = "
        CREATE TABLE users (name TEXT, age INTEGER);
        INSERT INTO users VALUES ('Alice', 42);
        INSERT INTO users VALUES ('Bob', 69);
    ";
    connection.execute(query).unwrap();

    connection.iterate("select * from users", |p| {
        //for &(name, value) in x.iter() {
        for &(key,val) in p.iter() {
            println!("{} / {}",key, val.unwrap());
            // println!("{} = {}", name, value.unwrap());
            //println!("{:#?}",User{name:String::from(u.0),age:0});
        }
        true
    }).unwrap();

}
