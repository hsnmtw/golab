using Pdf.Encoding;
using Pdf.Styling;
using SkiaSharp;

namespace Pdf.Rendering;

public enum PageSizes {A4}
public delegate void PageHeaderEvent(SKCanvas canvas, int page);
public class PdfDocument(Stream stream, PageSizes pageSize = PageSizes.A4) : IDisposable
{
    public const float A4_WIDTH = 595;
    public const float A4_HEIGHT = 842;    
    
    private readonly SKDocument document = SKDocument.CreatePdf(stream);    
    private Queue<PdfInstruction> instructions = [];

    public void AddInstruction(PdfInstruction instruction)
    {
        if(instruction.Instruction is Instruction.DrawText)
        {
            foreach (var i in TextRenderer.BreakdownInstruction(false,instruction.Text,1,instruction.Rect,instruction.Paint,instruction.Font))
                instructions.Enqueue(i);
            return;
        }
        instructions.Enqueue(instruction);
    }
    
    public void Close()
    { 
        int pages = instructions.Count(x=>x.Instruction==Instruction.BeginPage);
        if(pages<1) return;
        System.Console.WriteLine("\n\n\n[pages={0}]\n\n\n", pages);
        SKCanvas canvas = default!;
        int page = 1;
        while (instructions.Count != 0)
        {
            var i = instructions.Dequeue();
            switch(i.Instruction)
            {
                case Instruction.BeginPage: 
                // System.Console.WriteLine("[pdf] begin page");
                canvas = document.BeginPage(A4_WIDTH,A4_HEIGHT);
                canvas.DrawText(
                    AraibcPdf.Transform($"الصفحة {page++} من {pages}"),
                    A4_WIDTH-50,
                    20,
                    SKTextAlign.Right,
                    new SKFont(),
                    new SKPaint());
                canvas.DrawLine(10,25,A4_WIDTH-10,25,new SKPaint(){Color=SKColors.Red});
                break;
                
                case Instruction.EndPage:
                // System.Console.WriteLine("[pdf] end page");
                document.EndPage();
                break;

                case Instruction.DrawText:
                // System.Console.WriteLine("[pdf] draw text");
                canvas.DrawText(
                    i.Text,
                    i.Left,
                    i.Top,
                    SKTextAlign.Right,
                    i.Font,
                    i.Paint);
                break;
            }
        }
        document.Close();
    }

    public void Dispose()
    {
        Close();
    }
}