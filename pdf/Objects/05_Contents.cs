using System.Text;

namespace pdf.Objects;

public struct Contents
{
    public string Reference { get; set; }
    public string Length { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Length {Length} >>
        stream
        endstream
        endobj{'\r'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.ASCII.GetBytes(ToString());
    }
}