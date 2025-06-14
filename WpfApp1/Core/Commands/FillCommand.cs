using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PixelWallE.Core.Commands
{
    public class FillCommand : IPixelCommand
    {
        public string Name => "Fill";

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 0)
                throw new SyntaxException("Fill command does not take any parameters");
        }

        public void Execute(RuntimeState state)
        {
            if (!state.HasSpawned)
                throw new ExecutionException("Wall-E must be spawned before using Fill");

            if (state.CurrentColor == "Transparent")
                return; // No hacer nada

            int x = (int)state.WallEPosition.X;
            int y = (int)state.WallEPosition.Y;

            // Obtener el color del píxel actual
            string? targetColor = state.GetPixel(x, y);

            if (targetColor == null)
                throw new ExecutionException("Wall-E is outside the canvas");

            // Si el color objetivo es el mismo que el color actual, no hacer nada
            if (targetColor == state.CurrentColor)
                return;

            // Realizar el relleno
            FloodFill(state, x, y, targetColor, state.CurrentColor);
        }

        private void FloodFill(RuntimeState state, int startX, int startY, string targetColor, string replacementColor)
        {
            // Usamos un enfoque más eficiente con stack
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(startX, startY));

            while (stack.Count > 0)
            {
                Point point = stack.Pop();
                int x = (int)point.X;
                int y = (int)point.Y;

                // Verificar límites
                if (x < 0 || x >= state.CanvasSize || y < 0 || y >= state.CanvasSize)
                    continue;

                // Verificar si ya tiene el color de reemplazo o no es el objetivo
                string? currentColor = state.GetPixel(x, y);
                if (currentColor != targetColor || currentColor == replacementColor)
                    continue;

                // Pintar el píxel
                state.SetPixel(x, y, replacementColor);

                // Agregar vecinos
                stack.Push(new Point(x + 1, y));
                stack.Push(new Point(x - 1, y));
                stack.Push(new Point(x, y + 1));
                stack.Push(new Point(x, y - 1));
            }
        }
    }
}