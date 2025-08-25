using System.Text;

namespace pdf.Objects;

public enum StandardFonts : int
{
    TimesRoman = 0,
    TimesBold,
    TimesItalic,
    TimesBoldItalic,
    Helvetica,
    HelveticaBold,
    HelveticaOblique,
    HelveticaBoldOblique,
    Courier,
    CourierOblique,
    CourierBold,
    CourierBoldOblique,
    Symbol,
    ZapfDingbats
}
public struct Font
{
    public string Reference { get; set; }
    public string Subtype { get; set; }
    public string Name { get; set; }
    public string BaseFont { get; set; }
    public string Encoding { get; set; }
    public string FontDescriptor { get; set; }
    public int FirstChar { get; set; }
    public int LastChar { get; set; }
    public string Widths { get; set; }

    public static readonly string[] STANDARD_14_FONTS = [
        "Times-Roman",
        "Times-Bold",
        "Times-Italic",
        "Times-BoldItalic",
        "Helvetica",
        "Helvetica-Bold",
        "Helvetica-Oblique",
        "Helvetica-BoldOblique",
        "Courier",
        "Courier-Oblique",
        "Courier-Bold",
        "Courier-BoldOblique",
        "Symbol",
        "ZapfDingbats"
    ];
    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        <<  /Type     /{nameof(Font)}
            /Subtype  /{Subtype}
            /Name     /{Name}
            /BaseFont /{BaseFont}
            /Encoding /{Encoding}
        >>
        endobj

        """;
            // /FirstChar {FirstChar}
            // /LastChar {LastChar}
            // /Widths {Widths}
            // /FontDescriptor {FontDescriptor}
    }
}