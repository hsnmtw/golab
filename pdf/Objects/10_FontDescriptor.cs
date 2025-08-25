namespace pdf.Objects;

public enum FontDescriptorAttributes
{
    FontName, FontFamily, FontStretch, FontWeight, Flags, FontBBox,
    ItalicAngle, Ascent, Descent, Leading, CapHeight, XHeight,
    StemV, StemH, AvgWidth, MaxWidth, MissingWidth, FontFile,
    FontFile2, FontFile3, CharSet,
}

public struct FontDescriptor
{
    public string Reference { get; set; }
    public Dictionary<FontDescriptorAttributes, string> Attributes { get; set; }
    public override readonly string ToString()
    {
        return
        $"""
        {Reference} obj
        <<  /Type     /{nameof(FontDescriptor)}
        {string.Join("\r\n", Attributes.Select(x => $"    /{x.Key} {x.Value}"))}
            /ItalicAngle 0/Ascent 891/Descent -216/CapHeight 693/AvgWidth 401/MaxWidth 2614/FontWeight 400/XHeight 250/Leading 42/StemV 40/FontBBox[ -568 -216 2046 693] 
        >>
        endobj

        """;
    }
}