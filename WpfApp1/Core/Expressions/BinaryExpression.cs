using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Runtime;
using System;

namespace PixelWallE.Core.Expressions
{
    public class BinaryExpression : IPixelExpression
    {
        private readonly IPixelExpression _left;
        private readonly IPixelExpression _right;
        private readonly string _operator;

        public BinaryExpression(IPixelExpression left, IPixelExpression right, string op)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
            _operator = op ?? throw new ArgumentNullException(nameof(op));
        }

        public object Evaluate(RuntimeState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            var leftVal = _left.Evaluate(state);
            var rightVal = _right.Evaluate(state);

            // Validar valores nulos
            if (leftVal == null) throw new ExecutionException("Operando izquierdo es nulo");
            if (rightVal == null) throw new ExecutionException("Operando derecho es nulo");

            try
            {
                // Operadores lógicos
                if (_operator == "&&" || _operator == "||")
                {
                    bool leftBool = IsTrue(leftVal);
                    bool rightBool = IsTrue(rightVal);

                    return _operator switch
                    {
                        "&&" => leftBool && rightBool ? 1 : 0,
                        "||" => leftBool || rightBool ? 1 : 0,
                        _ => throw new ExecutionException($"Operador lógico no soportado: {_operator}")
                    };
                }

                // Operadores de comparación
                if (_operator == "==" || _operator == "!=" ||
                    _operator == "<" || _operator == ">" ||
                    _operator == "<=" || _operator == ">=")
                {
                    // Manejo especial para strings
                    if (leftVal is string || rightVal is string)
                    {
                        string leftStr = leftVal.ToString() ?? "";
                        string rightStr = rightVal.ToString() ?? "";

                        return _operator switch
                        {
                            "==" => leftStr == rightStr ? 1 : 0,
                            "!=" => leftStr != rightStr ? 1 : 0,
                            _ => throw new ExecutionException($"Operador no válido para strings: {_operator}")
                        };
                    }

                    // Para otros tipos, convertimos a double
                    double leftNum = state.ConvertToDouble(leftVal);
                    double rightNum = state.ConvertToDouble(rightVal);

                    return _operator switch
                    {
                        "==" => leftNum == rightNum ? 1 : 0,
                        "!=" => leftNum != rightNum ? 1 : 0,
                        "<" => leftNum < rightNum ? 1 : 0,
                        ">" => leftNum > rightNum ? 1 : 0,
                        "<=" => leftNum <= rightNum ? 1 : 0,
                        ">=" => leftNum >= rightNum ? 1 : 0,
                        _ => throw new ExecutionException($"Operador de comparación no soportado: {_operator}")
                    };
                }

                // Operadores aritméticos
                double leftNumArith = state.ConvertToDouble(leftVal);
                double rightNumArith = state.ConvertToDouble(rightVal);

                return _operator switch
                {
                    "+" => leftNumArith + rightNumArith,
                    "-" => leftNumArith - rightNumArith,
                    "*" => leftNumArith * rightNumArith,
                    "/" => rightNumArith == 0 ? throw new ExecutionException("División por cero") : leftNumArith / rightNumArith,
                    "%" => leftNumArith % rightNumArith,
                    "**" => Math.Pow(leftNumArith, rightNumArith),
                    _ => throw new ExecutionException($"Operador no soportado: {_operator}")
                };
            }
            catch (Exception ex)
            {
                throw new ExecutionException($"Error en operación: {leftVal} {_operator} {rightVal} - {ex.Message}");
            }
        }

        private bool IsTrue(object value)
        {
            if (value is int i) return i != 0;
            if (value is double d) return d != 0;
            if (value is float f) return f != 0;
            if (value is bool b) return b;
            if (value is string s) return !string.IsNullOrEmpty(s);
            return value != null;
        }
    }
}