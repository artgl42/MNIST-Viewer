using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MNIST_Viewer.Toolbox
{
    public static class MethodsExtension
    {
        public static WriteableBitmap ToWriteableBitmap(this byte[] source)
        {
            ushort imageSize = (ushort) Math.Sqrt(source.Length);
            WriteableBitmap writeableBitmap = new WriteableBitmap(imageSize, imageSize, 96, 96, PixelFormats.Gray8, null);
            Int32Rect rect = new Int32Rect(0, 0, imageSize, imageSize);

            int stride = writeableBitmap.PixelWidth * writeableBitmap.Format.BitsPerPixel / 8;
            byte[] _pixels = new byte[imageSize * imageSize * writeableBitmap.Format.BitsPerPixel / 8];
            int pixelOffset;

            for (int i = 0; i < writeableBitmap.PixelWidth; i++)
            {
                for (int j = 0; j < writeableBitmap.PixelHeight; j++)
                {
                    pixelOffset = (i * writeableBitmap.PixelWidth + j) * writeableBitmap.Format.BitsPerPixel / 8;
                    _pixels[pixelOffset] = (byte)(255 - source[pixelOffset]);
                }

                writeableBitmap.WritePixels(rect, _pixels, stride, 0);
            }

            return writeableBitmap;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ObservableCollection<T>(source);
        }
    }
}
