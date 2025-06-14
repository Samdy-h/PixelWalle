using System.Windows.Controls;
using PixelWallE.ViewModels;

namespace PixelWallE.Controls
{
    public partial class CanvasControl : UserControl
    {
        public CanvasControl()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(MainViewModel.CanvasPixels))
                        {
                            // Actualizar directamente el control con nombre
                            var binding = itemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty);
                            binding?.UpdateTarget();
                        }
                    };
                }
            };
        }
    }
}