using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PixelWallE.Core.Runtime
{
    public class RuntimeState
    {
        public bool HasSpawned { get; set; }
        public Point WallEPosition { get; set; } = new Point(0, 0);
        public string CurrentColor { get; set; } = "Transparent";
        public int BrushSize { get; set; } = 1;
        public int CanvasSize { get; set; } = 100;

        // Inicializamos con arrays vacíos para evitar CS8618
        private string[,] _canvas = new string[0, 0];
        private Dictionary<string, Color> _colorMap = new Dictionary<string, Color>();

        public RuntimeState(int canvasSize)
        {
            CanvasSize = canvasSize;
            InitializeCanvas();
            InitializeColorMap();
        }

        private void InitializeColorMap()
        {
            _colorMap = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
            {
                ["Red"] = Colors.Red,
                ["Blue"] = Colors.Blue,
                ["Green"] = Colors.Green,
                ["Yellow"] = Colors.Yellow,
                ["Orange"] = Colors.Orange,
                ["Purple"] = Colors.Purple,
                ["Black"] = Colors.Black,
                ["White"] = Colors.White,
                ["Transparent"] = Colors.Transparent
            };
        }

        private void InitializeCanvas()
        {
            _canvas = new string[CanvasSize, CanvasSize];
            for (int x = 0; x < CanvasSize; x++)
            {
                for (int y = 0; y < CanvasSize; y++)
                {
                    _canvas[x, y] = "White";
                }
            }
        }

        public void SetPixel(int x, int y, string color)
        {
            if (x >= 0 && x < CanvasSize && y >= 0 && y < CanvasSize)
            {
                _canvas[x, y] = color;
            }
        }

        // Cambiamos a string? para indicar que puede ser nulo
        public string? GetPixel(int x, int y)
        {
            if (x >= 0 && x < CanvasSize && y >= 0 && y < CanvasSize)
            {
                return _canvas[x, y];
            }
            return null; // Ahora es válido porque el tipo es nullable
        }

        public Color GetColorValue(string colorName)
        {
            if (_colorMap.TryGetValue(colorName, out var color))
            {
                return color;
            }
            return Colors.Transparent;
        }

        public void ResizeCanvas(int newSize)
        {
            CanvasSize = newSize;
            HasSpawned = false;
            WallEPosition = new Point(0, 0);
            InitializeCanvas();
        }
    }
}