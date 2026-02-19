



namespace HOS.App.Features.Security
{
    /*
    DROP TABLE T_LICENSE;
    CREATE TABLE T_LICENSE(
        [Id]       VARCHAR(36)    NOT NULL,
        [Name]     VARCHAR(200)   NOT NULL,
        [Address]  VARCHAR(200)   NOT NULL
    );
    INSERT INTO T_LICENSE(Id,Name,Address) 
    VALUES (
    '69fdac05-9a50-4de4-785d-718339bd187b',
    'Hussain A. Al-Mutawa Sons Trading and Contracting Co. L.L.C. (HAMTE Group)',
    'Jubail Industrial City - Saudi Arabia');

    */
    public class License
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";

        public override string ToString()
        {
            return string.Join("|",
                Id,
                Name,
                Address
            );
        }
        public static License Convert(string line)
        {
            var record = new License();
            if (string.IsNullOrEmpty(line)) return record;
            string[] parts = Utils.SplitAndTrim(line, '|');
            if (parts.Length > 00 && parts[00].Length > 0) record.Id      = Utils.Parse<Guid>(parts[00]);
            if (parts.Length > 01 && parts[01].Length > 0) record.Name    = parts[01];
            if (parts.Length > 02 && parts[02].Length > 0) record.Address = parts[02];
            return record;
        }

        public static License FromJson(string json)
        {
            return Json.Parse(json, (License record, KeyValue kv) =>
            {
                switch (kv.Key)
                {
                    case "Id": record.Id = Utils.Parse<Guid>(kv.Value); break;
                    case "Name": record.Name = kv.Value; break;
                    case "Address": record.Address = kv.Value; break;
                }
                return record;
            });
        }
    }
}