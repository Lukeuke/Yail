using Yail.Shared;

namespace Yail.Common;

public static class OperationsHelper
{
    public static ValueObj Add(ValueObj left, ValueObj right)
    {
        if (left.DataType == EDataType.String || right.DataType == EDataType.String)
        {
            // String concatenation
            return new ValueObj
            {
                IsConst = false,
                Value = left.Value + right.Value.ToString(),
                DataType = EDataType.String
            };
        }

        if (left.DataType == EDataType.Integer && right.DataType == EDataType.Integer)
        {
            // Integer addition
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value + (int)right.Value,
                DataType = EDataType.Integer
            };
        }

        if (left.DataType == EDataType.Double || right.DataType == EDataType.Double)
        {
            // Double addition (convert Integer to Double if needed)
            var leftValue = left.DataType == EDataType.Double ? (double)left.Value : Convert.ToDouble(left.Value);
            var rightValue = right.DataType == EDataType.Double ? (double)right.Value : Convert.ToDouble(right.Value);

            return new ValueObj
            {
                IsConst = false,
                Value = leftValue + rightValue,
                DataType = EDataType.Double
            };
        }

        if (left.DataType == EDataType.Char && right.DataType == EDataType.Char)
        {
            // Add character codes
            return new ValueObj
            {
                IsConst = false,
                Value = (char)((char)left.Value + (char)right.Value),
                DataType = EDataType.Char
            };
        }

        if (left.DataType == EDataType.Boolean || right.DataType == EDataType.Boolean)
        {
            throw new InvalidOperationException("Addition is not supported for boolean types.");
        }

        throw new InvalidOperationException($"Unsupported data types for addition: {left.DataType} and {right.DataType}");
    }
    
    public static ValueObj? Subtract(ValueObj left, ValueObj right)
    {
        if (left.DataType == EDataType.Integer && right.DataType == EDataType.Integer)
        {
            // Integer subtraction
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value - (int)right.Value,
                DataType = EDataType.Integer
            };
        }

        if (left.DataType == EDataType.Double || right.DataType == EDataType.Double)
        {
            // Double subtraction (convert Integer to Double if needed)
            var leftValue = left.DataType == EDataType.Double ? (double)left.Value : Convert.ToDouble(left.Value);
            var rightValue = right.DataType == EDataType.Double ? (double)right.Value : Convert.ToDouble(right.Value);

            return new ValueObj
            {
                IsConst = false,
                Value = leftValue - rightValue,
                DataType = EDataType.Double
            };
        }

        if (left.DataType == EDataType.Char && right.DataType == EDataType.Char)
        {
            // Subtract character codes
            return new ValueObj
            {
                IsConst = false,
                Value = (char)((char)left.Value - (char)right.Value),
                DataType = EDataType.Char
            };
        }

        if (left.DataType == EDataType.Boolean || right.DataType == EDataType.Boolean || left.DataType == EDataType.String || right.DataType == EDataType.String)
        {
            throw new InvalidOperationException("Subtraction is not supported for these types.");
        }

        throw new InvalidOperationException($"Unsupported data types for subtraction: {left.DataType} and {right.DataType}");
    }
    
    public static ValueObj Multiply(ValueObj left, ValueObj right)
    {
        if (left.Value == null || right.Value == null)
            throw new InvalidOperationException("Cannot multiply null values.");

        if (left.DataType == EDataType.Integer && right.DataType == EDataType.Integer)
        {
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value * (int)right.Value,
                DataType = EDataType.Integer
            };
        }

        if (left.DataType == EDataType.Double || right.DataType == EDataType.Double)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return new ValueObj
            {
                IsConst = false,
                Value = leftValue * rightValue,
                DataType = EDataType.Double
            };
        }

        throw new InvalidOperationException("Multiplication is not supported for these data types.");
    }
    
    public static ValueObj Divide(ValueObj left, ValueObj right)
    {
        if (left.Value == null || right.Value == null)
            throw new InvalidOperationException("Cannot divide null values.");

        if (right.DataType == EDataType.Integer && (int)right.Value == 0 || 
            right.DataType == EDataType.Double && (double)right.Value == 0.0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        if (left.DataType == EDataType.Integer && right.DataType == EDataType.Integer)
        {
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value / (int)right.Value,
                DataType = EDataType.Integer
            };
        }

        if (left.DataType == EDataType.Double || right.DataType == EDataType.Double)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return new ValueObj
            {
                IsConst = false,
                Value = leftValue / rightValue,
                DataType = EDataType.Double
            };
        }

        throw new InvalidOperationException("Division is not supported for these data types.");
    }
    
    public static ValueObj Modulo(ValueObj left, ValueObj right)
    {
        if (left.Value == null || right.Value == null)
            throw new InvalidOperationException("Cannot perform modulo on null values.");

        if (right.DataType == EDataType.Integer && (int)right.Value == 0)
        {
            throw new DivideByZeroException("Cannot modulo by zero.");
        }

        if (left.DataType == EDataType.Integer && right.DataType == EDataType.Integer)
        {
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value % (int)right.Value,
                DataType = EDataType.Integer
            };
        }

        throw new InvalidOperationException("Modulo is only supported for integer types.");
    }
}