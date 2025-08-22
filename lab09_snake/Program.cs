using graphix.Engine;

namespace graphix;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var _game = new Game();
        
            System.Console.WriteLine("hhhhhh");
            var timer = new System.Timers.Timer(500);
            timer.Elapsed += (s,e) => {
                System.Console.WriteLine("moving: {0} // {1}", _game.MoveDirection, _game.Snake.Head);
                _game.Snake.Move(_game.MoveDirection);
                _game.OnMove();
            };
            timer.Enabled=true;
        
        Application.Run(new MainForm(_game));
    }    
}