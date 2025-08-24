using System.Text;

namespace pdf.Objects;

public struct Font
{
    public string Reference { get; set; }
    public string Subtype { get; set; }
    public string Name { get; set; }
    public string BaseFont { get; set; }
    public string Encoding { get; set; }
    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        <<  /Type     {nameof(Font)}
            /Subtype  {Subtype}
            /Name     {Name}
            /BaseFont {BaseFont}
            /Encoding {Encoding}
        >>
        endobj
        """;
    }

    public readonly byte[] Bytes()
    {
        return System.Text.Encoding.UTF8.GetBytes(ToString());
    }
}