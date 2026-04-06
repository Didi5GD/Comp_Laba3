using System;
using System.Collections.Generic;
using System.Linq;

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
        private ScanTokin _previous;

        private List<SyntaxError> _errors;
        private int _errorCount;

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
            SkipErrorTokens();
        }

        public List<SyntaxError> GetErrors() => _errors;
        public int ErrorCount => _errorCount;

        private void Advance()
        {
            _previous = _current;
            _position++;
            SkipErrorTokens();
        }

        private void SkipErrorTokens()
        {
            while (_position < _tokens.Count && _tokens[_position].Type == ERROR)
                _position++;

            _current = _position < _tokens.Count ? _tokens[_position] : null;
        }

        private bool IsEnd() => _current == null;

        private string GetCurrentPlace() => _current?.Place ?? "(?, ?)";
        private string GetCurrentValue() => _current?.Lecsema ?? "EOF";
        private string GetPreviousValue() => _previous?.Lecsema ?? "";

        private bool Match(string type)
        {
            if (!IsEnd() && _current.Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }

        private void AddError(string fragment, string description)
        {
            string location = GetCurrentPlace();
            if (!_errors.Any(e => e.Location == location && e.Description == description))
            {
                _errors.Add(new SyntaxError
                {
                    Fragment = fragment,
                    Location = location,
                    Description = description
                });
                _errorCount++;
            }
        }

        public void ParseStart()
        {
            if (IsEnd()) return;

            // if
            if (!Match(IF))
                AddError(GetCurrentValue(), "Отсутствует 'if'");

            // (
            if (!Match(LPAREN))
                AddError(GetCurrentValue(), "Ожидается '('");

            ParseExpr();

            // )
            if (!Match(RPAREN))
                AddError(GetCurrentValue(), "Ожидается ')'");

            // тело if
            ParseBody();

            // else
            if (!IsEnd() && _current.Type == ELSE)
            {
                Advance();
                ParseBody();
            }

            // ; допустим
            if (!IsEnd() && _current.Type == SEMICOLON)
                Advance();

            if (!IsEnd())
                AddError(GetCurrentValue(), "Неожиданный токен");
        }

        private void ParseBody()
        {
            if (Match(LBRACE))
            {
                ParseStatementList();

                if (!Match(RBRACE))
                    AddError(GetCurrentValue(), "Ожидается '}'");
            }
            else
            {
                AddError(GetPreviousValue(), "Ожидается '{'");
                ParseStatement();
            }
        }

        private void ParseExpr()
        {
            ParseLogicTerm();

            while (!IsEnd() && _current.Type == OR)
            {
                Advance();

                if (_current.Type == OR)
                    AddError("||", "Два оператора подряд");

                ParseLogicTerm();
            }
        }

        private void ParseLogicTerm()
        {
            ParseCompare();

            while (!IsEnd() && _current.Type == AND)
            {
                Advance();

                if (_current.Type == AND)
                    AddError("&&", "Два оператора подряд");

                ParseCompare();
            }
        }

        private void ParseCompare()
        {
            if (!IsEnd() && _current.Type == LPAREN)
            {
                Advance();
                ParseExpr();

                if (!Match(RPAREN))
                    AddError(GetCurrentValue(), "Не закрыта ')'");
                return;
            }

            ParseValue();

            if (!IsEnd() && IsRelOp())
            {
                string op = _current.Lecsema;
                Advance();

                if (IsEnd() || _current.Type != IDENTIFIER)
                {
                    AddError(op, "После оператора сравнения ожидается переменная");
                    return;
                }

                ParseValue();
            }
        }

        private bool IsRelOp()
        {
            string[] ops = { LESS, LESSEQ, GREATER, GREATEREQ, EQUAL, NEQUAL };
            return !IsEnd() && ops.Contains(_current.Type);
        }

        private void ParseValue()
        {
            if (IsEnd())
            {
                AddError("EOF", "Ожидается переменная");
                return;
            }

            if (_current.Type == IDENTIFIER)
            {
                if (!_current.Lecsema.StartsWith("$"))
                    AddError(_current.Lecsema, "Переменная должна начинаться с '$'");

                Advance();
            }
            else
            {
                AddError(GetCurrentValue(), "Ожидается переменная");
                Advance();
            }
        }

        private void ParseStatementList()
        {
            while (!IsEnd() && (_current.Type == IDENTIFIER || _current.Type == SEMICOLON))
            {
                if (_current.Type == SEMICOLON)
                {
                    AddError(";", "Лишний ';'");
                    Advance();
                    continue;
                }

                ParseStatement();
            }
        }

        private void ParseStatement()
        {
            // переменная
            if (!IsEnd() && _current.Type == IDENTIFIER)
            {
                if (!_current.Lecsema.StartsWith("$"))
                    AddError(_current.Lecsema, "Переменная должна начинаться с '$'");

                Advance();
            }
            else
            {
                AddError(GetCurrentValue(), "Ожидается переменная");
                Advance();
                return;
            }

            // оператор
            if (Match(ASSIGN))
            {
                // нужен RHS
                if (!IsEnd() && _current.Type == IDENTIFIER)
                {
                    if (!_current.Lecsema.StartsWith("$"))
                        AddError(_current.Lecsema, "Некорректное имя переменной");

                    Advance();
                }
                else
                {
                    AddError(GetCurrentValue(), "Ожидается значение");
                    Advance();
                }
            }
            else if (Match(INCREMENT) || Match(DECREMENT))
            {
                // OK
            }
            else
            {
                AddError(GetCurrentValue(), "Ожидается оператор (=, ++, --)");
            }

            // ;
            if (!Match(SEMICOLON))
                AddError(GetCurrentValue(), "Ожидается ';'");
        }
    }
}