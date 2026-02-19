

namespace WebApp.Data.Tables
{
    /*
    DROP TABLE T_AUDIT;
    CREATE TABLE T_AUDIT(
        [Id]        VARCHAR(36)     NOT NULL PRIMARY KEY,                    
        [User]      VARCHAR(36)     NOT NULL,                      
        [Target]    VARCHAR(50)     NOT NULL,                      
        [Time]      DATETIME        NOT NULL,                   
        [Notes]     VARCHAR(500)                        
    );
    CREATE INDEX IX_T_AUDIT_User on T_AUDIT([User]);
    CREATE INDEX IX_T_AUDIT_Target on T_AUDIT([Target]);
    CREATE INDEX IX_T_AUDIT_Time on T_AUDIT([Time]);
    */
    public class Audit
    {
        public Guid Id { get; set; }
        public string User { get; set; } = "";
        public string Target { get; set; } = "";
        public DateTime Time { get; set; }
        public string? Notes { get; set; }

        public override string ToString()
        {
            return string.Join("|",
                Id,
                User,
                Target,
                Utils.ISODate(Time),
                Notes
            );
        }
        public static Audit Convert(string line)
        {
            var record = new Audit();
            if (string.IsNullOrEmpty(line)) return record;
            string[] parts = Utils.SplitAndTrim(line, '|');
            if (parts.Length > 00 && parts[00].Length>0) record.Id     = Utils.Parse<Guid>(parts[00]);
            if (parts.Length > 01 && parts[01].Length>0) record.User   = parts[01];
            if (parts.Length > 02 && parts[02].Length>0) record.Target = parts[02];
            if (parts.Length > 03 && parts[03].Length>0) record.Time   = Utils.Parse<DateTime>(parts[03]);
            if (parts.Length > 04 && parts[04].Length>0) record.Notes  = parts[04];
            return record;
        }

        public static Audit FromJson(string json)
        {
            return Json.Parse(json, (Audit record, KeyValue kv) =>
            {
                switch (kv.Key)
                {
                    case "Id"    : record.Id     = Utils.Parse<Guid>(kv.Value); break;
                    case "User"  : record.User   = kv.Value; break;
                    case "Target": record.Target = kv.Value; break;
                    case "Time"  : record.Time   = Utils.Parse<DateTime>(kv.Value); break;
                    case "Notes" : record.Notes  = kv.Value; break;
                }
                return record;
            });
        }
    }
}