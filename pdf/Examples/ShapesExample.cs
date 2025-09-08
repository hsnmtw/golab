using System.Text;

public static class ShapesExample
{
    public static void Run()
    {
        using var fs = File.Create("./testing/shapes.pdf");

        List<long> refs = [];

        fs.Write("%PDF-1.4\r\n"u8);
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
        fs.Write(Encoding.UTF8.GetBytes(
            """    
            5 0 obj
            <<>>
            stream
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
            1.0 0.0 0.0 rg             % Set nonstroking colour to red
            ( Hello World ) Tj
            ET
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
            /Subtype /Type1
            /Name /F1
            /BaseFont /Helvetica
            /Encoding /MacRomanEncoding
            >>
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
}