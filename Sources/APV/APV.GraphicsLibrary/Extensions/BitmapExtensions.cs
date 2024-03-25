using System.Drawing;
using APV.GraphicsLibrary.Helpers;
using APV.GraphicsLibrary.Images;

namespace APV.GraphicsLibrary.Extensions
{
    public static class BitmapExtensions
    {
        public static Bitmap MakeGrayScale(this Bitmap from)
        {
            return BitmapGrayScaleHelper.MakeGrayScale(from);
        }

        public static BitmapGrayMatrix ToBitmapMatrix(this Bitmap bitmap)
        {
            return BitmapGrayMatrix.FromBitmap(bitmap);
        }
    }
}