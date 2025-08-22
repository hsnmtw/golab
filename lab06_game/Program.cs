class Program
{
    internal record struct Position(int X, int Y);

    static void Main()
    {
        // Console.ForegroundColor = ConsoleColor.Blue;
        // Console.BackgroundColor = ConsoleColor.Blue;
        // for(int i=0;i<80;i++)
        // {
        //     System.Console.WriteLine("                                                                                ");
        // }
        // Console.ForegroundColor = ConsoleColor.White;
        // Console.BackgroundColor = ConsoleColor.Black;
        // int _y = 5;
        // int _x = 5;
        // int y = 5;
        // int x = 5;

        var lines = File.ReadAllLines("./maps/map1.txt");
        int[][] grid = lines.Select(line => line.Select(x => (int)x).ToArray()).ToArray();



        var p = new Position(0,0);
        var c = new Position(0,0);

        Console.SetCursorPosition(p.X,p.Y);

        Console.ForegroundColor = ConsoleColor.Yellow;

        foreach (var item in lines)
        {
            System.Console.WriteLine(item.Replace('1','\u2588').Replace('0', ' '));
        }

        void goUp() {
            if(c.Y>0) {
                if(grid[c.Y-1][c.X] is '1') {
                    System.Console.WriteLine("NOT ALLOWED");
                    return;
                }
                removePlayer();
                p = new Position(c.X, c.Y);
                c.Y--;
                drawPlayer();
            }
        }

        void goDown() {
            if(c.Y<80){
                if(grid[c.Y+1][c.X] is '1') {
                    System.Console.WriteLine("NOT ALLOWED");
                    return;
                }
                removePlayer(); 
                p = new Position(c.X, c.Y);
                c.Y++;
                drawPlayer();
            }
            
        }
        void goRight() {
            if(c.X<80) {
                if(grid[c.Y][c.X+1] is '1') {
                    System.Console.WriteLine("NOT ALLOWED");
                    return;
                }
                removePlayer();
                p = new Position(c.X, c.Y);
                c.X++;
                drawPlayer();
            }
        
        }
        void goLeft() {
            if(c.X>0) {
                if(grid[c.Y][c.X-1] is '1') {
                    System.Console.WriteLine("NOT ALLOWED");
                    return;
                }
                removePlayer();
                p = new Position(c.X, c.Y);
                c.X--;
                drawPlayer();
            }
        
        }

        void showStatus(){
            Console.SetCursorPosition(1,40);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            System.Console.Write("p={0}, c={1}",p, c);
        }

        void removePlayer(){
            Console.SetWindowPosition(0,0);
            Console.SetCursorPosition(p.X,p.Y);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Black;
            System.Console.Write("\u2588");
            System.Console.Out.Flush();
        }

        void drawPlayer(){
            Console.SetWindowPosition(0,0);
            Console.SetCursorPosition(c.X,c.Y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Red;
            System.Console.Write("\u2588");
            System.Console.Out.Flush();
            showStatus();
        }

/*

*/
        while(true){
            var k = Console.ReadKey();
            switch(k.Key){
                case ConsoleKey.UpArrow: goUp(); break;
                case ConsoleKey.DownArrow: goDown(); break;
                case ConsoleKey.LeftArrow: goLeft(); break;
                case ConsoleKey.RightArrow: goRight(); break;
            }
        }
    }

    
}