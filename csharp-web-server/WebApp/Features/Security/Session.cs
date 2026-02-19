

namespace WebApp.Data.Tables
{
    /*
    DROP TABLE T_SESSION;
    CREATE TABLE T_SESSION(
        [Id]        VARCHAR(36)     NOT NULL,                    
        [Name]      VARCHAR(50)     NOT NULL,                      
        [Cookie]    VARCHAR(200)    NOT NULL,                        
        [Expiry]    DATETIME        NOT NULL                        
    );
    CREATE INDEX IX_T_SESSION_Name on T_SESSION([Name]);
    CREATE UNIQUE INDEX IX_T_SESSION_Cookie on T_SESSION([Cookie]);
    */
    public class Session
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Cookie { get; set; } = "";
        public DateTime Expiry { get; set; }

        public override string ToString()
        {
            return string.Join("|",
                Id,
                Name,
                Cookie,
                Utils.ISODate(Expiry)
            );
        }
        public static Session Convert(string line)
        {
            var record = new Session();
            if (string.IsNullOrEmpty(line)) return record;
            string[] parts = Utils.SplitAndTrim(line, '|');

            if (parts.Length > 00 && parts[00].Length > 0) record.Id     = Utils.Parse<Guid>(parts[00]);
            if (parts.Length > 01 && parts[01].Length > 0) record.Name   = parts[01];
            if (parts.Length > 02 && parts[02].Length > 0) record.Cookie = parts[02];
            if (parts.Length > 03 && parts[03].Length > 0) record.Expiry = Utils.Parse<DateTime>(parts[03]);
            return record;
        }

        public static Session FromJson(string json)
        {
            return Json.Parse(json, (Session record, KeyValue kv) =>
            {
                switch (kv.Key)
                {
                    case "Id": record.Id = Utils.Parse<Guid>(kv.Value); break;
                    case "Name": record.Name = kv.Value; break;
                    case "Cookie": record.Cookie = kv.Value; break;
                    case "Expiry": record.Expiry = Utils.Parse<DateTime>(kv.Value); break;
                }
                return record;
            });
        }
    }
}