using Yail.Common;
using Yail.Grammar;
using Yail.Shared;

namespace Yail;

public sealed class ExpressionsVisitor : ExpressionsBaseVisitor<ValueObj?>
{
    private Dictionary<string, ValueObj?> _variables = new();

    public override ValueObj? VisitVariableCreation(ExpressionsParser.VariableCreationContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());
        
        if (_variables.TryGetValue(variableName, out _))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Variable '${variableName}' is already defined.");
            Environment.Exit(1);
        }

        _variables.Add(variableName, value);
        
        return null;
    }

    public override ValueObj? VisitAssignment(ExpressionsParser.AssignmentContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());

        if (!_variables.TryGetValue(variableName, out _))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Variable '${variableName}' is not defined.");
            Environment.Exit(1);
        }

        _variables[variableName] = value;
        
        return null;
    }

    public override ValueObj? VisitIdentifierExpr(ExpressionsParser.IdentifierExprContext context)
    {
        var variableName = context.IDENTIFIER().GetText();

        _variables.TryGetValue(variableName, out var value);

        if (value is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Variable '${variableName}' is not defined.");
            Environment.Exit(1);
        }
        
        return value;
    }

    public override ValueObj? VisitAddOp(ExpressionsParser.AddOpContext context)
    {
        return base.VisitAddOp(context);
    }

    public override ValueObj? VisitAddExpr(ExpressionsParser.AddExprContext context)
    {
        var left = Visit(context.expression(0));
        var right  = Visit(context.expression(1));

        left.ThrowIfNull();
        right.ThrowIfNull();

        var newValue = context.addOp().GetText() switch
        {
            "+" => OperationsHelper.Add((ValueObj)left, (ValueObj)right),
            "-" => OperationsHelper.Subtract((ValueObj)left, (ValueObj)right),
            _ => throw new ArgumentOutOfRangeException()
        };

        return newValue;
    }

    public override ValueObj? VisitMultiplyExpr(ExpressionsParser.MultiplyExprContext context)
    {
        var left = Visit(context.expression(0));
        var right  = Visit(context.expression(1));

        left.ThrowIfNull();
        right.ThrowIfNull();
        
        var newValue = context.multiplyOp().GetText() switch
        {
            "*" => OperationsHelper.Multiply((ValueObj)left, (ValueObj)right),
            "/" => OperationsHelper.Divide((ValueObj)left, (ValueObj)right),
            "%" => OperationsHelper.Modulo((ValueObj)left, (ValueObj)right),
            _ => throw new ArgumentOutOfRangeException()
        };

        return newValue;
    }

    public override ValueObj? VisitConstant(ExpressionsParser.ConstantContext context)
    {
        var result = new ValueObj();
        var value = context.GetText();
        
        if (context.INTEGER() is not null)
        {
            result.DataType = EDataType.Integer;
            result.Value = int.Parse(value);
        }
        else if (context.BOOL() is not null)
        {
            result.DataType = EDataType.Boolean;
            result.Value = bool.Parse(value);
        }
        else if (context.CHAR() is not null)
        {
            result.DataType = EDataType.Char;
            result.Value = char.Parse(value[1 ..^1]);
        }
        else if (context.STRING() is not null)
        {
            result.DataType = EDataType.String;
            result.Value = value[1 ..^1];
        }
        else if (context.DOUBLE() is not null)
        {
            result.DataType = EDataType.Double;
            result.Value = double.Parse(value.Replace(".", ","));
        }
        else if (context.NULL() is not null)
        {
            result.DataType = EDataType.Null;
            result.Value = null;
        }
        
        return result;
    }

    // TODO: refactor
    public override ValueObj? VisitFunctionCall(ExpressionsParser.FunctionCallContext context)
    {
        var functionName = context.IDENTIFIER().GetText();

        if (functionName == YailTokens.Print)
        {
            foreach (var exprContext in context.expression())
            {
                var valueObj = Visit(exprContext);
                if (valueObj != null)
                {
                    Console.Write(valueObj.Value.Value);
                }
            }
            return null; 
        }
        if (functionName == YailTokens.PrintLn)
        {
            foreach (var exprContext in context.expression())
            {
                var valueObj = Visit(exprContext);
                if (valueObj != null)
                {
                    Console.WriteLine(valueObj.Value.Value);
                }
            }
            return null; 
        }

        throw new InvalidOperationException($"Undefined function: {functionName}");
    }

}