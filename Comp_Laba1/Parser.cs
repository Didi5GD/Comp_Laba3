using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Comp_Laba1
{
    public class SyntaxError
    {
        public string Fragment { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public SyntaxError(string frag, string pos, string message)
        {
            Fragment = frag;
            Location = pos;
            Description = message;
        }
    }

    public class Parser
    {
        private List<ScanToken> _tokens;
        private int _currentPos;
        private ScanToken _currentToken;
        private List<SyntaxError> _errors;
        private int _errorCounter;

        private bool endFlag = false;

        public Parser(List<ScanToken> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<SyntaxError>();
            _currentToken = _tokens.Count > 0 ? _tokens[0] : null;
        }

        
        public List<SyntaxError> Parse()
        {
            try
            {
                Program();
            }
            catch (Exception ex)
            {
                AddError($"Критическая ошибка: {ex.Message}", "", "");
            }

            return _errors;
        }

        private void NextToken()
        {
            _currentPos++;
            if (_currentPos < _tokens.Count)
                _currentToken = _tokens[_currentPos];
            else
                _currentToken = null;
        }

        private void AddError(string frag, string pos, string message)
        {
            _errors.Add(new SyntaxError(frag, pos, message));
        }

        private bool IsValidToken()
        {
            return _currentPos < _tokens.Count && _currentToken != null;
        }

        private bool SkipToSynchronizingToken(int[] codes)
        {
            int pos = _currentPos;
            int i = 0;
            while (IsValidToken() && i < 4)
            {
                if (codes.Contains(_currentToken.Usl_code))
                {
                    return true;
                }
                NextToken();
                i++;
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
            return false;
        }

        private void ExpectToken(int[] codes, int[] syncro, string message)
        {
            ExpectEOF(message);

            if (endFlag) return;

            if (codes.Contains(_currentToken.Usl_code))
            {
                NextToken();
                _errorCounter = 0;
            }
            else
            {
                if (_errorCounter > 5) { endFlag = true; return; }
                AddError($"После {_tokens[_currentPos - 1].Lecsema}", $"{_currentPos}", message);
                SkipToSynchronizingToken(syncro);
                _errorCounter++;
            }
        }

        private void ExpectEOF(string message)
        {
            if (!IsValidToken())
            {
                AddError($"Перед {(_tokens[Math.Max(_currentPos - 1, 0)]).Lecsema}", $"{_currentPos}", message);
                endFlag = true;
                return;
            }
        }

        private void Program()
        {
            if (!IsValidToken()) return;

            DoParse();

            if (endFlag) return;
            if (IsValidToken())
            {
                AddError($"Ошибочный токен в конце строки", $"{_currentPos}", "");
            }
        }

        private void DoParse()
        {
            if (!IsValidToken()) return;

            if (_currentToken.Usl_code != 2)
            {
                AddError($"Ошибочный токен в конце строки", $"{_currentPos}", "");
                int[] collection = new int[] { 2, 11, 1, 13, 14, 15, 16, 19, 20,1};
                SkipToSynchronizingToken(collection);
            }

            int[] codes = new int[] { 2 };
            int[] syncro = new int[] { 2, 11, 1, 13, 14, 15, 16, 19, 20, 1 };
            ExpectToken(codes, syncro, "Ожидается ключевое слово if");

            if (endFlag) return;
            codes = new int[] { 11 };
            syncro = new int[] { 11, 1, 13, 14, 15, 16, 19, 20, 1, 12};
            ExpectToken(codes, syncro, "Ожидается (");

            if (endFlag) return;
            codes = new int[] { 1 };
            syncro = new int[] { 1, 13, 14, 15, 16, 19, 20, 1, 12, 9 };
            ExpectToken(codes, syncro, "Ожидается идентификатор");

            if (endFlag) return;
            codes = new int[] { 13, 14, 15, 16, 19, 20 };
            syncro = new int[] { 13, 14, 15, 16, 19, 20, 1, 12, 9, 1 };
            ExpectToken(codes, syncro, "Ожидается оператор сравнения");

            if (endFlag) return;
            codes = new int[] { 1 };
            syncro = new int[] { 1, 12, 9, 1, 18 };
            ExpectToken(codes, syncro, "Ожидается идентификатор");

            if (endFlag) return;
            codes = new int[] { 12 };
            syncro = new int[] { 12, 9, 1, 18, 1 };
            ExpectToken(codes, syncro, "Ожидается )");

            if (endFlag) return;
            codes = new int[] { 9 };
            syncro = new int[] {9, 1, 18, 1 , 17};
            ExpectToken(codes, syncro, "Ожидается {");


            if (endFlag) return;
            codes = new int[] {1 };
            syncro = new int[] { 1, 18, 1, 17, 10 };
            ExpectToken(codes, syncro, "Ожидается идентификатор");

            if (endFlag) return;
            codes = new int[] { 18 };
            syncro = new int[] { 18, 1, 17, 10, 3 };
            ExpectToken(codes, syncro, "Ожидается  =");

            if (endFlag) return;
            codes = new int[] { 1 };
            syncro = new int[] { 1, 17, 10, 3, 9 };
            ExpectToken(codes, syncro, "Ожидается идентификатор");

            if (endFlag) return;
            codes = new int[] { 17 };
            syncro = new int[] { 17, 10,3, 9, 1 };
            ExpectToken(codes, syncro, "Ожидается ;");

            if (endFlag) return;
            codes = new int[] { 10 };
            syncro = new int[] { 10, 3, 9, 1, 18 };
            ExpectToken(codes, syncro, "Ожидается }");

            if (endFlag) return;
            codes = new int[] { 3 };
            syncro = new int[] {  3, 9, 1, 18, 1 };
            ExpectToken(codes, syncro, "Ожидается else");

            if (endFlag) return;
            codes = new int[] { 9 };
            syncro = new int[] {9,  1, 18, 1, 17 };
            ExpectToken(codes, syncro, "Ожидается {");

            if (endFlag) return;
            codes = new int[] { 1 };
            syncro = new int[] { 1, 18, 1, 17, 10 };
            ExpectToken(codes, syncro, "Ожидается идентификатор");

            if (endFlag) return;
            codes = new int[] { 18 };
            syncro = new int[] { 18, 1, 17, 10, 17 };
            ExpectToken(codes, syncro, "Ожидается  =");

            if (endFlag) return;
            codes = new int[] { 1 };
            syncro = new int[] { 1, 17, 10, 17 };
            ExpectToken(codes, syncro, "Ожидается идентификатор");

            if (endFlag) return;
            codes = new int[] { 17 };
            syncro = new int[] { 17, 10, 17 };
            ExpectToken(codes, syncro, "Ожидается ;");

            if (endFlag) return;
            codes = new int[] { 10 };
            syncro = new int[] { 10, 17 };
            ExpectToken(codes, syncro, "Ожидается }");

            if (endFlag) return;
            codes = new int[] { 17 };
            syncro = new int[] { 17 };
            ExpectToken(codes, syncro, "Ожидается ;");


        }
    }

    //public class Parser
    //{
    //    private List<ScanTokin> _tokens;
    //    private int _position;
    //    private ScanTokin _current;
    //    private List<SyntaxError> _errors = new List<SyntaxError>();

    //    public Parser(List<ScanTokin> tokens) { _tokens = tokens; _position = 0; UpdateCurrent(); }

    //    private void UpdateCurrent() => _current = _position < _tokens.Count ? _tokens[_position] : null;

    //    private void Advance()
    //    {
    //        _position++;
    //        UpdateCurrent();
    //        // Ключевой момент: если встретили лексическую ошибку в процессе парсинга, 
    //        // просто пропускаем её как "невидимый мусор" для синтаксиса
    //        while (_current != null && _current.Type == "ERROR")
    //        {
    //            _position++;
    //            UpdateCurrent();
    //        }
    //    }

    //    private bool Match(string t) { if (_current?.Type == t) { Advance(); return true; } return false; }

    //    private void Error(string desc)
    //    {
    //        if (_errors.Count > 0 && _errors.Last().Location == (_current?.Place ?? "end")) return;
    //        _errors.Add(new SyntaxError { Fragment = _current?.Lecsema ?? "EOF", Location = _current?.Place ?? "end", Description = desc });
    //    }

    //    public List<SyntaxError> GetErrors() => _errors;

    //    public void ParseStart()
    //    {
    //        // Обработка случая, когда код начинается с мусора перед 'if'
    //        if (_current != null && _current.Type == "ERROR") Advance();

    //        if (!Match("IF")) Error("Отсутствует 'if'");
    //        if (!Match("LPAREN")) Error("Ожидается '('");
    //        ParseExpr();
    //        if (!Match("RPAREN")) Error("Ожидается ')'");

    //        ParseBody();

    //        if (!Match("ELSE")) Error("Отсутствует 'else'");

    //        ParseBody();

    //        if (!Match("SEMICOLON")) Error("Ожидается ';' в конце конструкции");
    //    }

    //    private void ParseBody()
    //    {
    //        if (Match("LBRACE"))
    //        {
    //            while (_current != null && _current.Type != "RBRACE" && _current.Type != "ELSE" && _current.Type != "SEMICOLON")
    //            {
    //                ParseStatement();
    //            }
    //            if (!Match("RBRACE")) Error("Ожидается '}'");
    //        }
    //        else
    //        {
    //            // Если забыли '{', парсим одну инструкцию, но ругаемся
    //            if (_current != null && _current.Type == "IDENTIFIER")
    //            {
    //                Error("Ожидается '{'");
    //                ParseStatement();
    //            }
    //            else if (_current?.Type != "ELSE" && _current?.Type != "SEMICOLON")
    //            {
    //                Error("Ожидается '{'");
    //            }
    //        }
    //    }

    //    private void ParseStatement()
    //    {
    //        if (_current == null || _current.Type == "RBRACE" || _current.Type == "ELSE" || _current.Type == "SEMICOLON") return;

    //        if (Match("IDENTIFIER"))
    //        {
    //            if (Match("ASSIGN"))
    //            {
    //                ParseOperand();
    //            }
    //            else if (!Match("INCREMENT") && !Match("DECREMENT"))
    //            {
    //                Error("Ожидается оператор");
    //            }
    //            if (!Match("SEMICOLON")) Error("Ожидается ';'");
    //        }
    //        else
    //        {
    //            Error("Ожидается инструкция");
    //            Advance();
    //        }
    //    }

    //    private void ParseExpr()
    //    {
    //        ParseOperand();
    //        string[] ops = { "LESS", "GREATER", "EQUAL" };
    //        if (_current != null && ops.Contains(_current.Type))
    //        {
    //            Advance();
    //            ParseOperand();
    //        }
    //    }

    //    private void ParseOperand()
    //    {
    //        if (!Match("IDENTIFIER")) Error("Ожидается переменная");
    //    }
    //}
}