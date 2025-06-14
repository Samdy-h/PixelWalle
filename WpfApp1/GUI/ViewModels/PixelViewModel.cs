using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PixelWallE.ViewModels
{
    public class PixelViewModel : ViewModelBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Size { get; set; }

        // Inicializamos con un valor por defecto
        private SolidColorBrush _color = Brushes.White;
        public SolidColorBrush Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
    }
}