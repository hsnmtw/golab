


using System.Text;
using HOS.Otp;
using WebApp.Otp;

namespace WebApp.Data;

public struct KeyValue {
    public string Key { get; set; }
    public string Value { get; set; }

    public readonly override string ToString()
    {
        return "k=`" + Key + "`/v=`" + Value+"`";
    }
}

// internal static class NullableDateTime
// {
//     public static Nullable<DateTime> Parse(string value)
//     {
//         if (string.IsNullOrEmpty(value)) return default;
//         DateTime? result = DateTime.Parse(value);
//         return result;
//     }
// }

public static class Utils
{
    private static readonly int TIME_ZONE_DELTA = 0; //OperatingSystem.IsWindows() ? 0 : 3;
    internal static readonly DateTime StartOfThisMonth = new DateTime(Today.Year, Today.Month, 1);

    public static DateTime Today => DateTime.Now.AddHours(TIME_ZONE_DELTA).Date;
    public static DateTime Now => DateTime.Now.AddHours(TIME_ZONE_DELTA);
    public static DateTime ThisMonthStart => new DateTime(Today.Year, Today.Month, 1);
    public static DateTime PreviousMonthEnd => ThisMonthStart.AddDays(-1);

    public static string[] SplitAndTrim(string input, char delim)
    {
        if (string.IsNullOrEmpty(input)) return new string[] { };
        return input.Split(delim).Select(x => x.Trim()).ToArray();
    }

    public static string ToBase64(string input)
    {
        if (string.IsNullOrEmpty(input)) throw new Exception("[empty string passed to base64]");
        return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }

    public static string FromBase64(string input)
    {
        if (string.IsNullOrEmpty(input)) throw new Exception("[empty string passed to base64]");
        return Encoding.UTF8.GetString(System.Convert.FromBase64String(input));
    }


    public static string MD5(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return string.Join("", ByteArrayToHexViaLookup(hashBytes)).ToLower(); // .NET 5 +
        }
    }

    private static string ByteArrayToHexViaLookup(byte[] bytes)
    {
        string[] hexStringTable = new string[] {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF",
        };
        StringBuilder result = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            result.Append(hexStringTable[b]);
        }
        return result.ToString();
    }

    public static string ISODate(DateTime? dateTime)
    {
        if (dateTime == null) return "";
        if (dateTime.Value.Year < 1990 || dateTime.Value.Year > 2100) return "";
        return string.Format("{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff}", dateTime);
    }

    public static string SkipPart(string input, string delim, int skip)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.IndexOf(delim) < 0) return input;
        return string.Join(delim, Skip(input.Split(new[] { delim }, StringSplitOptions.None), skip));
    }

    private const string TYPE_INT = "System.Int32";
    private const string TYPE_DBL = "System.Double";
    private const string TYPE_FLT = "System.Single";
    private const string TYPE_BLN = "System.Boolean";
    private const string TYPE_UID = "System.Guid";
    private const string TYPE_DTM = "System.DateTime";
    // private const string TYPE_NDT = "System.Nullable`1[[System.DateTime";

    private static readonly string[] TRUTHS = ["1", "TRUE", "True", "true"];

    public static T? Parse<T>(string value)
    {
        try
        {
            if (string.IsNullOrEmpty(value)) return default(T);
            Type type = typeof(T);
            string typeName = $"{type.FullName}".Split(',').First();
            T? result;
            switch (typeName)
            {
                case TYPE_INT: result = (T)Convert.ChangeType(int.Parse(value),      type); break;
                case TYPE_DBL: result = (T)Convert.ChangeType(double.Parse(value),   type); break;
                case TYPE_FLT: result = (T)Convert.ChangeType(float.Parse(value),    type); break;
                case TYPE_BLN: result = (T)Convert.ChangeType(TRUTHS.Contains(value),type); break;
                case TYPE_DTM: result = (T)Convert.ChangeType(DateTime.Parse(value), type); break;
                case TYPE_UID: result = (T)Convert.ChangeType(Guid.Parse(value),     type); break;
                default: result = default(T); break;
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Error.WriteLine(ex);
            OtpManager.Instance.SendMessageAdmin("Utils.Parse<"+typeof(T)+"> : [ERROR] / (" + ex.Message + ") value = '"+ value +"'");
            Console.ForegroundColor = ConsoleColor.White;
        }
        return default(T);
    }


    public static KeyValue GetKeyValue(string part)
    {
        if (string.IsNullOrEmpty(part)) return default(KeyValue);
        string[] kv = part.Split(':');
        if (kv.Length < 2) return default(KeyValue);
        string k = kv[0].Trim().Trim('"').Trim();
        string v = string.Join(":", Utils.Skip(kv, 1)).Trim().Trim('"').Trim();
        return new KeyValue() { Key = k, Value = v };
    }

    public static string BuildJsonObject(params object?[] tokens)
    {

        var sb = new System.Text.StringBuilder();
        sb.Append('{');
        for (int i = 0; i < tokens.Length; i += 2)
        {
            var k = tokens[i + 0];
            var v = tokens[i + 1];
            var isBool = v is bool;
            var isNull = v is null;
            var isNumber = v is not null && IsNumber(v);
            var isDate = v is DateTime || v is Nullable<DateTime>;
            sb.Append('"');
            sb.Append(k);
            sb.Append('"');
            sb.Append(':');
            if (isNull)
            {
                sb.Append("null");
            }
            else
            {
                if (!(isBool || isNumber)) sb.Append('"');
                sb.Append(isBool ? $"{v}".ToLower() : isDate ? string.Format("{0:s}", v) : v);
                if (!(isBool || isNumber)) sb.Append('"');
            }
            sb.Append(',');
        }
        if (sb.Length > 1) sb.Length--;
        sb.Append('}');
        return sb.ToString();
    }
    public static bool IsNumber(object value)
    {
        return value is sbyte
            || value is byte
            || value is short
            || value is ushort
            || value is int
            || value is uint
            || value is long
            || value is ulong
            || value is float
            || value is double
            || value is decimal;
    }


    public static T[] Skip<T>(T[] source, int index)
    {
        return Slice(source, index, source.Length - index);
    }

    public static T[] Take<T>(T[] source, int count)
    {
        return Slice(source, 0, count);
    }

    public static T[] Slice<T>(T[] source, int start, int length)
    {
        if (length > source.Length || length < 1) length = source.Length;
        if (start >= source.Length) start = 0;
        T[] dest = new T[length];
        if (source.Length == 0) return dest;
        Array.Copy(source, start, dest, 0, length);
        return dest;
    }
}