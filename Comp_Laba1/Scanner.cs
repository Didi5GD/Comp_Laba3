using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp_Laba1
{
    public class ScanTokin
    {
        public int Usl_code {  get; set; }
        public string Type { get; set; }
        public string Lecsema { get; set; }
        public string Place {  get; set; }

        public int Line
        {
            get
            {
                if (string.IsNullOrEmpty(Place)) return 1;
                var parts = Place.Trim('(', ')').Split(',');
                return int.Parse(parts[0]);
            }
        }

        public int Column
        {
            get
            {
                if (string.IsNullOrEmpty(Place)) return 1;
                var parts = Place.Trim('(', ')').Split(',');
                return int.Parse(parts[1]);
            }
        }

    }

    public class Scanner
    {
    
            private string input;
            private int pos;
            private int currentLine;
            private int currentColumn;
            private List<ScanTokin> tokens;

            private readonly Dictionary<string, int> typeToCode = new Dictionary<string, int>
        {
            { "IDENTIFIER", 1 },
            { "IF", 2 },
            { "ELSE", 3 },
            { "ELSEIF", 4 },
            { "PLUS", 5 },
            { "INCREMENT", 6 },
            { "MINUS", 7 },
            { "DECREMENT", 8 },
            { "LBRACE", 9 },
            { "RBRACE", 10 },
            { "ASSIGN", 11 },
            { "EQUAL", 12 },
            { "DIV", 13 },
            { "LPAREN", 14 },
            { "RPAREN", 15 },
            { "NEQUAL", 16 },
            { "MOD", 17 },
            { "LESS", 18 },
            { "LESSEQ", 19 },
            { "GREATER", 20 },
            { "GREATEREQ", 21 },
            { "SEMICOLON", 22 },
            { "MULTIPLY", 23 },
            { "POWER", 24 },
            { "OR", 25 },
            { "AND", 26 }
        };

            public Scanner()
            {
                tokens = new List<ScanTokin>();
            }

            public List<ScanTokin> Analyze(string source)
            {
                input = source;
                pos = 0;
                currentLine = 1;
                currentColumn = 1;
                tokens.Clear();

                while (pos < input.Length)
                {
                    char currentChar = input[pos];

                    if (currentChar == ' ' || currentChar == '\t')
                    {
                        Advance();
                        continue;
                    }

                    if (currentChar == '\n')
                    {
                        currentLine++;
                        currentColumn = 1;
                        pos++;
                        continue;
                    }

                    if (currentChar == '\r')
                    {
                        pos++;
                        continue;
                    }

                    if (!TryParseLexeme())
                    {
                        RecordError($"Недопустимый символ '{currentChar}'");
                        Advance();
                    }
                }

                return tokens;
            }

            private bool TryParseLexeme()
            {
                int startLine = currentLine;
                int startColumn = currentColumn;

                if (pos + 1 < input.Length)
                {
                    string twoChars = input.Substring(pos, 2);
                    switch (twoChars)
                    {
                        case "==":
                            AddToken("EQUAL", "==", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case "!=":
                            AddToken("NEQUAL", "!=", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case "<=":
                            AddToken("LESSEQ", "<=", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case ">=":
                            AddToken("GREATEREQ", ">=", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case "++":
                            AddToken("INCREMENT", "++", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case "--":
                            AddToken("DECREMENT", "--", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case "**":
                            AddToken("POWER", "**", startLine, startColumn);
                            Advance(); Advance();
                            return true;
                        case "||":
                             AddToken("OR", "||", startLine, startColumn);
                             Advance(); Advance();
                             return true;
                        case "&&":
                            AddToken("AND", "&&", startLine, startColumn);
                             Advance(); Advance();
                             return true;
                }
                }

                char first = input[pos];

                if (first == '$' && pos + 1 < input.Length && char.IsLetter(input[pos + 1]))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append('$');
                    Advance(); 

                    while (pos < input.Length && (char.IsLetterOrDigit(input[pos])))
                    {
                        sb.Append(input[pos]);
                        Advance();
                    }

                    AddToken("IDENTIFIER", sb.ToString(), startLine, startColumn);
                    return true;
                }

                if (char.IsLetter(first))
                {
                    StringBuilder sb = new StringBuilder();
                    while (pos < input.Length && char.IsLetter(input[pos]))
                    {
                        sb.Append(input[pos]);
                        Advance();
                    }
                    string word = sb.ToString();
                    switch (word)
                    {
                        case "if": AddToken("IF", "if", startLine, startColumn); return true;
                        case "else": AddToken("ELSE", "else", startLine, startColumn); return true;
                        case "elseif": AddToken("ELSEIF", "elseif", startLine, startColumn); return true;
                        default:
                            RecordError($"Недопустимая лексема '{word}'");
                            return true;
                    }
                }

                switch (first)
                {
                    case '+': AddToken("PLUS", "+", startLine, startColumn); Advance(); return true;
                    case '-': AddToken("MINUS", "-", startLine, startColumn); Advance(); return true;
                    case '*': AddToken("MULTIPLY", "*", startLine, startColumn); Advance(); return true;
                    case '/': AddToken("DIV", "/", startLine, startColumn); Advance(); return true;
                    case '%': AddToken("MOD", "%", startLine, startColumn); Advance(); return true;
                    case '=': AddToken("ASSIGN", "=", startLine, startColumn); Advance(); return true;
                    case '<': AddToken("LESS", "<", startLine, startColumn); Advance(); return true;
                    case '>': AddToken("GREATER", ">", startLine, startColumn); Advance(); return true;
                    case ';': AddToken("SEMICOLON", ";", startLine, startColumn); Advance(); return true;
                    case '(': AddToken("LPAREN", "(", startLine, startColumn); Advance(); return true;
                    case ')': AddToken("RPAREN", ")", startLine, startColumn); Advance(); return true;
                    case '{': AddToken("LBRACE", "{", startLine, startColumn); Advance(); return true;
                    case '}': AddToken("RBRACE", "}", startLine, startColumn); Advance(); return true;
                    default:
                        return false;
                }
            }

            private void Advance()
            {
                pos++;
                currentColumn++;
            }

            private void AddToken(string type, string lexeme, int line, int column)
            {
                tokens.Add(new ScanTokin
                {
                    Usl_code = typeToCode.ContainsKey(type) ? typeToCode[type] : 0,
                    Type = type,
                    Lecsema = lexeme,
                    Place = $"({line},{column})"
                });
            }

            private void RecordError(string message)
            {
                tokens.Add(new ScanTokin
                {
                    Usl_code = 0,
                    Type = "ERROR",
                    Lecsema = message,
                    Place = $"({currentLine},{currentColumn})"
                });
            }
    }
}

