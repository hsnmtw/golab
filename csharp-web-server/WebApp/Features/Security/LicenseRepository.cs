

using WebApp.Data.Tables;

namespace HOS.App.Features.Security
{
    public static class LicenseRepository
    {
        public static Option<string> Query(string q)
        {
            var sql = "select * from [T_LICENSE] where [Name] LIKE concat('%',@0,'%') ";
            var list = Database.Instance.Query(sql.ToString(), q);
            return list;
        }

        public static Option<int> Remove(Guid id)
        {
            return Database.Instance.Execute("delete from [T_LICENSE] where [Id]=@0", id);
        }

        public static Option<int> Store(License model)
        {
            var sql = "update [T_LICENSE] set [name]=@1,[address]=@2 where [id]=@0";
            if (model.Id == Guid.Empty)//!//
            {
                sql = "insert into [T_LICENSE] ([id],[name],[address]) values (@0,@1,@2)";
                model.Id = Guid.NewGuid();
            }
            return Database.Instance.Execute(sql, model.Id, model.Name, model.Address);
        }
    }
}