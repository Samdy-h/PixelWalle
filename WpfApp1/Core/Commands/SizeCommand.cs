// Core/Commands/SizeCommand.cs
using PixelWallE.Core.Runtime;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Exceptions;


namespace PixelWallE.Core.Commands
{
    public class SizeCommand : IPixelCommand
    {
        public string Name => "Size";

        private int _size;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 1)
                throw new SyntaxException("Size command requires exactly 1 parameter");

            if (!syntax.Parameters[0].IsInteger)
                throw new SyntaxException("Size parameter must be an integer");

            _size = syntax.Parameters[0].GetInteger();

            if (_size <= 0)
                throw new SyntaxException("Brush size must be greater than 0");
        }

        public void Execute(RuntimeState state)
        {
            // Ajustar a número impar si es par
            _size = _size % 2 == 0 ? _size - 1 : _size;
            state.BrushSize = _size;
        }
    }
}