using System;
using Xlang.CodeAnalysis.Syntax;
using Xlang.CodeAnalysis.Binding;

namespace Xlang.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        private int EvaluateExpression(BoundExpression node)
        {
            switch (node)
            {
                case BoundLiteralExpression l:
                    return (int)l.Value;
                case BoundUnaryExpression u:
                    {
                        var operand = EvaluateExpression(u.Operand);

                        return u.OperatorKind switch
                        {
                            BoundUnaryOperatorKind.Identity => operand,
                            BoundUnaryOperatorKind.Negation => -operand,
                            _ => throw new Exception($"Unexpected unary operator kind {u.OperatorKind}")
                        };
                    }

                case BoundBinaryExpression b:
                    {
                        var left = EvaluateExpression(b.Left);
                        var right = EvaluateExpression(b.Right);

                        return b.OperatorKind switch
                        {
                            BoundBinaryOperatorKind.Addition => left + right,
                            BoundBinaryOperatorKind.Substraction => left - right,
                            BoundBinaryOperatorKind.Multiplication => left * right,
                            BoundBinaryOperatorKind.Division => left / right,
                            _ => throw new Exception($"Unexpected binary operator {b.OperatorKind}")
                        };
                    }

                default:
                    throw new Exception($"Unsupported ExpressionSyntax type {node.GetType()}");
            }
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }
    }
}