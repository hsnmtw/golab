namespace WebApp.Data
{
    public class Option<T>
    {
        public T Value { get; private set; }
        public string? Error { get; private set; }

        public Option(T value, string? error = null)
        {
            Value = value;
            Error = error;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Value, Error);
        }

        public bool IsError { get { return !string.IsNullOrEmpty(Error); } }
    }

    public static class Options
    {
        public static Option<V> Create<V>(V value, string? error = null)
        {
            return new Option<V>(value, error);
        }
    }
}