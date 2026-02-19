


namespace WebApp.Data.Tables
{
    /*
    DROP TABLE T_USER;
    CREATE TABLE T_USER(
        [Id]             VARCHAR(36)    NOT NULL PRIMARY KEY,      
        [Name]           VARCHAR(36)    NOT NULL,        
        [Password]       VARCHAR(36)                   NOT NULL,            
        [Authorization]  VARCHAR(255)   NOT NULL,
        [ChatId]         VARCHAR(64),
        [Token]          VARCHAR(64),
        [Email]          VARCHAR(255),
        [Organization]   VARCHAR(255),
        [Mobile]         VARCHAR(10),
        [Otp]            VARCHAR(4),
        [OtpAttempts]    int,
        [OtpExpiry]      datetime,
        [PasswordExpiry] datetime
    );
    CREATE UNIQUE INDEX IX_T_USER_Name on T_USER([Name]);
    update t_user set [email] = UPPER(substring(cast(newid() as VARCHAR(36)),1,6));
    update t_user set [Token] = UPPER(substring(cast(newid() as VARCHAR(36)),1,6));
    CREATE UNIQUE INDEX IX_T_USER_Email on T_USER([Email]);
    CREATE UNIQUE INDEX IX_T_USER_Token on T_USER([Token]);
    alter table t_user add constraint DF_T_USER_Token DEFAULT left(cast(newid() as varchar(36)), 6) FOR [Token];
    */
    public enum Auhorizations { ADM, MGR, USR, SPV, MOD, FIN, TCH }
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public string Authorization { get; set; } = "";
        public string? ChatId { get; set; }
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Organization { get; set; }
        public string? Mobile { get; set; }
        public string? Otp { get; set; }
        public int OtpAttempts { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public DateTime? PasswordExpiry { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Password) && Password.Length < 32) Password = Utils.MD5(Password);
            return string.Join("|",
                Id,
                Name,
                Password,
                Authorization,
                ChatId,
                Token,
                Email,
                Organization,
                Mobile,
                Otp,
                OtpAttempts,
                OtpExpiry,
                PasswordExpiry
            );
        }

        public static User Convert(string line)
        {
            var record = new User();
            if (string.IsNullOrEmpty(line)) return record;
            string[] parts = Utils.SplitAndTrim(line, '|');

            if (parts.Length > 00 && parts[00].Length > 0) record.Id            = Utils.Parse<Guid>(parts[00]);
            if (parts.Length > 01 && parts[01].Length > 0) record.Name          = parts[01];
            if (parts.Length > 02 && parts[02].Length > 0) record.Password      = parts[02];
            if (parts.Length > 03 && parts[03].Length > 0) record.Authorization = parts[03];
            if (parts.Length > 04 && parts[04].Length > 0) record.ChatId        = parts[04];
            if (parts.Length > 05 && parts[05].Length > 0) record.Token         = parts[05];
            if (parts.Length > 06 && parts[06].Length > 0) record.Email         = parts[06];
            if (parts.Length > 07 && parts[07].Length > 0) record.Organization  = parts[07];
            if (parts.Length > 08 && parts[08].Length > 0) record.Mobile        = parts[08];
            if (parts.Length > 09 && parts[09].Length > 0) record.Otp           = parts[09];
            if (parts.Length > 10 && parts[10].Length > 0) record.OtpAttempts   = Utils.Parse<int>(parts[10]);
            if (parts.Length > 11 && parts[11].Length > 0) record.OtpExpiry     = Utils.Parse<DateTime>(parts[11]);
            if (parts.Length > 12 && parts[12].Length > 0) record.PasswordExpiry= Utils.Parse<DateTime>(parts[12]);
            return record;
        }

        public static User FromJson(string json)
        {
            return Json.Parse(json, (User record, KeyValue kv) =>
            {
                switch (kv.Key)
                {
                    case "Id":            record.Id            = Utils.Parse<Guid>(kv.Value); break;
                    case "Name":          record.Name          = kv.Value; break;
                    case "Password":      record.Password      = kv.Value; break;
                    case "Authorization": record.Authorization = kv.Value; break;
                    case "ChatId":        record.ChatId        = kv.Value; break;
                    case "Token":         record.Token         = kv.Value; break;
                    case "Email":         record.Email         = kv.Value; break;
                    case "Organization":  record.Organization  = kv.Value; break;
                    case "Mobile":        record.Mobile        = kv.Value; break;
                    case "Otp":           record.Otp           = kv.Value; break;
                    case "OtpAttempts":   record.OtpAttempts   = Utils.Parse<int>(kv.Value); break;
                    case "OtpExpiry":     record.OtpExpiry     = Utils.Parse<DateTime>(kv.Value); break;
                    case "PasswordExpiry":record.PasswordExpiry= Utils.Parse<DateTime>(kv.Value); break;
                }
                return record;
            });
        }
    }
}