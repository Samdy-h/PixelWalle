using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WallE.PixelArt.Utils
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Red" => Brushes.Red,
                "Blue" => Brushes.Blue,
                "Green" => Brushes.Green,
                "Yellow" => Brushes.Yellow,
                "Orange" => Brushes.Orange,
                "Purple" => Brushes.Purple,
                "Black" => Brushes.Black,
                "White" => Brushes.White,
                "Transparent" => Brushes.Transparent,
                _ => Brushes.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}