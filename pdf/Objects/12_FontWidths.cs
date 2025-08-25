using System.IO.Compression;
using System.Text;

namespace pdf.Objects;



public struct FontWidths
{
    public string Reference { get; set; }
    public readonly byte[] Bytes()
    {
        return
        Encoding.UTF8.GetBytes(
        $"""
        {Reference} obj
        [ 584 0 0 0 0 0 0 0 0 0 0 0 0 525 0 0 0 0 0 0 558 0 0 0 0 840 0 543] 
        endobj

        """);
    }
}