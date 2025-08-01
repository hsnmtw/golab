using SkiaSharp;
using Pdf.Styling;
using Pdf.Rendering;
using Pdf.HarfBuzz;

//run tests
Pdf.Tests.TestsRunner.Run();

string longText = //File.ReadAllText("./data/long.txt");
"""
بعض الكلام العربي

وقت رفع البيان ١٤٤٧/٠١/١٢هـ 2025/07/07 م الساعة 03:44

يليه كلام عربي و English Text composed of many words in between two arabic words بنفس السطر

Start with English و ينتهي عربي

يبدأ عربي and ends with English
""";
// ;

File.WriteAllText("./arch.txt", AraibcPdf.Transform(longText));

using var fs = File.Create("./mypdf.pdf");
using var pdf = new PdfDocument(fs);
// var font = new SKFont(SKFontManager.Default.CreateTypeface(@"c:\Windows\Fonts\segoeui.ttf")) { ScaleX = 1.1f };
pdf.Font = new SKFont(SKFontManager.Default.CreateTypeface(@"c:\Windows\Fonts\timesbd.ttf")) { ScaleX = 1f, Size = 12 };
var paint = new SKPaint(){ Color = ColorManager.FromHex("#333") };
pdf.AddInstruction(new PdfInstruction{ Instruction = Instruction.BeginPage });
pdf.AddInstruction(new PdfInstruction{ Instruction = Instruction.DrawImage, Left = 10, Top = 10, Right = 50, Bottom = 50, Content = "./data/logo.jpg"});


pdf.AddInstruction(new PdfInstruction{ 
    Instruction = Instruction.DrawTable, 
    Table = new Table {
        ColumnWidths = [1,1,1], // equaly distributed
        Data = [
            [1,2,3],
            [4,5,6],
            [7,8,9]
        ]
    }
});


pdf.AddInstruction(new PdfInstruction{
    Instruction = Instruction.DrawText,
    Top = 130,
    Left = 20,
    Right = PdfDocument.A4_WIDTH-30, 
    Bottom = PdfDocument.A4_HEIGHT - 70,
    Paint = paint,
    Font = pdf.Font,
    Content = longText
});

pdf.AddInstruction(new PdfInstruction{ Instruction = Instruction.EndPage });
pdf.Close();

ColorManager.FromHex("#ffeecc");
ColorManager.FromHex("#fbafff");


// static void DrawText(SKCanvas canvas, bool ltr, string longText, float lineHeight, SKRect rect, SKPaint paint, SKFont font)
// {
//     string[] lines = ltr 
//                      ? longText.Split("\r\n") 
//                      : AraibcPdfExtention.ArabicWithFontGlyphsToPfd(longText).Split("\r\n");
// 	float spaceWidth = font.MeasureText(" ");
    
//     canvas.DrawRect(rect,new SKPaint{
//         Style = SKPaintStyle.Stroke,
//         Color = ColorManager.FromHex("#333"),
//     });
//     canvas.DrawRect(new SKRect(rect.Left+5,rect.Top+5,rect.Right-5,rect.Bottom-5),new SKPaint{
//         Style = SKPaintStyle.Fill,
//         Color = ColorManager.FromHex("#ffeecc"),
//     });
//     rect = new SKRect(rect.Left+10,rect.Top+10,rect.Right-10,rect.Bottom-10);
//     float start = ltr ? rect.Left : rect.Right;
// 	float wordX = start;
// 	float wordY = rect.Top + font.Size;
//     foreach(var line in lines)
//     {
//         var words = ltr ? line.Split(' ') : line.Split(' ').Reverse();
//         foreach (string word in words)
//         {
//             float wordWidth = font.MeasureText(word);
//             if (wordX - wordWidth<rect.Left || wordWidth > rect.Left + wordX)
//             {
//                 wordY += font.Spacing * lineHeight;
//                 wordX = start;
//             }
//             if(wordY>rect.Bottom) break;
//             canvas.DrawText(word, wordX, wordY, ltr?SKTextAlign.Left:SKTextAlign.Right, font, paint);
//             wordX += (wordWidth + spaceWidth) * (ltr?1:-1);
            
//         }
//         if(wordY>=rect.Bottom) 
//         {
//             System.Console.WriteLine("[wrn] unable to render whole text");
//             break;
//         }
//         wordY += font.Spacing * lineHeight;
//         wordX = start;        
//     }
// }



// QuestPDF.Settings.License = LicenseType.Community;
// var pdf = QuestPDF.Fluent.Document.Create(document => {
//     document.Page(page => {
//         page.Size(PageSizes.A4);
//         page.Content().Element(el => {
//             el.Column(col => {
//                 col.Item().Text(longText);
//             });
//         });
//     });
// });

// pdf.GeneratePdf("./quest.pdf");