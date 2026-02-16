namespace baby_json_parser;

using System;

public class Program {
    public static void Main(string[] args) {
        Test1();
    }

    public static void Test1() {
        string json = @"{
            ""name"": ""John"",
            ""age"": 30,
            ""isStudent"": false,
            ""scores"": [85, 90, 92],
            ""address"": {
                ""street"": ""123 Main St"",
                ""city"": ""Anytown"",
                ""zip"": ""12345"",
            },
            ""unicodeTest"": ""\u0041\u0042\u0043"",
            ""escapedQuotes"": ""He said, \""Hello!\"""",
            ""escapedBackslash"": ""This is a backslash: \\\\"",
            ""escapedNewline"": ""This is a newline:\nNext line."",
            ""escapedTab"": ""This is a tab:\tNext part."",
            ""escapedCarriageReturn"": ""This is a carriage return:\rNext part.""
        }";

        JsonParser.Parse(json, (key, value) => {
            Console.WriteLine($"Key: {key}, Value: {value}");
        });
    }
}