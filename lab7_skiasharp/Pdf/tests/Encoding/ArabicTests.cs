using Pdf.Encoding;

namespace Pdf.Tests.Encoding;

public static class ArabicTest
{
    public static void Test1()
    {
        string input = File.ReadAllText("./data/ar1.txt");
        string expected = File.ReadAllText("./data/ar1-unshaped.txt");

        string actual = AraibcPdf.Transform(input);

        if(!Equals(expected, actual))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write("[FAIL]");
        }
        else
        {        
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.Write("[PASS]");
        }
        Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(" AraibcPdfExtention.ArabicWithFontGlyphsToPfd / Test2 : Archimedis :: {0:P1}", Strings.Compute(expected,actual));
    }

    public static void Test2()
    {
        string input = File.ReadAllText("./data/ar2.txt");
        string expected = File.ReadAllText("./data/ar2-unshaped.txt");

        string actual = AraibcPdf.Transform(input);

        if(!Equals(expected, actual))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write("[FAIL]");
        }
        else
        {        
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.Write("[PASS]");
        }
        Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(" AraibcPdfExtention.ArabicWithFontGlyphsToPfd / Test2 :: Newspaper :: {0:P1}", Strings.Compute(expected,actual));
    }

    public static void Test3()
    {
        string input = File.ReadAllText("./data/ar3.txt");
        string expected = File.ReadAllText("./data/ar3-unshaped.txt");

        string actual = AraibcPdf.Transform(input);

        if(!Equals(expected, actual))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write("[FAIL]");
        }
        else
        {        
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.Write("[PASS]");
        }
        Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(" AraibcPdfExtention.ArabicWithFontGlyphsToPfd / Test3 :: lam alef :: {0:P1}", Strings.Compute(expected,actual));
    }


    static class Strings
    {
        public static float Compute(string s, string t)
        {
            if(string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t)) return -1;
            float longest = Math.Max(s.Length, t.Length);
            float shortest = Math.Min(s.Length, t.Length);
            float unmatch = longest == shortest ? 0 : longest - shortest;
            for(int i=0;i<shortest;i++)
            {
                if(s[i] != t[i]) unmatch++;
            }
            return (longest-unmatch)/longest;
        }
    }

}
