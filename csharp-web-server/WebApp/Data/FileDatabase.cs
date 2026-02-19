// 
// using System.Text;
// using System.Text.RegularExpressions;
// using System.IO;
// 
// using System.Collections.Generic;
// using WebApp.Data.Tables;

// namespace WebApp.Data
// {
//     public class FileDatabase //: IDatabase
//     {
//         public const string REGEX_CONTAINS = "^([^|]+[|].*{0}.*)$";

//         public string Contains { get { return REGEX_CONTAINS; } }

//         private object _sync = new object();
//         private readonly string WORKING_DIRECTORY = new DirectoryInfo(".\\").FullName;

//         private readonly Dictionary<string,string> Files = new Dictionary<string, string> {
//             {typeof(Service  ).Name, @"\files\data\Service.txt"  },
//             {typeof(Customer ).Name, @"\files\data\Customer.txt" },
//             {typeof(Branch   ).Name, @"\files\data\Branch.txt"   },
//             {typeof(Contract ).Name, @"\files\data\Contract.txt" },
//             {typeof(Room     ).Name, @"\files\data\Room.txt"     },
//             {typeof(Resident ).Name, @"\files\data\Resident.txt" },
//             {typeof(User     ).Name, @"\files\data\User.txt"     },
//             {typeof(Session  ).Name, @"\files\data\Session.txt"  },
//             {typeof(Country  ).Name, @"\files\data\Country.txt"  },
//         };

//         public Option<bool> Remove<S>(Guid id) where S : class, new()
//         {
//             if (id == Guid.Empty) return Options.Create(false, "Guid is empty, cannot delete this record");
//             var sid = id.ToString();
//             lock (_sync)
//             {
//                 var path = Files[typeof(S).Name];
//                 if (string.IsNullOrEmpty(path)) return Options.Create(false, "path is empty/null");
//                 path = WORKING_DIRECTORY + path;
//                 if (!File.Exists(path)) return Options.Create(false, "path not exits");
//                 var lines = File.ReadAllLines(path).Where(line => !line.StartsWith(sid)).ToArray();
//                 File.WriteAllLines(path, lines);
//             }
//             return Options.Create(true);
//         }

//         public Option<bool> Store<S>(S record) where S : class, new()
//         {

//             var path = Files[typeof(S).Name];
//             if (string.IsNullOrEmpty(path)) return Options.Create(false);
//             path = WORKING_DIRECTORY + path;
//             if (!File.Exists(path)) File.Open(path, FileMode.Create).Close();
//             var tmp1 = WORKING_DIRECTORY + @"\files\tmp\" + Guid.NewGuid() + ".txt";
//             var tmp2 = WORKING_DIRECTORY + @"\files\tmp\" + Guid.NewGuid() + ".txt";
//             File.Copy(path, tmp1);
//             Stream fsr = null, fsw = null;
//             StreamReader sr = null;
//             StreamWriter sw = null;
//             string updatedRecord = record.ToString();
//             bool success = false;
//             try
//             {
//                 fsr = File.OpenRead(tmp1);
//                 fsw = File.Open(tmp2, FileMode.OpenOrCreate);
//                 sr = new StreamReader(fsr);
//                 sw = new StreamWriter(fsw);
//                 string line = null;
//                 bool found = false;
//                 while (!string.IsNullOrEmpty(line = sr.ReadLine()) && line.Length >= 36)
//                 {
//                     string id = line.Substring(0, 36);
//                     if (updatedRecord.StartsWith(id))
//                     {
//                         found = true;
//                         sw.WriteLine(updatedRecord);
//                         continue;
//                     }
//                     sw.WriteLine(line);
//                 }
//                 if (!found)
//                     sw.WriteLine(updatedRecord);
//                 success = true;
//             }
//             finally
//             {
//                 if (sr != null) sr.Close();
//                 if (sw != null)
//                 {
//                     sw.Flush();
//                     sw.Close();
//                     sw.Dispose();
//                 }
//                 if (fsr != null) fsr.Dispose();
//                 if (fsw != null) fsw.Dispose();
//                 if (success)
//                 {
//                     lock (_sync)
//                     {
//                         File.Copy(tmp2, path, true);
//                         File.Delete(tmp1);
//                         File.Delete(tmp2);
//                     }
//                 }
//             }

//             return Options.Create(true);
//         }

//         public Option<S> Get<S>(Guid id) where S : class, new()
//         {
//             if (id == Guid.Empty) return Options.Create(new S());
//             var data = Query<S>(QueryDef.Build("^{0}.*$",id), 0, 1);
//             if (!string.IsNullOrEmpty(data.Error)) return Options.Create(new S(), data.Error);
//             return Options.Create(data.Value.FirstOrDefault() ?? new S());       
//         }

//         public Option<List<S>> Query<S>(QueryDef queryDef, int skip = 0, int take = 0, int sortIndex = -1) where S : class, new()
//         {
//             var records = GetLines<S>(queryDef, skip, take, sortIndex).Value.Select(x => Convert<S>(x).Value);
//             return Options.Create(records.ToList());
//         }
        
//         public Option<int> Count<S>(QueryDef queryDef) where S : class, new()
//         {
//             return Options.Create(GetLines<S>(queryDef,0,0,0).Value.Count());
//         }
        
//         public Option<string> GetLines<S>(QueryDef queryDef, int skip = 0, int take = 0, int sortIndex = 0) where S : class, new()
//         {
//             Func<string, bool> predicate = (string line) =>
//             {
//                 if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(queryDef.ToString())) return true;
//                 return Regex.IsMatch(line, queryDef.ToString(), RegexOptions.IgnoreCase);
//             };

//             List<string> data = new List<string>();

//             var path = Files[typeof(S).Name];
//             if (string.IsNullOrEmpty(path)) return Options.Create(data);
//             path = WORKING_DIRECTORY + path;
//             if (!File.Exists(path)) return Options.Create(data);

//             var tmp = WORKING_DIRECTORY + @"\files\tmp\" + Guid.NewGuid() + ".txt";
//             File.Copy(path, tmp);
//             Stream fs = null;
//             StreamReader sr = null;
//             try
//             {
//                 fs = File.OpenRead(path);
//                 sr = new StreamReader(fs);
//                 string line = null;
//                 if (sortIndex < 1)
//                 {
//                     while ((line = sr.ReadLine()) != null)
//                     {
//                         if (string.IsNullOrEmpty(line) || !predicate(line)) continue;
//                         if (skip-- > 0) continue;
//                         data.Add(line);
//                         if (take-- > 0) break;
//                     }
//                 }
//                 else
//                 {
//                     //read the entire file, sort, skip, take
//                     List<string[]> lines = new List<string[]>();
//                     int minL = 1000;
//                     while ((line = sr.ReadLine()) != null)
//                     {
//                         if (string.IsNullOrEmpty(line) || !predicate(line)) continue;
//                         string[] parts = Utils.SplitAndTrim(line, '|');
//                         minL = Math.Min(minL, parts.Length);
//                         lines.Add(parts);
//                     }

//                     if (sortIndex < minL)
//                         lines = lines.OrderBy(parts => parts[sortIndex]).ToList();

//                     data = lines.Skip(skip)
//                                 .Take(take < 1 ? lines.Count : take)
//                                 .Select(parts => string.Join("|", parts))
//                                 .ToList();
//                 }
//             }
//             finally
//             {
//                 if (sr != null)
//                 {
//                     sr.Close();
//                     sr.Dispose();
//                 }
//                 if (fs != null) fs.Dispose();
//                 if (File.Exists(tmp)) File.Delete(tmp);
//             }

//             return Options.Create(data);
//         }

//         public Option<S> Convert<S>(string line) where S : class, new()
//         {
//             S record = null;
//             if (!string.IsNullOrEmpty(line))
//             {
//                 string type = typeof(S).Name;
//                 try
//                 {
//                     switch (type)
//                     {
//                         case "Service":  record = Service.Convert(line)  as S; break;
//                         case "Customer": record = Customer.Convert(line) as S; break;
//                         case "Branch":   record = Branch.Convert(line)   as S; break;
//                         case "Contract": record = Contract.Convert(line) as S; break;
//                         case "Room":     record = Room.Convert(line)     as S; break;
//                         case "Resident": record = Resident.Convert(line) as S; break;
//                         case "User":     record = User.Convert(line)     as S; break;
//                         case "Session":  record = Session.Convert(line)  as S; break;
//                         case "Country":  record = Country.Convert(line)  as S; break;
//                         default: throw new Exception("Unable to locate a method to convert data type : " + type);
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     System.Console.Error.WriteLine("[ERROR / data conversion failed to type {0}] :: `{1}`\n{2}", type, line, ex);
//                 }
//             }
//             return Options.Create(record);
//         }
//     }
// }