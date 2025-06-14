using PixelWallE.Core.Commands;
using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;

namespace PixelWallE.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private RuntimeState _runtimeState;
        private string _code = "";
        private string _executionResult = "";
        private int _canvasSize = 100;

        public MainViewModel()
        {
            _runtimeState = new RuntimeState(CanvasSize);
            ExecuteCommand = new RelayCommand(ExecuteCode);
            ResizeCanvasCommand = new RelayCommand(ResizeCanvas);
            LoadCommand = new RelayCommand(LoadFile);
            SaveCommand = new RelayCommand(SaveFile);
            UpdateLineNumbers();
            InitializeCanvasPixels();
        }

        public string Code
        {
            get => _code;
            set
            {
                if (SetProperty(ref _code, value))
                {
                    UpdateLineNumbers();
                }
            }
        }

        public string ExecutionResult
        {
            get => _executionResult;
            set => SetProperty(ref _executionResult, value);
        }

        public int CanvasSize
        {
            get => _canvasSize;
            set => SetProperty(ref _canvasSize, value);
        }

        // Propiedades para mostrar el estado
        public string WallEPositionDisplay => $"Posición: ({(int)_runtimeState.WallEPosition.X}, {(int)_runtimeState.WallEPosition.Y})";
        public string CurrentColorDisplay => $"Color: {_runtimeState.CurrentColor}";
        public string BrushSizeDisplay => $"Tamaño pincel: {_runtimeState.BrushSize}";
        public string CanvasSizeDisplay => $"Tamaño canvas: {_runtimeState.CanvasSize}x{_runtimeState.CanvasSize}";

        // Números de línea para el editor
        public ObservableCollection<string> LineNumbers { get; } = new ObservableCollection<string>();

        // Píxeles para el canvas
        public ObservableCollection<PixelViewModel> CanvasPixels { get; } = new ObservableCollection<PixelViewModel>();

        public ICommand ExecuteCommand { get; }
        public ICommand ResizeCanvasCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }

        public void UpdateLineNumbers()
        {
            LineNumbers.Clear();
            int lineCount = string.IsNullOrEmpty(Code) ? 1 : Code.Count(c => c == '\n') + 1;

            for (int i = 1; i <= lineCount; i++)
            {
                LineNumbers.Add(i.ToString());
            }
        }

        public void ExecuteCode()
        {
            try
            {
                ExecutionResult = "Ejecutando código...";

                if (string.IsNullOrWhiteSpace(Code))
                {
                    ExecutionResult = "Error: No hay código para ejecutar";
                    return;
                }

                var lines = Code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine)) continue;

                    CommandSyntax syntax;
                    string commandName;

                    // Manejar comandos sin paréntesis (como Fill)
                    if (!trimmedLine.Contains("(") || !trimmedLine.Contains(")"))
                    {
                        commandName = trimmedLine;
                        syntax = new CommandSyntax(commandName);
                    }
                    else // Comandos con parámetros
                    {
                        var openParen = trimmedLine.IndexOf('(');
                        var closeParen = trimmedLine.IndexOf(')');

                        if (openParen == -1 || closeParen == -1 || closeParen < openParen)
                        {
                            throw new SyntaxException("Sintaxis inválida - faltan paréntesis");
                        }

                        commandName = trimmedLine.Substring(0, openParen).Trim();
                        var parametersPart = trimmedLine.Substring(openParen + 1, closeParen - openParen - 1);
                        syntax = new CommandSyntax(commandName);

                        // Solo procesar parámetros si hay contenido
                        if (!string.IsNullOrWhiteSpace(parametersPart))
                        {
                            var parameters = parametersPart.Split(',');

                            foreach (var param in parameters)
                            {
                                var trimmedParam = param.Trim();

                                if (int.TryParse(trimmedParam, out int intValue))
                                {
                                    syntax.Parameters.Add(new CommandParameter(intValue));
                                }
                                else if (trimmedParam.StartsWith('\"') && trimmedParam.EndsWith('\"'))
                                {
                                    var stringValue = trimmedParam.Substring(1, trimmedParam.Length - 2);
                                    syntax.Parameters.Add(new CommandParameter(stringValue));
                                }
                                else
                                {
                                    throw new SyntaxException($"Parámetro inválido: {trimmedParam}");
                                }
                            }
                        }
                    }

                    var command = CommandFactory.CreateCommand(commandName);
                    command.ValidateSyntax(syntax);
                    command.Execute(_runtimeState);

                    // Actualizar propiedades de estado
                    OnPropertyChanged(nameof(WallEPositionDisplay));
                    OnPropertyChanged(nameof(CurrentColorDisplay));
                    OnPropertyChanged(nameof(BrushSizeDisplay));

                    // Actualizar canvas después de cada comando para ver progreso
                    UpdateCanvasDisplay();
                }

                ExecutionResult = "¡Código ejecutado con éxito!";
            }
            catch (Exception ex)
            {
                ExecutionResult = $"ERROR: {ex.Message}";
            }
        }

        private void ResizeCanvas()
        {
            _runtimeState.ResizeCanvas(CanvasSize);
            ExecutionResult = $"Canvas redimensionado a {CanvasSize}x{CanvasSize}";

            // Actualizar propiedades de estado
            OnPropertyChanged(nameof(CanvasSizeDisplay));

            // Reconstruir los píxeles del canvas
            InitializeCanvasPixels();
        }

        private void LoadFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Archivos Pixel Wall-E (*.pw)|*.pw|Todos los archivos (*.*)|*.*",
                    Title = "Cargar archivo de código"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    Code = File.ReadAllText(openFileDialog.FileName);
                    ExecutionResult = $"Archivo cargado: {openFileDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                ExecutionResult = $"Error al cargar archivo: {ex.Message}";
            }
        }

        private void SaveFile()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivos Pixel Wall-E (*.pw)|*.pw|Todos los archivos (*.*)|*.*",
                    Title = "Guardar archivo de código",
                    DefaultExt = ".pw"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, Code);
                    ExecutionResult = $"Archivo guardado: {saveFileDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                ExecutionResult = $"Error al guardar archivo: {ex.Message}";
            }
        }

        private void InitializeCanvasPixels()
        {
            CanvasPixels.Clear();
            for (int y = 0; y < _runtimeState.CanvasSize; y++)
            {
                for (int x = 0; x < _runtimeState.CanvasSize; x++)
                {
                    // Manejo seguro de nulos con el operador ??
                    string pixelColor = _runtimeState.GetPixel(x, y) ?? "White";
                    CanvasPixels.Add(new PixelViewModel
                    {
                        X = x,
                        Y = y,
                        Color = new SolidColorBrush(_runtimeState.GetColorValue(pixelColor)),
                        Size = 400.0 / _runtimeState.CanvasSize
                    });
                }
            }
            OnPropertyChanged(nameof(CanvasPixels));
        }

        private void UpdateCanvasDisplay()
        {
            for (int y = 0; y < _runtimeState.CanvasSize; y++)
            {
                for (int x = 0; x < _runtimeState.CanvasSize; x++)
                {
                    int index = y * _runtimeState.CanvasSize + x;
                    if (index < CanvasPixels.Count)
                    {
                        var pixel = CanvasPixels[index];
                        // Manejo seguro de nulos con el operador ??
                        string pixelColor = _runtimeState.GetPixel(x, y) ?? "White";
                        pixel.Color = new SolidColorBrush(_runtimeState.GetColorValue(pixelColor));
                    }
                }
            }
        }
    }
}