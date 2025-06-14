// Core/Commands/DrawCircleCommand.cs
using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class DrawCircleCommand : IPixelCommand
    {
        public string Name => "DrawCircle";

        private int _dirX;
        private int _dirY;
        private int _radius;

        private static readonly HashSet<int> ValidDirections = new HashSet<int> { -1, 0, 1 };

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 3)
                throw new SyntaxException("DrawCircle command requires exactly 3 parameters (dirX, dirY, radius)");

            if (!syntax.Parameters[0].IsInteger || !syntax.Parameters[1].IsInteger || !syntax.Parameters[2].IsInteger)
                throw new SyntaxException("DrawCircle parameters must be integers");

            _dirX = syntax.Parameters[0].GetInteger();
            _dirY = syntax.Parameters[1].GetInteger();
            _radius = syntax.Parameters[2].GetInteger();

            if (!ValidDirections.Contains(_dirX) || !ValidDirections.Contains(_dirY))
                throw new SyntaxException("DrawCircle direction parameters must be -1, 0, or 1");

            if (_radius <= 0)
                throw new SyntaxException("DrawCircle radius must be positive");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before drawing");

            if (state.CurrentColor == "Transparent")
                return; // No dibujar si el color es transparente

            // Calcular el centro del círculo
            Point center = new Point(
                state.WallEPosition.X + _dirX * _radius,
                state.WallEPosition.Y + _dirY * _radius
            );

            // Validar que el centro esté dentro del canvas
            if (!IsPointInCanvas(center, state.CanvasSize))
                throw new ExecutionException("Circle center is out of canvas bounds");

            // Dibujar el círculo
            DrawCircle(state, center);

            // Actualizar la posición de Wall-E al centro del círculo
            state.WallEPosition = center;
        }

        private bool IsPointInCanvas(Point point, int canvasSize)
        {
            return point.X >= 0 && point.X < canvasSize &&
                   point.Y >= 0 && point.Y < canvasSize;
        }

        private void DrawCircle(RuntimeState state, Point center)
        {
            int cx = (int)Math.Round(center.X);
            int cy = (int)Math.Round(center.Y);
            int r = _radius;

            // Usamos el algoritmo del punto medio para dibujar círculos
            int x = 0;
            int y = r;
            int d = 3 - 2 * r;

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
            // Dibujar en los 8 octantes (simetría del círculo)
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