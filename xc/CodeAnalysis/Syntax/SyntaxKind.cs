namespace Xlang.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        IdentifierToken,
        TrueToken,
        FalseToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EndOfFileToken,
        BadToken,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}