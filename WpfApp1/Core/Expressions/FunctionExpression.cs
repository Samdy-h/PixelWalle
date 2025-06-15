using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;

namespace PixelWallE.Core.Expressions
{
    public class FunctionExpression : IPixelExpression
    {
        private readonly string _functionName;
        private readonly List<IPixelExpression> _arguments;

        public FunctionExpression(string functionName, List<IPixelExpression> arguments)
        {
            _functionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public object Evaluate(RuntimeState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            // Evaluar todos los argumentos
            object[] evaluatedArgs = new object[_arguments.Count];
            for (int i = 0; i < _arguments.Count; i++)
            {
                evaluatedArgs[i] = _arguments[i].Evaluate(state);
            }

            // Llamar a la función en el estado
            return state.CallFunction(_functionName, evaluatedArgs);
        }
    }
}