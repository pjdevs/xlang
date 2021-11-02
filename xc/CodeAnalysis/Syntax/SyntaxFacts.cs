using System;

namespace Xlang.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PlusToken or
                SyntaxKind.MinusToken or
                SyntaxKind.BangToken => 5,
                _                    => 0
            };
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.StarToken or
                SyntaxKind.SlashToken              => 4,
                SyntaxKind.PlusToken or 
                SyntaxKind.MinusToken              => 3,                                
                SyntaxKind.AmpersandAmpersandToken => 2,
                SyntaxKind.PipePipeToken           => 1,
                _                                  => 0
            };
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