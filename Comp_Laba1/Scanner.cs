using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace Comp_Laba1
{
    public class TokenDict
    {
        public Dictionary<int, string> tokens { get; }
        public TokenDict()
        {
            tokens = new Dictionary<int, string>
            {
                {1, "id" },
                {2, "if" },
                {3, "else" },
                {5, "оператор сложения" },
                {6, "оператор инкремента" },
                {7, "оператор вычитания" },
                {8, "оператор декремента" },
                {9, "открывающая фигурная скоба" },
                {10, "закрывающая фигурная скобка" },
                {11, "открывающая круглая скобка" },
                {12, "закрывающая круглая скобка" },
                {13, "оператор меньше" },
                {14, "оператор меньше или равно" },
                {15, "оператор больше" },
                {16, "оператор больше или равно" },
                {17, "точка с запятой" },
                {18, "оператор присваивания" },
                {19, "оператор равенства" },
                {20, "оператор неравенства" },
                {-1, "ERROR: неизвестный токен" }
            };
        }
    }

    public class ScanToken
    {
        public int Usl_code { get; set; }
        public string Type { get; set; }
        public string Lecsema { get; set; }
        public string Place { get; set; }

        public ScanToken(int code, string type, string lexeme, int line, int start, int end)
        {
            Usl_code = code;
            Type = type;
            Lecsema = lexeme;
            Place = $"({line}, {start}-{end})";
        }
    }

    public class LexicalAnalyzer
    {
        public TokenDict tokenDict = new TokenDict();
        public LexicalAnalyzer()
        {
            tokenDict = new TokenDict();
        }
        public List<ScanToken> AnalyzeText(string filePath)
        {
            int lineNumber = -1;
            List<ScanToken> tabs = new List<ScanToken>();

            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    lineNumber++;

                    int pos = 0;
                    while (pos < line.Length)
                    {
                        if (char.IsWhiteSpace(line[pos]))
                        {
                            pos++;
                            continue;
                        }

                        bool found = false;

                        // code 17 ;
                        if (pos < line.Length && line.Substring(pos, 1) == ";")
                        {
                            tabs.Add(new ScanToken(17, tokenDict.tokens[17],
                                ";", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 2 if
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "if"
                            && (pos + 2 < line.Length && line[pos + 2] == ' ' || pos + 2 >= line.Length))
                        {
                            tabs.Add(new ScanToken(2, tokenDict.tokens[2], "if", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 3 else
                        else if (pos + 3 < line.Length && line.Substring(pos, 4) == "else"
                            && (pos + 4 < line.Length && line[pos + 4] == ' ' || pos + 4 >= line.Length))
                        {
                            tabs.Add(new ScanToken(3, tokenDict.tokens[3], "else", lineNumber, pos, pos + 4));
                            pos += 4;
                            found = true;
                        }

                        // code 6 ++
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "++")
                        {
                            tabs.Add(new ScanToken(6, tokenDict.tokens[6], "++", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 5 +
                        else if (pos < line.Length && line.Substring(pos, 1) == "+")
                        {
                            tabs.Add(new ScanToken(5, tokenDict.tokens[5], "+", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 8 --
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "--")
                        {
                            tabs.Add(new ScanToken(8, tokenDict.tokens[8], "--", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 7 -
                        else if (pos < line.Length && line.Substring(pos, 1) == "-")
                        {
                            tabs.Add(new ScanToken(7, tokenDict.tokens[7], "-", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 9 {
                        else if (pos < line.Length && line.Substring(pos, 1) == "{")
                        {
                            tabs.Add(new ScanToken(9, tokenDict.tokens[9],
                                "{", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 10 }
                        else if (pos < line.Length && line.Substring(pos, 1) == "}")
                        {
                            tabs.Add(new ScanToken(10, tokenDict.tokens[10],
                                "}", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 11 (
                        else if (pos < line.Length && line.Substring(pos, 1) == "(")
                        {
                            tabs.Add(new ScanToken(11, tokenDict.tokens[11],
                                "(", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 12 )
                        else if (pos < line.Length && line.Substring(pos, 1) == ")")
                        {
                            tabs.Add(new ScanToken(12, tokenDict.tokens[12],
                                ")", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 13 <
                        else if (pos < line.Length && line.Substring(pos, 1) == "<"
                            && line.Substring(pos, 2) != "<=")
                        {
                            tabs.Add(new ScanToken(13, tokenDict.tokens[13],
                                "<", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 14 <=
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "<=")
                        {
                            tabs.Add(new ScanToken(14, tokenDict.tokens[14],
                                "<=", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 15 >
                        else if (pos < line.Length && line.Substring(pos, 1) == ">"
                            && line.Substring(pos, 2) != ">=")
                        {
                            tabs.Add(new ScanToken(15, tokenDict.tokens[15],
                                ">", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 16 >=
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == ">=")
                        {
                            tabs.Add(new ScanToken(16, tokenDict.tokens[16],
                                ">=", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 18 =
                        else if (pos < line.Length && line.Substring(pos, 1) == "="
                            && (pos + 2 < line.Length && line.Substring(pos, 2) != "==")
                            || pos + 2 >= line.Length)
                        {
                            tabs.Add(new ScanToken(18, tokenDict.tokens[18],
                                "=", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 19 ==
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "==")
                        {
                            tabs.Add(new ScanToken(19, tokenDict.tokens[19],
                                "==", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 20 !=
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "!=")
                        {
                            tabs.Add(new ScanToken(20, tokenDict.tokens[20],
                                "!=", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 1 id
                        else if (line[pos] == '$')
                        {
                            int endPos = pos + 1;
                            while (endPos < line.Length &&
                                  (char.IsLetterOrDigit(line[endPos]) || line[endPos] == '_'))
                            {
                                endPos++;
                            }

                            string token = line.Substring(pos, endPos - pos);
                            tabs.Add(new ScanToken(1, tokenDict.tokens[1], token, lineNumber, pos, endPos));
                            pos = endPos;
                            found = true;
                        }

                        // code -1 ERROR
                        if (!found)
                        {
                            int errorEnd = pos + 1;
                            while (errorEnd < line.Length && !char.IsWhiteSpace(line[errorEnd])
                                && line[errorEnd] != '+' && line[errorEnd] != '-'
                                && line[errorEnd] != ';')
                            {
                                errorEnd++;
                            }

                            string errorToken = line.Substring(pos, errorEnd - pos);
                            tabs.Add(new ScanToken(-1, tokenDict.tokens[-1],
                                errorToken, lineNumber, pos, errorEnd));
                            pos = errorEnd;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}");
                return new List<ScanToken>();
            }

            return tabs;
        }
    }

    //public class Scanner
    //{
    //    private string input;
    //    private int pos, currentLine, currentColumn;
    //    private List<ScanTokin> tokens;

    //    private readonly Dictionary<string, int> typeToCode = new Dictionary<string, int>
    //    {
    //        { "IDENTIFIER", 1 }, { "IF", 2 }, { "ELSE", 3 }, { "LBRACE", 9 }, { "RBRACE", 10 },
    //        { "ASSIGN", 11 }, { "EQUAL", 12 }, { "LPAREN", 14 }, { "RPAREN", 15 },
    //        { "SEMICOLON", 22 }, { "GREATER", 23 }, { "LESS", 24 },
    //        { "INCREMENT", 25 }, { "DECREMENT", 26 }
    //    };

    //    public Scanner() => tokens = new List<ScanTokin>();

    //    public List<ScanTokin> Analyze(string source)
    //    {
    //        input = source; pos = 0; currentLine = 1; currentColumn = 1;
    //        tokens.Clear();
    //        while (pos < input.Length)
    //        {
    //            char c = input[pos];
    //            if (char.IsWhiteSpace(c))
    //            {
    //                if (c == '\n') { currentLine++; currentColumn = 1; }
    //                else currentColumn++;
    //                pos++; continue;
    //            }
    //            if (TryParseLexeme()) continue;

    //            // Собираем мусор до ближайшего разделителя
    //            int errLine = currentLine;
    //            int errCol = currentColumn;
    //            StringBuilder garbage = new StringBuilder();
    //            while (pos < input.Length && !char.IsWhiteSpace(input[pos]) && !IsPunctuation(input[pos]))
    //            {
    //                garbage.Append(input[pos]);
    //                Advance();
    //            }
    //            RecordError(garbage.ToString(), errLine, errCol);
    //        }
    //        return tokens;
    //    }

    //    private bool IsPunctuation(char c) => "=(){};><$".Contains(c);

    //    private bool TryParseLexeme()
    //    {
    //        int sL = currentLine, sC = currentColumn;
    //        if (pos + 1 < input.Length)
    //        {
    //            string two = input.Substring(pos, 2);
    //            string t = (two == "==") ? "EQUAL" : (two == "++") ? "INCREMENT" : (two == "--") ? "DECREMENT" : null;
    //            if (t != null) { AddToken(t, two, sL, sC); Advance(); Advance(); return true; }
    //        }

    //        char f = input[pos];
    //        if (f == '$')
    //        {
    //            int start = pos;
    //            Advance();
    //            while (pos < input.Length && char.IsLetterOrDigit(input[pos])) Advance();
    //            string raw = input.Substring(start, pos - start);
    //            if (raw.Length > 1 && char.IsLetter(raw[1])) AddToken("IDENTIFIER", raw, sL, sC);
    //            else RecordError(raw, sL, sC);
    //            return true;
    //        }

    //        if (char.IsLetter(f))
    //        {
    //            int start = pos;
    //            while (pos < input.Length && char.IsLetterOrDigit(input[pos])) Advance();
    //            string w = input.Substring(start, pos - start);
    //            if (w == "if" || w == "else") { AddToken(w.ToUpper(), w, sL, sC); return true; }
    //            RecordError(w, sL, sC);
    //            return true;
    //        }

    //        string st = f == '=' ? "ASSIGN" : f == '(' ? "LPAREN" : f == ')' ? "RPAREN" :
    //                    f == '{' ? "LBRACE" : f == '}' ? "RBRACE" : f == ';' ? "SEMICOLON" :
    //                    f == '>' ? "GREATER" : f == '<' ? "LESS" : null;
    //        if (st != null) { AddToken(st, f.ToString(), sL, sC); Advance(); return true; }
    //        return false;
    //    }

    //    private void Advance() { pos++; currentColumn++; }
    //    private void AddToken(string t, string l, int ln, int cl) =>
    //        tokens.Add(new ScanTokin { Usl_code = typeToCode.ContainsKey(t) ? typeToCode[t] : 0, Type = t, Lecsema = l, Place = $"({ln},{cl})" });
    //    private void RecordError(string lex, int ln, int cl) =>
    //        tokens.Add(new ScanTokin { Usl_code = 0, Type = "ERROR", Lecsema = lex, Place = $"({ln},{cl})" });
    //}
}