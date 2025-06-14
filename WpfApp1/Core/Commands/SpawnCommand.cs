using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System.Windows; // Para Point

namespace PixelWallE.Core.Commands
{
    public class SpawnCommand : IPixelCommand
    {
        public string Name => "Spawn";

        private int _x;
        private int _y;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 2)
                throw new SyntaxException("Spawn command requires exactly 2 parameters (x, y)");

            if (!syntax.Parameters[0].IsInteger || !syntax.Parameters[1].IsInteger)
                throw new SyntaxException("Spawn parameters must be integers");

            _x = syntax.Parameters[0].GetInteger();
            _y = syntax.Parameters[1].GetInteger();
        }

        public void Execute(RuntimeState state)
        {
            if (state.HasSpawned)
                throw new ExecutionException("Spawn command can only be used once");

            if (_x < 0 || _y < 0 || _x >= state.CanvasSize || _y >= state.CanvasSize)
                throw new ExecutionException($"Spawn coordinates ({_x}, {_y}) are outside canvas bounds");

            state.WallEPosition = new Point(_x, _y);
            state.HasSpawned = true;
        }
    }
}