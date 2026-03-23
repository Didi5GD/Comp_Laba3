using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comp_Laba1
{
    public class SyntaxError
    {
        public string Fragment { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }

    public class Parser
    {
        private List<ScanTokin> _tokens;
        private int _position;
        private ScanTokin _current;
        private List<SyntaxError> _errors;
        private int _errorCount;

        private enum RecoveryState
        {
            Normal,
            SkipToSemicolon,
            SkipToCloseBrace,
            SkipToRParen,
            SkipToNextKeyword
        }

        private RecoveryState _recoveryState;

        private const string IDENTIFIER = "IDENTIFIER";
        private const string IF = "IF";
        private const string ELSE = "ELSE";
        private const string LBRACE = "LBRACE";
        private const string RBRACE = "RBRACE";
        private const string LPAREN = "LPAREN";
        private const string RPAREN = "RPAREN";
        private const string ASSIGN = "ASSIGN";
        private const string SEMICOLON = "SEMICOLON";
        private const string INCREMENT = "INCREMENT";
        private const string DECREMENT = "DECREMENT";
        private const string LESS = "LESS";
        private const string LESSEQ = "LESSEQ";
        private const string GREATER = "GREATER";
        private const string GREATEREQ = "GREATEREQ";
        private const string EQUAL = "EQUAL";
        private const string NEQUAL = "NEQUAL";
        private const string OR = "OR";
        private const string AND = "AND";
        private const string ERROR = "ERROR";

        public Parser(List<ScanTokin> tokens)
        {
            _tokens = tokens;
            _position = 0;
            _errors = new List<SyntaxError>();
            _errorCount = 0;
            _recoveryState = RecoveryState.Normal;

            if (_tokens.Count > 0)
                _current = _tokens[0];
            else
                _current = null;
        }

        public List<SyntaxError> GetErrors() => _errors;
        public int ErrorCount => _errorCount;

        private void Advance()
        {
            _position++;
            if (_position < _tokens.Count)
                _current = _tokens[_position];
            else
                _current = null;
        }

        private bool IsEnd() => _current == null;

        private string GetCurrentPlace()
        {
            return _current != null ? _current.Place : "(?, ?)";
        }

        private string GetCurrentValue()
        {
            return _current != null ? _current.Lecsema : "EOF";
        }

        private string GetCurrentType()
        {
            return _current != null ? _current.Type : "EOF";
        }

        private bool Match(string expectedType)
        {
            if (IsEnd()) return false;

            if (_current.Type == expectedType)
            {
                Advance();
                return true;
            }

            return false;
        }

        private void AddError(string fragment, string description)
        {
            string location = GetCurrentPlace();

            bool duplicate = _errors.Any(e => e.Location == location && e.Description == description);

            if (!duplicate)
            {
                _errors.Add(new SyntaxError
                {
                    Fragment = fragment,
                    Location = location,
                    Description = description
                });
                _errorCount++;

                System.Diagnostics.Debug.WriteLine($"ОШИБКА: {fragment} | {location} | {description}");
            }
        }

        private void SkipErrorTokens()
        {
            while (!IsEnd() && _current.Type == ERROR)
            {
                AddError($"Недопустимая лексема '{_current.Lecsema}'", "Недопустимый символ в коде");
                Advance();
            }
        }

        private void Recover(RecoveryState targetState)
        {
            _recoveryState = targetState;
            int startPos = _position;

            while (!IsEnd())
            {
                switch (targetState)
                {
                    case RecoveryState.SkipToSemicolon:
                        if (_current.Type == SEMICOLON)
                        {
                            Advance();
                            _recoveryState = RecoveryState.Normal;
                            return;
                        }
                        break;

                    case RecoveryState.SkipToCloseBrace:
                        if (_current.Type == RBRACE)
                        {
                            _recoveryState = RecoveryState.Normal;
                            return;
                        }
                        break;

                    case RecoveryState.SkipToRParen:
                        if (_current.Type == RPAREN)
                        {
                            Advance();
                            _recoveryState = RecoveryState.Normal;
                            return;
                        }
                        break;

                    case RecoveryState.SkipToNextKeyword:
                        if (_current.Type == IF || _current.Type == ELSE || _current.Type == RBRACE)
                        {
                            _recoveryState = RecoveryState.Normal;
                            return;
                        }
                        break;
                }
                Advance();
            }

            _recoveryState = RecoveryState.Normal;
        }


        public void ParseStart()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            System.Diagnostics.Debug.WriteLine("=== НАЧАЛО РАЗБОРА ===");

            SkipErrorTokens();

            if (IsEnd())
            {
                return;
            }

            if (!Match(IF))
            {
                AddError(GetCurrentValue(), "Ожидается 'if' в начале конструкции");
                Recover(RecoveryState.SkipToNextKeyword);
                return;
            }

            ParseCondition();
            ParseBlock();
            ParseElsePart();
            ParseEndSemi();

            SkipErrorTokens();

            if (!IsEnd())
            {
                if (_current.Type == SEMICOLON)
                {
                    AddError(GetCurrentValue(), "Лишняя точка с запятой после завершения конструкции");
                    Advance();
                }
                else if (_current.Type == RBRACE)
                {
                    AddError(GetCurrentValue(), "Лишняя закрывающая скобка");
                    Advance();
                }
                else if (_current.Type == ERROR)
                {
                    AddError($"Недопустимая лексема '{_current.Lecsema}'", "Недопустимый символ в коде");
                    Advance();
                }
                else
                {
                    AddError(GetCurrentValue(), "Лишние символы после завершения конструкции");
                    while (!IsEnd())
                    {
                        Advance();
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("=== КОНЕЦ РАЗБОРА ===");
        }

        private void ParseCondition()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!Match(LPAREN))
            {
                AddError(GetCurrentValue(), "Ожидается '(' после if");
                Recover(RecoveryState.SkipToRParen);
                return;
            }

            ParseExpr();

            if (!Match(RPAREN))
            {
                AddError(GetCurrentValue(), "Ожидается ')'");
                Recover(RecoveryState.SkipToSemicolon);
            }
        }

        private void ParseExpr()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            ParseLogicTerm();

            while (!IsEnd() && _current.Type == OR)
            {
                Advance();
                ParseLogicTerm();
            }
        }

        private void ParseLogicTerm()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            ParseCompare();

            while (!IsEnd() && _current.Type == AND)
            {
                Advance();
                ParseCompare();
            }
        }

        private void ParseCompare()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!IsEnd() && _current.Type == LPAREN)
            {
                Advance();
                ParseExpr();
                if (!Match(RPAREN))
                {
                    AddError(GetCurrentValue(), "Ожидается ')'");
                    Recover(RecoveryState.SkipToRParen);
                }
                return;
            }

            int savePos = _position;
            ScanTokin saveCurrent = _current;
            int saveErrorCount = _errorCount;

            try
            {
                ParseValue();

                if (!IsEnd() && IsRelOp())
                {
                    ParseRelOp();
                    ParseValue();
                    return;
                }
            }
            catch
            {
                _position = savePos;
                _current = saveCurrent;
                _errorCount = saveErrorCount;
                while (_errors.Count > saveErrorCount)
                {
                    _errors.RemoveAt(_errors.Count - 1);
                }
            }

            AddError(GetCurrentValue(), "Ожидается оператор сравнения (>, <, >=, <=, ==, !=)");
            Recover(RecoveryState.SkipToSemicolon);
        }

        private bool IsRelOp()
        {
            if (IsEnd()) return false;
            string[] ops = { LESS, LESSEQ, GREATER, GREATEREQ, EQUAL, NEQUAL };
            return ops.Contains(_current.Type);
        }

        private void ParseRelOp()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            string[] ops = { LESS, LESSEQ, GREATER, GREATEREQ, EQUAL, NEQUAL };
            if (ops.Contains(_current?.Type ?? ""))
            {
                Advance();
            }
            else
            {
                AddError(GetCurrentValue(), "Ожидается оператор сравнения");
                Recover(RecoveryState.SkipToSemicolon);
            }
        }

        private void ParseValue()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!Match(IDENTIFIER))
            {
                AddError(GetCurrentValue(), "Ожидается переменная (например, $a)");
                Recover(RecoveryState.SkipToSemicolon);
            }
        }

        

        private void ParseStatementList()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            while (!IsEnd() && IsStartOfStatement())
            {
                ParseStatement();
            }
        }

        private bool IsStartOfStatement()
        {
            return !IsEnd() && _current.Type == IDENTIFIER;
        }

        private void ParseStatement()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            int savePos = _position;
            ScanTokin saveCurrent = _current;
            int saveErrorCount = _errorCount;

            try
            {
                ParseValue();
                ParseOp();
                ParseVar();

                if (!Match(SEMICOLON))
                {
                    string errorPlace = GetCurrentPlace();
                    string errorValue = GetCurrentValue();

                    AddError(errorValue, "Ожидается ';' в конце оператора");

                    while (!IsEnd() && _current.Type != SEMICOLON && _current.Type != RBRACE)
                    {
                        Advance();
                    }

                    if (!IsEnd() && _current.Type == SEMICOLON)
                    {
                        Advance();
                    }
                }
            }
            catch
            {
                _position = savePos;
                _current = saveCurrent;
                _errorCount = saveErrorCount;
                while (_errors.Count > saveErrorCount)
                {
                    _errors.RemoveAt(_errors.Count - 1);
                }

                AddError(GetCurrentValue(), "Некорректный оператор");
                Recover(RecoveryState.SkipToSemicolon);
            }
        }

        private void ParseOp()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (Match(ASSIGN) || Match(INCREMENT) || Match(DECREMENT))
            {
                return;
            }

            AddError(GetCurrentValue(), "Ожидается оператор (=, ++, --)");
            Recover(RecoveryState.SkipToSemicolon);
        }

        private void ParseVar()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!IsEnd() && _current.Type == IDENTIFIER)
            {
                Advance();
            }
        }

        private void ParseElsePart()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!IsEnd() && _current.Type == ELSE)
            {
                Advance();
                ParseElseBlock();
            }
        }

        private void ParseBlock()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!Match(LBRACE))
            {
                AddError(GetCurrentValue(), "Ожидается '{'");
                Recover(RecoveryState.SkipToCloseBrace);
                return;
            }

            ParseStatementList();

            if (!Match(RBRACE))
            {
                AddError(GetCurrentValue(), "Ожидается '}'");
                while (!IsEnd() && _current.Type != RBRACE)
                {
                    Advance();
                }
                if (!IsEnd() && _current.Type == RBRACE)
                {
                    Advance();
                }
            }
        }

        private void ParseElseBlock()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!Match(LBRACE))
            {
                AddError(GetCurrentValue(), "Ожидается '{' после else");
                return;
            }

            ParseStatementList();

            if (!Match(RBRACE))
            {
                AddError(GetCurrentValue(), "Ожидается '}'");
                while (!IsEnd() && _current.Type != RBRACE)
                {
                    Advance();
                }
                if (!IsEnd() && _current.Type == RBRACE)
                {
                    Advance();
                }
            }
        }

        private void ParseEndSemi()
        {
            if (_recoveryState != RecoveryState.Normal) return;

            if (!IsEnd() && _current.Type == SEMICOLON)
            {
                Advance();
            }
        }
    }


}
