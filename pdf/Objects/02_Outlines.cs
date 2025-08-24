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
        endobj{'\r'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.ASCII.GetBytes(ToString());
    }
}