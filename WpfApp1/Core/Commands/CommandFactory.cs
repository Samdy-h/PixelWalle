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
            { "Size", () => new SizeCommand() }
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