using System.Collections.Generic;
using System.Text;

namespace WebApp.Http{
public static class TemplatesHelper
{
    public static string FillTemplate(string content, Dictionary<string, object> items)
    {
        var sb = new StringBuilder();
        var collect = new StringBuilder();
        for (int i = 0; i < content.Length; ++i)
        {
            char ch = content[i];
            if (ch == '<')
                collect.Clear();
            collect.Append(ch);
            sb.Append(ch);
            if (ch == '>' && items.ContainsKey(collect.ToString()))
            {
                sb.Append(items[collect.ToString()]);
                collect.Clear();
            }
        }
        return sb.ToString();
    }
}
}