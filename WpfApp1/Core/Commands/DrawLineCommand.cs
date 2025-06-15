using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class DrawLineCommand : IPixelCommand
    {
        public string Name => "DrawLine";

        private IPixelExpression _dirXExpr = null!;
        private IPixelExpression _dirYExpr = null!;
        private IPixelExpression _distanceExpr = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 3)
                throw new SyntaxException("DrawLine command requires exactly 3 parameters (dirX, dirY, distance)");

            _dirXExpr = syntax.Parameters[0].Expression ?? throw new SyntaxException("Expresión inválida para dirX");
            _dirYExpr = syntax.Parameters[1].Expression ?? throw new SyntaxException("Expresión inválida para dirY");
            _distanceExpr = syntax.Parameters[2].Expression ?? throw new SyntaxException("Expresión inválida para distance");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before drawing");

            if (state.CurrentColor == "Transparent")
                return;

            try
            {
                // Evaluar expresiones y manejar posibles nulos
                object? dirXVal = _dirXExpr.Evaluate(state);
                object? dirYVal = _dirYExpr.Evaluate(state);
                object? distanceVal = _distanceExpr.Evaluate(state);

                // Convertir a enteros usando el método seguro
                int dirX = state.ConvertToInt(dirXVal);
                int dirY = state.ConvertToInt(dirYVal);
                int distance = state.ConvertToInt(distanceVal);

                if (dirX < -1 || dirX > 1 || dirY < -1 || dirY > 1)
                    throw new ExecutionException("DrawLine direction parameters must be -1, 0, or 1");

                if (distance < 0)
                    throw new ExecutionException("DrawLine distance must be non-negative");

                Point start = state.WallEPosition;
                Point end = new Point(
                    start.X + dirX * distance,
                    start.Y + dirY * distance
                );

                if (!IsPointInCanvas(start, state.CanvasSize) || !IsPointInCanvas(end, state.CanvasSize))
                    throw new ExecutionException("Line is out of canvas bounds");

                DrawLine(state, start, end);
                state.WallEPosition = end;
            }
            catch (ExecutionException ex)
            {
                throw new ExecutionException($"Error en DrawLine: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ExecutionException($"Error inesperado en DrawLine: {ex.Message}");
            }
        }

        private bool IsPointInCanvas(Point point, int canvasSize)
        {
            return point.X >= 0 && point.X < canvasSize &&
                   point.Y >= 0 && point.Y < canvasSize;
        }

        private void DrawLine(RuntimeState state, Point start, Point end)
        {
            // Convertir a enteros para el algoritmo
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