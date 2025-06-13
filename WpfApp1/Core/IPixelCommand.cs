// Core/Commands/IPixelCommand.cs
using PixelWallE.Core.Runtime;
using PixelWallE.Core.Parsing;

namespace PixelWallE.Core.Commands
{
    public interface IPixelCommand
    {
        string Name { get; }
        void Execute(RuntimeState state);
        void ValidateSyntax(CommandSyntax syntax);
    }
}