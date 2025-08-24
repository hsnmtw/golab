using System.Text;

namespace pdf.Objects;

public struct Outlines
{
    public string Reference { get; set; }
    public string Count { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Type /{nameof(Outlines)}
           /Count {Count}
        >>
        endobj{'\r'}{'\n'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }
}