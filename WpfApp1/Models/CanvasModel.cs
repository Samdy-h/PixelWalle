using System.Collections.Generic;

namespace WallE.PixelArt.Models
{
    public class CanvasModel
    {
        public int Size { get; private set; }
        public List<Pixel> Pixels { get; private set; } = new List<Pixel>();

        public void Initialize(int size)
        {
            Size = size;
            Pixels.Clear();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Pixels.Add(new Pixel(x, y));
                }
            }
        }
    }
}