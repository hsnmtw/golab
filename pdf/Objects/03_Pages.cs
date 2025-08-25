using System.Text;

namespace pdf.Objects;

public struct Pages
{
    public string Reference { get; set; }
    public string Kids { get; set; }
    public int Count { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Type /{nameof(Pages)}
           /Kids {Kids}
           /Count {Count}
        >>
        endobj
        
        """;
    }

    
}