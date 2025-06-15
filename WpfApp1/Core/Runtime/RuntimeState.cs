using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PixelWallE.Core.Exceptions;

namespace PixelWallE.Core.Runtime
{
    public class RuntimeState
    {
        public bool HasSpawned { get; set; }
        public Point WallEPosition { get; set; } = new Point(0, 0);
        public string CurrentColor { get; set; } = "Transparent";
        public int BrushSize { get; set; } = 1;
        public int CanvasSize { get; set; } = 100;

        public event Action ClearErrors = delegate { };

        private string[,] _canvas = new string[0, 0];
        private Dictionary<string, Color> _colorMap = new Dictionary<string, Color>();
        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();
        public Dictionary<string, int> Labels { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public int ProgramCounter { get; set; } = 0;
        public bool JumpRequested { get; set; } = false;

        // Diccionario de funciones disponibles
        private readonly Dictionary<string, Func<object[], object>> _functions = new Dictionary<string, Func<object[], object>>(StringComparer.OrdinalIgnoreCase);

        public RuntimeState(int canvasSize)
        {
            CanvasSize = canvasSize;
            InitializeCanvas();
            InitializeColorMap();
            RegisterFunctions();
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

        public HashSet<(int, int)> ModifiedPixels { get; } = new HashSet<(int, int)>();

        public void SetPixel(int x, int y, string color)
        {
            if (x >= 0 && x < CanvasSize && y >= 0 && y < CanvasSize)
            {
                _canvas[x, y] = color;
                ModifiedPixels.Add((x, y));
            }
        }

        public void ClearModifiedPixels() => ModifiedPixels.Clear();


        public string? GetPixel(int x, int y)
        {
            if (x >= 0 && x < CanvasSize && y >= 0 && y < CanvasSize)
            {
                return _canvas[x, y];
            }
            return null;
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
            ClearErrors?.Invoke();
        }

        // Método para convertir cualquier objeto a entero
        public int ConvertToInt(object? value)
        {
            if (value == null)
                throw new ExecutionException("El valor proporcionado es nulo");

            if (value is int i) return i;
            if (value is double d) return (int)Math.Round(d);
            if (value is float f) return (int)Math.Round(f);
            if (value is string s && int.TryParse(s, out int result)) return result;

            throw new ExecutionException($"Se esperaba un valor entero pero se obtuvo {value.GetType().Name}");
        }

        // Método para convertir cualquier objeto a double
        public double ConvertToDouble(object? value)
        {
            if (value == null)
                throw new ExecutionException("El valor proporcionado es nulo");

            if (value is int i) return i;
            if (value is double d) return d;
            if (value is float f) return f;
            if (value is string s && double.TryParse(s, out double result)) return result;

            throw new ExecutionException($"Se esperaba un valor numérico pero se obtuvo {value.GetType().Name}");
        }

        // Registrar todas las funciones disponibles
        private void RegisterFunctions()
        {
            // Funciones básicas de posición
            RegisterFunction("GetActualX", args => (int)WallEPosition.X);
            RegisterFunction("GetActualY", args => (int)WallEPosition.Y);
            RegisterFunction("GetCanvasSize", args => CanvasSize);

            // Funciones de información del canvas
            RegisterFunction("GetColorCount", GetColorCount);
            RegisterFunction("IsBrushColor", IsBrushColor);
            RegisterFunction("IsBrushSize", IsBrushSize);
            RegisterFunction("IsCanvasColor", IsCanvasColor);
        }

        public void RegisterFunction(string name, Func<object[], object> function)
        {
            _functions[name] = function;
        }

        public object CallFunction(string name, object[] args)
        {
            if (_functions.TryGetValue(name, out var function))
            {
                try
                {
                    return function(args);
                }
                catch (Exception ex)
                {
                    throw new ExecutionException($"Error en función {name}: {ex.Message}");
                }
            }
            throw new ExecutionException($"Función no definida: {name}");
        }

        #region Implementaciones de Funciones Específicas

        private object GetColorCount(object[] args)
        {
            // Validar número de parámetros
            if (args.Length != 5)
                throw new ExecutionException("GetColorCount requiere 5 parámetros: color, x1, y1, x2, y2");

            // Obtener y validar parámetros
            string color = args[0]?.ToString() ?? "";
            int x1 = ConvertToInt(args[1]);
            int y1 = ConvertToInt(args[2]);
            int x2 = ConvertToInt(args[3]);
            int y2 = ConvertToInt(args[4]);

            // Normalizar coordenadas
            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            // Validar límites del canvas
            minX = Math.Clamp(minX, 0, CanvasSize - 1);
            maxX = Math.Clamp(maxX, 0, CanvasSize - 1);
            minY = Math.Clamp(minY, 0, CanvasSize - 1);
            maxY = Math.Clamp(maxY, 0, CanvasSize - 1);

            // Contar píxeles del color especificado
            int count = 0;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (GetPixel(x, y) == color)
                        count++;
                }
            }

            return count;
        }

        private object IsBrushColor(object[] args)
        {
            if (args.Length != 1)
                throw new ExecutionException("IsBrushColor requiere 1 parámetro: color");

            string color = args[0]?.ToString() ?? "";
            return CurrentColor.Equals(color, StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        }

        private object IsBrushSize(object[] args)
        {
            if (args.Length != 1)
                throw new ExecutionException("IsBrushSize requiere 1 parámetro: size");

            int size = ConvertToInt(args[0]);
            return BrushSize == size ? 1 : 0;
        }

        private object IsCanvasColor(object[] args)
        {
            if (args.Length != 3)
                throw new ExecutionException("IsCanvasColor requiere 3 parámetros: color, vertical, horizontal");

            string color = args[0]?.ToString() ?? "";
            int vertical = ConvertToInt(args[1]);
            int horizontal = ConvertToInt(args[2]);

            // Calcular posición absoluta
            int x = (int)WallEPosition.X + horizontal;
            int y = (int)WallEPosition.Y + vertical;

            // Verificar si está dentro del canvas
            if (x < 0 || x >= CanvasSize || y < 0 || y >= CanvasSize)
                return 0;

            // Comparar colores
            return GetPixel(x, y) == color ? 1 : 0;
        }

        #endregion
        // Método para mostrar variables en depuración
        public string GetVariablesString()
        {
            return string.Join("\n", Variables.Select(v => $"{v.Key} = {v.Value}"));
        }

        // Método para mostrar funciones disponibles
        public string GetFunctionsString()
        {
            return string.Join("\n", _functions.Keys);
        }

        public bool ConvertToBool(object? value)
        {
            if (value == null)
                return false;

            if (value is int i) return i != 0;
            if (value is double d) return d != 0;
            if (value is float f) return f != 0;
            if (value is bool b) return b;
            if (value is string s) return !string.IsNullOrEmpty(s);

            return true; // Cualquier otro objeto se considera verdadero
        }

    }
}