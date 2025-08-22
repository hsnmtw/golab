using Pdf.HarfBuzz;
using SkiaSharp;

namespace Pdf.Rendering;

public static class TextRenderer
{
    public static bool CanRectContainAllText(SKRect rect, SKFont font, float lineHeight, string text)
    {
        float start = rect.Left;
        float wordX = rect.Left;
        float wordY = rect.Top;
        float spaceWidth = font.MeasureText(" ");
        string[] lines = text.Split("\r\n");
        foreach(var line in lines)
        {
            var words = line.Split(' ');
            foreach (string word in words)
            {
                float wordWidth = font.MeasureText(word);
                if (wordWidth > rect.Left + wordX)
                {
                    wordY += font.Spacing * lineHeight;
                    wordX = start;
                }
                if(wordY>rect.Bottom) break;
                wordX += wordWidth + spaceWidth;
                
            }
            if(wordY>=rect.Bottom) 
            {
                return false;
            }
            wordY += font.Spacing * lineHeight;
            wordX = start;        
        }

        return true;
    }

    public static IEnumerable<PdfInstruction> BreakdownInstruction(bool ltr, string longText, float lineHeight, SKRect rect, SKPaint paint, SKFont font)
    {       
        string[] lines = ltr 
                        ? longText.Split("\r\n") 
                        : AraibcPdf.Transform(longText).Split("\r\n");
        float spaceWidth = font.MeasureText(" ");
        float start = ltr ? rect.Left : rect.Right;
        float wordX = start;
        float wordY = rect.Top + font.Size;
        foreach(var line in lines)
        {
            var words = ltr ? line.Split(' ') : line.Split(' ').Reverse();
            foreach (string word in words)
            {
                float wordWidth = font.MeasureText(word);
                if (wordX - wordWidth<rect.Left || wordWidth > rect.Left + wordX)
                {
                    wordY += font.Spacing * lineHeight;
                    wordX = start;
                }
                if(wordY>rect.Bottom) break;
                //canvas.DrawText(word, wordX, wordY, ltr?SKTextAlign.Left:SKTextAlign.Right, font, paint);
                yield return new PdfInstruction {
                    Instruction = Instruction.DrawText,
                    Left = wordX,
                    Top = wordY,
                    Bottom = lineHeight + wordY+10,
                    Right = 200,
                    Font = font,
                    Paint = paint,
                    Content = $"{word}"
                };
                wordX += (wordWidth + spaceWidth) * (ltr?1:-1);
                
            }
            if(wordY>=rect.Bottom) 
            {
                yield return new PdfInstruction {
                    Instruction = Instruction.EndPage,
                };
                yield return new PdfInstruction {
                    Instruction = Instruction.BeginPage,
                };                
                wordY = rect.Top;
                wordX = start;            
            }
            wordY += font.Spacing * lineHeight;
            wordX = start;        
        }
    }
    
    public static void DrawText(SKCanvas canvas, bool ltr, string longText, float lineHeight, SKRect rect, SKPaint paint, SKFont font, PdfDocument? pdf = null)
    {
        // if(!CanRectContainAllText(rect,font,lineHeight,longText)) 
        // {
        //     DrawText(
        //         canvas,ltr,
        //         "**** SPACE NOT ENOUGH TO RENDER LONG TEXT ***",
        //         lineHeight,
        //         rect,
        //         paint,
        //         font);
        //     return;
        // }

        // SKCanvas? nextPage = default;

        // Func<SKCanvas> canvas = () => nextPage ?? skCanvas;

        string[] lines = ltr 
                        ? longText.Split("\r\n") 
                        : AraibcPdf.Transform(longText).Split("\r\n");
        float spaceWidth = font.MeasureText(" ");
        float start = ltr ? rect.Left : rect.Right;
        float wordX = start;
        float wordY = rect.Top + font.Size;
        foreach(var line in lines)
        {
            var words = ltr ? line.Split(' ') : line.Split(' ').Reverse();
            foreach (string word in words)
            {
                float wordWidth = font.MeasureText(word);
                if (wordX - wordWidth<rect.Left || wordWidth > rect.Left + wordX)
                {
                    wordY += font.Spacing * lineHeight;
                    wordX = start;
                }
                if(wordY>rect.Bottom) break;
                canvas.DrawText(word, wordX, wordY, ltr?SKTextAlign.Left:SKTextAlign.Right, font, paint);
                wordX += (wordWidth + spaceWidth) * (ltr?1:-1);
                
            }
            if(wordY>=rect.Bottom) 
            {
                if(pdf == null)
                {
                    System.Console.WriteLine("[wrn] unable to render whole text");
                    break;
                }
                // canvas = pdf.AddPage();
                // wordY = rect.Top;
                // wordX = start;            
            }
            wordY += font.Spacing * lineHeight;
            wordX = start;        
        }
    }
}