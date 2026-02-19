
using System.Text.RegularExpressions;

namespace WebApp.Data;
public static class Json 
{
    // const string PATTERN = "\\s*(?<k>\"[a-zA-Z0-9]+\")\\s*:\\s*((?<s>\".*\")|(?<n>[0-9]*[.]*[0-9]*)|(?<b>true|false))\\s*(,|})\\s*";
    const string PATTERN = "(?<s>\\s*\"[^\"]*\"\\s*:\\s*\"[^\"]*\"\\s*)(,|}){0,1}|(?<b>\\s*\"[^\"]*\"\\s*:\\s*[^\"]*\\s*)(,|}){0,1}";
    public static S Parse<S>(string json, Func<S,KeyValue,S> propertyHandler) where S: class,new()
    {
        S record = new S();
        if(!string.IsNullOrEmpty(json))
        {
            var re = Regex.Matches(json, PATTERN, RegexOptions.Multiline|RegexOptions.ExplicitCapture|RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace);
            foreach (Match m in re)
            {
                var t = (""+(!string.IsNullOrEmpty(m.Groups["s"].Value) ? m.Groups["s"].Value : m.Groups["b"].Value)).Trim('}').Trim('{').Trim(',').Trim();
                string[] ps = t.Split(':');
                if(ps.Length<2) continue;
                var k = ps[0].Trim().Trim('"').Trim();
                var v = string.Join(":", Utils.Skip(ps,1)).Trim().Trim('"').Trim();
                if(Equals(v,"null")) v = "";
                record = propertyHandler(record, new KeyValue { Key = k, Value = v });
            }
            

        }
        return record;
    }

    /*
    "key": bool,
    "key": null,
    "key": number,
    "key": "string/date",
    "key": {object},  [NOT SUPPORTED]
    "key": [list],    [NOT SUPPORTED]
    */

}