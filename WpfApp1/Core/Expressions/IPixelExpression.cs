using PixelWallE.Core.Runtime;

namespace PixelWallE.Core.Expressions
{
    public interface IPixelExpression
    {
        object Evaluate(RuntimeState state);
    }
}