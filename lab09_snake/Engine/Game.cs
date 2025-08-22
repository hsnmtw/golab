namespace graphix.Engine;

public class Game
{
    public MoveDirection MoveDirection { get; set; }
    public Snake Snake { get; set; } = new();
    public Action OnMove { get; set; } = () => {};
}