using System.IO.Compression;

namespace pdf.Objects;



public struct FontFile
{
    public string Reference { get; set; }
    public int Length { get; set; }
    public int Length1 { get; set; }

    public readonly byte[] Bytes()
    {
        var b2 = Compress(File.OpenRead("./times.ttf"));
        var b1 = System.Text.Encoding.UTF8.GetBytes(
            $"""
            {Reference} obj
            <<  /Filter/FlateDecode/Length {b2.Length} /Length1 {new FileInfo("./times.ttf").Length}
            stream

            """);//


        var b3 = System.Text.Encoding.UTF8.GetBytes(
        """

        endstream
        >>
        endobj

        """);
        return [.. b1, .. b2, .. b3];
    }

    private static byte[] Compress(Stream input)
    {
        using(var compressStream = new MemoryStream())
        using(var compressor = new DeflateStream(compressStream, CompressionMode.Compress, false))
        {
            input.CopyTo(compressor);
            compressor.Close();
            return compressStream.ToArray();
        }
    }
}