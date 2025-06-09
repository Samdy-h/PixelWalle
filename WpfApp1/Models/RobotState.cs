namespace WallE.PixelArt.Models
{
    public class RobotState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string BrushColor { get; set; } = "Transparent";
        public int BrushSize { get; set; } = 1;
    }
}