using System.Text;

namespace pdf.Objects;

public struct Metadata
{
    public string Reference { get; set; }
    
    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Type /{nameof(Metadata)}
           /Length 0
        >>
        endobj
        
        """;
    }

    
}