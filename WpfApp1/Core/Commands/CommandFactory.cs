using System;
using System.Collections.Generic;

namespace PixelWallE.Core.Commands
{
    public static class CommandFactory
    {
        private static readonly Dictionary<string, Func<IPixelCommand>> CommandCreators =
            new Dictionary<string, Func<IPixelCommand>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Spawn", () => new SpawnCommand() },
            { "Color", () => new ColorCommand() },
            { "Size", () => new SizeCommand() },
            { "DrawLine", () => new DrawLineCommand() },
            { "DrawCircle", () => new DrawCircleCommand() },
            { "DrawRectangle", () => new DrawRectangleCommand() },
            { "Fill", () => new FillCommand() },
            { "<-", () => new AssignmentCommand() }, // Nuevo comando de asignación
            { "GoTo", () => new GoToCommand() }
        };

        public static IPixelCommand CreateCommand(string commandName)
        {
            if (CommandCreators.TryGetValue(commandName, out var creator))
            {
                return creator();
            }

            throw new NotSupportedException($"Command '{commandName}' is not supported");
        }
    }
}