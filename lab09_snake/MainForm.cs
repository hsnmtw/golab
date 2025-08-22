using graphix.Engine;

namespace graphix;




public class MainForm : Form
{
    private Brush gameBackground = Brushes.Blue;
    private Game _game;
    public MainForm(Game game)
    {
        _game = game;

        Height = 600;
        Width = 800;
        StartPosition = FormStartPosition.CenterScreen;
        
        
    
        RefreshGame();

        _game.OnMove = ()=>RefreshGame();
        
        Label label = new Label(){
            Font = new Font("Arial Bold", 20),
            Location = new Point(20,20),
            Size = new Size(400,50),
            Text = "Press any Key"
        };
        Controls.Add(label);

        KeyDown += (s,e) => {
            switch(e.KeyCode)
            {
                case Keys.Down: 
                label.Text = "Down";
                _game.MoveDirection = MoveDirection.DOWN;
                break;

                case Keys.Up: 
                label.Text = "Up";
                _game.MoveDirection = MoveDirection.UP;
                break;

                case Keys.Left: 
                label.Text = "Left";
                _game.MoveDirection = MoveDirection.LEFT;
                break;

                case Keys.Right: 
                label.Text = "Right";
                _game.MoveDirection = MoveDirection.RIGHT;
                break;
            }
        };


    }

    private void RefreshGame()
    {
        System.Console.WriteLine("REFRESH ...");
        static void DrawSegment(Graphics gfx,PointF point)
        {
            gfx.DrawRectangle(Pens.White, new RectangleF(point,new SizeF(50,50)));
            gfx.FillRectangle(Brushes.YellowGreen, new RectangleF(new PointF(point.X+5,point.Y+5),new SizeF(40,40)));
        }
        //Paint += (s,e) => {
            Graphics gfx = this.CreateGraphics();
            gfx.FillRectangle(gameBackground, new RectangleF(new PointF(0,0), Size));
            var snake = _game.Snake;
            DrawSegment(gfx,snake.Head);
            snake.Tail.ForEach(x=>DrawSegment(gfx,x));
        //};
    }


}