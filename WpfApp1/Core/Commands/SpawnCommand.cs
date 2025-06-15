using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class SpawnCommand : IPixelCommand
    {
        public string Name => "Spawn";

        private IPixelExpression _xExpr = null!;
        private IPixelExpression _yExpr = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 2)
                throw new SyntaxException("Spawn command requires exactly 2 parameters (x, y)");

            _xExpr = syntax.Parameters[0].Expression ?? throw new SyntaxException("Invalid expression for x");
            _yExpr = syntax.Parameters[1].Expression ?? throw new SyntaxException("Invalid expression for y");
        }

        public void Execute(RuntimeState state)
        {
            // Verificar si ya se hizo un Spawn
            if (state.HasSpawned)
                throw new ExecutionException("Spawn command can only be used once");

            // Evaluar las expresiones
            var xObj = _xExpr.Evaluate(state);
            var yObj = _yExpr.Evaluate(state);

            if (!(xObj is int x) || !(yObj is int y))
                throw new ExecutionException("Spawn parameters must be integers");

            // Validar coordenadas dentro del canvas
            if (x < 0 || x >= state.CanvasSize || y < 0 || y >= state.CanvasSize)
                throw new ExecutionException($"Spawn coordinates ({x}, {y}) are outside canvas bounds");

            // Establecer posición y marcar como spawned
            state.WallEPosition = new Point(x, y);
            state.HasSpawned = true;
        }
    }
}