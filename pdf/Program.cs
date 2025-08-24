
using System.Text;
using pdf.Objects;

using var stream = File.Create("./testing/first.pdf");
stream.Write("%PDF-1.7\r"u8);
stream.Write("%нцςп\r"u8);
var xref = new Xref() { Refrences = [] };
xref.Refrences.Add("0000000000 65535 f");

xref.Add(stream); stream.Write(new Catalog   { Reference = "1 0", Outlines = "2 0 R", Pages = "3 0 R" }.Bytes());
xref.Add(stream); stream.Write(new Outlines  { Reference = "2 0", Count = "0" }.Bytes());
xref.Add(stream); stream.Write(new Pages     { Reference = "3 0", Kids = "[4 0 R]", Count = "1" }.Bytes());
xref.Add(stream); stream.Write(new Page      { Reference = "4 0", Parent = "3 0 R", MediaBox = "[0 0 300 300]", Contents = "5 0 R", Resources = "<< /ProcSet 6 0 R >>" }.Bytes());
xref.Add(stream); stream.Write(new Contents  { Reference = "5 0", Length = "0" }.Bytes());
xref.Add(stream); stream.Write(new Procedure { Reference = "6 0" }.Bytes());

var startXref = stream.Length + "";
stream.Write(xref.Bytes());
stream.Write(new Trailer() { Root = "1 0 R", Size = xref.Refrences.Count+"", StartXref = startXref }.Bytes());

stream.Write("%%EOF"u8);