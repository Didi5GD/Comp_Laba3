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

        private void UpdateCurrent() => _current = _position < _tokens.Count ? _tokens[_position] : null;

        private void Advance()
        {
            _position++;
            UpdateCurrent();
            // Ключевой момент: если встретили лексическую ошибку в процессе парсинга, 
            // просто пропускаем её как "невидимый мусор" для синтаксиса
            while (_current != null && _current.Type == "ERROR")
            {
                _position++;
                UpdateCurrent();
            }
        }

        private bool Match(string t) { if (_current?.Type == t) { Advance(); return true; } return false; }

        private void Error(string desc)
        {
            if (_errors.Count > 0 && _errors.Last().Location == (_current?.Place ?? "end")) return;
            _errors.Add(new SyntaxError { Fragment = _current?.Lecsema ?? "EOF", Location = _current?.Place ?? "end", Description = desc });
        }

        public List<SyntaxError> GetErrors() => _errors;

        public void ParseStart()
        {
            // Обработка случая, когда код начинается с мусора перед 'if'
            if (_current != null && _current.Type == "ERROR") Advance();

            if (!Match("IF")) Error("Отсутствует 'if'");
            if (!Match("LPAREN")) Error("Ожидается '('");
            ParseExpr();
            if (!Match("RPAREN")) Error("Ожидается ')'");

            ParseBody();

            if (!Match("ELSE")) Error("Отсутствует 'else'");

            ParseBody();

            if (!Match("SEMICOLON")) Error("Ожидается ';' в конце конструкции");
        }

        private void ParseBody()
        {
            if (Match("LBRACE"))
            {
                while (_current != null && _current.Type != "RBRACE" && _current.Type != "ELSE" && _current.Type != "SEMICOLON")
                {
                    ParseStatement();
                }
                if (!Match("RBRACE")) Error("Ожидается '}'");
            }
            else
            {
                // Если забыли '{', парсим одну инструкцию, но ругаемся
                if (_current != null && _current.Type == "IDENTIFIER")
                {
                    Error("Ожидается '{'");
                    ParseStatement();
                }
                else if (_current?.Type != "ELSE" && _current?.Type != "SEMICOLON")
                {
                    Error("Ожидается '{'");
                }
            }
        }

        private void ParseStatement()
        {
            if (_current == null || _current.Type == "RBRACE" || _current.Type == "ELSE" || _current.Type == "SEMICOLON") return;

            if (Match("IDENTIFIER"))
            {
                if (Match("ASSIGN"))
                {
                    ParseOperand();
                }
                else if (!Match("INCREMENT") && !Match("DECREMENT"))
                {
                    Error("Ожидается оператор");
                }
                if (!Match("SEMICOLON")) Error("Ожидается ';'");
            }
            else
            {
                Error("Ожидается инструкция");
                Advance();
            }
        }

        private void ParseExpr()
        {
            ParseOperand();
            string[] ops = { "LESS", "GREATER", "EQUAL" };
            if (_current != null && ops.Contains(_current.Type))
            {
                Advance();
                ParseOperand();
            }
        }

        private void ParseOperand()
        {
            if (!Match("IDENTIFIER")) Error("Ожидается переменная");
        }
    }
}