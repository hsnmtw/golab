using Pdf.Encoding;
using Pdf.Styling;
using SkiaSharp;

namespace Pdf.Rendering;

public static class TableRenderer
{
    public static IEnumerable<PdfInstruction> BreakDownTableInstruction(Table table, float top, SKFont font, PageSizes pageSize = PageSizes.A4)
    {
        float w = PdfDocument.A4_WIDTH;
        float h = PdfDocument.A4_HEIGHT;
        float minRowHeight = font.Spacing + 4;
        if(top+minRowHeight>h)
        {
            top = 60;
            yield return new PdfInstruction{ Instruction = Instruction.EndPage };
            yield return new PdfInstruction{ Instruction = Instruction.BeginPage };
        }
        float sum = table.ColumnWidths.Sum();
        float[] widths = [.. table.ColumnWidths.Select(x=>x/sum)];
        float width = w - 40;
        for(int i=0;i<table.Data.Length;i++)
        {
            float left = 20;
            for(int j=0;j<table.Data[i].Length;j++)
            {
                yield return new PdfInstruction{ 
                    Instruction = Instruction.DrawRect, 
                    Left = left, 
                    Top = top + 2, 
                    Bottom = top + 2 + minRowHeight, Right = left+ widths[j]*width, 
                };
                yield return new PdfInstruction{ 
                    Instruction = Instruction.DrawText, 
                    Left = left+13, 
                    Top = top+15, 
                    Bottom = top + minRowHeight - 6, Right = left + widths[j]*width - 6, 
                    Paint = new SKPaint(),
                    Content = AraibcPdf.Transform($"{table.Data[i][j]}")
                };
                left += widths[j]*width;
            }
            if(top + minRowHeight > h)
            {
                top = 60;
                yield return new PdfInstruction{ Instruction = Instruction.EndPage };
                yield return new PdfInstruction{ Instruction = Instruction.BeginPage };  
                continue;              
            }
            top += minRowHeight; 
        }
    }
}