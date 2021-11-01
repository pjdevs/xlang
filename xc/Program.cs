using System;
using System.Linq;
using Xlang.CodeAnalysis;
using Xlang.CodeAnalysis.Binding;
using Xlang.CodeAnalysis.Syntax;

namespace Xlang
{
    class Program
    {
        static void Main(string[] args)
        {
            var showTree = false;

            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                else if (line == "showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing syntax tree" : "Hiding syntax tree");
                    continue;
                }
                else if (line == "exit")
                    return;
                else if (line == "cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);
                var diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics);

                if (showTree)
                    PrettyPrint(syntaxTree.Root);

                if (diagnostics.Any())
                {
                    foreach (var diagnostic in diagnostics)
                        ErrorPrint(diagnostic);
                }
                else 
                {
                    var evaluator = new Evaluator(boundExpression);
                    Console.WriteLine(evaluator.Evaluate());
                }
            }
        }

        static void ErrorPrint(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
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

            indent += isLast ? "   " : "│  " ;

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);

            Console.ResetColor();
        }
    }
}
 