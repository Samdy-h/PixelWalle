using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;

namespace PixelWallE.Core.Commands
{
    public class AssignmentCommand : IPixelCommand
    {
        public string Name => "<-";

        private string _variableName = null!;
        private IPixelExpression _expression = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 2)
                throw new SyntaxException("Asignación requiere 2 parámetros (variable y expresión)");

            if (!syntax.Parameters[0].IsString)
                throw new SyntaxException("El nombre de variable debe ser una cadena");

            _variableName = syntax.Parameters[0].GetString() ?? throw new SyntaxException("Nombre de variable inválido");
            _expression = syntax.Parameters[1].Expression ?? throw new SyntaxException("Expresión inválida");
        }

        public void Execute(RuntimeState state)
        {
            // Validar nombre de variable
            if (string.IsNullOrWhiteSpace(_variableName))
                throw new ExecutionException("Nombre de variable inválido");

            var value = _expression.Evaluate(state);

            // Manejar valores nulos (depende de si quieres permitir nulls)
            state.Variables[_variableName] = value ?? 0; // O asigna un valor por defecto

            // O si no quieres permitir nulls:
            // if (value == null) throw new ExecutionException("Valor nulo no permitido");
            // state.Variables[_variableName] = value;
        }
    }
}