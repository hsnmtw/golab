using System.Text;

namespace pdf.Objects;

public struct Procedure
{
    public string Reference { get; set; }
    public string Instruction { get; set; }

    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        [{Instruction}]
        endobj
        
        """;
    }

    
}