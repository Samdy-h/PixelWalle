using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class DrawRectangleCommand : IPixelCommand
    {
        public string Name => "DrawRectangle";

        private IPixelExpression _dirXExpr = null!;
        private IPixelExpression _dirYExpr = null!;
        private IPixelExpression _distanceExpr = null!;
        private IPixelExpression _widthExpr = null!;
        private IPixelExpression _heightExpr = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 5)
                throw new SyntaxException("DrawRectangle command requires exactly 5 parameters (dirX, dirY, distance, width, height)");

            _dirXExpr = syntax.Parameters[0].Expression ?? throw new SyntaxException("Expresión inválida para dirX");
            _dirYExpr = syntax.Parameters[1].Expression ?? throw new SyntaxException("Expresión inválida para dirY");
            _distanceExpr = syntax.Parameters[2].Expression ?? throw new SyntaxException("Expresión inválida para distance");
            _widthExpr = syntax.Parameters[3].Expression ?? throw new SyntaxException("Expresión inválida para width");
            _heightExpr = syntax.Parameters[4].Expression ?? throw new SyntaxException("Expresión inválida para height");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before drawing");

            if (state.CurrentColor == "Transparent")
                return;

            // Evaluar expresiones
            var dirXObj = _dirXExpr.Evaluate(state);
            var dirYObj = _dirYExpr.Evaluate(state);
            var distanceObj = _distanceExpr.Evaluate(state);
            var widthObj = _widthExpr.Evaluate(state);
            var heightObj = _heightExpr.Evaluate(state);

            if (!(dirXObj is int dirX) || !(dirYObj is int dirY) ||
                !(distanceObj is int distance) || !(widthObj is int width) || !(heightObj is int height))
            {
                throw new ExecutionException("DrawRectangle parameters must be integers");
            }

            if (dirX < -1 || dirX > 1 || dirY < -1 || dirY > 1)
                throw new ExecutionException("DrawRectangle direction parameters must be -1, 0, or 1");

            if (distance < 0)
                throw new ExecutionException("DrawRectangle distance must be non-negative");

            if (width <= 0 || height <= 0)
                throw new ExecutionException("DrawRectangle width and height must be positive");

            Point center = new Point(
                state.WallEPosition.X + dirX * distance,
                state.WallEPosition.Y + dirY * distance
            );

            if (!IsPointInCanvas(center, state.CanvasSize))
                throw new ExecutionException("Rectangle center is out of canvas bounds");

            int left = (int)Math.Round(center.X - width / 2.0);
            int top = (int)Math.Round(center.Y - height / 2.0);
            int right = left + width;
            int bottom = top + height;

            if (left < 0 || top < 0 || right >= state.CanvasSize || bottom >= state.CanvasSize)
                throw new ExecutionException("Rectangle is out of canvas bounds");

            DrawLine(state, new Point(left, top), new Point(right, top));
            DrawLine(state, new Point(left, bottom), new Point(right, bottom));
            DrawLine(state, new Point(left, top), new Point(left, bottom));
            DrawLine(state, new Point(right, top), new Point(right, bottom));

            state.WallEPosition = center;
        }

        private bool IsPointInCanvas(Point point, int canvasSize)
        {
            return point.X >= 0 && point.X < canvasSize &&
                   point.Y >= 0 && point.Y < canvasSize;
        }

        private void DrawLine(RuntimeState state, Point start, Point end)
        {
            int x0 = (int)Math.Round(start.X);
            int y0 = (int)Math.Round(start.Y);
            int x1 = (int)Math.Round(end.X);
            int y1 = (int)Math.Round(end.Y);

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            int x = x0;
            int y = y0;

            int brushSize = state.BrushSize;
            int halfBrush = brushSize / 2;

            while (true)
            {
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

                if (x == x1 && y == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }
    }
}