namespace MNIST_Viewer.Model
{
    interface IMNISTLoader
    {
        ImageData[] LoadImages(string imagesFilePath, string labelsFilePath);
        ImageData[] LoadImages(string imagesFilePath, string labelsFilePath, ushort imagesCount);
        ImageData[] LoadImages(string imagesFilePath, string labelsFilePath, ushort beginImageNumber, ushort? lastImageNumber);
    }
}
