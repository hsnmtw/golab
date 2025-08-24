using System.Text;

namespace pdf.Objects;

public struct Procedure
{
    public string Reference { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        [/PDF]
        endobj{'\r'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.ASCII.GetBytes(ToString());
    }
}