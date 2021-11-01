using System;

namespace Xlang.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 2;

                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            return text switch
            {
                "true"  => SyntaxKind.TrueToken,
                "false" => SyntaxKind.FalseToken,
                _       => SyntaxKind.IdentifierToken 
            };
        }
    }
}