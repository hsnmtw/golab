
using System.Text;
using System.Text.Json.Serialization;
using json = System.Text.Json.JsonSerializer;

enum Direction { NONE,UP,DOWN,LEFT,RIGHT }

record struct Point(int X, int Y)
{
    // public int X { get; set; }
    // public int Y { get; set; }
    [JsonIgnore]
    public readonly Point N => new(X,Y-1);
    [JsonIgnore]
    public readonly Point S => new(X,Y+1);
    [JsonIgnore]
    public readonly Point W => new(X-1,Y);
    [JsonIgnore]
    public readonly Point E => new(X+1,Y);
    [JsonIgnore]
    public static Point Empty { get; } = new Point(-1,-1);
}

record Board(int Width, int Height)
{
    public Point Food { get; set; } = Point.Empty;
}

class Snake 
{
    public Point Head { get; set; }
    [JsonIgnore]

    public bool IsAlive { get; set; } = true;
    [JsonIgnore]
    public Point[] Body { get; set; } = [];
    [JsonIgnore]
    public Direction LastDirection { get; set; }

    public void Move(Direction direction, Board board, Action<bool>? action)
    {
        if(!IsAlive||direction == Direction.NONE) return;
        
        LastDirection = direction;
        
        action?.Invoke(Head == board.Food);

        

        // if(Head == board.Food) throw new Exception("???");

        Body = [Head,..Body[..^(Head == board.Food ? 0 : 1)]];

        // var nb = new Point[ (Head == board.Food ? 1 : 0) + Body.Length ];
        // nb[0] = Head;
        // for(int i=1;i<nb.Length;i++){
        //     if(i+1<Body.Length) nb[i] = Body[i+1];
        // }
        // Body = nb;

        // var newBody = new Point[ (board.Food == Head ? 1 : 0) + Body.Length ];
        // newBody[0] = Head;
        // for(int i=1;i<Body.Length-1;i++){
        //     newBody[i] = Body[i+1];
        // }
        // Body = newBody;

        if(Head == board.Food)
            board.Food = Point.Empty;

        Head = LastDirection switch {
            Direction.UP => Head.N,
            Direction.DOWN => Head.S,
            Direction.LEFT => Head.W,
            Direction.RIGHT => Head.E,
            _ => Head
        };



        var(x,y) = Head;
        var(w,h) = board;
        IsAlive=!(x<0||x>w||y<0||y>h||Body.Contains(Head));

        // File.AppendAllLines("c:/temp/move.txt",[
        //     json.Serialize(new{
        //         board,
        //         snake=this
        //     })
        // ]);
    }
}

class MyForm : Form
{
    private readonly Snake _snake; 
    private readonly Board _board; 
    public MyForm(Snake snake, Board board)
    {
        _snake = snake;
        _board = board;
        Redraw();
        KeyUp += (s,e) => {
            // Redraw();
            snake.LastDirection = e.KeyCode switch
            {
                Keys.Up => Direction.UP,
                Keys.Down => Direction.DOWN,
                Keys.Left => Direction.LEFT,
                Keys.Right => Direction.RIGHT,
                _ => Direction.NONE
            };
            // Text = $"{snake.LastDirection}";

            snake.Move(snake.LastDirection,board, 
                eaten => { Text = ""+(eaten); }
            );
            // if(snake.LastDirection != Direction.NONE) Redraw();
            if(board.Food == Point.Empty)
                // board.Food = new Point(14,11);
                board.Food = new Point(Random.Shared.Next(1,board.Width/2-1),Random.Shared.Next(1,board.Height/2-1));

            Redraw();
        };
    }

    private static bool draown = false; 
    const int factor = 25;
    public void Redraw(){
        var gfx = this.CreateGraphics();
            gfx.FillRectangle(Brushes.Black, new RectangleF(0, 0, Width, Height));
        
        var size = new SizeF(factor,factor);
        void DrawCell(Point p, Brush brush, Pen pen)
        {
            gfx.DrawRectangle(pen,new RectangleF(-1+p.X*factor,-1+p.Y*factor,factor+1,factor+1));
            gfx.FillRectangle(brush,new RectangleF(new PointF(p.X*factor,p.Y*factor),size));
        }

        foreach(var p in _snake.Body)
            DrawCell(p,Brushes.White,Pens.Red);
    
        if(_board.Food != Point.Empty)
            DrawCell(_board.Food,Brushes.Green, Pens.Wheat);

        DrawCell(_snake.Head,Brushes.Red, Pens.Wheat);
        
    }
} 

public class Program
{
    [STAThread]
    static void Main()
    {
    //     Point p1 = new Point(10,10);
    //     Point p2 = new Point(20,10);
    //     Point p3 = new Point(10,10);

    //     System.Console.WriteLine(p1==p2);
    //     System.Console.WriteLine(p2==p3);
    //     System.Console.WriteLine(p1==p3);
    
        // int h = 0;
        // int[] body = [1,2,3,4,5];

        // System.Console.WriteLine(json.Serialize(body)); // 1..5

        // body = [1,2,3,4,5];
        // body = [h,..body[..^1]];
        // System.Console.WriteLine(json.Serialize(body)); /// 0..4
        
        // body = [1,2,3,4,5];
        // body = [h,..body];
        // System.Console.WriteLine(json.Serialize(body)); /// 0..5

    // }
    // void test()
    // {
        ApplicationConfiguration.Initialize();

        Board board = new(50,50);
        var h = new Point(10,10);
        Snake snake = new()
        {
            Head = h,
            Body = [
                h.W,
                h.W.W,
                h.W.W.W,
                h.W.W.W.W,
            ]
        };
        MyForm form = new MyForm(snake,board){
            Size = new Size(500,500),
            FormBorderStyle = FormBorderStyle.FixedSingle,
            StartPosition = FormStartPosition.CenterScreen
        };
        // var direction = Direction.NONE;
        
        var timer = new System.Timers.Timer(TimeSpan.FromSeconds(0.1));
        timer.Elapsed += (s,e) => {
            form.Redraw();
            // System.Console.WriteLine("????????");
            //Print(snake,board);
            // if(Console.KeyAvailable)
            // {
            //     ConsoleKeyInfo k = Console.ReadKey(true);
            //     direction = k.Key switch
            //     {
            //         ConsoleKey.UpArrow => Direction.UP,
            //         ConsoleKey.DownArrow => Direction.DOWN,
            //         ConsoleKey.LeftArrow => Direction.LEFT,
            //         ConsoleKey.RightArrow => Direction.RIGHT,
            //         _ => Direction.NONE
            //     };
            // }
            // direction = snake.LastDirection;
            snake.Move(snake.LastDirection,board, eaten => { form.Text = ""+(eaten); });
            // if(board.Food == Point.Empty)
            //     board.Food = new Point(Random.Shared.Next(1,board.Width-1),Random.Shared.Next(1,board.Height-1));
        };
        timer.Enabled = true;
        Application.Run(form);
    }

    private static void Print(Snake snake, Board board)
    {
        static string Repeat(int n,char c) 
        {
            StringBuilder sb = new();
            int i=0;
            while(i++<n) sb.Append(c);
            return sb.ToString();
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(0,0);
        var empty = Repeat(board.Width,' ');
        for(int i=0;i<board.Height;i++) 
            Console.WriteLine(empty);
        Point[] points = [snake.Head,..snake.Body];
        foreach(var point in points){
            Console.SetCursorPosition(point.X,point.Y);
            System.Console.Write('\u25A0');
        }    
        if(board.Food != Point.Empty)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(board.Food.X,board.Food.Y);
            System.Console.Write('\u25A0');
        }

        Console.SetCursorPosition(0,board.Height-snake.Head.Y-10);
    }
}