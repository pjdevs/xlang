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
                int start = _position;

                while (char.IsDigit(Current))
                    Next();

                int length = _position - start;
                string text = _text.Substring(start, length);
                if (!int.TryParse(text, out int value))
                {
                    _diagnostics.Add($"ERROR::Lexer: Number {_text} is not a valid Int32");
                }

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }
            else if (char.IsWhiteSpace(Current))
            {
                int start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                int length = _position - start;
                string text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }


            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, Next(), "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, Next(), "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, Next(), "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, Next(), "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, Next(), "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, Next(), ")", null);
                default:
                    _diagnostics.Add($"ERROR::Lexer: Unrecognized token '{Current}'");
                    return new SyntaxToken(SyntaxKind.BadToken, Next(), _text.Substring(_position - 1, 1), null);
            }
        }
    }
}