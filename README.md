# Xlang

[![Build Status](https://pjmrl.visualstudio.com/Xlang/_apis/build/status/pjdevs.xlang?branchName=master)](https://pjmrl.visualstudio.com/Xlang/_build/latest?definitionId=1&branchName=master)

Making a compiler from scratch in C# to learn basics of compilation.

## Features

Currently the `xc` (X Compiler) is just a REPL that can evaluate expressions.

## Examples

```
> showTree
Showing syntax tree
> 5 * (2 + 3) / 5
└──BinaryExpression
   ├──BinaryExpression
   │  ├──LiteralExpression
   │  │  └──NumberToken 5
   │  ├──StarToken 
   │  └──ParenthesizedExpression
   │     ├──OpenParenthesisToken 
   │     ├──BinaryExpression
   │     │  ├──LiteralExpression
   │     │  │  └──NumberToken 2
   │     │  ├──PlusToken 
   │     │  └──LiteralExpression
   │     │     └──NumberToken 3
   │     └──CloseParenthesisToken 
   ├──SlashToken 
   └──LiteralExpression
      └──NumberToken 5
5
> exit
```