using UglyToad.PdfPig.Rendering.Skia;
using UglyToad.PdfPig;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Pdf.HarfBuzz;

var document = SKDocument.CreatePdf("output.pdf");
var paint = new SKPaint
{
    Color = SKColors.Blue,
};  
var tf = SKFontManager.Default.MatchCharacter(0x1f600);// SKTypeface.FromFile("NotoNaskh-Regular.ttf");
// using var tf = SKTypeface.FromFamilyName("Segoe UI");//.FromFile(@"c:\Windows\Fonts\times.ttf");//NotoNaskhArabic-Regular.ttf");
// if (tf.ContainsGlyphs("😍")) System.Console.WriteLine("YES");;
var font = new SKFont(tf);
var shaper = new SKShaper(tf);
using (var canvas = document.BeginPage(612, 792))
{
    
    // canvas.DrawShapedText("🐈‍⬛🐈👩🏽‍🚒👩🏼‍🎨🙂😉😍😶‍🌫️🤡🥸👧🏾🤢🤮🤬👩🏽‍🦰🎅🏽👨🏽‍🦽", 100, 100, SKTextAlign.Left, font, paint);
    // canvas.DrawText(AraibcPdf.Transform("مرحبا, PDF! 🐈‍⬛🐈👩🏽‍🚒👩🏼‍🎨🙂😉😍😶‍🌫️🤡👧🏾🤢🤮🤬👩🏽‍🦰🎅🏽👨🏽‍🦽"), 100, 100, SKTextAlign.Left, font, paint);
    canvas.DrawShapedText(shaper,"مرحبا, PDF! 🐈‍⬛🐈👩🏽‍🚒👩🏼‍🎨🙂😉😍😶‍🌫️🤡🥸👧🏾🤢🤮🤬👩🏽‍🦰🎅🏽👨🏽‍🦽", 100, 100, SKTextAlign.Left, font, paint);
    document.EndPage();
}
document.Close();

// Task.Delay(5000).Wait();
// QuestPDF.Settings.License = LicenseType.Community;
// Document.Create(pdf =>
// {
//     pdf.Page(page =>
//     {
//         page.Size(612, 792);
//         page.Margin(50);
//         page.Content().Element(e =>
//         {
//             e.AlignLeft()
//              .Text("مرحبا, PDF! 🐈‍⬛🐈👩🏽‍🚒👩🏼‍🎨🙂😉😍😶‍🌫️🤡🥸👧🏾🤢🤮🤬👩🏽‍🦰🎅🏽👨🏽‍🦽")
//              .FontFamily("Noto Naskh Arabic")
//              .FontSize(24);
//         });
//     });
    
// }).GeneratePdf("output.pdf");

// extract("output.pdf");

// static void extract(string _path){
//     using (var document = PdfDocument.Open(_path, SkiaRenderingParsingOptions.Instance))
//     {
//         string fileName = Path.GetFileName(_path);

//         document.AddSkiaPageFactory(); // Same as document.AddPageFactory<SKPicture, SkiaPageFactory>()
//         for (int p = 1; p <= document.NumberOfPages; p++)
//         {
//             using (var fs = new FileStream($"{fileName}_{p}.png", FileMode.Create))
//             using (var ms = document.GetPageAsPng(p,1,100))
//             {
//                 ms.WriteTo(fs);
//             }
//         }
//     }
// }