using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Windows; // Para Point

namespace PixelWallE.Core.Commands
{
    public class DrawLineCommand : IPixelCommand
    {
        public string Name => "DrawLine";

        private int _dirX;
        private int _dirY;
        private int _distance;

        private static readonly HashSet<int> ValidDirections = new HashSet<int> { -1, 0, 1 };

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 3)
                throw new SyntaxException("DrawLine command requires exactly 3 parameters (dirX, dirY, distance)");

            if (!syntax.Parameters[0].IsInteger || !syntax.Parameters[1].IsInteger || !syntax.Parameters[2].IsInteger)
                throw new SyntaxException("DrawLine parameters must be integers");

            _dirX = syntax.Parameters[0].GetInteger();
            _dirY = syntax.Parameters[1].GetInteger();
            _distance = syntax.Parameters[2].GetInteger();

            if (!ValidDirections.Contains(_dirX) || !ValidDirections.Contains(_dirY))
                throw new SyntaxException("DrawLine direction parameters must be -1, 0, or 1");

            if (_distance < 0)
                throw new SyntaxException("DrawLine distance must be non-negative");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before drawing");

            if (state.CurrentColor == "Transparent")
                return; // No dibujar si el color es transparente

            Point start = state.WallEPosition;
            Point end = new Point(
                start.X + _dirX * _distance,
                start.Y + _dirY * _distance
            );

            // Validar que la línea esté dentro del canvas
            if (!IsPointInCanvas(start, state.CanvasSize) || !IsPointInCanvas(end, state.CanvasSize))
                throw new ExecutionException("Line is out of canvas bounds");

            // Dibujar la línea
            DrawLine(state, start, end);

            // Actualizar la posición de Wall-E
            state.WallEPosition = end;
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
                // Pintar un cuadrado centrado en (x, y) con el tamaño del pincel
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