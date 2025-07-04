using System.Text;

namespace Pdf.Encoding;

public static class AraibcPdf
{

    private static readonly HashSet<char> ARABIC_ALPHABET = [
        '\u0622', '\uFE81', '\uFE81', '\uFE82', '\uFE82',
        '\u0623', '\uFE83', '\uFE83', '\uFE84', '\uFE84',
        '\u0624', '\uFE85', '\uFE85', '\uFE86', '\uFE86',
        '\u0625', '\uFE87', '\uFE87', '\uFE88', '\uFE88',
        '\u0626', '\uFE89', '\uFE8B', '\uFE8C', '\uFE8A',
        '\u0627', '\u0627', '\u0627', '\uFE8E', '\uFE8E',
        '\u0628', '\uFE8F', '\uFE91', '\uFE92', '\uFE90',
        '\u0629', '\uFE93', '\uFE93', '\uFE94', '\uFE94',
        '\u062A', '\uFE95', '\uFE97', '\uFE98', '\uFE96',
        '\u062B', '\uFE99', '\uFE9B', '\uFE9C', '\uFE9A',
        '\u062C', '\uFE9D', '\uFE9F', '\uFEA0', '\uFE9E',
        '\u062D', '\uFEA1', '\uFEA3', '\uFEA4', '\uFEA2',
        '\u062E', '\uFEA5', '\uFEA7', '\uFEA8', '\uFEA6',
        '\u062F', '\uFEA9', '\uFEA9', '\uFEAA', '\uFEAA',
        '\u0630', '\uFEAB', '\uFEAB', '\uFEAC', '\uFEAC',
        '\u0631', '\uFEAD', '\uFEAD', '\uFEAE', '\uFEAE',
        '\u0632', '\uFEAF', '\uFEAF', '\uFEB0', '\uFEB0',
        '\u0633', '\uFEB1', '\uFEB3', '\uFEB4', '\uFEB2',
        '\u0634', '\uFEB5', '\uFEB7', '\uFEB8', '\uFEB6',
        '\u0635', '\uFEB9', '\uFEBB', '\uFEBC', '\uFEBA',
        '\u0636', '\uFEBD', '\uFEBF', '\uFEC0', '\uFEBE',
        '\u0637', '\uFEC1', '\uFEC3', '\uFEC4', '\uFEC2',
        '\u0638', '\uFEC5', '\uFEC7', '\uFEC8', '\uFEC6',
        '\u0639', '\uFEC9', '\uFECB', '\uFECC', '\uFECA',
        '\u063A', '\uFECD', '\uFECF', '\uFED0', '\uFECE',
        '\u0641', '\uFED1', '\uFED3', '\uFED4', '\uFED2',
        '\u0642', '\uFED5', '\uFED7', '\uFED8', '\uFED6',
        '\u0643', '\uFED9', '\uFEDB', '\uFEDC', '\uFEDA',
        '\u0644', '\uFEDD', '\uFEDF', '\uFEE0', '\uFEDE',
        '\u0645', '\uFEE1', '\uFEE3', '\uFEE4', '\uFEE2',
        '\u0646', '\uFEE5', '\uFEE7', '\uFEE8', '\uFEE6',
        '\u0647', '\uFEE9', '\uFEEB', '\uFEEC', '\uFEEA',
        '\u0648', '\uFEED', '\uFEED', '\uFEEE', '\uFEEE',
        '\u0649', '\uFEEF', '\uFEEF', '\uFEF0', '\uFEF0',
        '\u0671', '\u0671', '\u0671', '\uFB51', '\uFB51',
        '\u064A', '\uFEF1', '\uFEF3', '\uFEF4', '\uFEF2',
        '\u066E', '\uFBE4', '\uFBE8', '\uFBE9', '\uFBE5',
        '\u06AA', '\uFB8E', '\uFB90', '\uFB91', '\uFB8F',
        '\u06C1', '\uFBA6', '\uFBA8', '\uFBA9', '\uFBA7',
        '\uFEFB', '\uFEFB', '\uFEFC', '\uFEFC', '\uFEFB',
        '\uFEF5', '\uFEF5', '\uFEF6', '\uFEF6', '\uFEF5',
        '\uFEF7', '\uFEF7', '\uFEF8', '\uFEF8', '\uFEF7',
        '\uFEF9', '\uFEF9', '\uFEFA', '\uFEFA', '\uFEF9',
        '\u06E4', '\u06E4', '\u06E4', '\u06E4', '\uFEEE',
        '\u064B',
        '\u064C',
        '\u064D',
        '\u064E',
        '\u064F',
        '\u0650',
        '\u0651',
        '\u0652',
        '\u0653',
        '\u0654',
        '\u0655',
        .."ضصثقفغعهخحجدشسيبلاتنمكطئءؤرلاىةوزظألآإآ"
    ]; 

    private static readonly HashSet<char> tashkeel = [
        '\u064B', // Tanween^ “◌ً”
        '\u064C', // TanweenW “◌ٌ”
        '\u064D', // TanweenV “◌ٍ”
        '\u064E', // Fatha    “◌َ”
        '\u064F', // Damma    “◌ُ”
        '\u0650', // Kasra    “◌ِ”
        '\u0651', // Shadda   “◌ّ”
        '\u0652', // Sokoon   “◌ْ”
        '\u0653', // Mada     “◌ٓ”
        '\u0654', // Hamza^   “◌ٔ”
        '\u0655', // HamzaV   “◌ٕ”
        'ـ',
    ];

    //For more information https://en.wikipedia.org/wiki/Arabic_script_in_Unicode
    //key is character shaped unicode, value is unicode of each letter's 4 cases
    private static readonly Dictionary<char, char[]> xUnicodeTable = new()
    {
        { '\u0622', ['\uFE81', '\uFE81', '\uFE82', '\uFE82', '2'] },// (آ) Alef maddah
        { '\u0623', ['\uFE83', '\uFE83', '\uFE84', '\uFE84', '2'] },// (أ) Alef With Hamza Above
        { '\u0624', ['\uFE85', '\uFE85', '\uFE86', '\uFE86', '2'] },// (ؤ) Waw With Hamza Above 
        { '\u0625', ['\uFE87', '\uFE87', '\uFE88', '\uFE88', '2'] },// (إ) Alef With Hamza Below 
        { '\u0626', ['\uFE89', '\uFE8B', '\uFE8C', '\uFE8A', '4'] },// (ئ) Yeh With Hamza Above 
        { '\u0627', ['\u0627', '\u0627', '\uFE8E', '\uFE8E', '2'] },// (ا) Alef 
        { '\u0628', ['\uFE8F', '\uFE91', '\uFE92', '\uFE90', '4'] },// (ب) Beh
        { '\u0629', ['\uFE93', '\uFE93', '\uFE94', '\uFE94', '2'] },// (ة) Teh Marbuta 
        { '\u062A', ['\uFE95', '\uFE97', '\uFE98', '\uFE96', '4'] },// (ت) Teh
        { '\u062B', ['\uFE99', '\uFE9B', '\uFE9C', '\uFE9A', '4'] },// (ث) Theh
        { '\u062C', ['\uFE9D', '\uFE9F', '\uFEA0', '\uFE9E', '4'] },// (ج) Jeem
        { '\u062D', ['\uFEA1', '\uFEA3', '\uFEA4', '\uFEA2', '4'] },// (ح) Hah
        { '\u062E', ['\uFEA5', '\uFEA7', '\uFEA8', '\uFEA6', '4'] },// (خ) Khah
        { '\u062F', ['\uFEA9', '\uFEA9', '\uFEAA', '\uFEAA', '2'] },// (د) Dal
        { '\u0630', ['\uFEAB', '\uFEAB', '\uFEAC', '\uFEAC', '2'] },// (ذ) Thal
        { '\u0631', ['\uFEAD', '\uFEAD', '\uFEAE', '\uFEAE', '2'] },// (ر) Reh
        { '\u0632', ['\uFEAF', '\uFEAF', '\uFEB0', '\uFEB0', '2'] },// (ز) Zain
        { '\u0633', ['\uFEB1', '\uFEB3', '\uFEB4', '\uFEB2', '4'] },// (س) Seen
        { '\u0634', ['\uFEB5', '\uFEB7', '\uFEB8', '\uFEB6', '4'] },// (ش) Sheen
        { '\u0635', ['\uFEB9', '\uFEBB', '\uFEBC', '\uFEBA', '4'] },// (ص) Sad
        { '\u0636', ['\uFEBD', '\uFEBF', '\uFEC0', '\uFEBE', '4'] },// (ض) Dad
        { '\u0637', ['\uFEC1', '\uFEC3', '\uFEC4', '\uFEC2', '4'] },// (ط) Tah
        { '\u0638', ['\uFEC5', '\uFEC7', '\uFEC8', '\uFEC6', '4'] },// (ظ) Zah
        { '\u0639', ['\uFEC9', '\uFECB', '\uFECC', '\uFECA', '4'] },// (ع) Ain
        { '\u063A', ['\uFECD', '\uFECF', '\uFED0', '\uFECE', '4'] },// (غ) Ghain
        { '\u0641', ['\uFED1', '\uFED3', '\uFED4', '\uFED2', '4'] },// (ف) Feh
        { '\u0642', ['\uFED5', '\uFED7', '\uFED8', '\uFED6', '4'] },// (ق) Qaf
        { '\u0643', ['\uFED9', '\uFEDB', '\uFEDC', '\uFEDA', '4'] },// (ك) Kaf
        { '\u0644', ['\uFEDD', '\uFEDF', '\uFEE0', '\uFEDE', '4'] },// (ل) Lam
        { '\u0645', ['\uFEE1', '\uFEE3', '\uFEE4', '\uFEE2', '4'] },// (م) Meem
        { '\u0646', ['\uFEE5', '\uFEE7', '\uFEE8', '\uFEE6', '4'] },// (ن) Noon
        { '\u0647', ['\uFEE9', '\uFEEB', '\uFEEC', '\uFEEA', '4'] },// (هـ) Heh
        { '\u0648', ['\uFEED', '\uFEED', '\uFEEE', '\uFEEE', '2'] },// (و) Waw
        { '\u0649', ['\uFEEF', '\uFEEF', '\uFEF0', '\uFEF0', '2'] },// (ى) Alef Maksura 
        { '\u0671', ['\u0671', '\u0671', '\uFB51', '\uFB51', '2'] },// (ٱ) Alef Wasla 
        { '\u064A', ['\uFEF1', '\uFEF3', '\uFEF4', '\uFEF2', '4'] },// (ي) Yeh
        { '\u066E', ['\uFBE4', '\uFBE8', '\uFBE9', '\uFBE5', '4'] },// (ٮ) Dotless Beh 
        { '\u06AA', ['\uFB8E', '\uFB90', '\uFB91', '\uFB8F', '4'] },// (ڪ) Swash Kaf 
        { '\u06C1', ['\uFBA6', '\uFBA8', '\uFBA9', '\uFBA7', '4'] },// (ه) Heh Goal
        { '\uFEFB', ['\uFEFC', '\uFEFB', '\uFEFC', '\uFEFB', '2'] },// (لا) Lam alef
        { '\uFEF5', ['\uFEF6', '\uFEF5', '\uFEF6', '\uFEF5', '2'] },// (ﻵ) Lam alef mada
        { '\uFEF7', ['\uFEF8', '\uFEF7', '\uFEF8', '\uFEF7', '2'] },// (ﻷ) Lam alef hamza
        { '\uFEF9', ['\uFEFA', '\uFEF9', '\uFEFA', '\uFEF9', '2'] },// (ﻹ) Lam alef kasra
        { '\u06E4', ['\u06E4', '\u06E4', '\u06E4', '\uFEEE', '2'] },// () Small High Madda 
    };

    // private static readonly Regex _reIsArabic = new Regex("[\u0600-\u06ff]");
    
    private static string GetUnShapedUnicode(string original)
    {
        if(string.IsNullOrEmpty(original)) return original;

        var words = original.Split(' ');
        StringBuilder sb = new();
        
        for (int wIdx = 0; wIdx<words.Length; wIdx++)
        {
            char prev = '\0';
            string word = words[wIdx];

            for (int cIdx = 0; cIdx < word.Length; cIdx++)
            {
                var ch = word[cIdx];

                if (!xUnicodeTable.TryGetValue(ch, out char[]? value))
                {
                    sb.Append(ch);
                    if(!tashkeel.Contains(ch)) prev = '\0';
                    continue;
                }
                
                if (!(cIdx == 0 || prev == '\0'))
                {
                    bool prevHas2shapes = xUnicodeTable.TryGetValue(prev,out var r) 
                                       && r is not null 
                                       && r.Length == 5
                                       && r[4] == '2';

                    if (cIdx == word.Length - 1 || !IsArabic($"{word[cIdx+1]}"))
                        sb.Append(value[prevHas2shapes ? 0 : 3]);                        
                    else
                        sb.Append(value[prevHas2shapes ? 1 : 2]);   
                    
                    if(!tashkeel.Contains(ch)) prev = ch;
                    continue;                         
                }
                
                sb.Append(value[1]);

                if(!tashkeel.Contains(ch)) prev = ch;
            }
            
            sb.Append(' ');
        }

        if(sb.Length>0) 
            sb.Length--;
        
        return sb.ToString();
    }
    
    private static bool IsArabic(string iWord)
    {
        if(string.IsNullOrEmpty(iWord) || iWord.Trim().Length is 0) return false;
        foreach(var ch in iWord)
        {
            if(ARABIC_ALPHABET.Contains(ch)) return true;
        }
        return false;
    }

    private static string Reverse(string v)
    {
        if(string.IsNullOrEmpty(v)) return v;
        
        List<string> result = [];
        List<string> lineWords = [];
        Stack<string> en = [];
        foreach(var line in v.Split("\r\n"))
        {
            string[] words = line.Split(' ');
            for(int wIdx=0;wIdx<words.Length;wIdx++)
            {
                string word = words[wIdx];

                if(word.Trim().Length > 0 && IsArabic(word))
                {
                    //System.Console.WriteLine("ar='{0}'", words[i]);
                    lineWords.AddRange(en);
                    en.Clear();
                    string pure = word.Trim();
                    string reversed = new([.. pure.ToCharArray().Reverse()]);
                    if(word.Length != reversed.Length) 
                    {
                        if($"{word[^1]}".Trim().Length == 0) reversed += $"{word[^1]}";
                        else reversed = $"{word[0]}{reversed}";
                    }
                    lineWords.Add(reversed);
                    if($"{word[^1]}".Trim().Length != 0)
                        continue;
                }

                en.Push(word);
            }
            if(en.Count>0)
            {
                if(lineWords.Count == 0) result.AddRange(en.Reverse()); 
                else result.AddRange(en);
            }
            lineWords.Reverse();
            result.AddRange(lineWords);
            result.Add("\r\n");
            lineWords.Clear();
            en.Clear();
        }
        
        if(result.Count>0)
            result.RemoveAt(result.Count-1);  //remove last line feed
        
        var r = string.Join(" ", result);

        return r;
    }


    public static string Transform(string source)
    {
        if(string.IsNullOrEmpty(source)) return source; 
        source = string.Join("\uFEFB", source.Split("لا"));
        source = string.Join("\uFEF5", source.Split("لآ"));
        source = string.Join("\uFEF7", source.Split("لأ"));
        source = string.Join("لــ\uFEF9", source.Split("للإ"));
        source = string.Join("\uFEF9", source.Split("لإ"));
        string result = Reverse(GetUnShapedUnicode(source));
        return result;
    }
}