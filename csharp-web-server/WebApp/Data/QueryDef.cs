namespace WebApp.Data
{
    public class QueryDef
    {
        public string Text { get; set; }
        public object[] Arguments { get; private set; }
        private QueryDef(string text, object[] arguments)
        {
            Text = text;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return string.Format(Text, Arguments);
        }

        public static readonly QueryDef All = Build(".*");

        public static QueryDef Build(string text, params object[] arguments)
        {
            return new QueryDef(text, arguments);
        }
    }
}