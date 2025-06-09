using WallE.PixelArt.Models;
using WallE.PixelArt.Utils;
using System.Windows.Input;

namespace WallE.PixelArt.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private CanvasModel _canvas = new CanvasModel();
        private RobotState _robot = new RobotState();
        private string _codeText = "Spawn(0, 0)\nColor(Black)\n";
        private int _canvasSize = 100;

        public CanvasModel Canvas
        {
            get => _canvas;
            set => SetProperty(ref _canvas, value);
        }

        public RobotState Robot
        {
            get => _robot;
            set => SetProperty(ref _robot, value);
        }

        public string CodeText
        {
            get => _codeText;
            set => SetProperty(ref _codeText, value);
        }

        public int CanvasSize
        {
            get => _canvasSize;
            set => SetProperty(ref _canvasSize, value);
        }

        public ICommand ExecuteCommand => new RelayCommand(_ => ExecuteCode());
        public ICommand ResizeCommand => new RelayCommand(_ => ResizeCanvas());

        public MainViewModel()
        {
            Canvas.Initialize(CanvasSize);
        }

        private void ExecuteCode()
        {
            // Lógica de ejecución de código
            // Por ahora solo un ejemplo básico
            Robot.X = 0;
            Robot.Y = 0;
            Robot.BrushColor = "Black";
        }

        private void ResizeCanvas()
        {
            Canvas.Initialize(CanvasSize);
            Robot = new RobotState(); // Resetear estado del robot
        }
    }
}