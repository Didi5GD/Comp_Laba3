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
            while (_position < _tokens.Count && _tokens[_position].Type == "ERROR") _position++;
            _current = _position < _tokens.Count ? _tokens[_position] : null;
        }

        private bool Match(string t) { if (_current?.Type == t) { Advance(); return true; } return false; }

        private void Expect(string type, string message) { if (!Match(type)) Error(message); }

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
                // Синхронизация: пропускаем мусор до начала блока или следующей команды
                while (_current != null && _current.Type != "LBRACE" && _current.Type != "SEMICOLON") Advance();
            }
            ParseBody();
            Expect("SEMICOLON", "Ожидается ';' в конце конструкции");
        }

        private void ParseBody()
        {
            if (Match("LBRACE"))
            {
                while (_current != null && _current.Type != "RBRACE" && _current.Type != "ELSE") ParseStatement();
                Expect("RBRACE", "Ожидается '}'");
            }
            else
            {
                if (_current?.Type != "SEMICOLON") Error("Ожидается '{'");
                ParseStatement();
            }
        }

        private void ParseStatement()
        {
            if (_current == null || _current.Type == "RBRACE" || _current.Type == "ELSE") return;
            if (Match("IDENTIFIER"))
            {
                if (Match("ASSIGN")) Expect("IDENTIFIER", "Ожидается значение");
                else if (!Match("INCREMENT") && !Match("DECREMENT")) Error("Ожидается оператор");
                Expect("SEMICOLON", "Ожидается ';'");
            }
            else { Error("Ожидается инструкция"); Advance(); }
        }

        private void ParseExpr()
        {
            if (!Match("IDENTIFIER")) Error("Ожидается переменная");
            string[] ops = { "LESS", "GREATER", "EQUAL" };
            if (_current != null && ops.Contains(_current.Type)) { Advance(); if (!Match("IDENTIFIER")) Error("Ожидается переменная"); }
        }
    }
}