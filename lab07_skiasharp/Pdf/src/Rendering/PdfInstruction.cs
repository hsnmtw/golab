using SkiaSharp;

namespace Pdf.Rendering;

public enum Instruction
{
    BeginPage,
    DrawText,
    DrawImage,
    DrawTable,
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
    public string Content { get; set; }
    public Table Table { get; set; }
    public SKColor Fill { get; set; }
    public readonly SKRect Rect { get => new SKRect(Left,Top,Right,Bottom); }
    public readonly SKPoint Point { get => new SKPoint(Left,Top); }

}

public struct Table {
    public float[] ColumnWidths { get; set; }
    public object[][] Data { get; set; }
}