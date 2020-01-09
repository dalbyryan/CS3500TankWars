## Code Style

I use the One True Brace Style (1TBS), essentially K&R. https://en.wikipedia.org/wiki/Indentation_style#K&R_style

1TBS style details:

Methods, classes, and namespaces have their opening brace on the next line. However, control structures inside a method
have their opening brace on the same line.

Example:

```
void Foo(string[] args)
{
    if (x) {
        a();
    } else {
        b();
        c();
    }
}
```

Furthermore,
files are automatically formatted according 
to this omnisharp config:

```
{
  "FormattingOptions": {
    "NewLine": "\n",
    "UseTabs": false,
    "TabSize": 4,
    "IndentationSize": 4,
    "SpacingAfterMethodDeclarationName": false,
    "SpaceWithinMethodDeclarationParenthesis": false,
    "SpaceBetweenEmptyMethodDeclarationParentheses": false,
    "SpaceAfterMethodCallName": false,
    "SpaceWithinMethodCallParentheses": false,
    "SpaceBetweenEmptyMethodCallParentheses": false,
    "SpaceAfterControlFlowStatementKeyword": true,
    "SpaceWithinExpressionParentheses": false,
    "SpaceWithinCastParentheses": false,
    "SpaceWithinOtherParentheses": false,
    "SpaceAfterCast": false,
    "SpacesIgnoreAroundVariableDeclaration": false,
    "SpaceBeforeOpenSquareBracket": false,
    "SpaceBetweenEmptySquareBrackets": false,
    "SpaceWithinSquareBrackets": false,
    "SpaceAfterColonInBaseTypeDeclaration": true,
    "SpaceAfterComma": true,
    "SpaceAfterDot": false,
    "SpaceAfterSemicolonsInForStatement": true,
    "SpaceBeforeColonInBaseTypeDeclaration": true,
    "SpaceBeforeComma": false,
    "SpaceBeforeDot": false,
    "SpaceBeforeSemicolonsInForStatement": false,
    "SpacingAroundBinaryOperator": "single",
    "IndentBraces": false,
    "IndentBlock": true,
    "IndentSwitchSection": true,
    "IndentSwitchCaseSection": true,
    "LabelPositioning": "oneLess",
    "WrappingPreserveSingleLine": true,
    "WrappingKeepStatementsOnSingleLine": true,
    "NewLinesForBracesInTypes": true,
    "NewLinesForBracesInMethods": true,
    "NewLinesForBracesInProperties": true,
    "NewLinesForBracesInAccessors": true,
    "NewLinesForBracesInAnonymousMethods": false,
    "NewLinesForBracesInControlBlocks": false,
    "NewLinesForBracesInAnonymousTypes": true,
    "NewLinesForBracesInObjectCollectionArrayInitializers": true,
    "NewLinesForBracesInLambdaExpressionBody": false,
    "NewLineForElse": false,
    "NewLineForCatch": false,
    "NewLineForFinally": false,
    "NewLineForMembersInObjectInit": true,
    "NewLineForMembersInAnonymousTypes": true,
    "NewLineForClausesInQuery": true
  }
}
```