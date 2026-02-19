
using System.Data.Common;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

const string CONNECTION_STRING = "Server=.\\sqlexpress;database=TAWFEEQ;User=U1;Password=123456;Trust Server Certificate=True";

DbConnection cn = new SqlConnection();
cn.ConnectionString = CONNECTION_STRING;
cn.Open();

// DbCommand cmd = cn.CreateCommand();
// cmd.CommandText = "INSERT INTO [T_PERSON] ([Name],[Age]) VALUES (@1,@2)";
// cmd.Parameters.Add(new SqlParameter("@1","Person 2"));
// cmd.Parameters.Add(new SqlParameter("@2",10));
// cmd.ExecuteNonQuery();

List<Person> people = [];


DbCommand cmd = cn.CreateCommand();
cmd.CommandText = "SELECT [Id],[Name],[Age] FROM T_PERSON";
DbDataReader reader = cmd.ExecuteReader();
while(reader.Read())
{
    Person p = new Person()
    {
        Id = reader.GetInt32(0),  
        Name = reader.GetString(1),  
        Age = reader.GetInt32(2),  
    };

    people.Add(p);

}
reader.Close();
cn.Close();

foreach(var p in people)
{
    System.Console.WriteLine(p);
}

QuestPDF.Settings.License = LicenseType.Community;
var document = QuestPDF.Fluent.Document.Create(container =>
{
    container.Page(page =>
    {
        //page.Size(PageSizes.A4);
        page.Size(new PageSize(width: 100, height: 75, unit: Unit.Millimetre));
        page.Margin(5, Unit.Millimetre);
        page.PageColor(Colors.Yellow.Medium);
        page.Header().Element(MyHeader);
        page.Footer().Element(MyFooter);
        page.Content().AlignRight().Element(MyContent);
    });
});



void MyHeader(IContainer container)
{
    container
    .Background(Colors.Blue.Medium)
    .Row(row =>
    {
        row.RelativeItem(1).Text("1");
        row.RelativeItem(1).Text("2");
        row.RelativeItem(1).Text("3");
        row.RelativeItem(1).Text("4");
        row.RelativeItem(1).Text(text =>
            {
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
    });
}

void MyContent(IContainer container)
{
    // container.Column(c => c.Item().Text(MyConstants.LongText));
    container.Table(table =>
    {
        table.ColumnsDefinition(cd =>
        {
            cd.RelativeColumn(1);
            cd.RelativeColumn(2);
            cd.RelativeColumn(1);
        });

        table.Header(header =>
        {
            header.Cell().Border(1).Background(Colors.Grey.Lighten1).Text("Id");
            header.Cell().Border(1).Background(Colors.Grey.Lighten1).Text("Name");
            header.Cell().Border(1).Background(Colors.Grey.Lighten1).Text("Age");
        });

        for(int i=0;i<people.Count;++i){
            Person p = people[i];
            table.Cell().Border(1).Background(Colors.White).Padding(3f, Unit.Millimetre).Text(p.Id.ToString());
            table.Cell().Border(1).Background(Colors.White).Padding(3f, Unit.Millimetre).Text(p.Name);
            table.Cell().Border(1).Background(Colors.White).Padding(3f, Unit.Millimetre).Text(p.Age.ToString());
        }
        table.Cell().ColumnSpan(3).Border(1).Background(Colors.White)
            .ContentFromRightToLeft()
            .Padding(3f, Unit.Millimetre).Text(
            "هل يدعم اللغة العربية");
    });
}
void MyFooter(IContainer container)
{
    container
    .Background(Colors.Red.Medium)
    .Row(row =>
    {
        row.RelativeItem(1).Text("a");
        row.RelativeItem(1).Text("b");
        row.RelativeItem(1).Text("c");
        row.RelativeItem(1).Text("d");
    });
}


document.GeneratePdf("test.pdf");

System.Console.WriteLine("DONE ....");


