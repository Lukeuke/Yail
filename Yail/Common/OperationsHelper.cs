using Yail.Shared;
using Yail.Shared.Objects;

namespace Yail.Common;

public static class OperationsHelper
{
    public static ValueObj Add(ValueObj left, ValueObj right)
    {
        if (left is ArrayObj leftArr && right is ArrayObj rightArr)
        {
            return leftArr + rightArr;
        }

        if (left is ArrayObj || right is ArrayObj)
        {
            return ScalarAdd(left, right);
        }
        
        if (left.DataType == EDataType.String || right.DataType == EDataType.String || right.DataType == EDataType.Char)
        {
            // String concatenation
            return new ValueObj
            {
                IsConst = false,
                Value = left.Value + right.Value.ToString(),
                DataType = EDataType.String
            };
        }

        if (left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32)
        {
            // Int32 addition
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value + (int)right.Value,
                DataType = EDataType.Int32
            };
        }

        if (left.DataType == EDataType.Double || right.DataType == EDataType.Double)
        {
            // Double addition (convert Int32 to Double if needed)
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
    
    public static ValueObj Subtract(ValueObj left, ValueObj right)
    {
        if (left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32)
        {
            // Int32 subtraction
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value - (int)right.Value,
                DataType = EDataType.Int32
            };
        }

        if (left.DataType == EDataType.Double || right.DataType == EDataType.Double)
        {
            // Double subtraction (convert Int32 to Double if needed)
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

        if (left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32)
        {
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value * (int)right.Value,
                DataType = EDataType.Int32
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

        if (right.DataType == EDataType.Int32 && (int)right.Value == 0 || 
            right.DataType == EDataType.Double && (double)right.Value == 0.0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        if (left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32)
        {
            var a =  new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value / (double)(int)right.Value, 
                DataType = EDataType.Double
            };
            return a;
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

        if (right.DataType == EDataType.Int32 && (int)right.Value == 0)
        {
            throw new DivideByZeroException("Cannot modulo by zero.");
        }

        if (left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32)
        {
            return new ValueObj
            {
                IsConst = false,
                Value = (int)left.Value % (int)right.Value,
                DataType = EDataType.Int32
            };
        }

        throw new InvalidOperationException("Modulo is only supported for integer types.");
    }
    
    public static int Compare(ValueObj left, ValueObj right)
    {
        if (left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32)
        {
            return ((int)left.Value).CompareTo((int)right.Value);
        }
        if (left.DataType == EDataType.Double && right.DataType == EDataType.Double)
        {
            return ((double)left.Value).CompareTo((double)right.Value);
        }
        if (left.DataType == EDataType.String && right.DataType == EDataType.String)
        {
            return ((string)left.Value).CompareTo((string)right.Value);
        }

        throw new InvalidOperationException("Cannot compare values of different or unsupported types.");
    }

    public static ValueObj Power(ValueObj left, ValueObj right)
    {
        if (left.DataType != EDataType.Int32 && left.DataType != EDataType.Double)
        {
            throw new InvalidOperationException("Power operation is only supported for integers and doubles.");
        }

        if (right.DataType != EDataType.Int32 && right.DataType != EDataType.Double)
        {
            throw new InvalidOperationException("Exponent must be an integer or a double.");
        }

        var baseValue = Convert.ToDouble(left.Value);
        var exponentValue = Convert.ToDouble(right.Value);

        var result = Math.Pow(baseValue, exponentValue);

        return new ValueObj
        {
            DataType = left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32
                ? EDataType.Int32
                : EDataType.Double,
            Value = left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32
                ? (object)(int)result
                : result
        };
    }

    public static ValueObj FloorDivide(ValueObj left, ValueObj right)
    {
        if (left.DataType != EDataType.Int32 && left.DataType != EDataType.Double)
        {
            throw new InvalidOperationException("Floor division is only supported for integers and doubles.");
        }

        if (right.DataType != EDataType.Int32 && right.DataType != EDataType.Double)
        {
            throw new InvalidOperationException("Divisor must be an integer or a double.");
        }

        var dividend = Convert.ToDouble(left.Value);
        var divisor = Convert.ToDouble(right.Value);

        if (Math.Abs(divisor) < double.Epsilon)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        var result = Math.Floor(dividend / divisor);

        return new ValueObj
        {
            DataType = left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32
                ? EDataType.Int32
                : EDataType.Double,
            Value = left.DataType == EDataType.Int32 && right.DataType == EDataType.Int32
                ? (object)(int)result
                : result
        };
    }
    
    public static object PerformOperation(ValueObj lhs, ValueObj rhs, string operatorText)
    {
        var lhsValue = lhs.Value!;
        var rhsValue = rhs.Value!;

        return (lhs.DataType, rhs.DataType, operatorText) switch
        {
            (EDataType.Int32, EDataType.Int32, "xor") => (int)lhsValue ^ (int)rhsValue,

            (EDataType.Int32, EDataType.Int32, "and") => ((int)lhsValue != 0 && (int)rhsValue != 0) ? rhsValue : 0,

            (EDataType.Int32, EDataType.Int32, "or") => ((int)lhsValue != 0 || (int)rhsValue != 0) ? rhsValue : 0,

            (EDataType.Boolean, EDataType.Boolean, "and") => (bool)lhsValue && (bool)rhsValue,

            (EDataType.Boolean, EDataType.Boolean, "or") => (bool)lhsValue || (bool)rhsValue,

            (EDataType.Boolean, EDataType.Boolean, "xor") => (bool)lhsValue ^ (bool)rhsValue,

            _ => throw new InvalidOperationException($"Unsupported operation for types {lhs.DataType} and {rhs.DataType}")
        };
    }

    public static ArrayObj ScalarAdd(ValueObj left, ValueObj right)
    {
        var output = new ArrayObj(new List<ValueObj>());
        
        // pre add
        if (left.DataType == EDataType.Int32 && right is ArrayObj rightArr)
        {
            var val1 = left.GetValue<int>();
            var val2 = rightArr.GetValue<List<ValueObj>>();

            foreach (var x in val2)
            {
                output.Push(new ValueObj(val1 + x.GetValue<int>(), EDataType.Int32));
            }
        }
        // post add
        else if (left is ArrayObj leftArr && right.DataType == EDataType.Int32)
        {
            var val1 = right.GetValue<int>();
            var val2 = leftArr.GetValue<List<ValueObj>>();

            foreach (var x in val2)
            {
                output.Push(new ValueObj(x.GetValue<int>() + val1, EDataType.Int32));
            }
        }
        // pre add
        else if (right is ArrayObj rightArr2 && rightArr2.Get().First().DataType == EDataType.String)
        {
            var val = rightArr2.GetValue<List<ValueObj>>();
            
            foreach (var x in val)
            {
                output.Push(new ValueObj(left.Value + x.GetValue<string>(), EDataType.String));
            }
        }
        // post add
        else if (left is ArrayObj leftArr2 && leftArr2.Get().First().DataType == EDataType.String)
        {
            var val = leftArr2.GetValue<List<ValueObj>>();
            
            foreach (var x in val)
            {
                output.Push(new ValueObj(x.GetValue<string>() + right.Value, EDataType.String));
            }
        }
        
        return output;
    }
}