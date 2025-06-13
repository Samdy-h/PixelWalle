using System.Windows;
using System.Windows.Controls;
using PixelWallE.ViewModels;

namespace PixelWallE.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.UpdateLineNumbers();
            }
        }
    }
}