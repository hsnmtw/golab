using SkiaSharp;

namespace Pdf.Rendering;

public enum Instruction
{
    BeginPage,
    DrawText,
    DrawLine,
    DrawRect,
    EndPage,
}

public struct PdfInstruction
{
    public Instruction Instruction { get; set; }
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }
    public SKFont Font { get; set; }
    public SKPaint Paint { get; set; }
    public string Text { get; set; }
    public PageSizes PageSize { get; set; }

    public readonly SKRect Rect { get => new SKRect(Left,Top,Right,Bottom); }

}