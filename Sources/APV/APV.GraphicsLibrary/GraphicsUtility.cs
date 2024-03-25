using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using APV.Common;
using APV.GraphicsLibrary.Images;

namespace APV.GraphicsLibrary
{
    public static class GraphicsUtility
    {
        public static global::System.Drawing.Imaging.ImageFormat Convert(this ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.Png:
                    return global::System.Drawing.Imaging.ImageFormat.Png;
                case ImageFormat.Bmp:
                    return global::System.Drawing.Imaging.ImageFormat.Bmp;
                case ImageFormat.Emf:
                    return global::System.Drawing.Imaging.ImageFormat.Emf;
                case ImageFormat.Exif:
                    return global::System.Drawing.Imaging.ImageFormat.Exif;
                case ImageFormat.Gif:
                    return global::System.Drawing.Imaging.ImageFormat.Gif;
                case ImageFormat.Icon:
                    return global::System.Drawing.Imaging.ImageFormat.Icon;
                case ImageFormat.Jpeg:
                    return global::System.Drawing.Imaging.ImageFormat.Jpeg;
                case ImageFormat.Tiff:
                    return global::System.Drawing.Imaging.ImageFormat.Tiff;
                case ImageFormat.Wmf:
                    return global::System.Drawing.Imaging.ImageFormat.Wmf;
                case ImageFormat.MemoryBmp:
                    return global::System.Drawing.Imaging.ImageFormat.MemoryBmp;
                default:
                    throw new NotSupportedException(string.Format("Unknown image format \"{0}\".", format));
            }
        }

        public static ImageFormat Convert(this global::System.Drawing.Imaging.ImageFormat format)
        {
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Png))
            {
                return ImageFormat.Png;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Bmp))
            {
                return ImageFormat.Bmp;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Emf))
            {
                return ImageFormat.Emf;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Exif))
            {
                return ImageFormat.Exif;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Gif))
            {
                return ImageFormat.Gif;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Icon))
            {
                return ImageFormat.Icon;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                return ImageFormat.Jpeg;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Tiff))
            {
                return ImageFormat.Tiff;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.Wmf))
            {
                return ImageFormat.Wmf;
            }
            if (format.Equals(global::System.Drawing.Imaging.ImageFormat.MemoryBmp))
            {
                return ImageFormat.MemoryBmp;
            }
            throw new NotSupportedException(string.Format("Unknown image format \"{0}\".", format));
        }

        public static byte[] CalcHashCode(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            return ImageContainer.CalcHash(bitmap);
        }

        public static byte[] CalcHashCode(this Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return ((Bitmap) image).CalcHashCode();
        }

        public static Bitmap CopyToMemory(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int width = bitmap.Width;
            int height = bitmap.Height;
            PixelFormat pixelFormat = bitmap.PixelFormat;
            var rect = new Rectangle(0, 0, width, height);
            var result = new Bitmap(width, height, pixelFormat);
            result.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);
            BitmapData sourceData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);
            int length = sourceData.Stride * height;
            IntPtr sourcePtr = sourceData.Scan0;
            IntPtr resultPtr = resultData.Scan0;
            Win32.CopyMemory(resultPtr, sourcePtr, length);
            bitmap.UnlockBits(sourceData);
            result.UnlockBits(resultData);

            return result;
        }

        public static byte[] ToByteArray(this Bitmap bitmap, ImageFormat? imageFormat = null, PixelFormat? pixelFormat = null)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            imageFormat = imageFormat ?? bitmap.RawFormat.Convert();
            pixelFormat = pixelFormat ?? bitmap.PixelFormat;

            if (imageFormat == ImageFormat.Icon)
            {
                return IconContainer.ToByteArray(bitmap);
            }

            if (imageFormat == ImageFormat.MemoryBmp)
            {
                imageFormat = ImageFormat.Png;
            }

            if (bitmap.RawFormat.Convert() == ImageFormat.Icon)
            {
                using (var copy = new Bitmap(bitmap))
                {
                    return copy.ToByteArray(imageFormat, pixelFormat);
                }
            }

            if (bitmap.PixelFormat != pixelFormat)
            {
                using (var copy = bitmap.SetPixelFormat(pixelFormat.Value))
                {
                    return copy.ToByteArray(imageFormat, pixelFormat);
                }
            }

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, imageFormat.Value.Convert());
                return ms.ToArray();
            }
        }

        public static Image ToImage(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (var ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        public static Bitmap SetPixelFormat(this Bitmap bitmap, PixelFormat pixelFormat)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if ((bitmap.Width == 0) || (bitmap.Height == 0))
                throw new ArgumentOutOfRangeException(nameof(bitmap), "((bitmap.Width == 0) || (bitmap.Height == 0))");

            if (pixelFormat == bitmap.PixelFormat)
            {
                return (Bitmap)bitmap.Clone();
            }

            //// Don't try to draw a new Bitmap with an indexed pixel format.
            if ((pixelFormat == PixelFormat.Format1bppIndexed) || (pixelFormat == PixelFormat.Format4bppIndexed) ||
                (pixelFormat == PixelFormat.Format8bppIndexed) || (pixelFormat == PixelFormat.Indexed))
            {
                return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), pixelFormat);
            }

            Bitmap result = null;
            try
            {
                //a holder for the result
                //result = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
                result = new Bitmap(bitmap.Width, bitmap.Height, pixelFormat);
                // set the resolutions the same to avoid cropping due to resolution differences
                result.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                //use a graphics object to draw the resized image into the bitmap
                using (Graphics graphics = Graphics.FromImage(result))
                {
                    //set the resize quality modes to high quality
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias; //SmoothingMode.HighQuality;
                    //draw the image into the target bitmap
                    graphics.DrawImage(bitmap, 0, 0, result.Width, result.Height);
                }

                //return the resulting bitmap
                return result;
            }
            catch (Exception)
            {
                result?.Dispose();
                throw;
            }
        }

        public static Bitmap Convert(this Bitmap bitmap, ImageFormat format)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            byte[] data = bitmap.ToByteArray(format);
            return (Bitmap)data.ToImage();
        }

        public static Bitmap Resize(this Bitmap bitmap, Size size, bool discardRatio = false)
        {
            int sourceWidth = bitmap.Width;
            int sourceHeight = bitmap.Height;

            int destWidth = size.Width;
            int destHeight = size.Height;

            if (discardRatio)
            {
                float nPercentW = ((float)size.Width / sourceWidth);
                float nPercentH = ((float)size.Height / sourceHeight);
                float nPercent = (nPercentH < nPercentW) ? nPercentH : nPercentW;

                destWidth = (int)System.Math.Round(sourceWidth * nPercent);
                destHeight = (int)System.Math.Round(sourceHeight * nPercent);
            }

            var newBitmap = new Bitmap(destWidth, destHeight, bitmap.PixelFormat);
            newBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(newBitmap))
            {
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(bitmap, 0, 0, destWidth, destHeight);
            }

            return newBitmap;
        }

        public static Bitmap Resize(this Bitmap bitmap, int width, int height, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "(width <= 0)");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "(height <= 0)");
            if ((bitmap.Width == 0) || (bitmap.Height == 0))
                throw new ArgumentOutOfRangeException(nameof(bitmap), "((bitmap.Width == 0) || (bitmap.Height == 0))");

            if ((width == bitmap.Width) && (height == bitmap.Height))
            {
                return (Bitmap)bitmap.Clone();
            }

            ImageFormat imageFormat = bitmap.RawFormat.Convert();
            using (var copy = bitmap.Convert(ImageFormat.MemoryBmp))
            {
                //a holder for the result
                using (var scaled = new Bitmap(width, height, copy.PixelFormat))
                {
                    // set the resolutions the same to avoid cropping due to resolution differences
                    scaled.SetResolution(copy.HorizontalResolution, copy.VerticalResolution);

                    //use a graphics object to draw the resized image into the bitmap
                    using (Graphics graphics = Graphics.FromImage(scaled))
                    {
                        //set the resize quality modes to high quality
                        graphics.CompositingQuality = compositingQuality;
                        graphics.InterpolationMode = interpolationMode;
                        graphics.SmoothingMode = smoothingMode;
                        //draw the image into the target bitmap
                        graphics.DrawImage(bitmap, 0, 0, scaled.Width, scaled.Height);
                    }

                    Bitmap result = Convert(scaled, imageFormat);
                    //return the resulting bitmap
                    return result;
                }
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        public static Image Resize(this Image image, int width, int height, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return Resize((Bitmap) image, width, height, interpolationMode, smoothingMode, compositingQuality);
        }

        /// <summary>
        /// Resize the image to the specified size.
        /// </summary>
        public static Image Resize(this Image image, int size)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            int width = image.Width;
            int height = image.Height;

            int currentSize = (width >= height) ? width : height;
            int dSize = size - currentSize;
            int newWidth = image.Width + dSize;
            int newHeight = image.Height + dSize;

            return Resize(image, newWidth, newHeight);
        }

        /// <summary>
        /// Resize the image to the specified size.
        /// </summary>
        public static Image ResizeHeight(this Image image, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            int dHeight = (image.Height - height);
            int newWidth = image.Width - dHeight;
            int newHeight = height;

            return Resize(image, newWidth, newHeight);
        }

        public static Bitmap ApplyMinimumSize(this Bitmap imgToResize, int size = 600)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            if ((sourceWidth > size) && (sourceHeight > size))
            {
                return imgToResize;
            }

            var newSize = new Size();

            if (sourceWidth > sourceHeight)
            {
                newSize.Height = size;
                newSize.Width = (int)System.Math.Round(sourceWidth * ((float)size / sourceHeight));
            }
            else
            {
                newSize.Width = size;
                newSize.Height = (int)System.Math.Round(sourceHeight * ((float)size / sourceWidth));
            }

            return Resize(imgToResize, newSize);
        }

        public static Bitmap ApplyMaximumSize(this Bitmap imgToResize, int size = 600)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            if ((sourceWidth <= size) && (sourceHeight <= size))
            {
                return imgToResize;
            }

            var newSize = new Size();

            if (sourceWidth > sourceHeight)
            {
                newSize.Height = size;
                newSize.Width = (int)System.Math.Round(sourceWidth * ((float)size / sourceHeight));
            }
            else
            {
                newSize.Width = size;
                newSize.Height = (int)System.Math.Round(sourceHeight * ((float)size / sourceWidth));
            }

            return Resize(imgToResize, newSize);
        }

        public static bool Equals(Image x, Image y)
        {
            if (x == y)
            {
                return true;
            }

            if ((x == null) || (y == null) || (x.Width != y.Width) || (x.Height != y.Height) || (x.PixelFormat != y.PixelFormat) || (!x.RawFormat.Equals(y.RawFormat)))
            {
                return false;
            }

            byte[] xData = x.CalcHashCode();
            byte[] yData = y.CalcHashCode();

            return Equals(xData, yData);
        }
    }
}