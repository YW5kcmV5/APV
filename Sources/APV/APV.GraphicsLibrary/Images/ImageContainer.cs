using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using APV.Common;
using APV.Common.Extensions;

namespace APV.GraphicsLibrary.Images
{
    //[Serializable]
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    [XmlRoot(Namespace = SystemConstants.NamespaceData)]
    public sealed class ImageContainer : ICloneable, IDisposable
    {
        private int _width;
        private int _height;
        private float _horizontalResolution;
        private float _verticalResolution;
        private PixelFormat _pixelFormat;
        private ImageFormat _imageFormat;
        private Bitmap _memoryBitmap;
        private byte[] _byteArray;
        private byte[] _hashCode;
        private bool _disposed;

        private Bitmap Copy(Bitmap bitmap)
        {
            Bitmap result;

            PixelFormat pixelFormat = bitmap.PixelFormat;
            if ((pixelFormat != PixelFormat.Format24bppRgb) &&
                (pixelFormat == PixelFormat.Format32bppArgb) &&
                (pixelFormat == PixelFormat.Format32bppPArgb) &&
                (pixelFormat == PixelFormat.Format32bppRgb) &&
                (pixelFormat == PixelFormat.Format64bppArgb) &&
                (pixelFormat == PixelFormat.Format64bppPArgb))
            {
                pixelFormat = PixelFormat.Format32bppArgb;
                result = bitmap.SetPixelFormat(pixelFormat);
            }
            else
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                var rect = new Rectangle(0, 0, width, height);
                result = new Bitmap(width, height, pixelFormat);
                result.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

                BitmapData sourceData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);
                BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);
                int length = sourceData.Stride * height;
                IntPtr sourcePtr = sourceData.Scan0;
                IntPtr resultPtr = resultData.Scan0;
                Win32.CopyMemory(resultPtr, sourcePtr, length);
                result.UnlockBits(resultData);
                bitmap.UnlockBits(sourceData);
            }

            return result;
        }

        private void Fill(Bitmap bitmap, bool dispose)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if ((bitmap.Width == 0) || (bitmap.Height == 0))
                throw new ArgumentOutOfRangeException(nameof(bitmap), "((bitmap.Width == 0) || (bitmap.Height == 0))");

            _width = bitmap.Width;
            _height = bitmap.Height;
            _horizontalResolution = bitmap.HorizontalResolution;
            _verticalResolution = bitmap.VerticalResolution;

            _pixelFormat = bitmap.PixelFormat;
            _imageFormat = bitmap.RawFormat.Convert();

            _memoryBitmap = Copy(bitmap);

            if (dispose)
            {
                bitmap.Dispose();
            }
        }

        private void Fill(byte[] data)
        {
            Image image;
            using (var stream = new MemoryStream(data))
            {
                image = Image.FromStream(stream);
            }
            Fill((Bitmap)image, true);
        }

        public ImageContainer()
        {
        }

        public ImageContainer(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentOutOfRangeException(nameof(filename), "Filename is empty or whitespace.");

            Image image = Image.FromFile(filename);
            Fill((Bitmap)image, true);
        }

        public ImageContainer(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            Fill((Bitmap)image, false);
        }

        public ImageContainer(Bitmap bitmap)
            : this((Image)bitmap)
        {
        }

        public ImageContainer(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Fill((Bitmap)Image.FromStream(stream), true);
        }

        public ImageContainer(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Fill(data);
        }

        public ImageContainer(ImageContainer from)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            _width = from._width;
            _height = from._height;
            _horizontalResolution = from._horizontalResolution;
            _verticalResolution = from._verticalResolution;
            _pixelFormat = from._pixelFormat;
            _imageFormat = from._imageFormat;
            _hashCode = Utility.Copy(@from._hashCode);

            _memoryBitmap = (Bitmap) from._memoryBitmap.Clone();
            _memoryBitmap.SetResolution(_horizontalResolution, _verticalResolution);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_memoryBitmap != null)
                {
                    _memoryBitmap.Dispose();
                    _memoryBitmap = null;
                }
                _hashCode = null;
                _byteArray = null;
            }
            GC.SuppressFinalize(this);
        }

        ~ImageContainer()
        {
            Dispose();
        }

        public byte[] ToBitmapArray()
        {
            var rect = new Rectangle(0, 0, _memoryBitmap.Width, _memoryBitmap.Height);
            BitmapData bitmapData = _memoryBitmap.LockBits(rect, ImageLockMode.ReadOnly, _memoryBitmap.PixelFormat);
            int length = bitmapData.Stride * _memoryBitmap.Height;
            var data = new byte[length];
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, data, 0, length);
            _memoryBitmap.UnlockBits(bitmapData);
            return data;
        }

        public MemoryStream ToStream()
        {
            return new MemoryStream(ToByteArray());
        }

        public byte[] ToByteArray()
        {
            return _byteArray ?? (_byteArray = _memoryBitmap.ToByteArray(_imageFormat, _pixelFormat));
        }

        public new byte[] GetHashCode()
        {
            if (_hashCode == null)
            {
                byte[] data = ToBitmapArray();
                _hashCode = data.Hash256();
            }
            return _hashCode;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        public void Resize(int width, int height, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "(width <= 0)");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "(height <= 0)");

            if ((width == _width) && (height == _height))
            {
                return;
            }

            //a holder for the result
            var scaled = new Bitmap(width, height, _memoryBitmap.PixelFormat);
            // set the resolutions the same to avoid cropping due to resolution differences
            scaled.SetResolution(_horizontalResolution, _verticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(scaled))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = compositingQuality;
                graphics.InterpolationMode = interpolationMode;
                graphics.SmoothingMode = smoothingMode;
                //draw the image into the target bitmap
                graphics.DrawImage(_memoryBitmap, 0, 0, scaled.Width, scaled.Height);
            }

            _memoryBitmap.Dispose();
            _memoryBitmap = scaled;

            _width = width;
            _height = height;
            _hashCode = null;
        }

        /// <summary>
        /// Resize the image to the specified size.
        /// </summary>
        public void Resize(int size, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            int newWidth;
            int newHeight;
            int dWidth = size - _width;
            int dHeight = size - _height;
            if (dWidth >= dHeight)
            {
                newWidth = _width + dHeight;
                newHeight = _height + dHeight;
            }
            else
            {
                newWidth = _width + dWidth;
                newHeight = _height + dWidth;
            }

            Resize(newWidth, newHeight, interpolationMode, smoothingMode, compositingQuality);
        }

        public object Clone()
        {
            return new ImageContainer(this);
        }

        public Bitmap ToBitmap()
        {
            using (MemoryStream stream = ToStream())
            {
                return (Bitmap)Image.FromStream(stream);
            }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int Size
        {
            get { return (Width > Height) ? Width : Height; }
        }

        public float HorizontalResolution
        {
            get { return _horizontalResolution; }
        }

        public float VerticalResolution
        {
            get { return _verticalResolution; }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
            set
            {
                if (_pixelFormat != value)
                {
                    _pixelFormat = value;
                    _hashCode = null;
                }
            }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public ImageFormat ImageFormat
        {
            get { return _imageFormat; }
            set
            {
                if (_imageFormat != value)
                {
                    _imageFormat = value;
                    _hashCode = null;
                }
            }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public ImageFormat FileFormat
        {
            get { return (_imageFormat != ImageFormat.MemoryBmp) ? _imageFormat : ImageFormat.Png; }
            set
            {
                if (value == ImageFormat.MemoryBmp)
                    throw new ArgumentOutOfRangeException(nameof(value), "MemoryBmp image format can no be used to store data into file.");

                if (_imageFormat != value)
                {
                    _imageFormat = value;
                    _hashCode = null;
                }
            }
        }

        [DataMember(Name = "Value", IsRequired = true)]
        [XmlElement(ElementName = "Value", IsNullable = false)]
        public string DataAsBase64String
        {
            get
            {
                byte[] data = ToByteArray();
                return Convert.ToBase64String(data);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));

                byte[] data = Convert.FromBase64String(value);

                if (data.Length == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Data can not be deserialized. Data array is empty.");

                Fill(data);
            }
        }

        public static explicit operator ImageContainer(Bitmap from)
        {
            return (from != null) ? new ImageContainer(from) : null;
        }

        public static explicit operator ImageContainer(Image from)
        {
            return (from != null) ? new ImageContainer(from) : null;
        }

        public static explicit operator Image(ImageContainer from)
        {
            return from?.ToBitmap();
        }

        public static explicit operator Bitmap(ImageContainer from)
        {
            return from?.ToBitmap();
        }

        public static byte[] CalcHash(Image image)
        {
            using (var container = new ImageContainer(image))
            {
                return container.GetHashCode();
            }
        }

        public static byte[] CalcHash(Bitmap bitmap)
        {
            using (var container = new ImageContainer(bitmap))
            {
                return container.GetHashCode();
            }
        }

        public static Bitmap Resize(Bitmap bitmap, int width, int height, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            using (var container = new ImageContainer(bitmap))
            {
                container.Resize(width, height, interpolationMode, smoothingMode, compositingQuality);
                return container.ToBitmap();
            }
        }

        public static Image Resize(Image image, int width, int height, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            using (var container = new ImageContainer(image))
            {
                container.Resize(width, height, interpolationMode, smoothingMode, compositingQuality);
                return container.ToBitmap();
            }
        }

        public static Bitmap Resize(Bitmap bitmap, int size, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            using (var container = new ImageContainer(bitmap))
            {
                container.Resize(size, interpolationMode, smoothingMode, compositingQuality);
                return container.ToBitmap();
            }
        }

        public static Image Resize(Image image, int size, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic, SmoothingMode smoothingMode = SmoothingMode.HighQuality, CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            using (var container = new ImageContainer(image))
            {
                container.Resize(size, interpolationMode, smoothingMode, compositingQuality);
                return container.ToBitmap();
            }
        }
    }
}