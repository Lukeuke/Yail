using Yail.Common;
using Yail.Grammar;
using Yail.Shared;

namespace Yail;

public sealed class ExpressionsVisitor : ExpressionsBaseVisitor<ValueObj?>
{
    private Dictionary<string, ValueObj?> _variables = new();
    private Dictionary<string, FunctionDefinition> _functions = new();
    private ValueObj? returnValueFromFunction;

    public override ValueObj? VisitVariableDeclaration(ExpressionsParser.VariableDeclarationContext context)
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
        if (value is null) return null;

        if (value.Value.DataType is EDataType.Void)
        {
            return null;
        }
        
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

        if (value is null) return null;
        
        if (value.Value.DataType is EDataType.Void)
        {
            return null;
        }
        
        if (value is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Variable '${variableName}' is not defined.");
            Environment.Exit(1);
        }
        
        return value;
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

        if (functionName == YailTokens.Input)
        {
            var inputValue = Console.ReadLine();

            var result = new ValueObj
            {
                DataType = EDataType.String,
                Value = inputValue
            };

            return result;
        }
        
        if (functionName == YailTokens.ParseInt)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ParseInt requires exactly one argument.");
            }

            var argument = Visit(context.expression(0));
            return IoHelper.ParseInt((ValueObj)argument);
        }
        
        if (functionName == YailTokens.ParseDouble)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ParseDouble requires exactly one argument.");
            }

            var argument = Visit(context.expression(0));
            return IoHelper.ParseDouble((ValueObj)argument);
        }
        
        if (functionName == YailTokens.ParseBool)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ParseBool requires exactly one argument.");
            }

            var argument = Visit(context.expression(0));
            return IoHelper.ParseBool((ValueObj)argument);
        }
        
        if (_functions.TryGetValue(functionName, out var functionInfo))
        {
            // Collect arguments for the function call
            var arguments = new List<ValueObj?>();
            foreach (var exprContext in context.expression())
            {
                var value = Visit(exprContext);
                arguments.Add(value);
            }

            // Check if the number of arguments matches
            if (arguments.Count != functionInfo.Parameters.Count)
            {
                throw new InvalidOperationException($"Function '{functionName}' expects {functionInfo.Parameters.Count} parameters.");
            }

            // Create a new scope for function parameters
            var functionScope = new Dictionary<string, ValueObj?>();
            for (var i = 0; i < arguments.Count; i++)
            {
                functionScope[functionInfo.Parameters[i].Name] = arguments[i];
            }

            // Save the current variable scope and switch to the function's scope
            var currentScope = _variables;
            _variables = new Dictionary<string, ValueObj?>(functionScope);

            var body = functionInfo.Body as ExpressionsParser.BlockContext;
            foreach (var statementContext in body.line())
            {
                if (returnValueFromFunction is not null)
                {
                    break;
                }

                _ = Visit(statementContext);

                if (returnValueFromFunction is not null)
                {
                    break;
                }
            }

            if (functionInfo.ReturnType == EDataType.Void)
            {
                return new ValueObj
                {
                    DataType = EDataType.Void,
                    Value = null,
                    IsConst = true
                };
            }
            
            _variables = currentScope;

            var returnValue = returnValueFromFunction;

            // dynamically set the function return type if any
            if (functionInfo.ReturnType == EDataType.Any)
            {
                functionInfo.ReturnType = returnValue.Value.DataType;
            }
            
            if (returnValue.Value.DataType != functionInfo.ReturnType)
            {
                throw new Exception($"Return type does not match on function {functionName}.\nExpected: '{returnValue.Value.DataType}' was '{functionInfo.ReturnType}'."); // TODO: implement custom exceptions
            }
            
            returnValueFromFunction = null;
            return returnValue;
        }
        
        throw new InvalidOperationException($"Undefined function: {functionName}");
    }

    public override ValueObj? VisitCompareExpr(ExpressionsParser.CompareExprContext context)
    {
        var left = Visit(context.expression(0));
        var right  = Visit(context.expression(1));
        
        left.ThrowIfNull();
        right.ThrowIfNull();
        
        var newValue = context.compareOp().GetText() switch
        {
            "==" => Equals((ValueObj)left, (ValueObj)right),
            "!=" => !Equals((ValueObj)left, (ValueObj)right),
            ">" => OperationsHelper.Compare((ValueObj)left, (ValueObj)right) > 0,
            "<" => OperationsHelper.Compare((ValueObj)left, (ValueObj)right) < 0,
            ">=" => OperationsHelper.Compare((ValueObj)left, (ValueObj)right) >= 0,
            "<=" => OperationsHelper.Compare((ValueObj)left, (ValueObj)right) <= 0,
            _ => throw new ArgumentOutOfRangeException($"Unknown comparison operator: {context.compareOp().GetText()}")
        };

        return new ValueObj
        {
            IsConst = true,
            Value = newValue,
            DataType = EDataType.Boolean
        };
    }

    public override ValueObj? VisitIfBlock(ExpressionsParser.IfBlockContext context)
    {
        var conditionResult = EvaluateCondition(context.expression());

        if (conditionResult)
        {
            Visit(context.block());
        }
        else
        {
            if (context.elseIfBlock() != null)
            {
                Visit(context.elseIfBlock());
            }
        }

        return null;
    }

    private bool _shouldBreak;
    public override ValueObj? VisitWhileBlock(ExpressionsParser.WhileBlockContext context)
    {
        while (EvaluateCondition(context.expression()))
        {
            _shouldBreak = false;

            Visit(context.block());

            if (_shouldBreak)
                break;
        }
        
        return null;
    }

    private bool EvaluateCondition(ExpressionsParser.ExpressionContext conditionContext)
    {
        var conditionResult = Visit(conditionContext);

        if (conditionResult is null || conditionResult.Value.DataType != EDataType.Boolean)
        {
            throw new InvalidOperationException("Condition in must evaluate to a boolean value.");
        }

        return (bool)conditionResult.Value.Value!;
    }

    public override ValueObj? VisitReturn(ExpressionsParser.ReturnContext context)
    {
        var returnValue = Visit(context.expression());

        returnValueFromFunction = returnValue;

        return returnValue;
    }
    
    public override ValueObj? VisitBreak(ExpressionsParser.BreakContext context)
    {
        _shouldBreak = true;
        return null;
    }

    public override ValueObj? VisitFunctionDeclaration(ExpressionsParser.FunctionDeclarationContext context)
    {
        var functionName = context.IDENTIFIER().GetText();
        
        var returnType = context.DATA_TYPES().GetText();
        
        var parameters = new List<(string, string)>();
        if (context.parameterList() != null)
        {
            foreach (var paramContext in context.parameterList().parameter())
            {
                var paramType = paramContext.DATA_TYPES().GetText();
                var paramName = paramContext.IDENTIFIER().GetText();
                parameters.Add((paramType, paramName));
            }
        }

        var body = context.block();

        var functionInfo = new FunctionDefinition(functionName, returnType.ToDataType(), parameters, body);
        _functions[functionName] = functionInfo;

        return null;
    }
}