using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp_Laba1
{
    public class Lexer
    {
    

            public List<ScanTokin> Tokenize(string code)
            {
                var tokens = new List<ScanTokin>();
                int line = 1;
                int column = 1;
                int tokenCode = 1;
                int i = 0;

                while (i < code.Length)
                {
                    char c = code[i];

                 
                    if (char.IsWhiteSpace(c))
                    {
                        if (c == '\n')
                        {
                            line++;
                            column = 1;
                        }
                        else
                        {
                            column++;
                        }
                        i++;
                        continue;
                    }

                    string place = $"({line}, {column})";

                    string[] twoCharOps = { ">=", "<=", "==", "!=", "&&", "||", "++", "--" };
                    bool matched = false;

                    foreach (var op in twoCharOps)
                    {
                        if (i + op.Length <= code.Length && code.Substring(i, op.Length) == op)
                        {
                            string type = GetOperatorType(op);
                            tokens.Add(new ScanTokin
                            {
                                Usl_code = tokenCode++,
                                Type = type,
                                Lecsema = op,
                                Place = place
                            });
                            i += op.Length;
                            column += op.Length;
                            matched = true;
                            break;
                        }
                    }
                    if (matched) continue;

                    string[] oneCharOps = { ">", "<", "=", ";", "(", ")", "{", "}", "$", "+", "-", "*", "/", "%", "!" };
                    foreach (var op in oneCharOps)
                    {
                        if (c.ToString() == op)
                        {
                            string type = GetOperatorType(op);
                            tokens.Add(new ScanTokin
                            {
                                Usl_code = tokenCode++,
                                Type = type,
                                Lecsema = op,
                                Place = place
                            });
                            i++;
                            column++;
                            matched = true;
                            break;
                        }
                    }
                    if (matched) continue;

                    if (i + 1 < code.Length && code.Substring(i, 2) == "if" &&
                        (i + 2 >= code.Length || !char.IsLetterOrDigit(code[i + 2])))
                    {
                        tokens.Add(new ScanTokin
                        {
                            Usl_code = tokenCode++,
                            Type = "IF",
                            Lecsema = "if",
                            Place = place
                        });
                        i += 2;
                        column += 2;
                        continue;
                    }

                    if (i + 3 < code.Length && code.Substring(i, 4) == "else" &&
                        (i + 4 >= code.Length || !char.IsLetterOrDigit(code[i + 4])))
                    {
                        tokens.Add(new ScanTokin
                        {
                            Usl_code = tokenCode++,
                            Type = "ELSE",
                            Lecsema = "else",
                            Place = place
                        });
                        i += 4;
                        column += 4;
                        continue;
                    }

                    if (i + 5 < code.Length && code.Substring(i, 6) == "elseif" &&
                        (i + 6 >= code.Length || !char.IsLetterOrDigit(code[i + 6])))
                    {
                        tokens.Add(new ScanTokin
                        {
                            Usl_code = tokenCode++,
                            Type = "ELSEIF",
                            Lecsema = "elseif",
                            Place = place
                        });
                        i += 6;
                        column += 6;
                        continue;
                    }


                    if (c == '$' || char.IsLetter(c) || c == '_')
                    {
                        string ident = "";
                        int startColumn = column;

                        while (i < code.Length && (code[i] == '$' || char.IsLetterOrDigit(code[i]) || code[i] == '_'))
                        {
                            ident += code[i];
                            i++;
                            column++;
                        }

                        if (ident.StartsWith("$") && ident.Length > 1)
                        {

                            tokens.Add(new ScanTokin
                            {
                                Usl_code = tokenCode++,
                                Type = "IDENTIFIER",
                                Lecsema = ident,
                                Place = $"({line}, {startColumn})"
                            });
                        }
                        else
                        {

                            tokens.Add(new ScanTokin
                            {
                                Usl_code = tokenCode++,
                                Type = "ERROR",
                                Lecsema = ident,
                                Place = $"({line}, {startColumn})"
                            });
                        }
                        continue;
                    }


                    if (char.IsDigit(c))
                    {
                        string number = "";
                        int startColumn = column;

                        while (i < code.Length && char.IsDigit(code[i]))
                        {
                            number += code[i];
                            i++;
                            column++;
                        }

                        tokens.Add(new ScanTokin
                        {
                            Usl_code = tokenCode++,
                            Type = "NUMBER",
                            Lecsema = number,
                            Place = $"({line}, {startColumn})"
                        });
                        continue;
                    }
                    string errorChar = c.ToString();
                    int errorStartColumn = column;

                    while (i < code.Length && !IsValidChar(code[i]))
                    {
                        errorChar += code[i];
                        i++;
                        column++;
                    }

                    tokens.Add(new ScanTokin
                    {
                        Usl_code = tokenCode++,
                        Type = "ERROR",
                        Lecsema = errorChar,
                        Place = $"({line}, {errorStartColumn})"
                    });
                }

                return tokens;
            }

            private bool IsValidChar(char c)
            {
                return c == '$' ||
                       c == '(' || c == ')' || c == '{' || c == '}' || c == ';' ||
                       c == '=' || c == '>' || c == '<' || c == '!' ||
                       c == '+' || c == '-' || c == '*' || c == '/' || c == '%' ||
                       c == '&' || c == '|' ||
                       char.IsLetter(c) || char.IsDigit(c) || c == '_' ||
                       char.IsWhiteSpace(c);
            }

            private string GetOperatorType(string op)
            {
                switch (op)
                {
                    case "++": return "INCREMENT";
                    case "--": return "DECREMENT";
                    case "+": return "PLUS";
                    case "-": return "MINUS";
                    case "*": return "MULTIPLY";
                    case "/": return "DIV";
                    case "%": return "MOD";
                    case "**": return "POWER";
                    case "=": return "ASSIGN";
                    case "==": return "EQUAL";
                    case "!=": return "NEQUAL";
                    case "<": return "LESS";
                    case "<=": return "LESSEQ";
                    case ">": return "GREATER";
                    case ">=": return "GREATEREQ";
                    case "&&": return "AND";
                    case "||": return "OR";
                    case "(": return "LPAREN";
                    case ")": return "RPAREN";
                    case "{": return "LBRACE";
                    case "}": return "RBRACE";
                    case ";": return "SEMICOLON";
                    case "$": return "DOLLAR";
                    default: return "OPERATOR";
                }
            }
        }
    }
