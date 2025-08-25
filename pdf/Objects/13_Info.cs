using System.Text;

namespace pdf.Objects;

public struct Info
{
    public string Reference { get; set; }

    public string Title { get; set; }
    public string Author { get; set; }
    public string Subject { get; set; }
    public string Keywords { get; set; }
    public string Creator { get; set; }
    public string Producer { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModDate { get; set; }
    
    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        << /Title        ({Title})
           /Author       ({Author})
           /Subject      ({Subject})
           /Keywords     ({Keywords})
           /Creator      ({Creator})
           /Producer     ({Producer})
           /CreationDate ( D : {CreationDate:yyyyMMddHHmmss} - 00 ' 00 ' )
           /ModDate      ( D : {ModDate     :yyyyMMddHHmmss} - 00 ' 00 ' )
        >>
        endobj
        
        """;
    }

    
}