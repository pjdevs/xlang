using System;
using System.Collections.Generic;
using System.Linq;

namespace xc.CodeAnalysis
{
    public sealed class SyntaxTree
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

        public static SyntaxTree Parse(string text)
        {
            return new Parser(text).Parse();
        }
    }
}