using PixelWallE.Core.Expressions;
using PixelWallE.Core.Runtime;
using System;

namespace PixelWallE.Core.Parsing
{
    public class CommandParameter
    {
        public object? Value { get; }
        public bool IsInteger { get; }
        public bool IsString { get; }
        public IPixelExpression Expression { get; }

        public CommandParameter(object value)
        {
            if (value is int intValue)
            {
                Value = intValue;
                IsInteger = true;
                IsString = false;
                Expression = new LiteralExpression(intValue);
            }
            else if (value is string stringValue)
            {
                Value = stringValue;
                IsInteger = false;
                IsString = true;
                Expression = new LiteralExpression(stringValue);
            }
            else
            {
                throw new ArgumentException("Unsupported parameter type");
            }
        }

        public CommandParameter(IPixelExpression expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Value = expression; // Solo para inicialización

            // No intentar evaluar aquí, se evaluará durante la ejecución
            IsInteger = false;
            IsString = false;
        }

        public int GetInteger() => IsInteger ? (int)Value! : throw new InvalidOperationException("Parameter is not an integer");
        public string GetString() => IsString ? (string)Value! : throw new InvalidOperationException("Parameter is not a string");
    }
}