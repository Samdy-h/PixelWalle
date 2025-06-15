using System;
using System.Collections.Generic;

namespace PixelWallE.Core.Parsing
{
    public class CommandSyntax
    {
        public string CommandName { get; }
        public List<CommandParameter> Parameters { get; } = new List<CommandParameter>();

        public CommandSyntax(string commandName)
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
        }
    }
}