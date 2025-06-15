using PixelWallE.Core.Runtime;

namespace PixelWallE.Core.Expressions
{
    public class LiteralExpression : IPixelExpression
    {
        private readonly object? _value;

        public LiteralExpression(object? value)
        {
            _value = value;
        }

        public object Evaluate(RuntimeState state)
        {
            // Devolver 0 si es nulo
            return _value ?? 0;
        }
    }
}