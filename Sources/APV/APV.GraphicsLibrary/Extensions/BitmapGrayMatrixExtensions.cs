using APV.GraphicsLibrary.Images;

namespace APV.GraphicsLibrary.Extensions
{
    public static class BitmapGrayMatrixExtensions
    {
        public static BitmapGrayMatrix AddBorder(this BitmapGrayMatrix bitmap, int dx, int dy, int color = 255)
        {
            return BitmapGrayMatrixUtility.AddBorder(bitmap, dx, dy, color);
        }

        public static BitmapGrayMatrix Convolution(BitmapGrayMatrix bitmap, int[,] kernel, bool inverse = false)
        {
            return BitmapGrayMatrixUtility.Convolution(bitmap, kernel, inverse);
        }
    }
}