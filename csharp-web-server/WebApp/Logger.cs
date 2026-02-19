
using System.IO;


namespace WebApp.Http;

public static class Logger {
    private static readonly object _sync = new();

    public static void Info(string message, string? user = default, string? ip = default) {
        lock (_sync) {
            Console.Out.Flush();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n{0:d'/'M hh':'mm':'ss}", Utils.Now);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" [inf] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("{0,-8}", user);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" @ ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("{0,-16}", ip);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
            Console.Out.Flush();
        }
    }
    public static void Warning(string message, string? user = default, string? ip = default) {
        lock (_sync) {
            Console.Out.Flush();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n{0:d'/'M hh':'mm':'ss}", Utils.Now);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" [wrn] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("{0,-8}", user);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" @ ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("{0,-16}", ip);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
            Console.Out.Flush();
        }
    }
    public static void Error(string message, string? user = default, string? ip = default) {
        lock (_sync) {
            Console.Out.Flush();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Error.Write("\n{0:d'/'M hh':'mm':'ss}", Utils.Now);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write(" [err] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Error.Write("{0,-8}", user);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Error.Write(" @ ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Error.Write("{0,-16}", ip);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Error.Write(message);
            Console.Error.Flush();

            try { File.AppendAllLines("./error.log", [$"{Utils.Now:s} | {message}"]); } catch { }
        }
    }
}