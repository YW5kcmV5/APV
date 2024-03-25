using System;
using System.Drawing;
using System.Drawing.Imaging;
using APV.Common.Extensions;
using APV.Math.MathObjects.Matrixes;
using APV.Math.MathObjects.Vectors;

namespace APV.GraphicsLibrary.Images
{
    public sealed class BitmapGrayMatrix : IntMatrix
    {
        public BitmapGrayMatrix(int width, int height)
            : base(height, width)
        {
        }

        public BitmapGrayMatrix(int[] value, int rowsCount, int columnsCount)
            : base(value, rowsCount, columnsCount)
        {
        }

        public BitmapGrayMatrix(IntMatrix matrix)
            : base(matrix)
        {
        }

        public BitmapGrayMatrix(int[,] value)
            : base(value)
        {
        }

        public int Width
        {
            get { return ColumnsCount; }
        }

        public int Height
        {
            get { return RowsCount; }
        }

        public string GetChecksum()
        {
            int[] value = Value;
            int length = sizeof(int) * value.Length;
            var data = new byte[length];

            Buffer.BlockCopy(value, 0, data, 0, length);

            return data.GetChecksum();
        }

        public new BitmapGrayMatrix CutRows(int index, int length)
        {
            return new BitmapGrayMatrix(base.CutRows(index, length));
        }

        public new BitmapGrayMatrix CutColumns(int index, int length)
        {
            return new BitmapGrayMatrix(base.CutColumns(index, length));
        }

        public new BitmapGrayMatrix MoveColumn(IntVector x, bool clone = true)
        {
            IntMatrix result = base.MoveColumn(x, clone);
            return (clone) ? new BitmapGrayMatrix(result) : this;
        }

        public BitmapGrayMatrix MoveColumn(int defaultValue = 0, bool clone = true)
        {
            IntMatrix result = base.MoveColumn(defaultValue);
            return (clone) ? new BitmapGrayMatrix(result) : this;
        }

        public new BitmapGrayMatrix MoveRow(IntVector x, bool clone = true)
        {
            IntMatrix result = base.MoveRow(x, clone);
            return (clone) ? new BitmapGrayMatrix(result) : this;
        }

        public BitmapGrayMatrix MoveRow(int defaultValue = 0, bool clone = true)
        {
            IntMatrix result = base.MoveRow(defaultValue);
            return (clone) ? new BitmapGrayMatrix(result) : this;
        }

        public override object Clone()
        {
            return new BitmapGrayMatrix(this);
        }

        public static BitmapGrayMatrix FromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                bitmap = bitmap.SetPixelFormat(PixelFormat.Format24bppRgb);
            }
                //throw new ArgumentOutOfRangeException(nameof(bitmap), "The pixel format should be only Format24bppRgb.");

            int width = bitmap.Width;
            int height = bitmap.Height;
            var result = new BitmapGrayMatrix(width, height);
            var rect = new Rectangle(0, 0, width, height);

            unsafe
            {
                //lock the original bitmap in memory
                BitmapData originalData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                try
                {
                    //set the number of bytes per pixel
                    const int pixelSize = 3;

                    for (int y = 0; y < height; y++)
                    {
                        //get the data from the original image
                        byte* oRow = (byte*)originalData.Scan0 + (y * originalData.Stride);

                        for (int x = 0; x < width; x++)
                        {
                            //create the grayscale version
                            int position = x * pixelSize;
                            var grayScale = (byte)(
                                (oRow[position] * .11) + //B
                                (oRow[position + 1] * .59) + //G
                                (oRow[position + 2] * .3)); //R

                            //set the new image's pixel to the grayscale version
                            result[y, x] = grayScale;
                        }
                    }
                }
                finally
                {
                    //unlock the bitmaps
                    bitmap.UnlockBits(originalData);
                }

                return result;
            }
        }

        public static Bitmap ToBitmap(BitmapGrayMatrix bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int width = bitmap.Width;
            int height = bitmap.Height;
            var result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, width, height);

            unsafe
            {
                //lock the new bitmap in memory
                BitmapData newData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                try
                {

                    //set the number of bytes per pixel
                    const int pixelSize = 3;

                    for (int y = 0; y < height; y++)
                    {
                        //get the data from the new image
                        byte* nRow = (byte*) newData.Scan0 + (y * newData.Stride);

                        for (int x = 0; x < width; x++)
                        {
                            //create the grayscale version
                            var grayScale = (byte) bitmap[y, x];

                            //set the new image's pixel to the grayscale version
                            int position = x * pixelSize;
                            nRow[position] = grayScale; //B
                            nRow[position + 1] = grayScale; //G
                            nRow[position + 2] = grayScale; //R
                        }
                    }
                }
                finally
                {

                    //unlock the bitmaps
                    result.UnlockBits(newData);
                }

                return result;
            }
        }
    }
}