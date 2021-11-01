using System.Collections.Generic;

namespace Xlang.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics;

        public IEnumerable<string> Diagnostics => _diagnostics;

        public Lexer(string text)
        {
            _text = text;
            _diagnostics = new List<string>();
        }

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\n';
                else return _text[_position];
            }
        }

        private int Next()
        {
            return _position++;
        }

        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            else if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out int value))
                {
                    _diagnostics.Add($"ERROR::Lexer: Number {_text} is not a valid Int32");
                }

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }
            else if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }
            else if (char.IsLetter(Current))
            {
                var start = _position;

                while (char.IsLetter(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);

                return new SyntaxToken(kind, start, text, null);
            }

            SyntaxToken makeToken(SyntaxKind kind, string text)
            {
                if (kind == SyntaxKind.BadToken)
                    _diagnostics.Add($"ERROR::Lexer: Unrecognized token '{Current}'");
                    
                return new SyntaxToken(kind, Next(), text, null);
            }

            return Current switch
            {
                '+' => makeToken(SyntaxKind.PlusToken, "+"),
                '-' => makeToken(SyntaxKind.PlusToken, "*"),
                '*' => makeToken(SyntaxKind.PlusToken, "*"),
                '/' => makeToken(SyntaxKind.PlusToken, "/"),
                '(' => makeToken(SyntaxKind.PlusToken, "("),
                ')' => makeToken(SyntaxKind.PlusToken, ")"),
                _   => makeToken(SyntaxKind.BadToken, _text.Substring(_position - 1, 1))
            };
        }
    }
}