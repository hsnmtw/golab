using System.Text;

namespace pdf.Objects;

public struct Catalog
{
    public string Reference { get; set; }
    public string Outlines { get; set; }
    public string Pages { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Type /{nameof(Catalog)}
           /Outlines {Outlines}
           /Pages {Pages}
        >>
        endobj{'\r'}{'\n'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }
}