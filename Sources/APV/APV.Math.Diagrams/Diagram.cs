using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using APV.Math.Diagrams.Devices;

namespace APV.Math.Diagrams
{
	public struct DiagramPoint
	{
        public Diagram Diagram { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public int Index { get; set; }

		public float ValueX { get; set; }

		public float ValueY { get; set; }

		public bool IsIn(int x, int y)
		{
			float dx = Width/2.0f;
			float dy = Height/2.0f;
			return ((x >= X - dx) && (x <= X + dx) && (y >= Y - dy) && (y <= Y + dy));
		}
	}

	public sealed class Diagram
	{
        private readonly IGraphDevice _graph;
        private readonly SimpleDeviceEventManager _eventManager;
        private readonly Diagram _parent;
		private readonly List<Diagram> _children = new List<Diagram>();
		
        private readonly float[] _x;
		private readonly float[] _y;
		private float[] _axisX;
		private float[] _axisY;
		private float _minX;
		private float _maxX;
		private float _minY;
		private float _maxY;

		private int _fromXIndex;
		private int _toXIndex;
		private float _fromX;
		private float _toX;
		private float _fromY;
		private float _toY;
		private readonly int _length;

		private int _mouseX;
		private int _mouseY;
		
		private readonly List<DiagramPoint> _points = new List<DiagramPoint>();
		private DiagramPoint? _selectedPoint;
		private DiagramPoint? _currentPoint;
		private DiagramPoint? _topSelectedPoint;

	    private bool _visible;

	    private void DrawAxes()
		{
			float dx = _graph.Width / (_toX - _fromX);
			float dy = _graph.Height / (_toY - _fromY);

			if (ShowAxisX)
			{
				for (int i = 0; i < _axisX.Length; i++)
				{
					Pen pen = (_axisX[i] == 0.0f) ? ZeroAxisPen : AxisPen;
					float x = dx*(_axisX[i] - _fromX);
					_graph.DrawLine((int) x, 0, (int) x, _graph.Height - 1, pen);
				}
			}

			if (ShowAxisY)
			{
				for (int i = 0; i < _axisY.Length; i++)
				{
					Pen pen = (_axisY[i] == 0.0f) ? ZeroAxisPen : AxisPen;
					float y = dy*(_axisY[i] - _fromY);
					_graph.DrawLine(0, (int) (_graph.Height - y), _graph.Width - 1, (int) (_graph.Height - y), pen);
				}
			}
		}

	    private void DrawPoint(int x, int y, bool selected, out int width, out int height)
		{
            if (_visible)
            {
                if (selected)
                {
                    _graph.DrawCircle(x - 3, y - 3, 5, new Pen(ForePen.Color, 2*ForePen.Width));
                }
                else
                {
                    _graph.DrawCircle(x - 2, y - 2, 4, ForePen);
                }
            }
		    width = 8;
			height = 8;
		}

	    private void DrawDynamicAxes()
		{
			//int y = _currentPoint.Value.Y;
			//_graph.DrawLine(0, y, _graph.Width - 1, y, ForePen);
			_graph.DrawLine(_mouseX, 0, _mouseX, _graph.Height - 1, new Pen(Color.White));
		}

		private bool IsSelected(int index)
		{
			bool selected = (_selectedPoint != null) && (_selectedPoint.Value.Index == index);
			selected &= (_parent == null) || ((_parent._topSelectedPoint != null) && (_parent._topSelectedPoint.Value.Index == index));
			return selected;
		}
		
		public void Draw()
		{
		    Enabled = true;

			_points.Clear();
			if (_parent == null)
			{
				_graph.StartDrawing();
				_graph.Clear(Background);
				DrawAxes();
			}

            float dx = _graph.Width / (_toX - _fromX);
            float dy = _graph.Height / (_toY - _fromY);

            int firstIndex = _fromXIndex;
            float x0 = dx * (_x[firstIndex] - _fromX);
            float y0 = dy * (_y[firstIndex] - _fromY);
            int pointWidth, pointHeight;

            if (ShowPoints)
            {
                var x = (int)x0;
                var y = (int)(_graph.Height - y0);
                y = y < _graph.Height ? y : _graph.Height - 1;
                bool selected = IsSelected(firstIndex);
                DrawPoint(x, y, selected, out pointWidth, out pointHeight);
                _points.Add(new DiagramPoint { Diagram = this, X = x, Y = y, Width = pointWidth, Height = pointHeight, Index = firstIndex, ValueX = _x[firstIndex], ValueY = _y[firstIndex] });
            }

            for (int i = _fromXIndex + 1; i <= _toXIndex; i++)
            {
                float x1 = dx * (_x[i] - _fromX);
                float y1 = dy * (_y[i] - _fromY);

                if (_visible)
                {
                    _graph.DrawLine((int) x0, (int) (_graph.Height - y0), (int) x1, (int) (_graph.Height - y1), ForePen);
                }
                if (ShowPoints)
                {
                    var x = (int)x1;
                    var y = (int)(_graph.Height - y1);
                    y = y < _graph.Height ? y : _graph.Height - 1;
                    bool selected = IsSelected(i);
                    DrawPoint(x, y, selected, out pointWidth, out pointHeight);
                    _points.Add(new DiagramPoint { Diagram = this, X = x, Y = y, Width = pointWidth, Height = pointHeight, Index = i, ValueX = _x[i], ValueY = _y[i] });
                }

                x0 = x1;
                y0 = y1;
            }

            if (_currentPoint != null)
            {
                DrawDynamicAxes();
            }

		    if (_parent == null)
			{
				foreach (Diagram child in _children)
				{
					child._fromXIndex = _fromXIndex;
					child._toXIndex = _toXIndex;
					child._fromY = _fromY;
					child._toY = _toY;
					child._fromX = _fromX;
					child._toX = _toX;
					child.Draw();
				}

				_graph.StopDrawing();
			}
		}

	    private void InitDefault()
		{
			Background = Color.Black;
			ForePen = new Pen(Color.Green);
			AxisPen = new Pen(Color.Gray);
			ZeroAxisPen = new Pen(Color.White);

			ShowAxisX = true;
			ShowAxisY = true;

			AxisStepInPercents = 10;
			WheelStepInPercents = 5;

			ShowPoints = true;
		}

	    private void FormAxis()
		{
			float dx = _maxX - _minX;
			float dy = _maxY - _minY;
			float kx = AxisStepInPercents * dx / 100.0f;
			float ky = AxisStepInPercents * dy / 100.0f;
			var indexX = (int)System.Math.Truncate(_minX / kx);
			var indexY = (int) System.Math.Truncate(_minY/ky);
			float x = kx * indexX;
			float y = ky * indexY;
			int countX = (kx != 0.0) ? (int)System.Math.Truncate(dx / kx) + 1 : 1;
			int countY = (ky != 0.0) ? (int)System.Math.Truncate(dy / ky) + 1 : 1;
			_axisX = new float[countX];
			_axisY = new float[countY];
			for (int i = 0; i < countX; i++)
			{
				_axisX[i] = indexX == 0 ? 0.0f : x;
				x += kx;
				indexX++;
			}
			for (int i = 0; i < countY; i++)
			{
				_axisY[i] = indexY == 0 ? 0.0f : y;
				y += ky;
				indexY++;
			}
		}

		private void InitMinMax()
		{
			_minX = _x[0];
			_maxX = _x[_length - 1];
			_minY = _y[0];
			_maxY = _y[0];
			for (int i = 1; i < _length; i++)
			{
				if (_y[i] < _minY)
				{
					_minY = _y[i];
				}
				if (_y[i] > _maxY)
				{
					_maxY = _y[i];
				}
			}
		}

		private void InitIndex()
		{
			_fromXIndex = 0;
			_toXIndex = _length - 1;
			_fromX = _minX;
			_toX = _maxX;
			_fromY = _minY;
			_toY = _maxY;
		}

        public Diagram(IEnumerable<int> x, IEnumerable<int> y, IGraphDevice graph, int? minY = null, int? maxY = null)
            : this(
            x != null ? x.Select(v => (float)v).ToArray() : null,
            y != null ? y.Select(v => (float)v).ToArray() : null,
            graph,
            minY,
            maxY)
        {
        }

        public Diagram(float[] x, float[] y, IGraphDevice graph, float? minY = null, float? maxY = null)
		{
            if (x == null)
                throw new ArgumentNullException("x");
            if (y == null)
                throw new ArgumentNullException("y");
            if (x.Length != y.Length)
                throw new ArgumentOutOfRangeException("x", "(x.Length != y.Length)");
            if (y.Length < 2)
            	throw new ArgumentOutOfRangeException("y", "Length should be more then 2 elements.");
            //if(y.Length <= 2)
			//	throw new ArgumentOutOfRangeException("y", "Length should be more then 3 elements.");
			if (graph == null)
				throw new ArgumentNullException("graph");

			_graph = graph;
			_x = x;
			_y = y;
			_length = y.Length;
		    _visible = true;

			InitMinMax();

            if (minY != null)
            {
                _minY = minY.Value;
            }
            if (maxY != null)
            {
                _maxY = maxY.Value;
            }

			InitIndex();

			InitDefault();
			FormAxis();
			//Draw();

            if (graph.Container as Control != null)
            {
                _eventManager = SimpleDeviceEventManager.Create(this, (Control)graph.Container);
            }
		}

        public Diagram(IEnumerable<int> x, IEnumerable<int> y, Diagram parent)
            : this(
            x != null ? x.Select(v => (float)v).ToArray() : null,
            y != null ? y.Select(v => (float)v).ToArray() : null,
            parent)
        {
        }

		public Diagram(float[] x, float[] y, Diagram parent)
		{
            if (x == null)
                throw new ArgumentNullException("x");
            if (y == null)
                throw new ArgumentNullException("y");
            if (x.Length != y.Length)
                throw new ArgumentOutOfRangeException("x", "(x.Length != y.Length)");
            if (y.Length < 2)
                throw new ArgumentOutOfRangeException("y", "Length should be more then 2 elements.");
            //if(y.Length <= 2)
            //	throw new ArgumentOutOfRangeException("y", "Length should be more then 3 elements.");
            if (parent == null)
                throw new ArgumentNullException("parent");
            
            parent._children.Add(this);

			_x = x;
			_y = y;
			_length = y.Length;

			_parent = parent;
			_graph = parent._graph;
            _visible = true;

			InitMinMax();

			_parent._minX = (_minX < parent._minX) ? _minX : parent._minX;
			_parent._maxX = (_maxX > parent._maxX) ? _maxX : parent._maxX;
			_parent._minY = (_minY < parent._minY) ? _minY : parent._minY;
			_parent._maxY = (_maxY > parent._maxY) ? _maxY : parent._maxY;

			_parent.InitIndex();
			_parent.FormAxis();

			_minX = parent._minX;
			_maxX = parent._maxX;
			_minY = parent._minY;
			_maxY = parent._maxY;

			InitDefault();
			FormAxis();
			//parent.Draw();
		}
		
		private void DefineXIndex()
		{
			_fromXIndex = 0;
			_toXIndex = _length - 1;

			for (int i = 1; i < _length; i++)
			{
				if ((_x[i - 1] < _fromX) && (_fromX <= _x[i]))
				{
					_fromXIndex = i - 1;
				}
				if ((_x[i - 1] < _toX) && (_toX <= _x[i]))
				{
					_toXIndex = i;
				}
			}
		}

	    private void Move(int dx, int dy, bool draw)
		{
			dx = -dx;
			float kx = _graph.Width / (_toX - _fromX);
			float ky = _graph.Height / (_toY - _fromY);

			float ndx = dx / kx;
			float ndy = dy / ky;

			float cdx = (_toX - _fromX);
			float cdy = (_toY - _fromY);

			_fromY = System.Math.Max(_minY, _fromY + ndy);
			if (_fromY + cdy > _maxY)
			{
				_fromY = _maxY - cdy;
			}
			_toY = _fromY + cdy;

			_fromX = System.Math.Max(_minX, _fromX + ndx);
			if (_fromX + cdx > _maxX)
			{
				_fromX = _maxX - cdx;
			}
			_toX = _fromX + cdx;

			DefineXIndex();

			if (draw)
			{
				Draw();
			}
		}

	    private void Scale(int x0, int y0, int x1, int y1, bool draw)
		{
			y0 = _graph.Height - y0;
			y1 = _graph.Height - y1;
			if (x1 < x0)
			{
				Utility.Swap(ref x0, ref x1);
			}
			if (y1 < y0)
			{
				Utility.Swap(ref y0, ref y1);
			}

			float kx = (_toX - _fromX) / _graph.Width;
			float ky = (_toY - _fromY) / _graph.Height;

			float cdx = kx * (x1 - x0);
			float cdy = ky * (y1 - y0);

			_fromX = _fromX + kx * x0;
			if (_fromX + cdx > _maxX)
			{
				_fromX = _maxX - cdx;
			}
			if (_fromX < _minX)
			{
				_fromX = _minX;
			}
			_toX = _fromX + cdx;
			if (_toX > _maxX)
			{
				_toX = _maxX;
			}

			_fromY = _fromY + ky * y0;
			if (_fromY + cdy > _maxY)
			{
				_fromY = _maxY - cdy;
			}
			if (_fromY < _minY)
			{
				_fromY = _minY;
			}
			_toY = _fromY + cdy;
			if (_toY > _maxY)
			{
				_toY = _maxY;
			}

			DefineXIndex();

			if (draw)
			{
				Draw();
			}
		}

	    private void Wheel(int x, int y, bool increase, bool draw)
		{
			var sx = (int)((WheelStepInPercents * _graph.Width / 100.0f) / 2);
			var sy = (int)((WheelStepInPercents * _graph.Height / 100.0f) / 2);
			if (increase)
			{
				Scale(sx, sy, _graph.Width - sx, _graph.Height - sy, false);
				if (MoveAfterWheel)
				{
					Move(_graph.Width/2 - x, (_graph.Height - y) - _graph.Height/2, false);
				}
			}
			else
			{
				Scale(-sx, -sy, _graph.Width + sx, _graph.Height + sy, false);
			}
			if (draw)
			{
				Draw();
			}
		}

	    private void Resize(int width, int height, bool draw)
		{
			_graph.Resize(width, height);

			if(draw)
			{
				Draw();
			}
		}

	    private void Restore(bool draw)
		{
			_fromXIndex = 0;
			_toXIndex = _length - 1;
			_fromX = _minX;
			_toX = _maxX;
			_fromY = _minY;
			_toY = _maxY;

			if(draw)
			{
				Draw();
			}
		}

		public void Move(int dx, int dy)
		{
			Move(dx, dy, true);
		}

		public void OnMouseMove(int x, int y)
		{
			_mouseX = x;
			_mouseY = y;

			//Define Selected Point
			DiagramPoint? point = null;
			for (int i = _children.Count - 1; i >= 0; i--)
			{
				Diagram child = _children[i];
				child.OnMouseMove(x, y);
				if (point == null)
				{
					point = child._selectedPoint;
				}
			}

			DiagramPoint? newPoint = null;
			for (int i = 0; i < _points.Count; i++)
			{
				if (_points[i].IsIn(x, y))
				{
					newPoint = _points[i];
					break;
				}
			}

			bool draw = false;
			if (
				((newPoint == null) && (_selectedPoint != null)) ||
				((newPoint != null) && (_selectedPoint == null)) ||
				((newPoint != null) && (newPoint.Value.Index != _selectedPoint.Value.Index)))
			{
				_selectedPoint = newPoint;
			}
			
			point = point ?? _selectedPoint;

			if(_parent == null)
			{
				if (
					((point == null) && (_topSelectedPoint != null)) ||
					((point != null) && (_topSelectedPoint == null)) ||
					((point != null) && (point.Value.Index != _topSelectedPoint.Value.Index)))
				{
                    if ((_topSelectedPoint != null) && (point == null) && (HideSelectedPointEvent != null))
                    {
                        Diagram source = _topSelectedPoint.Value.Diagram;
                        HideSelectedPointEvent(source, new HideSelectedPointEventArgs { Point = _topSelectedPoint.Value });
                    }
                    
                    _topSelectedPoint = point;

                    if (_topSelectedPoint != null)
                    {
                        if (HintSelectedPointEvent != null)
                        {
                            var hintArgs = new HintSelectedPointEventArgs { Point = _topSelectedPoint.Value };
                            Diagram source = _topSelectedPoint.Value.Diagram;
                            HintSelectedPointEvent(source, hintArgs);
                            if (ShowSelectedPointEvent != null)
                            {
                                ShowSelectedPointEvent(source, new ShowSelectedPointEventArgs { Point = _topSelectedPoint.Value, Hint = hintArgs.Hint });
                            }
                        }
                    }

                    draw = true;
				}
			}

			//Define current point
			for (int i = 0; i < _points.Count - 1; i++)
			{
				int x0 = _points[i].X;
				int x1 = _points[i + 1].X;
				if ((x >= x0) && (x < x1))
				{
					//_currentPoint = (x - _points[i].X < _points[i + 1].X - x)
					//					? _points[i]
					//					: _points[i + 1];

                    float dx = (_toX - _fromX) / _graph.Width;
                    float dy = (_toY - _fromY) / _graph.Height;

				    _currentPoint = new DiagramPoint
				                        {
                                            Diagram = this,
                                            X = _mouseX,
                                            Y = _mouseY,
                                            ValueX = _fromX + dx * _mouseX,
                                            ValueY = _fromY + dy * (_graph.Height - _mouseY),
                                        };

					draw = true;
					break;
				}
			}

			//Redraw
			if(_parent == null)
			{
				if(draw)
				{
					Draw();
				}
			}

            //Invoke events
            if ((MouseMoveEvent != null) && (_currentPoint != null))
		    {
		        MouseMoveEvent(this, new MouseMoveEventArgs {CurrentPoint = _currentPoint.Value});
		    }
		}

		public void Scale(int x0, int y0, int x1, int y1)
		{
			Scale(x0, y0, x1, y1, true);
		}

		public void Wheel(int x, int y, bool increase)
		{
			Wheel(x, y, increase, true);
		}

		public void Resize(int width, int height)
		{
			Resize(width, height, true);
		}

		public void Restore()
		{
			Restore(true);
		}

        public void SynchronizeTo(Diagram to)
        {
            if (to == null)
                throw new ArgumentNullException("to");
            if (to._eventManager == null)
                throw new ArgumentOutOfRangeException("to", "(to._eventManager == null)");
            if (_eventManager == null)
                throw new InvalidOperationException("(to._eventManager == null)");

            if (this == to)
            {
                return;
            }

            to._eventManager.SynchronizeTo = _eventManager;
            _eventManager.SynchronizeTo = to._eventManager;
        }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public Color Background { get; set; }

        public Pen ForePen { get; set; }

        public bool ShowAxisX { get; set; }

        public bool ShowAxisY { get; set; }

        public Pen AxisPen { get; set; }

        public Pen ZeroAxisPen { get; set; }

        public int AxisStepInPercents { get; set; }

        public int WheelStepInPercents { get; set; }

        public bool MoveAfterWheel { get; set; }

        public bool ShowPoints { get; set; }

	    public bool Enabled
	    {
            get { return ((_eventManager == null) || _eventManager.Enabled); }
            set
            {
                if (_eventManager != null)
                {
                    _eventManager.Enabled = value;
                }
            }
	    }

	    public bool Visible
	    {
            get { return _visible; }
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    if (_parent != null)
                    {
                        _parent.Draw();
                    }
                    else
                    {
                        Draw();
                    }
                }
            }
	    }

		public DiagramPoint CurrentPoint
		{
			get { return _currentPoint != null ? _currentPoint.Value : _points[0]; }
		}

		public IGraphDevice Device
		{
			get { return _graph; }
		}

		public APV.Math.Diagrams.Diagrams Diagrams
		{
			get
			{
				if(_parent != null)
				{
					return _parent.Diagrams;
				}
				var diagrams = new APV.Math.Diagrams.Diagrams {this};
				diagrams.AddRange(_children);
			    return diagrams;
			}
		}

        public event DiagramEventDelegate HintSelectedPointEvent;

        public event DiagramEventDelegate ShowSelectedPointEvent;

        public event DiagramEventDelegate HideSelectedPointEvent;

        public event DiagramEventDelegate MouseMoveEvent;
    }
}
