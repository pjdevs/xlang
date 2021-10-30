using System;
using System.Linq;
using System.Collections.Generic;

namespace xc
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");

                string line = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(line))
                    return;

                var parser = new Parser(line);
                var expression = parser.Parse();

                PrettyPrint(expression.Root);

                foreach (var diagnostic in parser.Diagnostics)
                {
                    ErrorPrint(diagnostic);
                }

                // Lexer lexer = new Lexer(line);
                // while (true)
                // {
                //     SyntaxToken token = lexer.NextToken();

                //     if (token.Kind == SyntaxKind.EndOfFileToken)
                //         break;

                //     Console.Write($"{token.Kind}: '{token.Text}'");
                //     if (token.Value != null)
                //         Console.WriteLine($" {token.Value}");
                //     else Console.WriteLine();
                // }
            }
        }

        static void ErrorPrint(string msg)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(msg);

            Console.ForegroundColor = color;
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;

            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│   " ;

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }

            Console.ForegroundColor = color;
        }
    }

    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        EndOfFileToken,
        BadToken,
        NumberExpression,
        BinaryExpression
    }

    class SyntaxToken : SyntaxNode
    {
        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    class Lexer
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

        public SyntaxToken NextToken()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }
            else if (char.IsDigit(Current))
            {
                int start = _position;

                while (char.IsDigit(Current))
                    Next();

                int length = _position - start;
                string text = _text.Substring(start, length);
                if (!int.TryParse(text, out int value))
                {
                    _diagnostics.Add($"ERROR: Number {_text} is not a valid int");
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
            else if (Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, Next(), "+", null);
            else if (Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, Next(), "-", null);
            else if (Current == '*')
                return new SyntaxToken(SyntaxKind.StarToken, Next(), "*", null);
            else if (Current == '/')
                return new SyntaxToken(SyntaxKind.SlashToken, Next(), "/", null);
            else if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, Next(), "(", null);
            else if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, Next(), ")", null);
            else
            {
                _diagnostics.Add($"ERROR: Unrecognized token '{Current}'");
                return new SyntaxToken(SyntaxKind.BadToken, Next(), _text.Substring(_position - 1, 1), null);
            }
        }
    }

    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private List<string> _diagnostics;

        public IEnumerable<string> Diagnostics => _diagnostics;

        public Parser(string text)
        {
            Lexer lexer = new Lexer(text);
            var tokens = new List<SyntaxToken>();
            SyntaxToken token;

            do
            {
                token = lexer.NextToken();

                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }

            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics = new List<string>();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Peek(int offset)
        {
            int index = _position + offset;

            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            else return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();
            else 
            {
                _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
                return new SyntaxToken(kind, Current.Position, null, null);
            }
        }

        public SyntaxTree Parse()
        {
            var expression = ParsExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);

            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParsExpression()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.PlusToken ||
                   Current.Kind == SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
        
            return left;
        }

        public ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }

    sealed class SyntaxTree
    {
        private IEnumerable<string> _diagnostics;

        public IEnumerable<string> Diagnostics => _diagnostics;
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
        

        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            _diagnostics = diagnostics;
            
            Root = root;
            EndOfFileToken = endOfFileToken;
        }
    }

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {
    }

    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken NumberToken { get; }

        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }
        
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    class Evaluator
    {
        private ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int EvaluateExpression(ExpressionSyntax root)
        {
            if (root is NumberExpressionSyntax n)
            {
                return (int)n.NumberToken.Value;
            }

            return 0;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }
    }
}
