using System.Drawing;

namespace PixelWallE.Core.Runtime
{
    public class RuntimeState
    {
        public bool HasSpawned { get; set; }
        public Point WallEPosition { get; set; }
        public string CurrentColor { get; set; } = "Transparent";
        public int BrushSize { get; set; } = 1;
        public int CanvasSize { get; set; } = 100;

        public RuntimeState(int canvasSize)
        {
            CanvasSize = canvasSize;
            WallEPosition = new Point(0, 0);
        }

        public void ResizeCanvas(int newSize)
        {
            CanvasSize = newSize;
            HasSpawned = false;
            WallEPosition = new Point(0, 0);
            CurrentColor = "Transparent";
            BrushSize = 1;
        }
    }
}