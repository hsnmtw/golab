using System.Text;

namespace pdf.Objects;

public struct Trailer
{
    public string StartXref { get; set; }
    public string Size { get; set; }
    public string Root { get; set; }
    public override readonly string ToString()
    {
        return
        $"""
        trailer
        <<  /Size {Size}
            /Root {Root} 
        >>
        startxref
        {StartXref}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.ASCII.GetBytes(ToString());
    }
}