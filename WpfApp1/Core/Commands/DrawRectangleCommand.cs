// Core/Commands/DrawRectangleCommand.cs
using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class DrawRectangleCommand : IPixelCommand
    {
        public string Name => "DrawRectangle";

        private int _dirX;
        private int _dirY;
        private int _distance;
        private int _width;
        private int _height;

        private static readonly HashSet<int> ValidDirections = new HashSet<int> { -1, 0, 1 };

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 5)
                throw new SyntaxException("DrawRectangle command requires exactly 5 parameters (dirX, dirY, distance, width, height)");

            if (!syntax.Parameters[0].IsInteger || !syntax.Parameters[1].IsInteger ||
                !syntax.Parameters[2].IsInteger || !syntax.Parameters[3].IsInteger ||
                !syntax.Parameters[4].IsInteger)
                throw new SyntaxException("DrawRectangle parameters must be integers");

            _dirX = syntax.Parameters[0].GetInteger();
            _dirY = syntax.Parameters[1].GetInteger();
            _distance = syntax.Parameters[2].GetInteger();
            _width = syntax.Parameters[3].GetInteger();
            _height = syntax.Parameters[4].GetInteger();

            if (!ValidDirections.Contains(_dirX) || !ValidDirections.Contains(_dirY))
                throw new SyntaxException("DrawRectangle direction parameters must be -1, 0, or 1");

            if (_distance < 0)
                throw new SyntaxException("DrawRectangle distance must be non-negative");

            if (_width <= 0 || _height <= 0)
                throw new SyntaxException("DrawRectangle width and height must be positive");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before drawing");

            if (state.CurrentColor == "Transparent")
                return; // No dibujar si el color es transparente

            // Calcular el centro del rectángulo
            Point center = new Point(
                state.WallEPosition.X + _dirX * _distance,
                state.WallEPosition.Y + _dirY * _distance
            );

            // Validar que el centro esté dentro del canvas
            if (!IsPointInCanvas(center, state.CanvasSize))
                throw new ExecutionException("Rectangle center is out of canvas bounds");

            // Calcular las coordenadas del rectángulo
            int left = (int)(center.X - _width / 2.0);
            int top = (int)(center.Y - _height / 2.0);
            int right = left + _width - 1;
            int bottom = top + _height - 1;

            // Validar que el rectángulo esté dentro del canvas
            if (left < 0 || top < 0 || right >= state.CanvasSize || bottom >= state.CanvasSize)
                throw new ExecutionException("Rectangle is out of canvas bounds");

            // Dibujar las cuatro líneas del rectángulo
            DrawLine(state, new Point(left, top), new Point(right, top)); // Línea superior
            DrawLine(state, new Point(left, bottom), new Point(right, bottom)); // Línea inferior
            DrawLine(state, new Point(left, top), new Point(left, bottom)); // Línea izquierda
            DrawLine(state, new Point(right, top), new Point(right, bottom)); // Línea derecha

            // Actualizar la posición de Wall-E al centro del rectángulo
            state.WallEPosition = center;
        }

        private bool IsPointInCanvas(Point point, int canvasSize)
        {
            return point.X >= 0 && point.X < canvasSize &&
                   point.Y >= 0 && point.Y < canvasSize;
        }

        // Método auxiliar para dibujar líneas (similar al de DrawLineCommand)
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