using System;

namespace xc.CodeAnalysis
{
    public sealed class Evaluator
    {
        private ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int EvaluateExpression(ExpressionSyntax root)
        {
            if (root is LiteralExpressionSyntax n)
                return (int)n.LiteralToken.Value;
            else if (root is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                else if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }
            else if (root is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);
            else throw new Exception($"Unsupported ExpressionSyntax type {root.GetType()}");
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }
    }
}