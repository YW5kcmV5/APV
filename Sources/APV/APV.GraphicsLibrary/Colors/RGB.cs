using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace APV.GraphicsLibrary.Colors
{
    [DebuggerDisplay("{Code} ({R}, {G}, {B})")]
    public sealed class RGB : IEquatable<RGB>
    {
        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;
        private readonly int _hashCode;
        private readonly string _code;

        private static bool Parse(string code, out byte r, out byte g, out byte b)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                code = code.Trim();
                if ((code.Length == 7) && (code[0] == '#'))
                {
                    code = code.Substring(1);
                }
                if (code.Length == 6)
                {
                    if ((int.TryParse(code, NumberStyles.HexNumber, null, out var value)) && (value >= 0) &&
                        (value <= 16777215))
                    {
                        r = (byte) ((value & 0xFF0000) >> 16);
                        g = (byte) ((value & 0x00FF00) >> 8);
                        b = (byte) (value & 0x0000FF);
                        return true;
                    }
                }
                string[] subitems = code.Split(new[] {",", ";", " ", "-"}, StringSplitOptions.RemoveEmptyEntries);
                subitems = subitems.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
                if (subitems.Length == 3)
                {
                    if ((byte.TryParse(subitems[0], out byte rValue)) &&
                        (byte.TryParse(subitems[1], out byte gValue)) &&
                        (byte.TryParse(subitems[2], out byte bValue)))
                    {
                        r = rValue;
                        g = gValue;
                        b = bValue;
                        return true;
                    }
                }
            }
            r = 0;
            g = 0;
            b = 0;
            return false;
        }

        private static int CalcHash(byte r, byte g, byte b)
        {
            return b + (g << 8) + (r << 16);
        }

        private static string CalcCode(byte r, byte g, byte b)
        {
            int value = b + (g << 8) + (r << 16);
            return $"#{value:X6}";
        }

        public RGB(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            _hashCode = CalcHash(r, g, b);
            _code = CalcCode(r, g, b);
        }

        public RGB(string code)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentOutOfRangeException(nameof(code), "Color mnemonic name is empty or white space.");

            bool success = Parse(code, out _r, out _g, out _b);

            if (!success)
                throw new ArgumentOutOfRangeException(nameof(code), $"Color mnemonic name should be in \"#FFFFFF\" (\"#RRGGBB\") or \"NNN, NNN, NNN\" (\"RRR, GGG BBB\")  formats, but \"{code}\".");

            _hashCode = CalcHash(_r, _g, _b);
            _code = CalcCode(_r, _g, _b);
        }
        
        public RGB(Color color)
            : this(color.R, color.G, color.B)
        {
        }

        public byte R
        {
            get { return _r; }
        }

        public byte G
        {
            get { return _g; }
        }

        public byte B
        {
            get { return _b; }
        }

        public string Code
        {
            get { return _code; }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RGB);
        }

        public bool Equals(RGB other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return (_hashCode == other._hashCode);
        }

        public override string ToString()
        {
            return _code;
        }

        public static RGB Parse(string code)
        {
            if (Parse(code, out byte r, out byte g, out byte b))
            {
                return new RGB(r, g, b);
            }
            return null;
        }

        public static bool operator ==(RGB x, RGB y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            return (x.Equals(y));
        }

        public static bool operator !=(RGB x, RGB y)
        {
            return !(x == y);
        }
    }
}