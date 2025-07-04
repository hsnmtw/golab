using SkiaSharp;
using AraibcPdfUnicodeGlyphsResharper;


string longText = 
"""
ارخميديس (باليونانية Αρχιμήδης وتلفظ [aɾ.çi.ˈmi.ðis] و [ar.kʰi.ˈmɛ:.dɛ:s] عند الأقدمين). أو أرشميدس كان عالم رياضيات يونانى ، فيزيائى ، ومهندس ، و مخترع ، وعالم فلك.
حياته

ارخميديس اتولد سنة 287 قبل الميلاد فى سيراكوسا ، ورغم ان مش كل تفاصيل حياته معروفه ، لكنه كان واحد من ابرز العلما فى العصور القديمه . ارشميدس اسس علم استاتيكا الموائع وشرح مبدأ العتله و صمم الات مبتكره ، بما فيها المحركات والمضخات اللولبيه اللى اتسمت على اسمه. و هو اللى اكتشف قانون الطفو
وفاه

ارخميديس مات سنة 212 قبل الميلاد 

"""
;

const float A4_WIDTH = 595;
const float A4_HEIGHT = 842;

using var document = SKDocument.CreatePdf("./mypdf.pdf");

using var canvas = document.BeginPage(A4_WIDTH, A4_HEIGHT);

DrawText(
    canvas, 
    false, 
    longText, 
    1.1f, 
    new SKRect(20,20, A4_WIDTH-30, 230),
    new SKPaint(){ Color = SKColors.DarkBlue }, 
    new SKFont(SKFontManager.Default.CreateTypeface(@"c:\Windows\Fonts\segoeui.ttf"))
    );

canvas.Save();

document.Close();

static void DrawText(SKCanvas canvas, bool ltr, string longText, float lineHeight, SKRect rect, SKPaint paint, SKFont font)
{
    string[] lines = ltr 
                     ? longText.Split("\r\n") 
                     : AraibcPdfExtention.ArabicWithFontGlyphsToPfd(longText).Split("\r\n");
	float spaceWidth = font.MeasureText(" ");
    
    canvas.DrawRect(rect,new SKPaint{
        Style = SKPaintStyle.Stroke,
        Color = SKColors.BlueViolet,
    });
    canvas.DrawRect(new SKRect(rect.Left+5,rect.Top+5,rect.Right-5,rect.Bottom-5),new SKPaint{
        Style = SKPaintStyle.Fill,
        Color = new SKColor(55,255,11,100),
    });
    rect = new SKRect(rect.Left+10,rect.Top+10,rect.Right-10,rect.Bottom-10);
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
            System.Console.WriteLine("[wrn] unable to render whole text");
            break;
        }
        wordY += font.Spacing * lineHeight;
        wordX = start;        
    }
}



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