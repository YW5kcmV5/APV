using System;

namespace APV.GraphicsLibrary.Images
{
    public static class BitmapGrayMatrixUtility
    {
        public static BitmapGrayMatrix AddBorder(BitmapGrayMatrix bitmap, int dx, int dy, int color = 255)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int newWidth = bitmap.Width + 2 * dx;
            int newHeight = bitmap.Height + 2 * dy;

            var value = new int[newHeight, newWidth];
            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    int newColor = color;
                    if ((y - dy >= 0) && (y - dy <= bitmap.Height - 1) &&
                        (x - dx >= 0) && (x - dx <= bitmap.Width - 1))
                    {
                        newColor = bitmap[y - dy, x - dx];
                    }
                    value[y, x] = newColor;
                }
            }

            return new BitmapGrayMatrix(value);
        }

        public static BitmapGrayMatrix Convolution(BitmapGrayMatrix bitmap, int[,] kernel, bool inverse = false)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));

            //Получаем байты изображения
            int[] inputBytes = bitmap.Value;
            var outputBytes = new int[inputBytes.Length];

            int width = bitmap.Width;
            int height = bitmap.Height;

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            //Производим вычисления
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float colorSum = 0;
                    float kSum = 0;

                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int pixelPosX = x + (i - (kernelWidth / 2));
                            int pixelPosY = y + (j - (kernelHeight / 2));

                            if ((pixelPosX < 0) || (pixelPosX >= width) || (pixelPosY < 0) || (pixelPosY >= height))
                            {
                                continue;
                            }

                            int color = inputBytes[(width * pixelPosY + pixelPosX)];

                            float kernelVal = kernel[i, j];

                            colorSum += color * kernelVal;

                            kSum += kernelVal;
                        }
                    }

                    if (kSum <= 0)
                    {
                        kSum = 1;
                    }

                    //Контролируем переполнения переменных
                    colorSum /= kSum;

                    if (inverse)
                    {
                        colorSum = 255 - colorSum;
                    }

                    if (colorSum < 0)
                    {
                        colorSum = 0;
                    }
                    else if (colorSum > 255)
                    {
                        colorSum = 255;
                    }

                    //Записываем значения в результирующее изображение
                    outputBytes[(width * y + x)] = (int)colorSum;
                }
            }

            //Возвращаем отфильтрованное изображение
            return new BitmapGrayMatrix(outputBytes, bitmap.RowsCount, bitmap.ColumnsCount);
        }
    }
}