using System.Collections.Generic;
using BlazorDatasheet.Formula.Core;
using BlazorDatasheet.Formula.Core.Interpreter.Syntax;
using BlazorDatasheet.Formula.Functions.Logical;
using BlazorDatasheet.Test.Formula;
using FluentAssertions;
using NUnit.Framework;

namespace BlazorDatasheet.Test.Functions;

public class LogicalFunctionTests
{
    private TestEnvironment _env;

    [SetUp]
    public void Setup()
    {
        _env = new();
    }

    public object? Eval(string formulaString)
    {
        var eval = new FormulaEvaluator(_env);
        var parser = new FormulaParser();
        return eval.Evaluate(parser.FromString(formulaString));
    }

    [Test]
    public void If_Function_Tests()
    {
        _env.RegisterFunction("IF", new IfFunction());
        Eval("=IF(true,5,4)").Should().Be(5);
        Eval("=IF(false,5,4)").Should().Be(4);
        Eval("=IF(\"s\",5,4)").Should().BeOfType<FormulaError>();
        Eval("=IF(\"true\",5,4)").Should().Be(5);
        Eval("=IF(\"false\",5,4)").Should().Be(4);
        Eval("=IF(2,5,4)").Should().Be(5);
        Eval("=IF(0,5,4)").Should().Be(4);

        _env.SetCellValue(0, 0, new FormulaError(ErrorType.Div0));
        Eval("=IF(A1,5,4)").Should().BeOfType<FormulaError>();
        Eval("=IF(A1,5,4)").As<FormulaError>().ErrorType.Should().Be(ErrorType.Div0);

        _env.SetCellValue(6, 4, true);
        Eval("=IF(E7,5,4)").Should().Be(5);
    }

    [Test]
    public void And_Function_Tests()
    {
        _env.RegisterFunction("AND", new AndFunction());
        Eval("=AND(true)").Should().Be(true);
        Eval("=AND(false)").Should().Be(false);
        Eval("=AND(true, true, true, true)").Should().Be(true);
        Eval("=AND(true, false, true, true)").Should().Be(false);
        _env.SetCellValue(0, 0, true);
        _env.SetCellValue(1, 0, true);
        _env.SetCellValue(2, 0, false);
        _env.SetCellValue(3, 0, "test");
        _env.SetCellValue(4, 0, true);
        Eval("=AND(A1:A2)").Should().Be(true);
        Eval("=AND(A1:A3)").Should().Be(false);
        Eval("=AND(A4)").Should().BeOfType<FormulaError>();
        Eval("=AND(A4:A5)").Should().Be(true);
    }

    [Test]
    public void Or_Function_Tests()
    {
        _env.RegisterFunction("OR", new OrFunction());
        Eval("=OR(true)").Should().Be(true);
        Eval("=OR(false)").Should().Be(false);
        Eval("=OR(true, true, true, true)").Should().Be(true);
        Eval("=OR(false, false, \"false\", false)").Should().Be(false);
        Eval("=OR(true, false, true, true)").Should().Be(true);
        _env.SetCellValue(0, 0, true);
        _env.SetCellValue(1, 0, true);
        _env.SetCellValue(2, 0, false);
        _env.SetCellValue(3, 0, "test");
        _env.SetCellValue(4, 0, false);
        Eval("=OR(A1:A2)").Should().Be(true);
        Eval("=OR(A1:A3)").Should().Be(true);
        Eval("=OR(A4)").Should().BeOfType<FormulaError>();
        Eval("=OR(A4:A5)").Should().Be(false);
    }
}