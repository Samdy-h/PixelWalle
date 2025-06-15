using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;

namespace PixelWallE.Core.Commands
{
    public class SizeCommand : IPixelCommand
    {
        public string Name => "Size";

        private IPixelExpression _sizeExpression = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 1)
                throw new SyntaxException("Size command requires exactly 1 parameter");

            _sizeExpression = syntax.Parameters[0].Expression ?? throw new SyntaxException("Expresión inválida");
        }

        public void Execute(RuntimeState state)
        {
            var sizeObj = _sizeExpression.Evaluate(state);
            if (!(sizeObj is int size))
                throw new ExecutionException("Size parameter must be an integer");

            if (size <= 0)
                throw new ExecutionException("Brush size must be greater than 0");

            size = size % 2 == 0 ? size - 1 : size;
            state.BrushSize = size;
        }
    }
}