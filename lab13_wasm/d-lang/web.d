extern(C): // disable D mangling

     void callback(double a, double b, double c);


     double add(double a, double b) {
         double c = a + b;
         callback(a,b,c);
         return c;
     }

     // seems to be the required entry point
     void _start() {}