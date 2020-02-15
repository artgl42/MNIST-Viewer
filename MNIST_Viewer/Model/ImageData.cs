using System.Windows.Media.Imaging;
using MNIST_Viewer.Toolbox;

namespace MNIST_Viewer.Model
{
    public struct ImageData
    {
        public ImageData(byte[] pixels, byte label)
        {
            Pixels = pixels;
            Label = label;
        }

        public byte[] Pixels { get; }
        public byte Label { get; }

        public WriteableBitmap Image
        {
            get { return Pixels?.ToWriteableBitmap(); }
        }
    }
}
