
using System.Text;
using System.IO;

using System.Collections.Generic;
using WebApp.Data.Tables;

namespace WebApp.Data
{
    public interface IDatabase
    {
        Option<int> Execute(string sql, params object?[] args);
        Option<string> Query(string sql, params object?[] args);
        string GetPage(int skip, int take);
    }
}