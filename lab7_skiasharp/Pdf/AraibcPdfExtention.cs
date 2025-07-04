using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AraibcPdfUnicodeGlyphsResharper;

public static class AraibcPdfExtention
{

    private static readonly char[] _someArabic = [
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
    ]; 

    //For more information https://en.wikipedia.org/wiki/Arabic_script_in_Unicode
    //key is character shaped unicode, value is unicode of each letter's 4 cases
    private static readonly Dictionary<string, string[]> xUnicodeTable = new()
    {
        { "\\u0622", ["\\uFE81", "\\uFE81", "\\uFE82", "\\uFE82", "2"] },// (آ) Alef maddah
        { "\\u0623", ["\\uFE83", "\\uFE83", "\\uFE84", "\\uFE84", "2"] },// (أ) Alef With Hamza Above
        { "\\u0624", ["\\uFE85", "\\uFE85", "\\uFE86", "\\uFE86", "2"] },// (ؤ) Waw With Hamza Above 
        { "\\u0625", ["\\uFE87", "\\uFE87", "\\uFE88", "\\uFE88", "2"] },// (إ) Alef With Hamza Below 
        { "\\u0626", ["\\uFE89", "\\uFE8B", "\\uFE8C", "\\uFE8A", "4"] },// (ئ) Yeh With Hamza Above 
        { "\\u0627", ["\\u0627", "\\u0627", "\\uFE8E", "\\uFE8E", "2"] },// (ا) Alef 
        { "\\u0628", ["\\uFE8F", "\\uFE91", "\\uFE92", "\\uFE90", "4"] },// (ب) Beh
        { "\\u0629", ["\\uFE93", "\\uFE93", "\\uFE94", "\\uFE94", "2"] },// (ة) Teh Marbuta 
        { "\\u062A", ["\\uFE95", "\\uFE97", "\\uFE98", "\\uFE96", "4"] },// (ت) Teh
        { "\\u062B", ["\\uFE99", "\\uFE9B", "\\uFE9C", "\\uFE9A", "4"] },// (ث) Theh
        { "\\u062C", ["\\uFE9D", "\\uFE9F", "\\uFEA0", "\\uFE9E", "4"] },// (ج) Jeem
        { "\\u062D", ["\\uFEA1", "\\uFEA3", "\\uFEA4", "\\uFEA2", "4"] },// (ح) Hah
        { "\\u062E", ["\\uFEA5", "\\uFEA7", "\\uFEA8", "\\uFEA6", "4"] },// (خ) Khah
        { "\\u062F", ["\\uFEA9", "\\uFEA9", "\\uFEAA", "\\uFEAA", "2"] },// (د) Dal
        { "\\u0630", ["\\uFEAB", "\\uFEAB", "\\uFEAC", "\\uFEAC", "2"] },// (ذ) Thal
        { "\\u0631", ["\\uFEAD", "\\uFEAD", "\\uFEAE", "\\uFEAE", "2"] },// (ر) Reh
        { "\\u0632", ["\\uFEAF", "\\uFEAF", "\\uFEB0", "\\uFEB0", "2"] },// (ز) Zain
        { "\\u0633", ["\\uFEB1", "\\uFEB3", "\\uFEB4", "\\uFEB2", "4"] },// (س) Seen
        { "\\u0634", ["\\uFEB5", "\\uFEB7", "\\uFEB8", "\\uFEB6", "4"] },// (ش) Sheen
        { "\\u0635", ["\\uFEB9", "\\uFEBB", "\\uFEBC", "\\uFEBA", "4"] },// (ص) Sad
        { "\\u0636", ["\\uFEBD", "\\uFEBF", "\\uFEC0", "\\uFEBE", "4"] },// (ض) Dad
        { "\\u0637", ["\\uFEC1", "\\uFEC3", "\\uFEC4", "\\uFEC2", "4"] },// (ط) Tah
        { "\\u0638", ["\\uFEC5", "\\uFEC7", "\\uFEC8", "\\uFEC6", "4"] },// (ظ) Zah
        { "\\u0639", ["\\uFEC9", "\\uFECB", "\\uFECC", "\\uFECA", "4"] },// (ع) Ain
        { "\\u063A", ["\\uFECD", "\\uFECF", "\\uFED0", "\\uFECE", "4"] },// (غ) Ghain
        { "\\u0641", ["\\uFED1", "\\uFED3", "\\uFED4", "\\uFED2", "4"] },// (ف) Feh
        { "\\u0642", ["\\uFED5", "\\uFED7", "\\uFED8", "\\uFED6", "4"] },// (ق) Qaf
        { "\\u0643", ["\\uFED9", "\\uFEDB", "\\uFEDC", "\\uFEDA", "4"] },// (ك) Kaf
        { "\\u0644", ["\\uFEDD", "\\uFEDF", "\\uFEE0", "\\uFEDE", "4"] },// (ل) Lam
        { "\\u0645", ["\\uFEE1", "\\uFEE3", "\\uFEE4", "\\uFEE2", "4"] },// (م) Meem
        { "\\u0646", ["\\uFEE5", "\\uFEE7", "\\uFEE8", "\\uFEE6", "4"] },// (ن) Noon
        { "\\u0647", ["\\uFEE9", "\\uFEEB", "\\uFEEC", "\\uFEEA", "4"] },// (هـ) Heh
        { "\\u0648", ["\\uFEED", "\\uFEED", "\\uFEEE", "\\uFEEE", "2"] },// (و) Waw
        { "\\u0649", ["\\uFEEF", "\\uFEEF", "\\uFEF0", "\\uFEF0", "2"] },// (ى) Alef Maksura 
        { "\\u0671", ["\\u0671", "\\u0671", "\\uFB51", "\\uFB51", "2"] },// (ٱ) Alef Wasla 
        { "\\u064A", ["\\uFEF1", "\\uFEF3", "\\uFEF4", "\\uFEF2", "4"] },// (ي) Yeh
        { "\\u066E", ["\\uFBE4", "\\uFBE8", "\\uFBE9", "\\uFBE5", "4"] },// (ٮ) Dotless Beh 
        { "\\u06AA", ["\\uFB8E", "\\uFB90", "\\uFB91", "\\uFB8F", "4"] },// (ڪ) Swash Kaf 
        { "\\u06C1", ["\\uFBA6", "\\uFBA8", "\\uFBA9", "\\uFBA7", "4"] },// (ه) Heh Goal
        { "\\uFEFB", ["\\uFEFC", "\\uFEFB", "\\uFEFC", "\\uFEFB", "2"] },// (لا) Lam alef
        { "\\uFEF5", ["\\uFEF6", "\\uFEF5", "\\uFEF6", "\\uFEF5", "2"] },// (ﻵ) Lam alef mada
        { "\\uFEF7", ["\\uFEF8", "\\uFEF7", "\\uFEF8", "\\uFEF7", "2"] },// (ﻷ) Lam alef hamza
        { "\\uFEF9", ["\\uFEFA", "\\uFEF9", "\\uFEFA", "\\uFEF9", "2"] },// (ﻹ) Lam alef kasra
        { "\\u06E4", ["\\u06E4", "\\u06E4", "\\u06E4", "\\uFEEE", "2"] },// () Small High Madda 
    };

    // private static readonly Regex _reIsArabic = new Regex("[\u0600-\u06ff]");
    
    private static string GetUnShapedUnicode(this string original)
    {
        //remove escape characters
        original = Regex.Unescape(original.Trim());

        var xWords = original.Split(' ');
        StringBuilder xBuilder = new StringBuilder();
        
        foreach (var iWord in xWords)
        {
            string? xPrevious = null;
            int xIndex = 0;
            foreach (var character in iWord)
            {
                string xShapedUnicode = @"\u" + ((int)character).ToString("X4");

                //if unicode doesn't exist in unicode table then character isn't a letter hence shaped unicode is fine
                if (!xUnicodeTable.TryGetValue(xShapedUnicode, out string[]? value))
                {
                    xBuilder.Append(xShapedUnicode);
                    xPrevious = null;
                    continue;
                }
                else
                {
                    //first character in word or previous character isn't a letter
                    if (xIndex == 0 || xPrevious == null)
                    {
                        xBuilder.Append(value[1]);
                    }
                    else
                    {
                        bool xPreviousCharHasOnlyTwoCases = xUnicodeTable[xPrevious][4] == "2";
                        //if last character in word
                        if (xIndex == iWord.Length - 1)
                        {
                            if (!string.IsNullOrEmpty(xPrevious) && xPreviousCharHasOnlyTwoCases)
                            {
                                xBuilder.Append(value[0]);
                            }
                            else
                                xBuilder.Append(value[3]);
                        }
                        else
                        {
                            if (xPreviousCharHasOnlyTwoCases)
                                xBuilder.Append(value[1]);
                            else
                                xBuilder.Append(value[2]);
                        }
                    }
                }

                xPrevious = xShapedUnicode;
                xIndex++;
            }
            //if not last word then add a space unicode
            if (xWords.ToList().IndexOf(iWord) != xWords.Length - 1)
                xBuilder.Append(@"\u" + ((int)' ').ToString("X4"));
        }

        return xBuilder.ToString();
    }

    private static string DecodeEncodedNonAsciiCharacters(this string value)
    {
        return Regex.Replace(
            value,
            @"\\u(?<Value>[a-zA-Z0-9]{4})",
            m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
    }
    private static string ReverseString(this string original)
    {
        //to check each word as alone may contain english word and may not
        var xWords = original.Split(' ');

        for (int i = 0; i < xWords.Length; i++)
        {
            var iWord = xWords[i];
            if (!string.IsNullOrEmpty(iWord) && IsArabic(iWord))
            {
                var xArray = iWord.ToCharArray();
                Array.Reverse(xArray);
                xWords[i] = new string(xArray);
            }

        }
        Array.Reverse(xWords);

        return string.Join(' ', xWords);
    }

    private static bool IsArabic(string iWord)
    {
        if(string.IsNullOrEmpty(iWord) || iWord.Trim().Length is 0) return false;
        foreach(var ch in iWord)
        {
            if(_someArabic.Contains(ch)) return true;
            if(ch >= '\u0600' && ch<= '\u06FF') return true; 
        }
        return false;
    }

    

    private static string ReverseText(string v)
    {
        return v;
    }

    private static string ReverseStatement(string v)
    {
        if(string.IsNullOrEmpty(v)) return v;
        
        List<string> result = [];
        List<string> lineWords = [];
        Stack<string> en = [];
        foreach(var line in v.Split("\r\n"))
        {
            string[] words = line.Split(' ');
            for(int i=0;i<words.Length;i++)
            {
                
                if(words[i].Trim().Length > 0 && IsArabic(words[i]))
                {
                    //System.Console.WriteLine("ar='{0}'", words[i]);
                    lineWords.AddRange(en);
                    en.Clear();
                    string pure = words[i].Trim();
                    string reversed = new([.. pure.ToCharArray().Reverse()]);
                    if(words[i].Length != reversed.Length) 
                    {
                        if($"{words[i][^1]}".Trim().Length == 0) reversed += $"{words[i][^1]}";
                        else reversed = $"{words[i][0]}{reversed}";
                    }
                    lineWords.Add(reversed);
                    if($"{words[i][^1]}".Trim().Length != 0)
                        continue;
                }

                en.Push(words[i]);
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


    public static string ArabicWithFontGlyphsToPfd(string source)
    {
        
        // return source;
        if(string.IsNullOrEmpty(source)) return source; 
        source = string.Join("\uFEFB", source.Split("لا"));
        source = string.Join("\uFEF5", source.Split("لآ"));
        source = string.Join("\uFEF7", source.Split("لأ"));
        source = string.Join("\uFEF9", source.Split("لإ"));
        source = string.Join(" \uFD3F ", source.Split("("));
        source = string.Join(" \uFD3E ", source.Split(")"));
        source = string.Join(" \r\n ", source.Split("\r\n"));
        string result = ReverseStatement(DecodeEncodedNonAsciiCharacters(GetUnShapedUnicode(source)));
        // result = string.Join("\r\n", result.Split(" ↲ "));
        return result;
    }
}