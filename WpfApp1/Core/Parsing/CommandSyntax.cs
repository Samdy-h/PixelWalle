using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System;

namespace PixelWallE.Core.Parsing
{
    public class CommandSyntax
    {
        public string CommandName { get; }
        public List<CommandParameter> Parameters { get; } = new List<CommandParameter>();

        public CommandSyntax(string commandName)
        {
            CommandName = commandName;
        }
    }

    public class CommandParameter
    {
        public object Value { get; }
        public bool IsInteger { get; }
        public bool IsString { get; }

        public CommandParameter(int value)
        {
            Value = value;
            IsInteger = true;
            IsString = false;
        }

        public CommandParameter(string value)
        {
            Value = value;
            IsInteger = false;
            IsString = true;
        }

        public int GetInteger() => IsInteger ? (int)Value : throw new InvalidOperationException("Parameter is not an integer");
        public string GetString() => IsString ? (string)Value : throw new InvalidOperationException("Parameter is not a string");
    }
}