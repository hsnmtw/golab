
using System.Text;
var xref = new Dictionary<string, Int64>();
using var stream = File.Create("./testing/first.pdf");
stream.Write("%PDF-1.7\r"u8);
stream.Write("%нцςп\r"u8);
xref.Add("1 0 R", stream.Length);
stream.Write(
"""
1 0 obj
    <<
        /Type /Catalog 
        /Pages 2 0 R
        /Lang(en-US) 
        /StructTreeRoot 10 0 R
        /MarkInfo
            <<
                /Marked true
            >>
    >>
endobj
"""u8);
stream.Write("\r"u8);
xref.Add("2 0 R", stream.Length);
stream.Write(
"""
2 0 obj
    <<
        /Type Outlines
        /Count 0
    >>
endobj
"""u8);
stream.Write("\r"u8);
xref.Add("3 0 R", stream.Length);
stream.Write(
"""
3 0 obj
    <<
        /Type Pages
        /Kids [4 0 R]
        /Count 1
    >>
endobj
"""u8);
stream.Write("\r"u8);
xref.Add("4 0 R", stream.Length);
stream.Write(
"""
4 0 obj
    <<
        /Type Page
        /Parent 3 0 R
        /MediaBox [0 0 612 792]
        /Contents 5 0 R
    >>
endobj
"""u8);
stream.Write("\r"u8);
xref.Add("5 0 R", stream.Length);
stream.Write(
"""
5 0 obj
    <<
        /Length 0
    >>
    stream
    endstream
endobj
"""u8);
stream.Write("\r"u8);
xref.Add("6 0 R", stream.Length);
stream.Write(
"""
6 0 obj
    [/PDF]
endobj
"""u8);
stream.Write("\r"u8);

var startxref = stream.Length;

stream.Write(
Encoding.ASCII.GetBytes($"""
xref
0 {xref.Count}
0000000000 65535 f
{
    string.Join("\r\n", xref.Select(x=>x).OrderBy(x=>x.Key).Select(x=>$"{x.Value:0000000000} 00000 n"))
}
"""));
stream.Write("\r"u8);

stream.Write(
Encoding.ASCII.GetBytes($"""
trailer
    <<
        /Size {xref.Count}
        /Root 1 0 R
    >>
"""));
stream.Write("\r"u8);

stream.Write("startxref\r"u8);
stream.Write(Encoding.ASCII.GetBytes($"{startxref}\r"));
stream.Write("%%EOF"u8);