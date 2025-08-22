using System.Text.RegularExpressions;
using SkiaSharp;

namespace Pdf.Styling;


public static class ColorManager
{
    /// <summary>
    /// tries to convert hex to rgba, if it fails, it returns Black
    /// </summary>
    public static SKColor FromHex(string hex)
    {
        if(Regex.IsMatch(hex,@"^#([A-Fa-f0-9]{3}){1,2}$"))
        {
            var c = hex[1..].ToCharArray();
            if(c.Length == 3)
                c = [c[0], c[0], c[1], c[1], c[2], c[2]];
            var bytes = Convert.FromHexString(c);
            if(bytes.Length == 3)
                return new SKColor(bytes[0],bytes[1],bytes[2]);
        }
        return SKColors.Black;
    }
}