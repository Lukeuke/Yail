using Yail.Common;
using Yail.Common.Extentions;
using Yail.Grammar;
using Yail.Shared;
using Yail.Shared.Abstract;
using Yail.Shared.Constants;
using Yail.Shared.Helpers;
using Yail.Shared.Objects;

namespace Yail;

public sealed class ExpressionsVisitor : ExpressionsBaseVisitor<ValueObj?>
{
    private Dictionary<string, ValueObj?> _variables = new();
    private Dictionary<string, ValueObj?> _instances = new();
    private Dictionary<string, FunctionDefinition> _functions = new();
    private ValueObj? returnValueFromFunction;
    private readonly HashSet<string> _activeDirectives = new();
    private readonly HashSet<string> _usings = new();
    private string _currentPackage = "main";
    private Stack<FunctionDefinition> _callStack = new();

    private bool isReference;
    
    public override ValueObj? VisitVariableDeclaration(ExpressionsParser.VariableDeclarationContext context)
    {
        isReference = context.REFERENCE() is not null;
        var variableName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());

        if (_insideIterable)
        {
            AddOrAssignVariable(variableName, value);
        }
        else
        {
            AddVariable(variableName, value);
        }
        
        return null;
    }

    public override ValueObj? VisitAssignment(ExpressionsParser.AssignmentContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());
        if (value is null) return null;

        if (value.DataType is EDataType.Void)
        {
            return null;
        }
        
        if (_currentStruct is not null)
        {
            if (!_instances.TryGetValue(_currentStruct, out var currStruct))
                ExceptionHelper.PrintError($"Struct '{_currentStruct}' is not defined.");
            
            (currStruct as StructObj)!.Update(variableName, value);
            return null;
        }
        
        if (!_variables.TryGetValue(variableName, out var prevVal))
        {
            ExceptionHelper.PrintError($"Variable '${variableName}' is not defined.");
        }

        if (prevVal is IAccessible accessible)
        {
            var arrayAccessor = context.arrayAccessor();

            // if is not null then its this x[]; otherwise is just simple assigment
            if (arrayAccessor is not null)
            {
                var index = Visit(arrayAccessor.expression()).Value;
                
                accessible.Set(index, value);
                _variables[variableName] = prevVal;

                ReferenceExtension.UpdateTheReferenceVariables(accessible, _variables);
                
                return accessible.Get(index);
            }

            _variables[variableName] = value;

            return null;
        }
        
        if (_activeDirectives.Contains(YailTokens.DisableTypeChecks))
        {
            _variables[variableName] = value;
            return null;
        }
        
        if (prevVal.DataType != value.DataType)
        {
            ExceptionHelper.PrintError($"Variable '${variableName}' type does not match.");
        }
        
        _variables[variableName] = value;
        
        return null;
    }

    public override ValueObj? VisitOperationAssignment(ExpressionsParser.OperationAssignmentContext context)
    {
        var variableName = context.IDENTIFIER().GetText();

        if (!_variables.TryGetValue(variableName, out var existingValue))
        {
            throw new InvalidOperationException($"Variable '{variableName}' is not defined.");
        }

        var rhsValue = Visit(context.expression());
        rhsValue.ThrowIfNull();

        var operation = context.addOp()?.GetText() ?? context.multiplyOp()?.GetText();
        if (operation == null)
        {
            throw new InvalidOperationException($"Unknown operation in compound assignment for variable '{variableName}'.");
        }

        if (!_activeDirectives.Contains(YailTokens.DisableTypeChecks))
        {
            if (existingValue.DataType != rhsValue.DataType)
                throw new Exception($"Data on variable '{existingValue}' type mismatch");
        }
        
        var newValue = operation switch
        {
            "+" => OperationsHelper.Add((ValueObj)existingValue, (ValueObj)rhsValue),
            "-" => OperationsHelper.Subtract((ValueObj)existingValue, (ValueObj)rhsValue),
            "*" => OperationsHelper.Multiply((ValueObj)existingValue, (ValueObj)rhsValue),
            "/" => OperationsHelper.Divide((ValueObj)existingValue, (ValueObj)rhsValue),
            "%" => OperationsHelper.Modulo((ValueObj)existingValue, (ValueObj)rhsValue),
            _ => throw new InvalidOperationException($"Unsupported operation '{operation}' in compound assignment.")
        };

        _variables[variableName] = newValue;

        return null;
    }

    public override ValueObj? VisitSelfOperation(ExpressionsParser.SelfOperationContext context)
    {
        var variableName = context.IDENTIFIER().GetText();

        if (!_variables.TryGetValue(variableName, out var valueObj))
        {
            throw new InvalidOperationException($"Variable '{variableName}' is not defined.");
        }

        valueObj.ThrowIfNull();

        var preOp = context.children.FirstOrDefault()?.GetText();

        var postOp = context.children.LastOrDefault()?.GetText();

        var newValue = valueObj;

        if (preOp != null && preOp != variableName)
        {
            newValue = PerformOperation(preOp, (ValueObj)valueObj);
        }

        _variables[variableName] = newValue;

        if (postOp != null && postOp != variableName)
        {
            var oldValue = valueObj;
            _variables[variableName] = PerformOperation(postOp, (ValueObj)valueObj);
            return oldValue;
        }

        return newValue;
    }
    
    public override ValueObj? VisitIdentifierExpr(ExpressionsParser.IdentifierExprContext context)
    {
        var variableName = context.IDENTIFIER().GetText();

        _variables.TryGetValue(variableName, out var value);

        if (value is null) return null;
        
        if (value.DataType is EDataType.Void)
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

    public override ValueObj VisitConstant(ExpressionsParser.ConstantContext context)
    {
        var result = new ValueObj
        {
            IsConst = true
        };
        var value = context.GetText();
        
        if (context.INTEGER() is not null)
        {
            result.DataType = EDataType.Int32;
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

    public override ValueObj? VisitParenthesizedExpr(ExpressionsParser.ParenthesizedExprContext context)
    {
        return Visit(context.expression());
    }

    #region Functions
    
    public override ValueObj? VisitSimpleFunctionCall(ExpressionsParser.SimpleFunctionCallContext context)
    {
        var functionName = context.IDENTIFIER().GetText();
        
        var functionDefinition = GetFunction(_currentPackage, functionName);
        if (functionDefinition is not null)
        {
            //_currentPackage = functionDefinition.Package;
            _callStack.Push(functionDefinition);
        }
        /*else
        {
            _callStack.Clear();
        }*/
        
        if (functionName == YailBuiltInFunctions.Print)
        {
            foreach (var exprContext in context.expression())
            {
                var valueObj = Visit(exprContext);
                if (valueObj != null)
                {
                    valueObj.Print();
                }
            }
            return null;
        }
        if (functionName == YailBuiltInFunctions.PrintLn)
        {
            foreach (var exprContext in context.expression())
            {
                var valueObj = Visit(exprContext);
                if (valueObj != null)
                {
                    valueObj.Print(true);
                }
            }
            return null;
        }
        if (functionName == YailBuiltInFunctions.Input)
        {
            var inputValue = Console.ReadLine();
            var result = new ValueObj
            {
                DataType = EDataType.String,
                Value = inputValue
            };
            return result;
        }
        if (functionName == YailBuiltInFunctions.ParseInt)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ParseInt requires exactly one argument.");
            }
            var argument = Visit(context.expression(0));
            return IoHelper.ParseInt((ValueObj)argument);
        }
        if (functionName == YailBuiltInFunctions.ParseDouble)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ParseDouble requires exactly one argument.");
            }
            var argument = Visit(context.expression(0));
            return IoHelper.ParseDouble((ValueObj)argument);
        }
        if (functionName == YailBuiltInFunctions.ParseBool)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ParseBool requires exactly one argument.");
            }
            var argument = Visit(context.expression(0));
            return IoHelper.ParseBool((ValueObj)argument);
        }
        if (functionName == YailBuiltInFunctions.ToString)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("string function requires exactly one argument.");
            }

            var argument = Visit(context.expression(0));
            if (argument == null)
            {
                throw new InvalidOperationException("string function requires a valid argument.");
            }

            return new ValueObj
            {
                DataType = EDataType.String,
                Value = argument.Value!.ToString()
            };
        }
        if (functionName == YailBuiltInFunctions.ToCharArray)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("ToCharArray function requires exactly one argument.");
            }

            var argument = Visit(context.expression(0));
            if (argument == null)
            {
                throw new InvalidOperationException("ToCharArray function requires a valid argument.");
            }

            var n = argument.Value!.ToString()!;
            var arr = n.Select(x => new ValueObj(x, EDataType.Char)).ToList();
            return new ArrayObj(arr);
        }
        if (functionName == YailBuiltInFunctions.Typeof)
        {
            if (context.expression().Length != 1)
            {
                throw new InvalidOperationException("Typeof function requires exactly one argument.");
            }

            var argument = Visit(context.expression(0));
            if (argument == null)
            {
                throw new InvalidOperationException("Typeof function requires a valid argument.");
            }

            return new ValueObj
            {
                DataType = EDataType.String,
                Value = argument.DataType.ToString()
            };
        }

        var functionInfo = GetFunction(_currentPackage, functionName);
        if (functionInfo is not null)
        {
            var isPrivate = functionInfo.AccessModifier == EAccessModifier.Private;

            if (isPrivate)
            {
                bool any = false;
                foreach (var ancestor in _callStack)
                {
                    // check if ancesor is public and is from the same package
                    if (ancestor.AccessModifier == EAccessModifier.Public && ancestor.Package == functionInfo.Package) 
                    {
                        any = true;
                        break;
                    }
                }

                var canCallPrivate = any || _currentPackage == functionInfo.Package;

                if (!canCallPrivate)
                {
                    throw new Exception($"Cannot call private function '{functionName}'.");
                }
            }
            
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

            // Dynamically set the function return type if any
            if (functionInfo.ReturnType == EDataType.Any)
            {
                functionInfo.ReturnType = returnValue.DataType;
            }

            if (returnValue.DataType != functionInfo.ReturnType)
            {
                throw new Exception($"Return type does not match on function {functionName}.\nExpected: '{returnValue.DataType}' was '{functionInfo.ReturnType}'.");
            }

            returnValueFromFunction = null;
            return returnValue;
        }

        throw new InvalidOperationException($"Undefined function: {functionName}");
    }

    public override ValueObj? VisitNamespacedFunctionCall(ExpressionsParser.NamespacedFunctionCallContext context)
    {
        var package = context.IDENTIFIER(0).GetText();
        var functionName = context.IDENTIFIER(1).GetText();

        var functionDefinition = GetFunction(package, functionName);

        if (functionDefinition is not null)
        {
            //_currentPackage = functionDefinition.Package;
            _callStack.Push(functionDefinition);
        }
        /*else
        {
            _callStack.Clear();
        }*/
        
        var functionInfo = GetFunction(package, functionName);
        if (functionInfo is not null)
        {
            var isPrivate = functionInfo.AccessModifier == EAccessModifier.Private;

            if (isPrivate)
            {
                bool any = false;
                foreach (var ancestor in _callStack)
                {
                    // check if ancesor is public and is from the same package
                    if (ancestor.AccessModifier == EAccessModifier.Public && ancestor.Package == functionInfo.Package) 
                    {
                        any = true;
                        break;
                    }
                }

                var canCallPrivate = any || _currentPackage == functionInfo.Package;

                if (!canCallPrivate)
                {
                    throw new Exception($"Cannot call private function '{functionName}'.");
                }
            }
            
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

            // Dynamically set the function return type if any
            if (functionInfo.ReturnType == EDataType.Any)
            {
                functionInfo.ReturnType = returnValue.DataType;
            }

            if (returnValue.DataType != functionInfo.ReturnType)
            {
                throw new Exception($"Return type does not match on function {functionName}.\nExpected: '{returnValue.DataType}' was '{functionInfo.ReturnType}'.");
            }

            returnValueFromFunction = null;
            return returnValue;
        }

        throw new InvalidOperationException($"Undefined function: {functionName}");
    }

    public override ValueObj? VisitFunctionDeclaration(ExpressionsParser.FunctionDeclarationContext context)
    {
        var functionName = context.IDENTIFIER().GetText();
        var accessLevel = context.accessLevels();
        var accessLevelName = string.Empty;
        
        if (accessLevel is not null)
        {
            accessLevelName = accessLevel.GetText();
        }
        
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

        var functionInfo = new FunctionDefinition(functionName, accessLevelName.ToAccessLevel(),returnType.ToDataType(), parameters, body, _currentPackage);
        SetFunction(_currentPackage, functionName, functionInfo);

        return null;
    }

    public override ValueObj? VisitMethodCall(ExpressionsParser.MethodCallContext context)
    {
        var objectName = context.IDENTIFIER(0).GetText();
        var methodName = context.IDENTIFIER(1).GetText();

        if (!_variables.TryGetValue(objectName, out var obj))
            throw new InvalidOperationException($"Object '{objectName}' is not defined.");

        if (obj is ArrayObj array)
        {
            return HandleArrayMethod(array, methodName, context.expression());
        }

        throw new InvalidOperationException($"'{methodName}' is not a valid method for type '{obj.DataType}'.");
    }
    
    #endregion

    public override ValueObj? VisitUsingDirective(ExpressionsParser.UsingDirectiveContext context)
    {
        _usings.Add(context.IDENTIFIER().GetText());
        return base.VisitUsingDirective(context);
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
            "is" => Equals((ValueObj)left, (ValueObj)right),
            "!=" => !Equals((ValueObj)left, (ValueObj)right),
            "is not" => !Equals((ValueObj)left, (ValueObj)right),
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

    public override ValueObj? VisitNegationExpr(ExpressionsParser.NegationExprContext context)
    {
        var val = Visit(context.expression());

        var result =  new ValueObj
        {
            DataType = EDataType.Boolean,
            IsConst = true,
            Value = false
        };
        
        if (val is null || val.DataType == EDataType.Null)
        {
            result.Value = true;
            return result;
        }

        if (val.DataType == EDataType.Boolean)
        {
            result.Value = !(bool)val.Value!;
            return result;
        }

        return result;
    }

    #region Iterations
    
    private bool _shouldBreak;
    private bool _shouldContinue;
    private bool _insideIterable;

    public override ValueObj? VisitWhileBlock(ExpressionsParser.WhileBlockContext context)
    {
        _insideIterable = true;
        while (EvaluateCondition(context.expression()))
        {
            _shouldBreak = false;
            _shouldContinue = false;
        
            foreach (var line in context.block().line())
            {
                Visit(line);

                if (_shouldBreak)
                    break;

                if (_shouldContinue)
                    break;
            }

            if (_shouldBreak)
                break;
        }

        _insideIterable = false;
        return null;
    }

    public override ValueObj? VisitForBlock(ExpressionsParser.ForBlockContext context)
    {
        // Variable declaration or assignment
        if (context.variableDeclaration() != null)
        {
            Visit(context.variableDeclaration());
        }
        else if (context.assignment() != null)
        {
            Visit(context.assignment());
        }

        // Expression
        while (context.expression() == null || EvaluateCondition(context.expression()))
        {
            _shouldBreak = false;
            _shouldContinue = false;
            _insideIterable = true;
            
            // before doing work inside, check for break and continue conditions
            if (_shouldBreak)
                break;

            if (_shouldContinue)
                continue;

            // Do work on lines inside for-loop
            Visit(context.block());
            
            // Increment/Update (executed at the end of each iteration)
            if (context.assignment() != null)
            {
                Visit(context.assignment());
            }
            else if (context.selfOperation() != null)
            {
                Visit(context.selfOperation());
            }
        }

        _insideIterable = false;
        return null;
    }

    public override ValueObj? VisitForeachBlock(ExpressionsParser.ForeachBlockContext context)
    {
        var loopVarName = context.IDENTIFIER().GetText();
        var collection = Visit(context.expression());

        if (collection is null)
            throw new InvalidOperationException("Collection cannot be null.");

        switch (collection.DataType)
        {
            case EDataType.String:
                IterateOverString(loopVarName, collection.Value!.ToString(), context.block());
                break;

            case EDataType.Array when collection is ArrayObj array:
                IterateOverArray(loopVarName, array, context.block());
                break;

            default:
                throw new InvalidOperationException("You can only iterate over strings or arrays.");
        }

        return null;
    }

    #endregion
    
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

    public override ValueObj? VisitContinue(ExpressionsParser.ContinueContext context)
    {
        _shouldContinue = true;
        return null;
    }

    public override ValueObj? VisitDirective(ExpressionsParser.DirectiveContext context)
    {
        var directiveName = context.GetText().Replace("#use", "").Trim();
        _activeDirectives.Add(directiveName);

        return null;
    }

    public override ValueObj? VisitPackageDeclaration(ExpressionsParser.PackageDeclarationContext context)
    {
        var identifier = context.IDENTIFIER();
        _currentPackage = identifier.GetText() ?? "main";
        return null;
    }

    public override ValueObj? VisitBoolExpr(ExpressionsParser.BoolExprContext context)
    {
        var lhsValue = (ValueObj)Visit(context.expression(0));
        var rhsValue = (ValueObj)Visit(context.expression(1));
        
        var operatorText = context.boolOp().GetText();
        
        var result = OperationsHelper.PerformOperation(lhsValue, rhsValue, operatorText);

        return new ValueObj
        {
            IsConst = lhsValue.IsConst && rhsValue.IsConst,
            Value = result,
            DataType = DataTypeHelper.DetermineResultType(lhsValue, rhsValue)
        };
    }

    public override ValueObj? VisitCastExpr(ExpressionsParser.CastExprContext context)
    {
        var value = Visit(context.expression());
        var targetType = context.DATA_TYPES().GetText();

        return value?.CastTo(targetType);
    }

    #region Arrays

    public override ValueObj? VisitArrayIndexExpr(ExpressionsParser.ArrayIndexExprContext context)
    {
        var accessedValue = (ValueObj)Visit(context.expression());
        var indexValue = (ValueObj)Visit(context.arrayAccessor().expression());

        if (accessedValue.DataType == EDataType.String)
        {
            return SquareBracketAccessExtension.AccessString(indexValue, accessedValue, isReference);
        }
        
        if (accessedValue.GetType() == typeof(ArrayObj))
        {
            return SquareBracketAccessExtension.AccessArrayValue(indexValue, (ArrayObj)accessedValue, isReference);
        }

        if (accessedValue.GetType() == typeof(DictionaryObj))
        {
            return SquareBracketAccessExtension.AccessDictionaryValue(indexValue, (DictionaryObj)accessedValue, isReference);
        }
        
        throw new Exception($"Index accessor cannot be used on {accessedValue.DataType.ToString()}.");
    }

    public override ValueObj? VisitArrayLiteralExpr(ExpressionsParser.ArrayLiteralExprContext context)
    {
        var array = Visit(context.arrayLiteral()) as ArrayObj;
        return array;
    }

    public override ValueObj? VisitArrayLiteral(ExpressionsParser.ArrayLiteralContext context)
    {
        var values = context.expression().Select(expr => (ValueObj)Visit(expr)).ToList();
        return new ArrayObj(values);
    }

    public override ValueObj? VisitArrayLength(ExpressionsParser.ArrayLengthContext context)
    {
        var value = Visit(context.expression());

        var output = new ValueObj(0, EDataType.Int32, true);

        if (value.DataType == EDataType.String)
        {
            var str = value.Value as string;
            output.Value = str!.Length;
            return output;
        }

        if (value.GetType() == typeof(ArrayObj))
        {
            var arr = value as ArrayObj;
            output.Value = arr!.Get().Count;
            return output;
        }

        throw new Exception("len() function does not support that data type");
    }

    #endregion

    #region Dictionaries

    public override ValueObj VisitDictionaryEntry(ExpressionsParser.DictionaryEntryContext context)
    {
        var value = Visit(context.expression());
        var key = context.STRING().GetText()[1.. ^1];

        return new KeyValuePairObj(key, value);
    }

    public override ValueObj VisitDictionaryLiteral(ExpressionsParser.DictionaryLiteralContext context)
    {
        var dict = new Dictionary<string, ValueObj>();
        
        foreach (var entry in context.dictionaryEntry())
        {
            var kvp = Visit(entry) as KeyValuePairObj;
            
            if (!dict.TryAdd(kvp.Key, kvp.Value as ValueObj))
            {
                throw new ArgumentException($"Cannot add '{kvp.Key}' key to the dictionary.");
            }
        }
        
        return new DictionaryObj(dict);
    }

    #endregion

    #region Structs

    public override ValueObj VisitStructBlock(ExpressionsParser.StructBlockContext context)
    {
        var structName = context.IDENTIFIER().GetText();
        var isPublic = context.accessLevels()?.GetText()?.Equals("pub") ?? false;

        var structObj = new StructObj
        {
            Name = $"{_currentPackage}::{structName}",
            IsPublic = isPublic,
        };
        
        foreach (var structLine in context.structBody().structLine())
        {
            var varName = structLine.variableDefine().IDENTIFIER().GetText();
            var dataType = structLine.variableDefine().DATA_TYPES().GetText().ToDataType();

            var expr = structLine.variableDefine().expression();

            if (expr is not null)
            {
                var defaultVal = Visit(expr);
                
                structObj.Set(varName, defaultVal);
            }
            else
            {
                structObj.Set(varName, new ValueObj(dataType));
            }
        }

        if (!_instances.TryAdd(structObj.Name, structObj))
        {
            throw new Exception("Instance with this identifier already exists in this package.");
        }
        
        return structObj;
    }

    private string? _currentStruct;
    public override ValueObj? VisitInstanceCreateExpr(ExpressionsParser.InstanceCreateExprContext context)
    {
        var instanceName = string.Empty;
        var packageName = "main";
        
        try
        {
            instanceName = context.instanceCreate().IDENTIFIER(1).GetText();
            packageName = context.instanceCreate().IDENTIFIER(0).GetText();
        }
        catch
        {
            instanceName = context.instanceCreate().IDENTIFIER(0).GetText();
        }

        if (context.instanceCreate().instanceBody() is not null)
        {
            _currentStruct = $"{packageName}::{instanceName}";
            Visit(context.instanceCreate().instanceBody());
            _currentStruct = null;
        }
        
        var valueObj = _instances[$"{packageName}::{instanceName}"];
        
        valueObj.ThrowIfNull();

        if (!_variables.TryAdd(instanceName, valueObj))
        {
            ExceptionHelper.PrintError($"Variable with name '{instanceName}' is already declared");
        }
        
        return valueObj;
    }

    public override ValueObj? VisitInstancePropAssign(ExpressionsParser.InstancePropAssignContext context)
    {
        var instanceName = context.IDENTIFIER(0).GetText();

        var valueObj = _variables[instanceName];

        valueObj.ThrowIfNull();
        
        if (valueObj is not IInstantiable instance)
        {
            throw new Exception("This expression is only valid on instances.");
        }

        var propName = context.IDENTIFIER(1).GetText();

        var prop = instance.Get(propName);

        var value = Visit(context.expression());
        value.ThrowIfNull();
        
        prop.Value = value!.Value;

        return null;
    }

    public override ValueObj VisitInstancePropCallExpr(ExpressionsParser.InstancePropCallExprContext context)
    {
        var instanceName = context.instancePropCall().IDENTIFIER(0).GetText();
        var propName = context.instancePropCall().IDENTIFIER(1).GetText();

        var valueObj = _variables[instanceName];

        valueObj.ThrowIfNull();
        
        if (valueObj is not IInstantiable instance)
        {
            throw new Exception("This expression is only valid on instances.");
        }

        return instance.Get(propName);
    }

    #endregion

    #region Helpers

    private ValueObj PerformOperation(string operation, ValueObj valueObj)
    {
        if (valueObj.DataType != EDataType.Int32 && valueObj.DataType != EDataType.Double)
        {
            throw new InvalidOperationException($"Operation '{operation}' is not supported for type '{valueObj.DataType}'.");
        }

        return operation switch
        {
            "++" => OperationsHelper.Add(valueObj, new ValueObj { DataType = valueObj.DataType, Value = 1 }),
            "--" => OperationsHelper.Subtract(valueObj, new ValueObj { DataType = valueObj.DataType, Value = 1 }),
            "**" => OperationsHelper.Power(valueObj, new ValueObj { DataType = valueObj.DataType, Value = 2 }),
            "//" => OperationsHelper.FloorDivide(valueObj, new ValueObj { DataType = valueObj.DataType, Value = 2 }),
            _ => throw new InvalidOperationException($"Unknown operation '{operation}'.")
        };
    }
    
    private bool EvaluateCondition(ExpressionsParser.ExpressionContext conditionContext)
    {
        var conditionResult = Visit(conditionContext);

        if (conditionResult is null)
        {
            return false;
        }

        if (_shouldBreak)
        {
            return false;
        }
        
        if (conditionResult is null || conditionResult.DataType != EDataType.Boolean)
        {
            throw new InvalidOperationException("Condition in must evaluate to a boolean value.");
        }

        return (bool)conditionResult.Value!;
    }
    
    private void AddVariable(string name, ValueObj value)
    {
        if (_variables.TryGetValue(name, out _))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Variable '${name}' is already defined.");
            Environment.Exit(1);
        }

        _variables.Add(name, value);
    }
    
    private void AddOrAssignVariable(string name, ValueObj value)
    {
        if (_variables.TryGetValue(name, out _))
        {
            _variables[name] = value;
            return;
        }
        
        _variables.Add(name, value);
    }

    private FunctionDefinition? GetFunction(string package, string name)
    {
        _functions.TryGetValue($"{package}::{name}", out var fun);
        return fun;
    }
    
    private bool SetFunction(string package, string name, FunctionDefinition function)
    {
        return _functions.TryAdd($"{package}::{name}", function);
    }
    
    private void IterateOverString(string loopVarName, string str, ExpressionsParser.BlockContext block)
    {
        _insideIterable = true;
        
        foreach (var item in str)
        {
            AddOrAssignVariable(loopVarName, new ValueObj(item, EDataType.Char));
            Visit(block);
        }
        
        _insideIterable = false;
    }
    
    private void IterateOverArray(string loopVarName, ArrayObj array, ExpressionsParser.BlockContext block)
    {
        _insideIterable = true;

        foreach (var item in array.Get())
        {
            AddOrAssignVariable(loopVarName, item);
            Visit(block);
        }

        _insideIterable = false;
    }

    private ValueObj? HandleArrayMethod(ArrayObj array, string methodName, IList<ExpressionsParser.ExpressionContext> arguments)
    {
        switch (methodName)
        {
            case "push":
                if (arguments.Count != 1)
                    throw new ArgumentException("push() requires exactly 1 argument.");
                var valueToPush = Visit(arguments[0]);
                array.Push(valueToPush);
                return null;

            case "pop":
                if (arguments.Count != 0)
                    throw new ArgumentException("pop() requires no arguments.");
                return array.Pop();

            case "removeAt":
                if (arguments.Count != 1)
                    throw new ArgumentException("removeAt() requires exactly 1 argument.");
                var idx = Visit(arguments[0]);
                array.RemoveAt((int)idx.Value);
                return null;
            
            case "count":
                if (arguments.Count != 0)
                    throw new ArgumentException("count() requires no arguments.");
                return new ValueObj(array.Items.Count, EDataType.Int32, true);
                
            default:
                throw new InvalidOperationException($"Unknown method '{methodName}' for arrays.");
        }
    }

    #endregion
}