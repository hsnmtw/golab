using System.Text;

namespace pdf.Objects;

public struct Page
{
    public string Reference { get; set; }
    public string Parent { get; set; }
    public string MediaBox { get; set; }
    public string Contents { get; set; }
    public string Resources { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Type /{nameof(Page)}
           /Parent {Parent}
           /MediaBox {MediaBox}
           /Contents {Contents}
           /Resources {Resources}
        >>
        endobj{'\r'}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.ASCII.GetBytes(ToString());
    }
}