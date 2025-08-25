using System.Text;

namespace pdf.Objects;

public struct Contents
{
    public string Reference { get; set; }
    public readonly int Length => StreamData.Length;
    public string StreamData { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Length {Length} >>
        stream
        {StreamData}
        endstream
        endobj
        
        """;
    }
}