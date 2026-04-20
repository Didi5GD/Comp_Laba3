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
            SkipErrors();
        }

        public List<SyntaxError> GetErrors() => _errors;

        private void Advance()
        {
            _position++;
            SkipErrors();
        }

        private void SkipErrors()
        {
            while (_position < _tokens.Count && _tokens[_position].Type == ERROR)
                _position++;

            _current = _position < _tokens.Count ? _tokens[_position] : null;
        }

        private bool IsEnd() => _current == null;

        private string Val() => _current?.Lecsema ?? "EOF";
        private string Pos() => _current?.Place ?? "(?, ?)";

        private bool Match(string t)
        {
            if (!IsEnd() && _current.Type == t)
            {
                Advance();
                return true;
            }
            return false;
        }

        private void Error(string desc)
        {
            _errors.Add(new SyntaxError
            {
                Fragment = Val(),
                Location = Pos(),
                Description = desc
            });
        }

        public void ParseStart()
        {
            if (!Match(IF))
                Error("Отсутствует 'if'");

            if (!Match(LPAREN))
                Error("Ожидается '('");

            ParseExpr();

            if (!Match(RPAREN))
                Error("Ожидается ')'");

            ParseBody();

            if (Match(ELSE))
                ParseBody();

            if (!Match(SEMICOLON))
                Error("Ожидается ';' в конце конструкции");
        }

        private void ParseBody()
        {
            if (Match(LBRACE))
            {
                ParseStatementList();

                if (!Match(RBRACE))
                    Error("Ожидается '}'");
            }
            else
            {
                Error("Ожидается '{'");
                ParseStatement();
                if (!IsEnd() && _current.Type == RBRACE)
                {
                    Advance();
                }
            }
        }


        private void ParseStatementList()
        {
            while (!IsEnd())
            {

                if (_current.Type == RBRACE)
                    return;

                ParseStatement();
                if (_current != null && _current.Type == RBRACE)
                    return;
            }
        }

        private void ParseStatement()
        {
            if (IsEnd()) return;

            if (_current.Type == IDENTIFIER)
            {
                Advance();
            }
            else
            {
                Error("Ожидается переменная");
                Advance();
                return;
            }

            if (Match(ASSIGN))
            {
                if (!Match(IDENTIFIER))
                    Error("Ожидается значение");
            }
            else if (Match(INCREMENT) || Match(DECREMENT))
            {
            }
            else
            {
                Error("Ожидается оператор (=, ++, --)");
            }
            if (!Match(SEMICOLON))
            {
                Error("Ожидается ';'");

                if (_current != null && _current.Type == RBRACE)
                    return;


                while (!IsEnd() &&
                       _current.Type != SEMICOLON &&
                       _current.Type != RBRACE)
                {
                    Advance();
                }

                Match(SEMICOLON);
            }
        }

        private void ParseExpr()
        {
            ParseValue();

            if (!IsEnd() && IsRel())
            {
                Advance();
                ParseValue();
            }
        }

        private bool IsRel()
        {
            string[] ops = { LESS, LESSEQ, GREATER, GREATEREQ, EQUAL, NEQUAL };
            return ops.Contains(_current?.Type);
        }

        private void ParseValue()
        {
            if (!Match(IDENTIFIER))
                Error("Ожидается переменная");
        }
    }
}