namespace WallE.PixelArt.Models
{
    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; } = "White";

        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}