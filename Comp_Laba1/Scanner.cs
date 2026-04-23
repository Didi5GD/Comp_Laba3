using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comp_Laba1
{
    public class ScanTokin
    {
        public int Usl_code { get; set; }
        public string Type { get; set; }
        public string Lecsema { get; set; }
        public string Place { get; set; }
    }



    public class Scanner
    {
        private string input;
        private int pos, currentLine, currentColumn;
        private List<ScanTokin> tokens;

        private readonly Dictionary<string, int> typeToCode = new Dictionary<string, int>
        {
            { "IDENTIFIER", 1 }, { "IF", 2 }, { "ELSE", 3 }, { "LBRACE", 9 }, { "RBRACE", 10 },
            { "ASSIGN", 11 }, { "EQUAL", 12 }, { "LPAREN", 14 }, { "RPAREN", 15 },
            { "SEMICOLON", 22 }, { "GREATER", 23 }, { "LESS", 24 },
            { "INCREMENT", 25 }, { "DECREMENT", 26 }
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
                if (TryParseLexeme()) continue;

                // Собираем мусор до ближайшего разделителя
                int errLine = currentLine;
                int errCol = currentColumn;
                StringBuilder garbage = new StringBuilder();
                while (pos < input.Length && !char.IsWhiteSpace(input[pos]) && !IsPunctuation(input[pos]))
                {
                    garbage.Append(input[pos]);
                    Advance();
                }
                RecordError(garbage.ToString(), errLine, errCol);
            }
            return tokens;
        }

        private bool IsPunctuation(char c) => "=(){};><$".Contains(c);

        private bool TryParseLexeme()
        {
            int sL = currentLine, sC = currentColumn;
            if (pos + 1 < input.Length)
            {
                string two = input.Substring(pos, 2);
                string t = (two == "==") ? "EQUAL" : (two == "++") ? "INCREMENT" : (two == "--") ? "DECREMENT" : null;
                if (t != null) { AddToken(t, two, sL, sC); Advance(); Advance(); return true; }
            }

            char f = input[pos];
            if (f == '$')
            {
                int start = pos;
                Advance();
                while (pos < input.Length && char.IsLetterOrDigit(input[pos])) Advance();
                string raw = input.Substring(start, pos - start);
                if (raw.Length > 1 && char.IsLetter(raw[1])) AddToken("IDENTIFIER", raw, sL, sC);
                else RecordError(raw, sL, sC);
                return true;
            }

            if (char.IsLetter(f))
            {
                int start = pos;
                while (pos < input.Length && char.IsLetterOrDigit(input[pos])) Advance();
                string w = input.Substring(start, pos - start);
                if (w == "if" || w == "else") { AddToken(w.ToUpper(), w, sL, sC); return true; }
                RecordError(w, sL, sC);
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
        private void RecordError(string lex, int ln, int cl) =>
            tokens.Add(new ScanTokin { Usl_code = 0, Type = "ERROR", Lecsema = lex, Place = $"({ln},{cl})" });
    }
}