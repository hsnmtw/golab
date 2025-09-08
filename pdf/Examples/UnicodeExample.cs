using System.IO.Compression;
using System.Text;

public static class UnicodeExample
{
    public static void Run()
    {
        using var fs = File.Create("./testing/unicode.pdf");

        List<long> refs = [];

        fs.Write("%PDF-1.5\r\n"u8);
        fs.Write("%нцςп\r\n"u8);
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """
            1 0 obj
            << /Type /Catalog /Outlines 2 0 R /Pages 3 0 R >>
            endobj

            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """
            2 0 obj
            << /Type Outlines /Count 0 >>
            endobj

            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            3 0 obj
            << /Type /Pages /Kids [4 0 R] /Count 1 >>
            endobj

            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            4 0 obj
            << /Type /Page
            /Parent 3 0 R
            /MediaBox [ 0 0 612 792 ]
            /Contents 5 0 R
            /Resources << /ProcSet 6 0 R /Font << /F1 7 0 R >> >>
            >>
            endobj

            """));
        refs.Add(fs.Length);
        var bytes = Encoding.UTF8.GetBytes(
            $"""
            0.0 G                      % Set stroking colour to black
            1.0 1.0 0.0 rg             % Set nonstroking colour to yellow
            25 780 80 -120 re         % Construct rectangular path
            B                          % Fill and stroke path
            25 780 155 -150 re         % Construct rectangular path
            010 700 m                    % move to
            520 700 l                    % line
            %f                          % Fill path
            S                         % stroke only
            BT                         % Begin text object
            /F1 24 Tf                  % Set text font and size
            0 Tc                       % Set character spacing
            0 Tw                       % Set word spacing
            40 720 Td                  % Move text position
            0.0 0.5 1.0 rg             % Set nonstroking colour to red
            ( Test ) Tj
            ET
            """
        );


        bytes = Encoding.UTF8.GetBytes(
            $"""
            BT                         % Begin text object
            /F1 24 Tf                  % Set text font and size
            40 720 Td                  % Move text position
            ( Test ) Tj
            ET
            """
        );
            // ( {"\uFE97\uFEA0\uFEAE\uFE91\uFEEA"} ) Tj
        var scompressed = Compress(bytes);
        fs.Write(Encoding.UTF8.GetBytes(
            $"""    
            5 0 obj
            <</Length2 {bytes.Length} /Length {scompressed.Length} /Filter/FlateDecode />>
            stream
            
            """));

        fs.Write(scompressed);

        fs.Write(Encoding.UTF8.GetBytes(
            """

            endstream
            endobj

            """));    
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            6 0 obj
            [ /PDF /Text ]
            endobj

            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            7 0 obj
            << /Type /Font
            /Subtype /TrueType
            /Name /F1
            /BaseFont /Times#20New#20Roman
            /Encoding /WinAnsiEncoding
            /FontDescriptor 8 0 R      
            /FirstChar 32/LastChar 116  
            /Widths 10 0 R
            >>
            endobj
            
            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            8 0 obj
            << 
            /Type /FontDescriptor
            /FontName/Times#20New#20Roman
            /Flags 32
            /ItalicAngle 0
            /Ascent 891
            /Descent -216
            /CapHeight 693
            /AvgWidth 401
            /MaxWidth 2614
            /FontWeight 400
            /XHeight 250
            /Leading 42
            /StemV 40
            /FontBBox[ -568 -216 2046 693]            
            /FontFile2 9 0 R
            >>
            endobj
            
            """));
        refs.Add(fs.Length);
        var font = File.ReadAllBytes("./times.ttf");
        // using var ms = new MemoryStream();
        // using var ds = new ZLibStream( ms, CompressionMode.Compress, false );

        // ds.Write(font);
        // ds.Close();
        // var compressed = ms.ToArray();
        var compressed = Compress(font);
        fs.Write(Encoding.UTF8.GetBytes(
            $"""    
            9 0 obj
            <</Length {compressed.Length}/Filter/FlateDecode>>
            stream
            
            """));
        fs.Write(compressed);
        fs.Write(Encoding.UTF8.GetBytes(
            """

            endstream
            endobj
            
            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            10 0 obj
            [ 250 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 667 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 444 0 0 0 0 0 0 0 0 0 0 0 0 0 389 333] 
            endobj

            """));
        refs.Add(fs.Length);
        fs.Write(Encoding.UTF8.GetBytes(
            $"""    
            xref
            0 {refs.Count-1}
            0000000000 65535 f
            {string.Join("\n", refs[..^1].Select(x=>$"{x:0000000000} 00000 n"))}

            """
        ));
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            trailer
            << /Size 8 /Root 1 0 R >>

            """
        ));
        fs.Write("startxref\n"u8);
        fs.Write(Encoding.UTF8.GetBytes($"{refs[^1]}\n"));
        fs.Write("%%EOF"u8);

    }

    private static ReadOnlySpan<byte> Compress(byte[] bytes)
    {
        using var ms = new MemoryStream();
        using var ds = new ZLibStream( ms, CompressionMode.Compress, false );
        
        ds.Write(bytes);
        ds.Close();
        return ms.ToArray();
    }
}