﻿namespace BlazorDatasheet.Formula.Core.Interpreter.Syntax;

public enum SyntaxKind
{
    //Tokens
    BadToken,
    EndOfFileToken,
    WhitespaceToken,
    NumberToken,
    ColonToken,
    CommaToken,
    StringToken,
    LeftCurlyBracketToken,
    RightCurlyBracketToken,
    SemiColonToken,

    // Operators
    PlusToken,
    MinusToken,
    SlashToken,
    StarToken,
    LeftParenthesisToken,
    RightParenthesisToken,
    IdentifierToken,
    BangToken,
    AmpersandAmpersandToken,
    PipePipeToken,
    EqualsEqualsToken,
    NotEqualsToken,
    LessThanToken,
    GreaterThanToken,
    LessThanEqualToToken,
    GreaterThanEqualToToken,

    // Keywords
    FalseKeyword,
    TrueKeyword,
    AndKeyword,

    // Expressions
    LiteralExpression,
    NameExpression,
    UnaryExpression,
    BinaryExpression,
    ParenthesizedExpression,
    EqualsToken,
    FunctionCallExpression,
    RangeReferenceExpression,
    CellReferenceExpression,
    ArrayConstantExpression,
    ErrorExpression,
}