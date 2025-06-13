using PixelWallE.Core.Commands;
using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32; // Para OpenFileDialog y SaveFileDialog
using System.IO; // Para operaciones de archivo


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
        public string WallEPosition => $"Posición: ({_runtimeState.WallEPosition.X}, {_runtimeState.WallEPosition.Y})";
        public string CurrentColor => $"Color: {_runtimeState.CurrentColor}";
        public string BrushSize => $"Tamaño pincel: {_runtimeState.BrushSize}";
        public string CanvasSizeDisplay => $"Tamaño canvas: {_runtimeState.CanvasSize}x{_runtimeState.CanvasSize}";

        // Números de línea para el editor
        public ObservableCollection<string> LineNumbers { get; } = new ObservableCollection<string>();

        public ICommand ExecuteCommand { get; }
        public ICommand ResizeCanvasCommand { get; }
        public ICommand LoadCommand { get; } // Implementar después
        public ICommand SaveCommand { get; } // Implementar después

        public void UpdateLineNumbers()
        {
            LineNumbers.Clear();
            int lineCount = string.IsNullOrEmpty(Code) ? 1 : Code.Count(c => c == '\n') + 1;

            for (int i = 1; i <= lineCount; i++)
            {
                LineNumbers.Add(i.ToString());
            }
        }

        private void ExecuteCode()
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

                    var openParen = trimmedLine.IndexOf('(');
                    var closeParen = trimmedLine.IndexOf(')');

                    if (openParen == -1 || closeParen == -1 || closeParen < openParen)
                    {
                        throw new SyntaxException("Sintaxis inválida - faltan paréntesis");
                    }

                    var commandName = trimmedLine.Substring(0, openParen).Trim();
                    var parametersPart = trimmedLine.Substring(openParen + 1, closeParen - openParen - 1);
                    var parameters = parametersPart.Split(',');

                    var syntax = new CommandSyntax(commandName);

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

                    var command = CommandFactory.CreateCommand(commandName);
                    command.ValidateSyntax(syntax);
                    command.Execute(_runtimeState);

                    // Actualizar propiedades de estado
                    OnPropertyChanged(nameof(WallEPosition));
                    OnPropertyChanged(nameof(CurrentColor));
                    OnPropertyChanged(nameof(BrushSize));
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
    }
}