using Yail.Grammar;

namespace Yail;

public enum EDataType
{
    Null,
    Integer,
    Boolean,
    Char,
    String,
    Double
}

public struct ValueObj
{
    public bool IsConst;
    public object? Value;
    public EDataType DataType;
}

public class ExpressionsVisitor : ExpressionsBaseVisitor<ValueObj?>
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
            result.DataType = EDataType.String;
            result.Value = double.Parse(value.Replace(".", ","));
        }
        else if (context.NULL() is not null)
        {
            result.DataType = EDataType.Null;
            result.Value = null;
        }
        
        return result;
    }
}