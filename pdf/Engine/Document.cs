using System.Text;
using pdf.Objects;

namespace pdf.Engine;

public class Document : IDisposable
{
    private readonly Stream _stream;
    private readonly Xref xref = new Xref() { Refrences = [] };

    private int _nextRef = 1;
    private string NextRef() => $"{_nextRef++} 0";

    public Document(Stream stream)
    {
        _stream = stream;

        float h = 800;
        // float w = 500;

        var catalog = new Catalog
        {
            Reference = NextRef(),
        };

        var outlines = new Outlines { Reference = NextRef(), Count = "0" };

        var pages = new Pages
        {
            Reference = NextRef(),
        };

        var page1 = new Page
        {
            Reference = NextRef(),
            Parent = $"{pages.Reference} R",
            MediaBox = $"[0 0 {h} {h}]",
        };

        // var page2 = new Page
        // {
        //     Reference = NextRef(),
        //     Parent = $"{pages.Reference} R",
        //     MediaBox = $"[0 0 {w} {h}]",
        // };

        var canvas1 = new Contents
        {
            Reference = NextRef(),
            StreamData = $"""
            BT
                /F2 22 Tf
                10 {h-30} Td
                (This is the first line in Times Font) Tj
            ET
            """
        };

        page1.Contents = $"{canvas1.Reference} R";

        // var procedure1 = new Procedure { Reference = NextRef(), Instruction = "/PDF/Text" };
        
        // var canvas2 = new Contents
        // {
        //     Reference = NextRef(),
        //     StreamData =
        //     $"""
        //     BT
        //         /F1 22 Tf
        //         10 {h-60} Td
        //         (Testing having another page only) Tj
        //     ET
        //     """
        // };
        // page2.Contents = $"{canvas2.Reference} R";

        var procedure1 = new Procedure { Reference = NextRef(), Instruction = "/PDF/Text" };
        // var procedure2 = new Procedure { Reference = NextRef(), Instruction = "/PDF/Text" };


        var kids = new[] { page1 };
        pages.Kids = $"[{string.Join(" ", kids.Select(p => $"{p.Reference} R"))}]";
        pages.Count = kids.Length; 
        
        var f1 = new Font
        {
            Reference = NextRef(),
            Subtype = "Type1",
            Name = "F1",
            BaseFont = Font.STANDARD_14_FONTS[(int)StandardFonts.TimesRoman],
            Encoding = "MacRomanEncoding", // StandardEncoding, BaseEncoding, MacRomanEncoding, MacExpertEncoding, or WinAnsiEncoding
        };
        var f2 = new Font
        {
            Reference = NextRef(),
            Subtype = "Type1",
            Name = "F2",
            BaseFont = Font.STANDARD_14_FONTS[(int)StandardFonts.Helvetica],
            Encoding = "MacRomanEncoding",
        };

        var info = new Info(){
            Reference    = NextRef(),
            Title        = "Example PDF Document",
            Author       = "Hussain Al Mutawa",
            Subject      = "Testing creating pdfs",
            Keywords     = "re-inventing the wheel",
            Creator      = "Coded by hand",
            Producer     = "Some program",
            CreationDate = DateTime.Now,
            ModDate      = DateTime.Now,
        };
        var metadata = new Metadata() { Reference = NextRef() } ;

        

        page1.Resources = $"<< /ProcSet {procedure1.Reference} R /Font << /F1 {f1.Reference} R /F2 {f2.Reference} R >> >>";
        // page2.Resources = $"<< /ProcSet {procedure2.Reference} R /Font << /F1 {f1.Reference} R >> >>";


        catalog.Outlines = outlines.Reference + " R";
        catalog.Pages = pages.Reference + " R";
        catalog.Metadata = metadata.Reference + " R";


        _stream.Write("%PDF-1.4\r\n"u8);
        _stream.Write("%нцςп\r\n"u8);
        xref.Refrences.Add("0000000000 65535 f");

        var elements = new object[]{
            catalog,outlines,pages,page1,canvas1,procedure1,f1,f2,info,metadata
        };
        foreach (var el in elements)
        {
            xref.AddXrefPosition(_stream);
            _stream.Write(Encoding.UTF8.GetBytes($"{el}"));
        }
        var trailer = new Trailer()
        {
            ID = Guid.NewGuid(),
            Root = catalog.Reference + " R",
            Info = info.Reference + " R",
            Size = xref.Refrences.Count + "",
            StartXref = _stream.Length + "",
        };
        _stream.Write(Encoding.UTF8.GetBytes($"{xref}"));
        _stream.Write(Encoding.UTF8.GetBytes($"{trailer}"));
        _stream.Write("\r\n%%EOF"u8);
    }

    public void Dispose()
    {
        if (_stream is not null && !_stream.CanWrite)
        {
            _stream.Flush();
            _stream.Close();
        }
        _stream?.Dispose();

        GC.SuppressFinalize(this);
    }

    ~Document() {
        Dispose();
    }
}