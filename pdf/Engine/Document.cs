using pdf.Objects;

namespace pdf.Engine;

public class Document : IDisposable
{
    private readonly Stream _stream;
    private readonly Xref xref = new Xref() { Refrences = [] };

    public Document(Stream stream)
    {
        _stream = stream;
        _stream.Write("%PDF-1.7\r\n"u8);
        _stream.Write("%нцςп\r\n"u8);
        xref.Refrences.Add("0000000000 65535 f");
        xref.Add(_stream); _stream.Write(new Catalog { Reference = "1 0", Outlines = "2 0 R", Pages = "3 0 R" }.Bytes());
        xref.Add(_stream); _stream.Write(new Outlines { Reference = "2 0", Count = "0" }.Bytes());
    }

    public void Page(float w, float h, string text)
    {        
        xref.Add(_stream); _stream.Write(new Pages { Reference = "3 0", Kids = "[4 0 R]", Count = "1" }.Bytes());
        xref.Add(_stream); _stream.Write(new Page { Reference = "4 0", Parent = "3 0 R", MediaBox = $"[0 0 {w} {h}]", Contents = "5 0 R", Resources = "<< /ProcSet 6 0 R >>" }.Bytes());
        xref.Add(_stream); _stream.Write(new Contents { Reference = "5 0", Length = "0", StreamData =
        """
        /F1 24 Tf
        10 10 Td
        (Hello World) Tj
        """}.Bytes());
        xref.Add(_stream); _stream.Write(new Procedure { Reference = "6 0", Instruction = "/PDF /Text" }.Bytes());
        xref.Add(_stream); _stream.Write(new Font
        {
            Reference = "7 0",
            Subtype = "Type1",
            Name = "F1",
            BaseFont = "Helvetica",
            Encoding = "MacRomanEncoding"
        }.Bytes());
        
    }

    public void Close()
    {
        var startXref = _stream.Length + "";
        _stream.Write(xref.Bytes());
        _stream.Write("\r\n"u8);
        _stream.Write(new Trailer() { Root = "1 0 R", Size = xref.Refrences.Count + "", StartXref = startXref }.Bytes());

        _stream.Write("%%EOF"u8);

        //Dispose();
    }

    public void Dispose()
    {
        if (_stream is not null && !_stream.CanWrite)
        {
            _stream.Flush();
            _stream.Close();
        }
        _stream?.Dispose();
    }

    ~Document() {
        Dispose();
    }
}