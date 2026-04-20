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
        private List<SyntaxError> _errors = new List<SyntaxError>();

        public Parser(List<ScanTokin> tokens) { _tokens = tokens; _position = 0; UpdateCurrent(); }

        private void Advance() { _position++; UpdateCurrent(); }

        private void UpdateCurrent()
        {
            _current = _position < _tokens.Count ? _tokens[_position] : null;
        }

        private bool Match(string t) { if (_current?.Type == t) { Advance(); return true; } return false; }

        private void Expect(string type, string message)
        {
            if (Match(type)) return;
            Error(message);
        }

        private void Error(string desc) =>
            _errors.Add(new SyntaxError { Fragment = _current?.Lecsema ?? "EOF", Location = _current?.Place ?? "end", Description = desc });

        public List<SyntaxError> GetErrors() => _errors;

        public void ParseStart()
        {
            Expect("IF", "Отсутствует 'if'");
            Expect("LPAREN", "Ожидается '('");
            ParseExpr();
            Expect("RPAREN", "Ожидается ')'");

            ParseBody();

            if (!Match("ELSE"))
            {
                Error("Отсутствует 'else'");
            }

            ParseBody();

            Expect("SEMICOLON", "Ожидается ';' в конце конструкции");
        }

        private void ParseBody()
        {
            if (Match("LBRACE"))
            {
                while (_current != null && _current.Type != "RBRACE" && _current.Type != "ELSE" && _current.Type != "SEMICOLON")
                {
                    ParseStatement();
                }
                Expect("RBRACE", "Ожидается '}'");
            }
            else
            {
                Error("Ожидается '{'");
                ParseStatement();
                // Проверяем, нет ли закрывающей скобки, если она все же стоит случайно
                if (!Match("RBRACE"))
                {
                    Error("Ожидается '}'");
                }
            }
        }

        private void ParseStatement()
        {
            if (_current == null || _current.Type == "RBRACE" || _current.Type == "ELSE" || _current.Type == "SEMICOLON") return;

            // Если лексер пометил фрагмент как ошибку (например, $ma@@ или max)
            if (_current.Type == "ERROR")
            {
                Error("Ожидается инструкция");
                // Пропускаем все до точки с запятой, чтобы не плодить ошибки на операторах
                while (_current != null && _current.Type != "SEMICOLON" && _current.Type != "RBRACE" && _current.Type != "ELSE")
                {
                    Advance();
                }
                Match("SEMICOLON");
                return;
            }

            if (Match("IDENTIFIER"))
            {
                if (Match("ASSIGN"))
                {
                    if (_current != null && _current.Type == "ERROR")
                    {
                        Error("Ожидается значение");
                        Advance();
                    }
                    else if (!Match("IDENTIFIER"))
                    {
                        Error("Ожидается значение");
                    }
                }
                else if (!Match("INCREMENT") && !Match("DECREMENT"))
                {
                    Error("Ожидается оператор");
                }
                Expect("SEMICOLON", "Ожидается ';'");
            }
            else
            {
                Error("Ожидается инструкция");
                Advance();
            }
        }

        private void ParseExpr()
        {
            if (_current != null && _current.Type == "ERROR")
            {
                Error("Ожидается переменная");
                Advance();
            }
            else if (!Match("IDENTIFIER")) Error("Ожидается переменная");

            string[] ops = { "LESS", "GREATER", "EQUAL" };
            if (_current != null && ops.Contains(_current.Type))
            {
                Advance();
                if (_current != null && _current.Type == "ERROR")
                {
                    Error("Ожидается переменная");
                    Advance();
                }
                else if (!Match("IDENTIFIER")) Error("Ожидается переменная");
            }
        }
    }

}