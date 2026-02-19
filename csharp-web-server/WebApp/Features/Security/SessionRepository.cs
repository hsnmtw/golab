


using WebApp.Data.Tables;

namespace HOS.App.Features.Security
{
    public static class SessionRepository
    {
        public static string GetUsername(string sessionId)
        {
            var sql = "select [name] from [T_SESSION] where [cookie]=@0";
            var username = Database.Instance.Query(sql, sessionId).Value ?? "Unknown";
            return username.Trim();
        }

        public static Option<string> Query(string q)
        {
            var sql = "select * from [T_SESSION] where [cookie]=@0  ";
            var list = Database.Instance.Query(sql.ToString(), q);
            return list;
        }


        public static Option<int> Remove(Guid id)
        {
            return Database.Instance.Execute("delete from [T_SESSION] where [id]=@0", id);
        }
        public static Option<int> Store(Session model)
        {
            var sql = "update [T_SESSION] set [name]=@1,[cookie]=@2,[expiry]=@3 where [id]=@0";
            if (model.Id == Guid.Empty)//!//
            {
                sql = "insert into [T_SESSION] ([id],[name],[cookie],[expiry]) values (@0,@1,@2,@3)";
                model.Id = Guid.NewGuid();
            }
            return Database.Instance.Execute(sql, model.Id, model.Name, model.Cookie, model.Expiry);
        }

        internal static User? GetUser(string sessionId)
        {
            var sql = "select u.* from [T_USER] as u join [T_SESSION] as s on s.[Name] = u.[Name] where s.[cookie]=@0";
            var user = Database.Instance.Query(sql, sessionId).Value?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(User.Convert).FirstOrDefault();
            return user;

        }
    }
}