
namespace APV.Math.Primitive3D.Entities
{
    public struct Line3D : IPrimitive3D
    {
        public Vector3D A;
        public Vector3D B;
        public int Color;
        public bool IsNormal;

        public Line3D(Vector3D a, Vector3D b, int color, bool isNormal = false)
        {
            A = a;
            B = b;
            Color = color;
            IsNormal = isNormal;
        }

        #region IPrimitive3D

        public IPrimitive3D Tranform(Matrix3D transform)
        {
            return Mul(transform, this);
        }

        public void Draw(Bitmap3D bitmap)
        {
            bitmap.DrawLine(this);
        }

        public bool Visible(float dist)
        {
            bool visible = (!IsNormal) || (A.I3 > B.I3);
            return visible;
        }

        #endregion

        public static Line3D Mul(Line3D x, float y)
        {
            float a1 = x.A.I1 * y;
            float a2 = x.A.I2 * y;
            float a3 = x.A.I3 * y;
            float b1 = x.B.I1 * y;
            float b2 = x.B.I2 * y;
            float b3 = x.B.I3 * y;
            float d1 = b1 - a1;
            float d2 = b2 - a2;
            float d3 = b3 - a3;
            var result = new Line3D
            {
                A = x.A,
                B = new Vector3D
                {
                    I1 = x.A.I1 + d1,
                    I2 = x.A.I2 + d2,
                    I3 = x.A.I3 + d3,
                },
                Color = x.Color,
                IsNormal = x.IsNormal
            };
            return result;
        }

        public static Line3D Mul(Matrix3D x, Line3D y)
        {
            var result = new Line3D
            {
                A = Matrix3D.Mul(x, y.A),
                B = Matrix3D.Mul(x, y.B),
                Color = y.Color,
                IsNormal = y.IsNormal
            };
            return result;
        }

        public static Line3D operator *(Matrix3D transform, Line3D line)
        {
            return Mul(transform, line);
        }
    }
}