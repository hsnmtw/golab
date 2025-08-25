
using System.Text;
using pdf.Engine;
using pdf.Objects;

using var stream = File.Create("./testing/first.pdf");
_ = new Document(stream);
