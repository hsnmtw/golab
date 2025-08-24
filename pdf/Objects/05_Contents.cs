using System.Text;

namespace pdf.Objects;

public struct Contents
{
    public string Reference { get; set; }
    public string Length { get; set; }
    public string StreamData { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Length {Length} >>
        stream
        BT
        {StreamData}
        ET
        endstream
        endobj{'\r'}{'\n'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }
}