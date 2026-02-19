
using System.Text;
using System.Collections.Generic;

using WebApp.Data.Tables;


namespace HOS.App.Features.Security
{
    public static class UserRepository
    {
        public static Option<int> Count(string q)
        {
            var sql = "select count(*) from [T_USER] where [name] like concat('%',@0,'%') ";

            var count = Utils.Parse<int>(Database.Instance.Query(sql, q).Value);
            return Options.Create(count);
        }
        public static Option<string> Query(string q, int skip, int take, int sortIndex = 2)
        {
            var sql = new StringBuilder();
            sql.Append("select * from [T_USER] where [name] like concat('%',@0,'%') ");
            sql.Append(sortIndex > 0 ? " ORDER BY " + sortIndex : "");
            sql.Append(Database.Instance.GetPage(skip, take));
            var list = Database.Instance.Query(sql.ToString(), q);
            return list;
        }

        public static User GetUser(User user)
        {
            var result = Database.Instance.Query("select * from [T_USER] where upper([Name])=@0", user.Name.ToUpper());
            return User.Convert(result.Value);
        }

        public static Option<int> Store(User model, bool insert = false)
        {
            var sql =
            """
                update [T_USER] SET
                  [Name]           = @1       
                , [Password]       = @2           
                , [Authorization]  = @3                
                , [ChatId]         = @4         
                , [Token]          = @5             
                , [Organization]   = @6               
                , [Mobile]         = @7         
                , [Email]          = @8        
                , [Otp]            = @9      
                , [OtpAttempts]    = @10      
                , [OtpExpiry]      = @11
                , [PasswordExpiry] = @12      
                where [Id]         = @0
            """;
            if (model.Id == Guid.Empty || insert)//!//
            {
                sql =
                """
                insert into [T_USER] 
                (
                  [Name]
                , [Password]
                , [Authorization]
                , [ChatId]
                , [Token]
                , [Organization]
                , [Mobile]
                , [Email]
                , [Otp]
                , [OtpAttempts]
                , [OtpExpiry]
                , [PasswordExpiry]
                )
                values (
                  @0
                , @1       
                , @2           
                , @3                
                , @4         
                , @5             
                , @6               
                , @7         
                , @8        
                , @9   
                , @10
                , @11
                , @12
                )   
                """;
                if (model.Id == Guid.Empty) model.Id = Guid.NewGuid();
            }
            return Database.Instance.Execute(sql,
                model.Id,
                model.Name,
                model.Password,
                model.Authorization,
                model.ChatId,
                model.Token,
                model.Organization,
                model.Mobile,
                model.Email,
                model.Otp,
                model.OtpAttempts,
                model.OtpExpiry,
                model.PasswordExpiry
                );
        }

        internal static User? GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return default;
            var query = Database.Instance.Query("select * from T_USER where [Email]=@0", email);
            if (query.IsError || string.IsNullOrEmpty(query.Value)) return default;
            return User.Convert(query.Value.Split('\n').First());
        }

        internal static User? GetByToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return default;
            var query = Database.Instance.Query("select * from T_USER where [Token]=@0", token);
            if (query.IsError || string.IsNullOrEmpty(query.Value)) return default;
            return User.Convert(query.Value.Split('\n').First());
        }
        
        internal static User? GetByChatId(string chat_id)
        {
            if (string.IsNullOrEmpty(chat_id)) return default;
            var query = Database.Instance.Query("select * from T_USER where [ChatId]=@0", chat_id);
            if (query.IsError || string.IsNullOrEmpty(query.Value)) return default;
            return User.Convert(query.Value.Split('\n').First());
        }

        internal static string CrossTable(string? auths)
        {
            if (!$"{auths}".Split(',').Intersect([Authorizations.ADM,Authorizations.MGR,Authorizations.TCH]).Any())
                return ".";
            string sql = 
            $$"""
            select [Name],
                   case when CHARINDEX('ADMIN'     , [Authorization]) < 1 then null else 'Y' end as [ADM],
                   case when CHARINDEX('MANAGER'   , [Authorization]) < 1 then null else 'Y' end as [MGR],
                   case when CHARINDEX('SUPERVISOR', [Authorization]) < 1 then null else 'Y' end as [SPV],
                   case when CHARINDEX('MODERATOR' , [Authorization]) < 1 then null else 'Y' end as [MOD],
                   case when CHARINDEX('USER'      , [Authorization]) < 1 then null else 'Y' end as [USR],
                   case when CHARINDEX('FINANCE'   , [Authorization]) < 1 then null else 'Y' end as [FIN],
                   case when CHARINDEX('TECHNOLOGY', [Authorization]) < 1 then null else 'Y' end as [TCH],
                   case when [ChatId] is null then [Token] else null end as [Token],
                   [Mobile],
                   [Email]
            from t_user
            order by 1;
            """;
            return Database.Instance.Query(sql,[]).Value;
        }

    }
}