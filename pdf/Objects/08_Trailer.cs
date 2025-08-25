using System.Text;

namespace pdf.Objects;

public struct Trailer
{
    public string StartXref { get; set; }
    public string Size { get; set; }
    public string Info { get; set; }
    public string Root { get; set; }
    public Guid ID { get; set; }
    public override readonly string ToString()
    {
        return
        $"""
        trailer
        <<  /Size {Size}
            /Root {Root} 
            /Info {Info} 
            /ID   [<{ID.ToString("n").ToUpper()}>]
        >>
        startxref
        {StartXref}
        """;
    }

    
}