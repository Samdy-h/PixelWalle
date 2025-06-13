using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System.Collections.Generic;

namespace PixelWallE.Core.Commands
{
    public class ColorCommand : IPixelCommand
    {
        public string Name => "Color";

        private string _color = string.Empty;

        private static readonly HashSet<string> ValidColors = new HashSet<string>
        {
            "Red", "Blue", "Green", "Yellow", "Orange",
            "Purple", "Black", "White", "Transparent"
        };

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax.Parameters.Count != 1)
                throw new SyntaxException("Color command requires exactly 1 parameter");

            if (!syntax.Parameters[0].IsString)
                throw new SyntaxException("Color parameter must be a string");

            _color = syntax.Parameters[0].GetString() ?? string.Empty;

            if (string.IsNullOrEmpty(_color) || !ValidColors.Contains(_color))
                throw new SyntaxException($"Invalid color '{_color}'. Valid colors are: {string.Join(", ", ValidColors)}");
        }

        public void Execute(RuntimeState state)
        {
            state.CurrentColor = _color;
        }
    }
}