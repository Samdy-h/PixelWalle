using PixelWallE.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE.Core.Expressions
{
    public static class ExpressionParser
    {
        private static readonly Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int>
        {
            // Operadores lógicos (OR tiene mayor precedencia que AND)
            {"||", 1},
            {"&&", 2},
            
            // Operadores de comparación
            {"==", 3}, { "!=", 3 },
            { "<", 3 }, { ">", 3 }, { "<=", 3 }, { ">=", 3 },
            
            // Operadores aritméticos
            {"+", 4}, {"-", 4},
            {"*", 5}, {"/", 5}, {"%", 5},
            {"**", 6}  // Mayor precedencia
        };

        public static IPixelExpression Parse(string expression)
        {
            expression = expression.Trim();

            // Manejar paréntesis
            if (expression.StartsWith("(") && expression.EndsWith(")"))
            {
                return Parse(expression.Substring(1, expression.Length - 2));
            }

            // Manejar funciones
            int openParen = expression.IndexOf('(');
            if (openParen > 0 && expression.EndsWith(")"))
            {
                string funcName = expression.Substring(0, openParen).Trim();
                string argsPart = expression.Substring(openParen + 1, expression.Length - openParen - 2);
                var args = ParseArguments(argsPart);
                return new FunctionExpression(funcName, args);
            }

            // Buscar operadores con menor precedencia
            int parenCount = 0;
            int lowestPrecedence = int.MaxValue;
            int operatorIndex = -1;
            string currentOperator = null!;

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];
                if (c == '(') parenCount++;
                else if (c == ')') parenCount--;

                if (parenCount == 0)
                {
                    // Verificar operadores de 2 caracteres primero
                    if (i + 1 < expression.Length)
                    {
                        string twoCharOp = expression.Substring(i, 2);
                        if (OperatorPrecedence.ContainsKey(twoCharOp))
                        {
                            int precedence = OperatorPrecedence[twoCharOp];
                            if (precedence <= lowestPrecedence)
                            {
                                lowestPrecedence = precedence;
                                operatorIndex = i;
                                currentOperator = twoCharOp;
                                i++; // Saltar el siguiente carácter
                                continue;
                            }
                        }
                    }

                    // Verificar operadores de 1 carácter
                    string oneCharOp = expression[i].ToString();
                    if (OperatorPrecedence.ContainsKey(oneCharOp))
                    {
                        int precedence = OperatorPrecedence[oneCharOp];
                        if (precedence <= lowestPrecedence)
                        {
                            lowestPrecedence = precedence;
                            operatorIndex = i;
                            currentOperator = oneCharOp;
                        }
                    }
                }
            }

            // Si encontramos un operador
            if (operatorIndex != -1)
            {
                string leftPart = expression.Substring(0, operatorIndex).Trim();
                string rightPart = expression.Substring(operatorIndex + currentOperator.Length).Trim();

                return new BinaryExpression(
                    Parse(leftPart),
                    Parse(rightPart),
                    currentOperator
                );
            }

            // Manejar valores booleanos
            if (expression.Equals("true", StringComparison.OrdinalIgnoreCase))
                return new LiteralExpression(1);

            if (expression.Equals("false", StringComparison.OrdinalIgnoreCase))
                return new LiteralExpression(0);

            // Manejar números enteros
            if (int.TryParse(expression, out int intValue))
            {
                return new LiteralExpression(intValue);
            }

            // Manejar números decimales
            if (double.TryParse(expression, out double doubleValue))
            {
                return new LiteralExpression(doubleValue);
            }

            // Manejar variables
            return new VariableExpression(expression);
        }

        private static List<IPixelExpression> ParseArguments(string args)
        {
            var arguments = new List<IPixelExpression>();
            if (string.IsNullOrWhiteSpace(args))
                return arguments;

            int start = 0;
            int parenCount = 0;
            for (int i = 0; i < args.Length; i++)
            {
                char c = args[i];
                if (c == '(') parenCount++;
                else if (c == ')') parenCount--;
                else if (c == ',' && parenCount == 0)
                {
                    arguments.Add(Parse(args.Substring(start, i - start)));
                    start = i + 1;
                }
            }
            arguments.Add(Parse(args.Substring(start)));

            return arguments;
        }
    }
}