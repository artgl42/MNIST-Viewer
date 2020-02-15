using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MNIST_Viewer.Toolbox
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                string[] hexNums = stringValue.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                byte[] pixels = new byte[hexNums.Length];

                for (int i = 0; i < hexNums.Length; i++)
                {     
                    pixels[i] = byte.TryParse(hexNums[i], NumberStyles.HexNumber, culture, out byte hexNum) ? hexNum : default(byte);
                }

                return pixels.ToWriteableBitmap();
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
