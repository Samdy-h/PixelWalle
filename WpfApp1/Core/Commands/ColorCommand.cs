using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System.Collections.Generic;
using System;

namespace PixelWallE.Core.Commands
{
    public class ColorCommand : IPixelCommand
    {
        public string Name => "Color";

        private IPixelExpression _colorExpression = null!;

        private static readonly HashSet<string> ValidColors = new HashSet<string>
        {
            "Red", "Blue", "Green", "Yellow", "Orange",
            "Purple", "Black", "White", "Transparent"
        };

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (syntax.Parameters.Count != 1)
                throw new SyntaxException("Color command requires exactly 1 parameter");

            _colorExpression = syntax.Parameters[0].Expression ?? throw new SyntaxException("Invalid expression");
        }

        public void Execute(RuntimeState state)
        {
            var colorObj = _colorExpression.Evaluate(state);

            // Convertir a string si es necesario
            string color = colorObj?.ToString() ?? "";

            // Validar que sea un color permitido
            if (string.IsNullOrEmpty(color) || !ValidColors.Contains(color))
                throw new ExecutionException($"Invalid color '{color}'. Valid colors are: {string.Join(", ", ValidColors)}");

            state.CurrentColor = color;
        }
    }
}