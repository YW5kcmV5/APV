using APV.Math.Primitive3D.Entities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace APV.Math.Primitive3D
{
    public class Primitive3DPanel : Panel
    {
        public const int FrameIntervalInMlsec = 100;
        public const int FramesCount = (1000/FrameIntervalInMlsec);

        private Bitmap3D _bitmap3D;
        private Bitmap _bitmap;
        private Rectangle _rect;
        private int _frameNumber;
        private bool _render;

        private void ResizeBitmap(int width, int height)
        {
            _bitmap3D = new Bitmap3D(width, height);
            _bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            _rect = new Rectangle(0, 0, width, height);
        }

        private void Copy(Graphics to)
        {
            int width = Width;
            int height = Height;

            int count = _bitmap3D.Count;
            int[] colors = new int[count];
            int index = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    colors[index] = _bitmap3D.Data[j][i];
                    index++;
                }
            }

            BitmapData data = _bitmap.LockBits(_rect, ImageLockMode.WriteOnly, _bitmap.PixelFormat);
            Marshal.Copy(colors, 0, data.Scan0, count);
            _bitmap.UnlockBits(data);

            to.DrawImage(_bitmap, 0, 0);
        }

        public Primitive3DPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            ResizeBitmap(Width, Height);

            _frameNumber = 1;
            Timer timer = new Timer {Interval = FrameIntervalInMlsec, Enabled = true};
            timer.Tick += (sender, args) =>
            {
                _frameNumber++;
                if (_frameNumber > FramesCount)
                {
                    _frameNumber = 1;
                }
                if (!_render)
                {
                    Invalidate();
                }
            };
        }

        public float AngleX = 0;
        public float AngleY = 0;
        public float AngleZ = 0;
        public float D = 0;
        public bool Pause;

        private float _angle = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                _render = true;

                _bitmap3D.Clear();
                _bitmap3D.ClearBuffers();



                ////var triangle2 = new Triangle3D(new Vector3D(300, 600, 150), new Vector3D(500, 400, 50), new Vector3D(500, 600, 350), Color.Green.ToArgb());
                ////var triangle1 = new Triangle3D(new Vector3D(500, 600, 50), new Vector3D(500, 500, 0), new Vector3D(300, 550, 100), Color.Red.ToArgb());
                ////var triangle1 = new Triangle3D(new Vector3D(400, 650, 150), new Vector3D(500, 550, 150), new Vector3D(300, 500, 250), Color.Red.ToArgb());
                ////var triangle1 = new Triangle3D(new Vector3D(300, 400, 200, 100000f / 200), new Vector3D(300, 600, 200, 100000f / 200), new Vector3D(500, 500, 50, 100000f / 50), Color.Red.ToArgb());
                ////var triangle2 = new Triangle3D(new Vector3D(400, 400, 50, 100000f / 50), new Vector3D(400, 600, 50, 100000f / 50), new Vector3D(600, 400, 200, 100000f / 200), Color.Green.ToArgb());
                //var triangle1 = new Triangle3D(new Vector3D(400, 400, 50, 100000f / 50), new Vector3D(400, 600, 100, 100000f / 100), new Vector3D(600, 500, 100, 100000f / 100), Color.Red.ToArgb());
                //var triangle2 = new Triangle3D(new Vector3D(400, 400, 50, 100000f / 50), new Vector3D(400, 600, 98, 100000f / 98), new Vector3D(600, 500, 150, 100000f / 150), Color.Green.ToArgb());

                //_bitmap3D.DrawTriangle3(triangle1);
                //_bitmap3D.DrawTriangle3(triangle2);

                ////_bitmap3D.DrawTriangle2(379, 713, 520, 424, 322, 473, Color.Red.ToArgb());
                ////_bitmap3D.DrawTriangle2(321, 348, 520, 424, 669, 449, Color.Green.ToArgb());

                ////_bitmap3D.DrawLine(1, 1, 3, 1, Color.White.ToArgb());
                ////_bitmap3D.DrawLine(3, 1, 4, 6, Color.White.ToArgb());
                ////_bitmap3D.DrawLine(1, 1, 4, 6, Color.White.ToArgb());
                ////_bitmap3D.DrawTriangle2(1, 1, 3, 1, 4, 6, Color.Blue.ToArgb());

                ////const int k = 30;
                ////var bitmap3D = new Bitmap3D(Width, Height);
                ////for (int x = 0; x < 10; x++)
                ////{
                ////    for (int y = 0; y < 10; y++)
                ////    {
                ////        int color = _bitmap3D.Data[x][y];
                ////        if (color != 0)
                ////        {
                ////            int newX = k * x;
                ////            int newY = k * y;
                ////            bitmap3D.DrawRectangle(newX + 1, newY + 1, newX + k - 1, newY + k - 1, color);
                ////        }
                ////    }
                ////}
                ////_bitmap3D = bitmap3D;







                var triangle1 = new Triangle3D(new Vector3D(0.5f, 0f, 0f), new Vector3D(0f, 0f, 0f), new Vector3D(0f, 0.5f, 0f), Color.Green.ToArgb());
                var triangle2 = new Triangle3D(new Vector3D(0.5f, 0f, 0f), new Vector3D(0f, 0f, 0.5f), new Vector3D(0f, 0f, 0f), Color.Red.ToArgb());
                var triangle3 = new Triangle3D(new Vector3D(0, 0.5f, 0f), new Vector3D(0f, 0f, 0f), new Vector3D(0f, 0f, 0.5f), Color.Blue.ToArgb());
                var triangle4 = new Triangle3D(new Vector3D(0.5f, 0, 0f), new Vector3D(0f, 0.5f, 0f), new Vector3D(0f, 0f, 0.5f), Color.Orange.ToArgb());

                //float triangleM = (0.5f + 0.5f + 0f + 0.5f) / 4.0f;

                var axisX = new Line3D(new Vector3D(0, 0, 0), new Vector3D(0.7f, 0, 0), Color.DarkRed.ToArgb());
                var axisY = new Line3D(new Vector3D(0, 0, 0), new Vector3D(0, 0.7f, 0), Color.DarkGreen.ToArgb());
                var axisZ = new Line3D(new Vector3D(0, 0, 0), new Vector3D(0, 0, 0.7f), Color.DarkBlue.ToArgb());

                Line3D triangle1NormalLine = Triangle3D.GetNormalLine(triangle1, Color.LightGray.ToArgb());
                Line3D triangle2NormalLine = Triangle3D.GetNormalLine(triangle2, Color.LightGray.ToArgb());
                Line3D triangle3NormalLine = Triangle3D.GetNormalLine(triangle3, Color.LightGray.ToArgb());
                Line3D triangle4NormalLine = Triangle3D.GetNormalLine(triangle4, Color.LightGray.ToArgb());

                Object3D[] objects =
                {
                    //new Object3D
                    //{
                    //    Primitives = new IPrimitive3D[0]
                    //},
                    new Object3D
                    {
                        ModelTransform = Matrix3D.E,
                        Primitives = new IPrimitive3D[]
                        {
                            triangle1, triangle2, triangle3, triangle4,
                            triangle1NormalLine, triangle2NormalLine, triangle3NormalLine, triangle4NormalLine
                        }
                    },
                    new Cube3D(0.25f, Color.DarkKhaki.ToArgb()),
                    new Object3D
                    {
                        ModelTransform = Matrix3D.E,
                        Primitives = new IPrimitive3D[]
                        {
                            axisX, axisY, axisZ,
                        }
                    },
                };

                objects[0].Apply(Matrix3D.Transfer(-0.125f, -0.125f, -0.125f));

                _bitmap3D.ClearBuffers();

                if (!Pause)
                {
                    _angle += ((float)System.Math.PI / 100);
                }

                var xSina = (float)System.Math.Sin(_angle + AngleX);
                var xCosa = (float)System.Math.Cos(_angle + AngleX);
                var ySina = (float)System.Math.Sin(_angle + AngleY);
                var yCosa = (float)System.Math.Cos(_angle + AngleY);
                var zSina = (float)System.Math.Sin(_angle + AngleZ);
                var zCosa = (float)System.Math.Cos(_angle + AngleZ);

                Matrix3D rotateZ = Matrix3D.RotateZ(zCosa, zSina);
                Matrix3D rotateX = Matrix3D.RotateX(xCosa, xSina);
                Matrix3D rotateY = Matrix3D.RotateY(yCosa, ySina);

                float cos45 = (float)System.Math.Cos(System.Math.PI / 4);
                float sin45 = (float)System.Math.Sin(System.Math.PI / 4);
                float cos90 = (float)System.Math.Cos(System.Math.PI / 2);
                float sin90 = (float)System.Math.Sin(System.Math.PI / 2);

                //Camera = Projection * View;
                //GlobalTransform = Viewport * Camera * Model
                //Transform =  GlobalTransform * modelTransform
                //TransformedObject = Transform * Object

                const int depth = 10000;// int.MaxValue;
                float dist = 2.0f;
                Matrix3D projection = Matrix3D.Perspective((float)System.Math.PI / 2, (float)Width / Height, -dist, +dist);
                Matrix3D view = Matrix3D.E;
                Matrix3D model = Matrix3D.E;
                Matrix3D viewport = Matrix3D.Viewport(0, 0, Width, Height, depth);
                Matrix3D camera = projection * view;
                Matrix3D globalTransform = viewport * camera * model;

                //Matrix3D globalTransform = viewport * camera * model;
                //Matrix3D transform = globalTransform * modelTransform;

                int objectsCount = objects.Length;
                for (int i = 0; i < objectsCount; i++)
                {
                    Object3D @object = objects[i];
                    //@object.ModelTransform = (i == 0) ? rotateX*rotateY*rotateZ : Matrix3D.E;
                    @object.ModelTransform = (i == 0) ? rotateX * rotateY * rotateZ : Matrix3D.E;
                    @object.Draw(_bitmap3D, globalTransform, dist);
                }

                Copy(e.Graphics);
            }
            finally
            {
                _render = false;
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            try
            {
                _render = true;
                ResizeBitmap(Width, Height);
            }
            finally
            {
                _render = false;
            }
        }
    }
}
 