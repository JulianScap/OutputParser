using System;
using System.Collections.Generic;
using System.IO;
using Sprache;

namespace TestSprache
{
    class Token
    {
        public string Text { get; set; }
        public ConsoleColor Colour { get; set; }

        public Token(string text, ConsoleColor colour)
        {
            Text = text;
            Colour = colour;
        }

        public Token(char c, ConsoleColor colour)
        {
            Text = c.ToString();
            Colour = colour;
        }
    }

    class Program
    {
        protected Program() { }

        static readonly Parser<string> guid = Parse.Regex(@"[{(]?[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?", "Guid");
        static readonly Parser<string> url = Parse.Regex(@"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?", "URL");
        static readonly Parser<string> ip = Parse.Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}", "IP address");
        static readonly Parser<string> dateTime = Parse.Regex(@"\d+[/ :](?:\d+[/ :])+\d+", "Date time");
        static readonly Parser<string> namespaces = Parse.Regex(@"(?:\w+\.)+\w+", "Namespaces or classes");
        static readonly Parser<string> unc = Parse.Regex(@"\\\\[-\w\.\\\$]+", "Unc path");
        static readonly Parser<string> path = Parse.Regex(@"[^\W\d]\:\\[-\w\.\\\$]+", "Path");

        static readonly Parser<Token> keyword =
            url
            .Or(ip)
            .Or(dateTime)
            .Or(namespaces)
            .Or(unc)
            .Or(path)
            .Or(guid)
            .Or(Parse.Decimal)
            .Named("keywords")
            .Select(x => new Token(x, ConsoleColor.Red));

        static readonly Parser<Token> rest = Parse.AnyChar
            .Named("nothing")
            .Select(x => new Token(x, ConsoleColor.Green));

        static readonly Parser<Token> all = keyword.XOr(rest);

        static void Main(string[] args)
        {
            var text = File.ReadAllText(@"C:\Users\julian.adler\Desktop\TestSprache\patch.20200318.011718.log");
            Write(all.Many().Parse(text));
        }

        private static void Write(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Write(token);
            }
        }

        private static void Write(Token token)
        {
            var before = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = token.Colour;
                Console.Write(token.Text);
            }
            finally
            {
                Console.ForegroundColor = before;
            }
        }
    }
}
