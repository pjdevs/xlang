using System;
using System.Collections.Generic;
using Xlang.CodeAnalysis.Syntax;

namespace Xlang.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
                SyntaxKind.UnaryExpression   => BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxKind.BinaryExpression  => BindBinaryExpression((BinaryExpressionSyntax)syntax),
                _                            => throw new Exception($"Unexpected syntax {syntax.Kind}")
            };
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperatorKind == null)
            {
                _diagnostics.Add($"ERROR::Binder: Unary operator '{syntax.OperatorToken.Text}' is not defined for type {boundOperand.Type}");
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperatorKind.Value, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperatorKind = BindBinaryOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if (boundOperatorKind == null)
            {
                _diagnostics.Add($"ERROR::Binder: Binary operator '{syntax.OperatorToken.Text}' is not defined for types {boundLeft.Type} and {boundRight.Type}");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorKind.Value , boundRight);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            var operandTypeOk = kind switch
            {
                SyntaxKind.PlusToken  or
                SyntaxKind.MinusToken => operandType == typeof(int),
                SyntaxKind.BangToken  => operandType == typeof(bool),
                _                     => throw new Exception($"Unexpected unary operator {kind}")
            };

            if (operandTypeOk)
                return kind switch
                {
                    SyntaxKind.PlusToken  => BoundUnaryOperatorKind.Identity,
                    SyntaxKind.MinusToken => BoundUnaryOperatorKind.Negation,
                    SyntaxKind.BangToken  => BoundUnaryOperatorKind.LogicalNegation,
                    _                       => throw new Exception($"Unexpected unary operator {kind}")
                };
            else return null;
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            var operandTypeOk = kind switch
            {
                SyntaxKind.PlusToken  or
                SyntaxKind.MinusToken                 => leftType == typeof(int) && rightType == typeof(int),
                SyntaxKind.AmpersandAmpersandToken or
                SyntaxKind.PipePipeToken              => leftType == typeof(bool) && rightType == typeof(bool),
                _                                     => throw new Exception($"Unexpected binary operator {kind}")
            };
 
            if (operandTypeOk)
                return kind switch
                {
                    SyntaxKind.PlusToken               => BoundBinaryOperatorKind.Addition,
                    SyntaxKind.MinusToken              => BoundBinaryOperatorKind.Substraction,
                    SyntaxKind.StarToken               => BoundBinaryOperatorKind.Multiplication,
                    SyntaxKind.SlashToken              => BoundBinaryOperatorKind.Division,
                    SyntaxKind.AmpersandAmpersandToken => BoundBinaryOperatorKind.LogicalAnd,
                    SyntaxKind.PipePipeToken           => BoundBinaryOperatorKind.LogicalOr,
                    _ => throw new Exception($"Unexpected binary operator {kind}")
                };
            else return null;
        }
    }
}