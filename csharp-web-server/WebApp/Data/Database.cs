namespace WebApp.Data;
public static class Database
{
    public static readonly IDatabase Instance = new SqliteDatabase();
}