using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using APV.Common.Extensions;

namespace APV.GraphicsLibrary.Images
{
    public sealed class IconContainer : IDisposable
    {
        private readonly List<Bitmap> _bitmaps = new List<Bitmap>();
        private byte[] _byteArray;
        private byte[] _hashCode;
        private bool _disposed;

        internal enum IconType : short
        {
            /// <summary>
            /// .ICO (для значков)
            /// </summary>
            Ico = 1,

            /// <summary>
            /// .CUR (для курсоров)
            /// </summary>
            Cur = 1,
        }

        internal class IconHeader
        {
            /// <summary>
            /// Зарезервировано. Всегда 0.
            /// </summary>
            public short Reserved { get; set; }

            /// <summary>
            /// Тип файла.
            /// </summary>
            public IconType Type { get; set; }

            /// <summary>
            /// Количество изображений в файле, минимум 1.
            /// </summary>
            public short Count { get; set; }

            public bool IsCorrect()
            {
                return (Reserved == 0) && ((Type == IconType.Ico) || (Type == IconType.Cur)) && (Count > 0);
            }

            public void Read(BinaryReader reader)
            {
                Reserved = reader.ReadInt16();
                Type = (IconType) reader.ReadInt16();
                Count = reader.ReadInt16();
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(Reserved);
                writer.Write((short)Type);
                writer.Write(Count);
            }
        }

        internal class IconEntry
        {
            /// <summary>
            /// Указывает ширину изображения в точках. Может принимать значения от 0 до 255. Если указано 0, то изображение имеет ширину 256 точек.
            /// </summary>
            public byte Width { get; set; }

            /// <summary>
            /// Указывает высоту изображения в точках. Может принимать значения от 0 до 255. Если указано 0, то изображение имеет высоту 256 точек.
            /// </summary>
            public byte Height { get; set; }

            /// <summary>
            /// Указывает количество цветов в палитре изображения. Для полноцветных значков должно быть 0.
            /// </summary>
            public byte Colors { get; set; }

            /// <summary>
            /// Зарезервировано. Должно быть 0. В технической документации Microsoft указано, что это значение всегда должно быть 0, однако значки,
            ///  которые создаются встроенными средствами .NET (System.Drawing.Icon.Save) содержат в этом поле значение 255.
            /// </summary>
            public byte Reserved { get; set; }

            /// <summary>
            /// В .ICO определяет количество плоскостей. Может быть 0 или 1.
            /// В .CUR определяет горизонтальную координату "горячей точки" в пикселях относительно левого края изображения.
            /// </summary>
            public short Planes { get; set; }

            /// <summary>
            /// В .ICO определяет количество битов на пиксель (bits-per-pixel).
            ///   Это значение может быть 0, так как легко получается из других данных; например, если изображение не хранится в формате PNG,
            ///   тогда количество битов на пиксель рассчитывается на основе информации о размере растра, а также его ширине и высоте.
            ///   Если же изображение хранится в формате PNG, то соответствующая информация хранится в самом PNG.
            ///   Однако указывать в этом поле 0 не рекомендуется, так как логика выбора наилучшего изображения в различных версиях Windows неизвестна.
            /// В .CUR определяет вертикальную координату "горячей точки" в пикселях относительно верхнего края изображения.
            /// </summary>
            public short BitsPerPixel { get; set; }

            /// <summary>
            /// Указывает размер растра в байтах
            /// </summary>
            public int Size { get; set; }

            /// <summary>
            /// Указывает абсолютное смещение растра в файле.
            /// </summary>
            public int Offset { get; set; }

            public void Read(BinaryReader reader)
            {
                Width = reader.ReadByte();
                Height = reader.ReadByte();
                Colors = reader.ReadByte();
                Reserved = reader.ReadByte();
                Planes = reader.ReadInt16();
                BitsPerPixel = reader.ReadInt16();
                Size = reader.ReadInt32();
                Offset = reader.ReadInt32();
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Colors);
                writer.Write(Reserved);
                writer.Write(Planes);
                writer.Write(BitsPerPixel);
                writer.Write(Size);
                writer.Write(Offset);
            }
        }

        private Bitmap GetBitmap(IconEntry entry, IconHeader header, Stream source)
        {
            const short number = 1;
            const int offset = 22;

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(header.Reserved);
                    writer.Write((short) header.Type);
                    writer.Write(number);
                    writer.Write(entry.Width);
                    writer.Write(entry.Height);
                    writer.Write(entry.Colors);
                    writer.Write(entry.Reserved);
                    writer.Write(entry.Planes);
                    writer.Write(entry.BitsPerPixel);
                    writer.Write(entry.Size);
                    writer.Write(offset);

                    var buffer = new byte[entry.Size];
                    source.Position = entry.Offset;
                    source.Read(buffer, 0, entry.Size);
                    writer.Write(buffer);
                    writer.Flush();

                    return (Bitmap)Image.FromStream(stream);
                }
            }
        }

        private void Parse(Stream stream)
        {
            try
            {
                _bitmaps.Clear();
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var header = new IconHeader();
                    header.Read(reader);

                    if (!header.IsCorrect())
                    {
                        var bitmap = (Bitmap) Image.FromStream(stream);
                        _bitmaps.Add(bitmap);
                        return;
                    }

                    int count = header.Count;

                    var entries = new List<IconEntry>();
                    for (int i = 0; i < count; i++)
                    {
                        var entry = new IconEntry();
                        entry.Read(reader);
                        entries.Add(entry);
                    }

                    foreach (IconEntry entry in entries)
                    {
                        Bitmap bitmap = GetBitmap(entry, header, stream);
                        _bitmaps.Add(bitmap);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FormatException("Specified data does not correspond to icon format.", ex);
            }
        }

        private static byte[] Save(Bitmap[] images)
        {
            int length = images.Length;
            var entries = new Dictionary<IconEntry, byte[]>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    // Write header
                    var header = new IconHeader
                        {
                            Reserved = 0,
                            Type = IconType.Ico,
                            Count = Convert.ToInt16(length)
                        };
                    header.Write(writer);

                    // The offset of the first icon.
                    var offset = 6 + length*16;

                    // Write all the icon entries
                    for (var i = 0; i < length; i++)
                    {
                        Bitmap image = images[i];

                        // This extension method saves an Image to a png-format byte array.
                        byte[] data = image.ToByteArray(ImageFormat.Png);

                        var entry = new IconEntry
                            {
                                Width = image.Width < 256 ? Convert.ToByte(image.Width) : (byte) 0,
                                Height = image.Height < 256 ? Convert.ToByte(image.Height) : (byte) 0,
                                Colors = 0,
                                Reserved = 0,
                                Planes = 1,
                                BitsPerPixel = 32,
                                Size = data.Length,
                                Offset = offset,
                            };

                        entry.Write(writer);

                        offset += data.Length;

                        entries.Add(entry, data);
                    }

                    // Write the Icons.
                    foreach (KeyValuePair<IconEntry, byte[]> keyValuePair in entries)
                    {
                        IconEntry iconEntry = keyValuePair.Key;
                        byte[] data = keyValuePair.Value;
                        writer.Seek(iconEntry.Offset, SeekOrigin.Begin);
                        writer.Write(data);
                    }
                }
                return stream.ToArray();
            }
        }

        public IconContainer()
        {
        }

        public IconContainer(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Parse(stream);
            }
        }

        public IconContainer(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            using (var stream = new MemoryStream(data))
            {
                Parse(stream);
            }
        }

        public IconContainer(Bitmap bitmap)
        {
            Add(bitmap);
        }

        public IconContainer(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Parse(stream);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                foreach (Bitmap bitmap in _bitmaps)
                {
                    bitmap.Dispose();
                }
                _bitmaps.Clear();

                _byteArray = null;
                _hashCode = null;
            }
        }

        ~IconContainer()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public Bitmap[] GetBitmaps()
        {
            return _bitmaps.OrderBy(bitmap => bitmap.Width > bitmap.Height ? bitmap.Width : bitmap.Height).ToArray();
        }

        public void Add(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");
            if ((bitmap.Width <= 0) || (bitmap.Width > 256))
                throw new ArgumentOutOfRangeException("bitmap", "((bitmap.Width <= 0) || (bitmap.Width > 256))");
            if ((bitmap.Height <= 0) || (bitmap.Height > 256))
                throw new ArgumentOutOfRangeException("bitmap", "((bitmap.Height <= 0) || (bitmap.Height > 256))");

            Bitmap copy = bitmap.SetPixelFormat(PixelFormat.Format32bppArgb);
            _bitmaps.Add(copy);

            _byteArray = null;
            _hashCode = null;
        }

        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            byte[] data = Save(_bitmaps.ToArray());
            File.WriteAllBytes(path, data);
        }

        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] data = Save(_bitmaps.ToArray());
            stream.Write(data, 0, data.Length);
        }

        public byte[] ToByteArray()
        {
            return _byteArray ?? (_byteArray = Save(_bitmaps.ToArray()));
        }

        public new byte[] GetHashCode()
        {
            return _hashCode ?? (_hashCode = ToByteArray().Hash256());
        }

        public static Bitmap[] Extract(string path)
        {
            var container = new IconContainer(path);
            return container.GetBitmaps();
        }

        public static Bitmap[] Extract(byte[] data)
        {
            var container = new IconContainer(data);
            return container.GetBitmaps();
        }

        public static byte[] ToByteArray(Bitmap bitmap)
        {
            using (var container = new IconContainer(bitmap))
            {
                return container.ToByteArray();
            }
        }

        public static byte[] GetHashCode(Bitmap bitmap)
        {
            using (var container = new IconContainer(bitmap))
            {
                return container.GetHashCode();
            }
        }

        public static Bitmap ToIconFormat(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            byte[] data = ToByteArray(bitmap);
            using (var stream = new MemoryStream(data))
            {
                return (Bitmap)Image.FromStream(stream);
            }
        }
    }
}