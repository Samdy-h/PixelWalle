using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class DrawCircleCommand : IPixelCommand
    {
        public string Name => "DrawCircle";

        private IPixelExpression _dirXExpr = null!;
        private IPixelExpression _dirYExpr = null!;
        private IPixelExpression _radiusExpr = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 3)
                throw new SyntaxException("DrawCircle command requires exactly 3 parameters (dirX, dirY, radius)");

            _dirXExpr = syntax.Parameters[0].Expression ?? throw new SyntaxException("Expresión inválida para dirX");
            _dirYExpr = syntax.Parameters[1].Expression ?? throw new SyntaxException("Expresión inválida para dirY");
            _radiusExpr = syntax.Parameters[2].Expression ?? throw new SyntaxException("Expresión inválida para radius");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before drawing");

            if (state.CurrentColor == "Transparent")
                return;

            var dirXObj = _dirXExpr.Evaluate(state);
            var dirYObj = _dirYExpr.Evaluate(state);
            var radiusObj = _radiusExpr.Evaluate(state);

            if (!(dirXObj is int dirX) || !(dirYObj is int dirY) || !(radiusObj is int radius))
                throw new ExecutionException("DrawCircle parameters must be integers");

            if (dirX < -1 || dirX > 1 || dirY < -1 || dirY > 1)
                throw new ExecutionException("DrawCircle direction parameters must be -1, 0, or 1");

            if (radius <= 0)
                throw new ExecutionException("DrawCircle radius must be positive");

            Point center = new Point(
                state.WallEPosition.X + dirX * radius,
                state.WallEPosition.Y + dirY * radius
            );

            if (!IsPointInCanvas(center, state.CanvasSize))
                throw new ExecutionException("Circle center is out of canvas bounds");

            DrawCircle(state, center, radius);
            state.WallEPosition = center;
        }

        private bool IsPointInCanvas(Point point, int canvasSize)
        {
            return point.X >= 0 && point.X < canvasSize &&
                   point.Y >= 0 && point.Y < canvasSize;
        }

        private void DrawCircle(RuntimeState state, Point center, int radius)
        {
            int cx = (int)Math.Round(center.X);
            int cy = (int)Math.Round(center.Y);
            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            DrawCirclePoints(state, cx, cy, x, y);

            while (y >= x)
            {
                x++;
                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                {
                    d = d + 4 * x + 6;
                }
                DrawCirclePoints(state, cx, cy, x, y);
            }
        }

        private void DrawCirclePoints(RuntimeState state, int cx, int cy, int x, int y)
        {
            DrawBrushAt(state, cx + x, cy + y);
            DrawBrushAt(state, cx - x, cy + y);
            DrawBrushAt(state, cx + x, cy - y);
            DrawBrushAt(state, cx - x, cy - y);
            DrawBrushAt(state, cx + y, cy + x);
            DrawBrushAt(state, cx - y, cy + x);
            DrawBrushAt(state, cx + y, cy - x);
            DrawBrushAt(state, cx - y, cy - x);
        }

        private void DrawBrushAt(RuntimeState state, int x, int y)
        {
            int brushSize = state.BrushSize;
            int halfBrush = brushSize / 2;

            for (int i = -halfBrush; i <= halfBrush; i++)
            {
                for (int j = -halfBrush; j <= halfBrush; j++)
                {
                    int px = x + i;
                    int py = y + j;
                    if (px >= 0 && px < state.CanvasSize && py >= 0 && py < state.CanvasSize)
                    {
                        state.SetPixel(px, py, state.CurrentColor);
                    }
                }
            }
        }
    }
}