using System.Windows;
using WallE.PixelArt.Views;

namespace WallE.PixelArt
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Inicialización de la ventana principal
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}