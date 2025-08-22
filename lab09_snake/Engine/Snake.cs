namespace graphix.Engine;

public class Snake
{
    public PointF Head { get; set; }
    public List<PointF> Tail { get; set; }
    const float INCREMENT = 50;

    public void Move(MoveDirection moveDirection)
    {
        switch(moveDirection){
            case MoveDirection.RIGHT: moveRight(); break;
            case MoveDirection.LEFT: moveLeft(); break;
            case MoveDirection.DOWN: moveDown(); break;
            case MoveDirection.UP: moveUp(); break;
        }
    }

    private void moveTail()
    {
        for(int i=Tail.Count-1;i>0;i--)
        {
            var x = Tail[i-1].X;
            var y = Tail[i-1].Y;
            Tail[i] = new(x,y);
        }
        Tail[0] = new(Head.X,Head.Y);
    }

    private void moveRight()
    {
        System.Console.WriteLine("i am moving right: {0}", Head);
        moveTail();
        Head = new PointF(Head.X+INCREMENT, Head.Y);
    }

    private void moveLeft()
    {
        moveTail();
        Head = new PointF(Head.X-INCREMENT, Head.Y);
    }

    private void moveUp()
    {
        moveTail();
        Head = new PointF(Head.X, Head.Y-INCREMENT);
    }

    private void moveDown()
    {
        moveTail();
        Head = new PointF(Head.X, Head.Y+INCREMENT);
    }


    public Snake()
    {
        Head = new PointF(450-(INCREMENT*0),150);
        Tail = [
               new PointF(450-(INCREMENT*1),150),
               new PointF(450-(INCREMENT*2),150),
               new PointF(450-(INCREMENT*3),150),
               new PointF(450-(INCREMENT*4),150),
               new PointF(450-(INCREMENT*5),150),
        ];
    }
}