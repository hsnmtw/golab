

using WebApp.Data.Tables;

namespace HOS.App.Features.Security
{
    public static class AuditRepository
    {
        public static Option<string> Query(string q)
        {
            var sql = "select [Id],[User],[Target],[Time],[Notes] from [T_AUDIT] where [Target] = @0 order by [Time]";
            var list = Database.Instance.Query(sql.ToString(), q);
            return list;
        }

        public static Option<int> Store(Audit model)
        {
            var sql = "update [T_AUDIT] set [user]=@1,[target]=@2,[time]=@3,[notes]=@4 where [id]=@0";
            if (model.Id == Guid.Empty)//!//
            {
                sql = "insert into [T_AUDIT] ([id],[user],[target],[time],[notes]) values (@0,@1,@2,@3,@4)";
                model.Id = Guid.NewGuid();
            }
            return Database.Instance.Execute(sql,
                model.Id,
                model.User,
                model.Target,
                model.Time,
                model.Notes);
        }
    }
}