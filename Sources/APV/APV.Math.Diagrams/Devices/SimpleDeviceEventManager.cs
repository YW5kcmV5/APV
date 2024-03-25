using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace APV.Math.Diagrams.Devices
{
    internal class SimpleDeviceEventManager : IDisposable
    {
        private static readonly List<KeyValuePair<object, SimpleDeviceEventManager>> Links = new List<KeyValuePair<object, SimpleDeviceEventManager>>();

        private Control _container;
        private Diagram _diagram;
        private int _x0, _y0, _x1, _y1;
        private int _lastX, _lastY;
        private bool _showR;
        private ToolTip _tooltip;
        private bool _disposed;
        private bool _enabled;
        private bool _eventInvoked;

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _lastX = e.X;
            _lastY = e.Y;
            if (_diagram != null)
            {
                if ((e.Button == MouseButtons.Right) && (_showR))
                {
                    _diagram.Device.StartDrawing();
                    _diagram.Device.DrawReversibleRectangle(_x0, _y0, _x1, _y1);
                    _diagram.Device.StopDrawing();
                    _showR = false;
                }
                if ((e.Button == MouseButtons.Left) && ((Control.MouseButtons & MouseButtons.Right) != MouseButtons.Right))
                {
                    _showR = true;
                    _x0 = _x1 = e.X;
                    _y0 = _y1 = e.Y;
                    _diagram.Device.StartDrawing();
                    _diagram.Device.DrawReversibleRectangle(_x0, _y0, _x1, _y1);
                    _diagram.Device.StopDrawing();
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_diagram != null)
            {
                if ((_showR) && (e.Button == MouseButtons.Left))
                {
                    _diagram.Device.StartDrawing();
                    _diagram.Device.DrawReversibleRectangle(_x0, _y0, _x1, _y1);
                    _showR = false;

                    if ((_x0 == _x1) || (_y0 == _y1))
                    {
                        _diagram.Device.StopDrawing();
                    }
                    else
                    {
                        _diagram.Scale(_x0, _y0, _x1, _y1);
                    }
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_eventInvoked)
            {
                return;
            }

            if (_diagram != null)
            {
                if (_showR)
                {
                    _diagram.Device.StartDrawing();
                    _diagram.Device.DrawReversibleRectangle(_x0, _y0, _x1, _y1);
                    _x1 = e.X;
                    _y1 = e.Y;
                    _diagram.Device.DrawReversibleRectangle(_x0, _y0, _x1, _y1);
                    _diagram.Device.StopDrawing();
                }
                if (((Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left) && ((Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right))
                {
                    _diagram.Move(e.X - _lastX, e.Y - _lastY);
                }
                if (Control.MouseButtons == MouseButtons.None)
                {
                    _diagram.OnMouseMove(e.X, e.Y);
                }

                //var value = new StringBuilder();
                //value.AppendFormat("X={0} Y= ", _diagram.CurrentPoint.ValueX);
                //for (int i = 0; i < _diagram.Diagrams.Length; i++)
                //{
                //    value.Append(_diagram.Diagrams[i].CurrentPoint.ValueY + " ");
                //}
                //textBox1.Text = value.ToString();
            }
            _lastX = e.X;
            _lastY = e.Y;

            _eventInvoked = true;
            if (SynchronizeTo != null)
            {
                SynchronizeTo.OnMouseMove(sender, e);
            }
            _eventInvoked = false;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (_diagram != null)
            {
                _diagram.Wheel(e.X, e.Y, e.Delta > 0);
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (_diagram != null)
            {
                _diagram.Resize(_container.Width, _container.Height);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_diagram != null)
            {
                _diagram.Restore();
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            _container.Focus();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (_diagram != null)
            {
                _diagram.Draw();
            }
        }

        private static void OnHintSelectedPointEvent(Diagram diagram, DiagramEventArgs args)
        {
            var eventArgs = (HintSelectedPointEventArgs) args;
            if (eventArgs.Handled)
            {
                return;
            }

            DiagramPoint point = eventArgs.Point;
            eventArgs.Hint =
                (!string.IsNullOrEmpty(diagram.Description))
                    ? string.Format("\"{0}\"\nX={1:0.00}\nY={2:0.00}", diagram.Description, point.ValueX, point.ValueY)
                    : string.Format("X={0:0.00}\nY={1:0.00}", point.ValueX, point.ValueY);
        }

        private void OnShowSelectedPointEvent(Diagram diagram, DiagramEventArgs args)
        {
            var eventArgs = (ShowSelectedPointEventArgs) args;
            if (eventArgs.Handled)
            {
                return;
            }

            if (_tooltip == null)
            {
                _tooltip = new ToolTip
                               {
                                   ShowAlways = true,
                                   InitialDelay = 0,
                                   AutomaticDelay = 0,
                                   AutoPopDelay = int.MaxValue,
                                   Active = true
                               };
            }
            _tooltip.SetToolTip(_container, eventArgs.Hint);
        }

        private void DisableHint()
        {
            if (_tooltip != null)
            {
                _tooltip.Active = false;
                _tooltip.Dispose();
                _tooltip = null;
            }
        }

        private void OnHideSelectedPointEvent(Diagram diagram, DiagramEventArgs args)
        {
            var eventArgs = (HideSelectedPointEventArgs)args;
            if (eventArgs.Handled)
            {
                return;
            }

            DisableHint();
        }

        private void AttachEvents()
        {
            if (_container != null)
            {
                _container.MouseDown += OnMouseDown;
                _container.MouseUp += OnMouseUp;
                _container.MouseMove += OnMouseMove;
                _container.MouseWheel += OnMouseWheel;
                _container.Resize += OnResize;
                _container.MouseDoubleClick += OnMouseDoubleClick;
                _container.MouseClick += OnMouseClick;
                _container.Paint += OnPaint;
            }
            if (_diagram != null)
            {
                _diagram.HintSelectedPointEvent += OnHintSelectedPointEvent;
                _diagram.ShowSelectedPointEvent += OnShowSelectedPointEvent;
                _diagram.HideSelectedPointEvent += OnHideSelectedPointEvent;
            }
        }

        private void DeattachEvents()
        {
            DisableHint();

            if (_container != null)
            {
                _container.MouseDown -= OnMouseDown;
                _container.MouseUp -= OnMouseUp;
                _container.MouseMove -= OnMouseMove;
                _container.MouseWheel -= OnMouseWheel;
                _container.Resize -= OnResize;
                _container.MouseDoubleClick -= OnMouseDoubleClick;
                _container.MouseClick -= OnMouseClick;
                _container.Paint -= OnPaint;
            }
            if (_diagram != null)
            {
                _diagram.HintSelectedPointEvent -= OnHintSelectedPointEvent;
                _diagram.ShowSelectedPointEvent -= OnShowSelectedPointEvent;
                _diagram.HideSelectedPointEvent -= OnHideSelectedPointEvent;
            }
        }

        private SimpleDeviceEventManager(Diagram diagram, Control container)
        {
            if (diagram == null)
                throw new ArgumentNullException("diagram");
            if (container == null)
                throw new ArgumentNullException("container");

            _diagram = diagram;
            _container = container;

            Enabled = true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Enabled = false;

                DeattachEvents();

                _container = null;
                _diagram = null;
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    if (_enabled)
                    {
                        AttachEvents();
                    }
                    else
                    {
                        DeattachEvents();
                    }
                }
            }
        }

        public SimpleDeviceEventManager SynchronizeTo
        {
            get;
            set;
        }

        public static SimpleDeviceEventManager Create(Diagram diagram, Control container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            lock(Links)
            {
                SimpleDeviceEventManager eventManager = null;
                //int index = -1;

                int length = Links.Count;
                for (int i = 0; i < length; i++)
                {
                    KeyValuePair<object, SimpleDeviceEventManager> item = Links[i];
                    object key = item.Key;
                    if (ReferenceEquals(container, key))
                    {
                        //index = i;
                        eventManager = item.Value;
                        break;
                    }
                }

                if (eventManager != null)
                {
                    //eventManager.Dispose();
                    //Links.RemoveAt(index);
                }

                eventManager = new SimpleDeviceEventManager(diagram, container);
                Links.Add(new KeyValuePair<object, SimpleDeviceEventManager>(container, eventManager));
                return eventManager;
            }
        }
    }
}