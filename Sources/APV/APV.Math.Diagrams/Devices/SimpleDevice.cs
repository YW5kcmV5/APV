using System.Drawing;
using System.Windows.Forms;

namespace APV.Math.Diagrams.Devices
{
	public sealed class SimpleDevice : IGraphDevice
	{
		private readonly object _container;
		private Bitmap _bitmap;
		private Graphics _graphics;
		private Graphics _graph;

		private static Color Invert(Color colorToInvert)
		{
			return Color.FromArgb((byte)~colorToInvert.R, (byte)~colorToInvert.G, (byte)~colorToInvert.B);
		}

		private static Graphics GetGraphics(object container)
		{
			if (container is Image)
			{
				return Graphics.FromImage((Image)container);
			}
			if (container is Control)
			{
				return ((Control) container).CreateGraphics();
			}
			return null;
		}

		public int Width { get { return _bitmap.Width; } }

		public int Height { get { return _bitmap.Height; } }

        public object Container
        {
            get { return _container; }
        }

		public void DrawCircle(int x0, int y0, int d, Pen pen)
		{
			_graph.DrawEllipse(pen, x0, y0, d, d);
		}

		public void DrawLine(int x0, int y0, int x1, int y1, Pen pen)
		{
			_graph.DrawLine(pen, x0, y0, x1, y1);
		}

		public void DrawReactangle(int x1, int y1, int width, int height, Pen pen)
		{
			_graph.DrawRectangle(pen, x1, y1, width, height);
		}

		public void DrawReversibleRectangle(int x0, int y0, int x1, int y1)
		{
			if (x1 < x0)
			{
				Utility.Swap(ref x0, ref x1);
			}
			if (y1 < y0)
			{
				Utility.Swap(ref y0, ref y1);
			}
			x0 = x0 < 0 ? 0 : x0;
			x1 = x1 >= Width ? Width - 1 : x1;
			y0 = y0 <= 0 ? 1 : y0;
			y1 = y1 >= Height ? Height - 1 : y1;

			for (int x = x0; x <= x1; x++)
			{
				_bitmap.SetPixel(x, y0, Invert(_bitmap.GetPixel(x, y0)));
				_bitmap.SetPixel(x, y1, Invert(_bitmap.GetPixel(x, y1)));
			}
			for (int y = y0; y <= y1; y++)
			{
				_bitmap.SetPixel(x0, y, Invert(_bitmap.GetPixel(x0, y)));
				_bitmap.SetPixel(x1, y, Invert(_bitmap.GetPixel(x1, y)));
			}
		}

		public void DrawReversibleCross(int x, int y)
		{
			for (int i = 0; i < Width; i++)
			{
				_bitmap.SetPixel(i, y, Invert(_bitmap.GetPixel(i, y)));
			}
			for (int i = 0; i < Height; i++)
			{
				_bitmap.SetPixel(x, i, Invert(_bitmap.GetPixel(x, i)));
			}
		}

        public void SetPixel(int x, int y, Color color)
        {
            _graph.DrawLine(new Pen(color), x, y, x + 1, y + 1);
        }

		public void Clear(Color color)
		{
			_graph.Clear(color);
		}
		
		public void StartDrawing()
		{
			_graph = _graph ?? Graphics.FromImage(_bitmap);
		}
		
		public void StopDrawing()
		{
			_graphics.DrawImage(_bitmap, 0, 0, Width, Height);
			_graph = null;
		}
		
		public SimpleDevice(Image container)
		{
			_container = container;
			_graphics = GetGraphics(container);
			_bitmap = new Bitmap(container.Width, container.Height);
		}

		public SimpleDevice(Control container, int width, int height)
		{
			_container = container;
			_graphics = GetGraphics(container);
			_bitmap = new Bitmap(width, height);
        }

        public SimpleDevice(Control container)
            : this(container, container.Width, container.Height)
        {
        }

        public void Resize(int width, int height)
		{
			width = width > 0 ? width : 1;
			height = height > 0 ? height : 1;
			if ((width > Width) || (height > Height))
			{
				_graphics = GetGraphics(_container);
			}
			_bitmap = new Bitmap(width, height);
		}
	}
}
