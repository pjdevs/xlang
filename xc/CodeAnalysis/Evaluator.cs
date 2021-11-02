using System;
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

        private object EvaluateExpression(BoundExpression node)
        {
            switch (node)
            {
                case BoundLiteralExpression l:
                    return l.Value;
                case BoundUnaryExpression u:
                    {
                        var operand = EvaluateExpression(u.Operand);

                        return u.OperatorKind switch
                        {
                            BoundUnaryOperatorKind.Identity        => operand,
                            BoundUnaryOperatorKind.Negation        => -(int)operand,
                            BoundUnaryOperatorKind.LogicalNegation => !(bool)operand,
                            _                                      => throw new Exception($"Unexpected unary operator kind {u.OperatorKind}")
                        };
                    }

                case BoundBinaryExpression b:
                    {
                        var left = EvaluateExpression(b.Left);
                        var right = EvaluateExpression(b.Right);

                        return b.OperatorKind switch
                        {
                            BoundBinaryOperatorKind.Addition       => (int)left + (int)right,
                            BoundBinaryOperatorKind.Substraction   => (int)left - (int)right,
                            BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
                            BoundBinaryOperatorKind.Division       => (int)left / (int)right,
                            BoundBinaryOperatorKind.LogicalAnd     => (bool)left && (bool)right,
                            BoundBinaryOperatorKind.LogicalOr      => (bool)left || (bool)right,
                            _                                      => throw new Exception($"Unexpected binary operator {b.OperatorKind}")
                        };
                    }

                default:
                    throw new Exception($"Unsupported ExpressionSyntax type {node.GetType()}");
            }
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }
    }
}