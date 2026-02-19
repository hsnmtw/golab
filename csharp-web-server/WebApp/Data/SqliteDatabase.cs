 
 using System.Text;
 using System.Data;
 using System.Data.Common;
 using Microsoft.Data.Sqlite;
 using System.Collections.Generic;
 

 namespace WebApp.Data
 {

     public class SqliteDatabase : IDatabase
     {
         private const string CONNECTION_STRING = "data source=./database.sqlite";

         internal SqliteDatabase() { }

         public string GetPage(int skip, int take)
         {
             if (take < 1) return "";
             return string.Format(" LIMIT {1} OFFSET {0} ", skip, take);
             SqlClient $" OFFSET {skip} ROWS FETCH FIRST {take} ROWS ONLY "
            
         }

         private static void Error(string ex)
         {
             Console.ForegroundColor = ConsoleColor.Red;
             System.Console.Error.WriteLine(ex);
             Console.ForegroundColor = ConsoleColor.White;
         }

         private ValueTuple<DbConnection, DbCommand> CreateCommand(string sql, params object?[] args)
         {
             var connection = new SqliteConnection(CONNECTION_STRING);
             connection.Open();
 	        connection.CreateCollation("NOCASE",(x, y) => String.Compare(x, y, StringComparison.InvariantCultureIgnoreCase));
             connection.CreateFunction("getdate",() => Utils.Now);
             connection.CreateFunction("guid",   () => Guid.NewGuid().ToString().ToLower());
             connection.CreateFunction("rnd",    () => string.Join("",Guid.NewGuid().ToString().Where(Char.IsNumber)));
             connection.CreateFunction("concat", (object s1) => string.Concat(s1));
             connection.CreateFunction("concat", (object s1,object s2) => string.Concat(s1,s2));
             connection.CreateFunction("concat", (object s1,object s2,object s3) => string.Concat(s1,s2,s3));
             connection.CreateFunction("concat", (object s1,object s2,object s3,object s4) => string.Concat(s1,s2,s3,s4));
            
             var command = connection.CreateCommand();

             command.CommandText = sql;
             command.Prepare();

             var dbParameters = ConvertToDbParameters(args);
             foreach (var p in dbParameters)
                 command.Parameters.Add(p);

             return new ValueTuple<DbConnection, DbCommand>(connection, command);
         }
        
         public static readonly DateTime MINIMUM_ALLOWED_DATE_VALUE = new DateTime(1900,1,1);
         public static readonly DateTime MAXIMUM_ALLOWED_DATE_VALUE = new DateTime(2100,1,1);

         private static readonly Dictionary<Type, DbType> typeMap = new()
         {
             {typeof(byte)                   , DbType.Byte}             ,                              
             {typeof(sbyte)                  , DbType.SByte}            ,                              
             {typeof(short)                  , DbType.Int16}            ,                              
             {typeof(ushort)                 , DbType.UInt16}           ,                              
             {typeof(int)                    , DbType.Int32}            ,                              
             {typeof(uint)                   , DbType.UInt32}           ,                              
             {typeof(long)                   , DbType.Int64}            ,                              
             {typeof(ulong)                  , DbType.UInt64}           ,                              
             {typeof(float)                  , DbType.Single}           ,                              
             {typeof(double)                 , DbType.Double}           ,                              
             {typeof(decimal)                , DbType.Decimal}          ,                              
             {typeof(bool)                   , DbType.Boolean}          ,                              
             {typeof(string)                 , DbType.String}           ,                              
             {typeof(char)                   , DbType.StringFixedLength},                              
             {typeof(Guid)                   , DbType.Guid}             ,                              
             {typeof(DateTime)               , DbType.DateTime}         ,                              
             {typeof(DateOnly)               , DbType.DateTime}             ,     
             {typeof(TimeOnly)               , DbType.DateTime}             ,
             {typeof(DateTimeOffset)         , DbType.DateTimeOffset}   ,                              
             {typeof(byte[])                 , DbType.Binary}           ,                              
             {typeof(byte?)                  , DbType.Byte}             ,                              
             {typeof(sbyte?)                 , DbType.SByte}            ,                              
             {typeof(short?)                 , DbType.Int16}            ,                              
             {typeof(ushort?)                , DbType.UInt16}           ,                              
             {typeof(int?)                   , DbType.Int32}            ,                              
             {typeof(uint?)                  , DbType.UInt32}           ,                              
             {typeof(long?)                  , DbType.Int64}            ,                              
             {typeof(ulong?)                 , DbType.UInt64}           ,                              
             {typeof(float?)                 , DbType.Single}           ,                              
             {typeof(double?)                , DbType.Double}           ,                              
             {typeof(decimal?)               , DbType.Decimal}          ,                              
             {typeof(bool?)                  , DbType.Boolean}          ,                              
             {typeof(char?)                  , DbType.StringFixedLength},                              
             {typeof(Guid?)                  , DbType.Guid}             ,                              
             {typeof(DateTime?)              , DbType.DateTime}         ,                              
             {typeof(DateOnly?)              , DbType.DateTime}             ,                              
             {typeof(TimeOnly?)              , DbType.DateTime}             ,                              
             {typeof(DateTimeOffset?)        , DbType.DateTimeOffset}   ,                              
         };

         private static DbType Map(Type type)
         {
             if(typeMap.TryGetValue(type, out var dbType)) return dbType;
             throw new Exception($"unknown type : {type} and cannot be mapped to db type");
         }
         public IDbDataParameter[] ConvertToDbParameters(object?[] parameters)
         {
             IDbDataParameter[] result = new IDbDataParameter[parameters.Length];
             for (int i = 0; i < parameters.Length; i++)
             {
                 IDbDataParameter prm = SqliteFactory.Instance.CreateParameter(); ?? throw new Exception("Unable to create DB Parameter");

                 prm.ParameterName = $"@{i}";

                 var /*Type?*/ paramType = parameters[i]?.GetType();

                 if (paramType is null || Equals(string.Empty, parameters[i]) || parameters[i] is null || parameters[i] is DBNull)
                 {
                     prm.Value = (object)DBNull.Value;
                 }
                 else if (typeof(Guid) == paramType)
                 {
                     prm.Value = $"{parameters[i]}".ToLower();
                 }
                 else if (typeof(double) == paramType || typeof(float) == paramType)
                 {

                     double val = Convert.ToDouble(parameters[i]);
                     prm.DbType = DbType.Decimal;
                     prm.Value = val;
                     if (double.IsNaN(val) || double.IsNegativeInfinity(val) || double.IsInfinity(val))
                     {
                         prm.Value = 0;
                     }
                 }
                 else if (parameters[i] is not null && parameters[i] is not DBNull && (typeof(DateTime) == paramType || typeof(DateTime?) == paramType))
                 {
                     DateTime val = Convert.ToDateTime(parameters[i]);
                     if (val < MINIMUM_ALLOWED_DATE_VALUE || val > MAXIMUM_ALLOWED_DATE_VALUE)
                         throw new Exception($"# the value '{val:d'/'M'/'yyyy}' entered is out of the accepted range : " + string.Join("/", parameters));
                     prm.DbType = DbType.DateTime;
                     prm.Value = val;
                 }
                 else if (paramType.IsEnum)
                 {
                     prm.DbType = DbType.String;
                     prm.Value = $"{parameters[i]}";
                 }
                 else
                 {
                     prm.DbType = Map(paramType);
                     prm.Value = parameters[i];
                 }
                 result[i] = prm;
             }
             return result;
         }

         public Option<int> Execute(string sql, params object?[] args)
         {
             try
             {
                 var tuple = CreateCommand(sql, args);
                 var cn = tuple.Item1;
                 var cmd = tuple.Item2;
                 var result = cmd.ExecuteNonQuery();
                 cn.Close();

                 if (result < 1) return Options.Create(result, "failed to store information");
                 return Options.Create(result);
             }
             catch (Exception ex)
             {
                 Error(string.Format("{0}  {1}", sql, ex));
                 return Options.Create(-1, ex.Message);
             }

         }
         public Option<string> Query(string sql, params object?[] args)
         {
             try
             {
                 var tuple = CreateCommand(sql, args);
                 var cn = tuple.Item1;
                 var cmd = tuple.Item2;
                 var reader = cmd.ExecuteReader();
                 var sb = new StringBuilder();
                 while (reader.Read())
                 {
                     for (int i = 0; i < reader.FieldCount; ++i)
                     {
                         sb.Append(reader.GetValue(i));
                         sb.Append('|');
                     }
                     sb.Length--;
                     sb.Append('\n');
                 }
                 reader.Close();
                 cn.Close();
                 return Options.Create(sb.ToString());
                
             }
             catch (Exception ex)
             {
                 Error(string.Format("{0}  {1}", sql, ex));
                 return Options.Create("", ex.Message);
             }
         }
     }
 }