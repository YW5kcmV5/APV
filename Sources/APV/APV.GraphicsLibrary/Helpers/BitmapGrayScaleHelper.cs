using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace APV.GraphicsLibrary.Helpers
{
    public static class BitmapGrayScaleHelper
    {
        private static readonly ColorMatrix GrayColorMatrix = new ColorMatrix(
            new[]
            {
                new[] {.3f, .3f, .3f, 0, 0},
                new[] {.59f, .59f, .59f, 0, 0},
                new[] {.11f, .11f, .11f, 0, 0},
                new[] {0f, 0, 0, 1, 0},
                new[] {0f, 0, 0, 0, 1}
            });

        /// <summary>
        /// Преобразует цветное изображение в изображение с градацией серого
        /// Очень быстрый вариант
        /// </summary>
        /// <remarks>
        /// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale&usg=ALkJrhhTK4lxONsZJYBeCDOh35WF86_Z2w
        /// http://translate.google.ru/translate?hl=ru&sl=en&tl=ru&u=http%3A%2F%2Fwww.c-sharpcorner.com%2FUploadFile%2Fmahesh%2FTransformations0512192005050129AM%2FTransformations05.aspx&anno=2
        /// </remarks>
        public static Bitmap MakeGrayScale(Bitmap from)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            var newBitmap = new Bitmap(from.Width, from.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {

                var imgAttr = new ImageAttributes();
                imgAttr.SetColorMatrix(GrayColorMatrix);

                // Рисуем исходное изображение на новое изображение
                // С помощью оттенков серого цвета матрицы
                var dest = new Rectangle(0, 0, from.Width, from.Height);
                g.DrawImage(from, dest, 0, 0, from.Width, from.Height, GraphicsUnit.Pixel, imgAttr);

                return newBitmap;
            }
        }
    }
}