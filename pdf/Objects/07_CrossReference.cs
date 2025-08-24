using System.Text;

namespace pdf.Objects;

public struct Xref
{
    public List<string> Refrences { get; set; }

    public readonly void Add(Stream stream)
    {
        Refrences.Add(string.Format("{0:0000000000} 00000 n", stream.Length));    
    }

    public override readonly string ToString()
    {
        return
        $"""
        xref
        0 {Refrences.Count}
        {string.Join("\r\n", Refrences)}
        """;
    }

    public readonly byte[] Bytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }
}