using Microsoft.Win32;
using PixelWallE.Core.Commands;
using PixelWallE.Core.Exceptions;
using PixelWallE.Core.Expressions;
using PixelWallE.Core.Parsing;
using PixelWallE.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            _runtimeState.ClearErrors += () => Errors.Clear();
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

        public string WallEPositionDisplay =>
            _runtimeState.HasSpawned
                ? $"Posición: ({(int)_runtimeState.WallEPosition.X}, {(int)_runtimeState.WallEPosition.Y})"
                : "No posicionado";

        public string CurrentColorDisplay => $"Color: {_runtimeState.CurrentColor}";
        public string BrushSizeDisplay => $"Tamaño pincel: {_runtimeState.BrushSize}";
        public string CanvasSizeDisplay => $"Tamaño canvas: {_runtimeState.CanvasSize}x{_runtimeState.CanvasSize}";
        public string SpawnStatus =>
            _runtimeState.HasSpawned
                ? $"Spawned at ({_runtimeState.WallEPosition.X}, {_runtimeState.WallEPosition.Y})"
                : "Not spawned";

        public ObservableCollection<string> LineNumbers { get; } = new ObservableCollection<string>();
        public ObservableCollection<PixelViewModel> CanvasPixels { get; } = new ObservableCollection<PixelViewModel>();
        public ObservableCollection<ErrorInfo> Errors { get; } = new ObservableCollection<ErrorInfo>();

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

        private void ExecuteCode()
        {
            try
            {
                ExecutionResult = "Ejecutando código...";
                Errors.Clear();
                _runtimeState = new RuntimeState(CanvasSize); // Reset state
                _runtimeState.HasSpawned = false; // Reset spawn status

                if (string.IsNullOrWhiteSpace(Code))
                {
                    ExecutionResult = "Error: No hay código para ejecutar";
                    return;
                }

                var lines = Code.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
                bool spawnCommandFound = false;

                // Primera pasada: recolectar etiquetas
                _runtimeState.Labels.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmedLine = lines[i].Trim();
                    if (trimmedLine.EndsWith(":") && !trimmedLine.Contains(" "))
                    {
                        string label = trimmedLine.TrimEnd(':');
                        if (!string.IsNullOrWhiteSpace(label))
                        {
                            if (!IsValidLabelName(label))
                            {
                                throw new SyntaxException($"Nombre de etiqueta inválido: '{label}'");
                            }
                            _runtimeState.Labels[label] = i;
                        }
                    }
                }

                // Segunda pasada: ejecución
                _runtimeState.ProgramCounter = 0;
                while (_runtimeState.ProgramCounter < lines.Length)
                {
                    string line = lines[_runtimeState.ProgramCounter];
                    string trimmedLine = line.Trim();
                    int currentLine = _runtimeState.ProgramCounter;

                    // Avanzar al siguiente comando por defecto (a menos que haya un salto)
                    _runtimeState.ProgramCounter++;
                    _runtimeState.JumpRequested = false;

                    if (string.IsNullOrWhiteSpace(trimmedLine))
                        continue;

                    // Manejar etiquetas (no son comandos ejecutables)
                    if (trimmedLine.EndsWith(":"))
                        continue;

                    try
                    {
                        ProcessLine(trimmedLine, ref spawnCommandFound);

                        // Actualizar UI
                        OnPropertyChanged(nameof(WallEPositionDisplay));
                        OnPropertyChanged(nameof(CurrentColorDisplay));
                        OnPropertyChanged(nameof(BrushSizeDisplay));
                        OnPropertyChanged(nameof(SpawnStatus));
                        UpdateCanvasDisplay();

                        // Si se solicitó un salto, continuar sin avanzar más (ProgramCounter ya fue actualizado)
                        if (_runtimeState.JumpRequested)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Errors.Add(new ErrorInfo
                        {
                            LineNumber = currentLine + 1,
                            Message = ex.Message
                        });
                        ExecutionResult = $"ERROR en línea {currentLine + 1}: {ex.Message}";
                        return;
                    }
                }

                if (!spawnCommandFound)
                {
                    throw new ExecutionException("El código debe contener un comando Spawn");
                }

                ExecutionResult = "¡Código ejecutado con éxito!";
            }
            catch (Exception ex)
            {
                ExecutionResult = $"ERROR: {ex.Message}";
            }
        }

        private void ProcessLine(string line, ref bool spawnCommandFound)
        {
            // Manejar GoTo
            if (line.StartsWith("GoTo"))
            {
                ProcessGoToCommand(line);
                return;
            }

            // Manejar asignaciones
            if (line.Contains("<-"))
            {
                ProcessAssignment(line);
                return;
            }

            // Comandos regulares
            ProcessRegularCommand(line, ref spawnCommandFound);
        }

        private void ProcessGoToCommand(string line)
        {
            // Parse: GoTo [label] (condition)
            var parts = line.Split(new[] { '[', ']', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new SyntaxException("Sintaxis de GoTo inválida");

            string label = parts[1].Trim();
            string condition = parts.Length > 2 ? parts[2].Trim() : "1"; // Default to true

            if (!_runtimeState.Labels.TryGetValue(label, out int targetLine))
                throw new ExecutionException($"Etiqueta '{label}' no definida");

            // Evaluate condition
            var conditionExpr = ExpressionParser.Parse(condition);
            var conditionResult = conditionExpr.Evaluate(_runtimeState);

            bool shouldJump = _runtimeState.ConvertToBool(conditionResult);

            if (shouldJump)
            {
                _runtimeState.ProgramCounter = targetLine;
                _runtimeState.JumpRequested = true;
            }
        }

        private void ProcessAssignment(string line)
        {
            var parts = line.Split(new[] { "<-" }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new SyntaxException("Sintaxis de asignación inválida");

            string varName = parts[0].Trim();
            string expr = parts[1].Trim();

            var syntax = new CommandSyntax("<-");
            syntax.Parameters.Add(new CommandParameter(varName));
            syntax.Parameters.Add(new CommandParameter(ExpressionParser.Parse(expr)));

            var command = CommandFactory.CreateCommand("<-");
            command.ValidateSyntax(syntax);
            command.Execute(_runtimeState);
        }

        private void ProcessRegularCommand(string line, ref bool spawnCommandFound)
        {
            CommandSyntax syntax;
            string commandName;

            if (!line.Contains('(') || !line.Contains(')'))
            {
                commandName = line;
                syntax = new CommandSyntax(commandName);
            }
            else
            {
                var openParen = line.IndexOf('(');
                var closeParen = line.IndexOf(')');

                if (openParen == -1 || closeParen == -1 || closeParen < openParen)
                {
                    throw new SyntaxException("Sintaxis inválida - faltan paréntesis");
                }

                commandName = line.Substring(0, openParen).Trim();
                var parametersPart = line.Substring(openParen + 1, closeParen - openParen - 1);
                syntax = new CommandSyntax(commandName);

                if (!string.IsNullOrWhiteSpace(parametersPart))
                {
                    var parameters = SplitParameters(parametersPart);

                    foreach (var param in parameters)
                    {
                        var trimmedParam = param.Trim();

                        // Handle quoted strings
                        if (trimmedParam.StartsWith('\"') && trimmedParam.EndsWith('\"'))
                        {
                            var stringValue = trimmedParam.Substring(1, trimmedParam.Length - 2);
                            syntax.Parameters.Add(new CommandParameter(stringValue));
                        }
                        // Handle integers
                        else if (int.TryParse(trimmedParam, out int intValue))
                        {
                            syntax.Parameters.Add(new CommandParameter(intValue));
                        }
                        // Handle expressions
                        else
                        {
                            try
                            {
                                var expression = ExpressionParser.Parse(trimmedParam);
                                syntax.Parameters.Add(new CommandParameter(expression));
                            }
                            catch
                            {
                                throw new SyntaxException($"Parámetro inválido: {trimmedParam}");
                            }
                        }
                    }
                }
            }

            // Spawn validation
            if (commandName.Equals("Spawn", StringComparison.OrdinalIgnoreCase))
            {
                if (spawnCommandFound)
                    throw new ExecutionException("Spawn solo puede usarse una vez");

                spawnCommandFound = true;
            }
            else if (!spawnCommandFound)
            {
                throw new ExecutionException("El primer comando debe ser Spawn");
            }

            var command = CommandFactory.CreateCommand(commandName);
            command.ValidateSyntax(syntax);
            command.Execute(_runtimeState);
        }

        private string[] SplitParameters(string input)
        {
            var result = new List<string>();
            int start = 0;
            int depth = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '(') depth++;
                else if (c == ')') depth--;
                else if (c == ',' && depth == 0)
                {
                    result.Add(input.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }

            result.Add(input.Substring(start).Trim());
            return result.ToArray();
        }

        private void ResizeCanvas()
        {
            _runtimeState.ResizeCanvas(CanvasSize);
            ExecutionResult = $"Canvas redimensionado a {CanvasSize}x{CanvasSize}";
            OnPropertyChanged(nameof(CanvasSizeDisplay));
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
            // Actualizar solo píxeles modificados
            if (_runtimeState.ModifiedPixels != null && _runtimeState.ModifiedPixels.Count > 0)
            {
                foreach (var (x, y) in _runtimeState.ModifiedPixels)
                {
                    var pixel = CanvasPixels.FirstOrDefault(p => p.X == x && p.Y == y);
                    if (pixel != null)
                    {
                        string color = _runtimeState.GetPixel(x, y) ?? "White";
                        pixel.Color = new SolidColorBrush(_runtimeState.GetColorValue(color));
                    }
                }
                _runtimeState.ClearModifiedPixels();
            }
        }

        private bool IsValidLabelName(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return false;
            if (char.IsDigit(label[0]) || label[0] == '_') return false;

            foreach (char c in label)
            {
                if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
                    return false;
            }

            return true;
        }
    }

    public class ErrorInfo
    {
        public int LineNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}