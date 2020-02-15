using System.Collections.Generic;
using System.IO;

namespace MNIST_Viewer.Model
{
    internal class MNISTLoader : IMNISTLoader
    {
        private const int StartBytePositionOfImage = 16;
        private const int ByteCountOfImage = 784;
        private const int StartBytePositionOfLabel = 8;
        private const int ByteCountOfLabel = 1;

        public ImageData[] LoadImages(string imagesFilePath, string labelsFilePath)
        {
            return LoadImages(imagesFilePath, labelsFilePath, 0, null);
        }

        public ImageData[] LoadImages(string imagesFilePath, string labelsFilePath, ushort imagesCount)
        {
            return LoadImages(imagesFilePath, labelsFilePath, 0, imagesCount);
        }

        public ImageData[] LoadImages(string imagesFilePath, string labelsFilePath, ushort beginImageNumber, ushort? lastImageNumber)
        {
            var startPosition = (uint)(StartBytePositionOfImage + ByteCountOfImage * beginImageNumber);            
            var endPosition = lastImageNumber != null ? StartBytePositionOfImage + ByteCountOfImage * lastImageNumber : null;
            var pixels = ReadBytes(imagesFilePath, ByteCountOfImage, startPosition, (uint)endPosition);

            startPosition = (uint)(StartBytePositionOfLabel + ByteCountOfLabel * beginImageNumber);
            endPosition = lastImageNumber != null ? StartBytePositionOfLabel + ByteCountOfLabel * lastImageNumber : null;
            var labels = ReadBytes(labelsFilePath, ByteCountOfLabel, startPosition, (uint)endPosition);

            if (pixels != null && labels != null)
            {
                var digitalImages = new ImageData[pixels.Count];
                for (int i = 0; i < digitalImages.Length; i++)
                {
                    digitalImages[i] = new ImageData(pixels[i], labels[i][0]);
                }

                return digitalImages;
            }
            return null;
        }

        private List<byte[]> ReadBytes(string filePath, uint byteCount, uint startPosition, uint? endPosition)
        {
            if (File.Exists(filePath) && byteCount <= int.MaxValue)
            {
                using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    List<byte[]> data = new List<byte[]>();
                    reader.BaseStream.Position = startPosition;

                    if (endPosition != null)
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length && reader.BaseStream.Position < endPosition)
                        {
                            data.Add(reader.ReadBytes((int)byteCount));
                        }
                    }
                    else
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            data.Add(reader.ReadBytes((int)byteCount));
                        }
                    }
                    return data;
                }
            }
            return null;
        }
    }
}
