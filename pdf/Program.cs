
using System.Text;
using pdf.Engine;
using pdf.Objects;

using var stream = File.Create("./testing/first.pdf");
using var pdf = new Document(stream);
pdf.Page(200, 100,"");
pdf.Close();