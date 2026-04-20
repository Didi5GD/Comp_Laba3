using System;
using System.Collections.Generic;
using System.Text;

namespace Comp_Laba1
{
    public class ScanTokin
    {
        public int Usl_code { get; set; }
        public string Type { get; set; }
        public string Lecsema { get; set; }
        public string Place { get; set; }

        public int Line => int.Parse(Place.Trim('(', ')').Split(',')[0]);
        public int Column => int.Parse(Place.Trim('(', ')').Split(',')[1]);
    }

    public class Scanner
    {
        private string input;
        private int pos, currentLine, currentColumn;
        private List<ScanTokin> tokens;

        private readonly Dictionary<string, int> typeToCode = new Dictionary<string, int>
        {
            { "IDENTIFIER", 1 }, { "IF", 2 }, { "ELSE", 3 }, { "LBRACE", 9 }, { "RBRACE", 10 },
            { "ASSIGN", 11 }, { "EQUAL", 12 }, { "LPAREN", 14 }, { "RPAREN", 15 }, { "SEMICOLON", 22 }
        };

        public Scanner() => tokens = new List<ScanTokin>();

        public List<ScanTokin> Analyze(string source)
        {
            input = source; pos = 0; currentLine = 1; currentColumn = 1;
            tokens.Clear();
            while (pos < input.Length)
            {
                char c = input[pos];
                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n') { currentLine++; currentColumn = 1; }
                    else currentColumn++;
                    pos++; continue;
                }
                if (!TryParseLexeme()) { RecordError("Недопустимый символ " + input[pos]); Advance(); }
            }
            return tokens;
        }

        private bool TryParseLexeme()
        {
            int sL = currentLine, sC = currentColumn;
            if (pos + 1 < input.Length)
            {
                string two = input.Substring(pos, 2);
                string t = two == "==" ? "EQUAL" : two == "++" ? "INCREMENT" : two == "--" ? "DECREMENT" : null;
                if (t != null) { AddToken(t, two, sL, sC); Advance(); Advance(); return true; }
            }

            char f = input[pos];
            if (f == '$')
            {
                StringBuilder sb = new StringBuilder().Append(f); Advance();
                while (pos < input.Length && char.IsLetterOrDigit(input[pos])) { sb.Append(input[pos]); Advance(); }
                if (sb.Length > 1) AddToken("IDENTIFIER", sb.ToString(), sL, sC);
                else RecordError("Ожидалось имя переменной после $");
                return true;
            }

            if (char.IsLetter(f))
            {
                StringBuilder sb = new StringBuilder();
                while (pos < input.Length && char.IsLetter(input[pos])) { sb.Append(input[pos]); Advance(); }
                string w = sb.ToString();
                if (w == "if") AddToken("IF", w, sL, sC);
                else if (w == "else") AddToken("ELSE", w, sL, sC);
                else RecordError("Недопустимая лексема " + w);
                return true;
            }

            string st = f == '=' ? "ASSIGN" : f == '(' ? "LPAREN" : f == ')' ? "RPAREN" :
                        f == '{' ? "LBRACE" : f == '}' ? "RBRACE" : f == ';' ? "SEMICOLON" :
                        f == '>' ? "GREATER" : f == '<' ? "LESS" : null;
            if (st != null) { AddToken(st, f.ToString(), sL, sC); Advance(); return true; }
            return false;
        }

        private void Advance() { pos++; currentColumn++; }
        private void AddToken(string t, string l, int ln, int cl) =>
            tokens.Add(new ScanTokin { Usl_code = typeToCode.ContainsKey(t) ? typeToCode[t] : 0, Type = t, Lecsema = l, Place = $"({ln},{cl})" });
        private void RecordError(string m) =>
            tokens.Add(new ScanTokin { Usl_code = 0, Type = "ERROR", Lecsema = m, Place = $"({currentLine},{currentColumn})" });
    }
}