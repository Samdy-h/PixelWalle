using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Runtime;
using System;

namespace PixelWallE.Core.Expressions
{
    public class VariableExpression : IPixelExpression
    {
        private readonly string _name;

        public VariableExpression(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public object Evaluate(RuntimeState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (state.Variables.TryGetValue(_name, out object? value))
            {
                // Devolver 0 si es nulo en lugar de lanzar excepción
                return value ?? 0;
            }

            throw new ExecutionException($"Variable '{_name}' no definida");
        }
    }
}