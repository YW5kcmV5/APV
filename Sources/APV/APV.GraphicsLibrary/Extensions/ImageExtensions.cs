using System.Drawing;
using APV.GraphicsLibrary.Images;

namespace APV.GraphicsLibrary.Extensions
{
    public static class ImageExtensions
    {
        public static BitmapGrayMatrix ToBitmapMatrix(this Image image)
        {
            return BitmapGrayMatrix.FromBitmap(image as Bitmap);
        }
    }
}