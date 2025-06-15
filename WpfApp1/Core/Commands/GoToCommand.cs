using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;

namespace PixelWallE.Core.Commands
{
    public class GoToCommand : IPixelCommand
    {
        public string Name => "GoTo";

        private string _label = null!;
        private IPixelExpression _condition = null!;

        public void ValidateSyntax(CommandSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));

            // Sintaxis: GoTo [label] (condition)
            if (syntax.Parameters.Count != 2)
                throw new SyntaxException("GoTo command requires exactly 2 parameters: label and condition");

            if (!syntax.Parameters[0].IsString)
                throw new SyntaxException("Label must be a string");

            _label = syntax.Parameters[0].GetString() ?? throw new SyntaxException("Invalid label");
            _condition = syntax.Parameters[1].Expression ?? throw new SyntaxException("Invalid condition expression");
        }

        public void Execute(RuntimeState state)
        {
            var conditionResult = _condition.Evaluate(state);
            bool shouldJump = state.ConvertToBool(conditionResult);

            if (shouldJump)
            {
                if (state.Labels.TryGetValue(_label, out int lineNumber))
                {
                    state.ProgramCounter = lineNumber;
                    state.JumpRequested = true;
                }
                else
                {
                    throw new ExecutionException($"Label '{_label}' not defined");
                }
            }
        }
    }
}