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

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
                return '\0';
            else return _text[index];
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

            SyntaxToken makeToken(SyntaxKind kind, string text = "", int offset = 1)
            {
                if (kind == SyntaxKind.BadToken)
                {
                    _diagnostics.Add($"ERROR::Lexer: Unrecognized token '{Current}'");
                    return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
                }
                else return new SyntaxToken(kind, _position += offset, text, null);
            }

            return Current switch
            {
                '+' => makeToken(SyntaxKind.PlusToken, "+"),
                '-' => makeToken(SyntaxKind.PlusToken, "-"),
                '*' => makeToken(SyntaxKind.PlusToken, "*"),
                '/' => makeToken(SyntaxKind.PlusToken, "/"),
                '(' => makeToken(SyntaxKind.PlusToken, "("),
                ')' => makeToken(SyntaxKind.PlusToken, ")"),
                '!' => makeToken(SyntaxKind.BangToken, "!"),
                '&' => Lookahead == '&' ? makeToken(SyntaxKind.AmpersandAmpersandToken, "&&", 2) : makeToken(SyntaxKind.BadToken),
                '|' => Lookahead == '|' ? makeToken(SyntaxKind.PipePipeToken, "||", 2) : makeToken(SyntaxKind.BadToken),
                _   => makeToken(SyntaxKind.BadToken)
            };
        }
    }
}