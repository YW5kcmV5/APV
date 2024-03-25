
namespace APV.Math.Primitive3D.Entities
{
    public struct Triangle3D : IPrimitive3D
    {
        public Vector3D A;
        public Vector3D B;
        public Vector3D C;
        public int Color;

        public Triangle3D(Vector3D a, Vector3D b, Vector3D c, int color)
        {
            A = a;
            B = b;
            C = c;
            Color = color;
        }

        #region IPrimitive3D

        public IPrimitive3D Tranform(Matrix3D transform)
        {
            return Mul(transform, this);
        }

        public void Draw(Bitmap3D bitmap)
        {
            bitmap.DrawTriangle(this);
        }

        public bool Visible(float dist)
        {
            Vector3D normal = GetNormal(this);

            //float nx = normal.I1;
            //float ny = normal.I2;
            //float nz = normal.I3;

            //float vx = A.I1;
            //float vy = B.I2;
            //float vz = C.I3;

            //float k = (nz * dist + nx * vx + ny * vy + nz * vz);
            //bool visible = (k < 0);

            bool visible = (normal.I3 < 0);

            return visible;
        }

        #endregion

        public static Triangle3D Mul(Matrix3D transform, Triangle3D triangle)
        {
            var result = new Triangle3D
            {
                A = Matrix3D.Mul(transform, triangle.A),
                B = Matrix3D.Mul(transform, triangle.B),
                C = Matrix3D.Mul(transform, triangle.C),
                Color = triangle.Color
            };
            return result;
        }

        public static Triangle3D operator *(Matrix3D x, Triangle3D y)
        {
            return Mul(x, y);
        }

        /// <summary>
        /// Центр треугольника
        /// </summary>
        /// <param name="triangle"></param>
        public static Vector3D GetCenter(Triangle3D triangle)
        {
            var result = new Vector3D
            {
                I1 = (triangle.A.I1 + triangle.B.I1 + triangle.C.I1) / 3f,
                I2 = (triangle.A.I2 + triangle.B.I2 + triangle.C.I2) / 3f,
                I3 = (triangle.A.I3 + triangle.B.I3 + triangle.C.I3) / 3f,
                I4 = 1
            };
            return result;
        }

        /// <summary>
        /// Нормаль к треугольнику
        /// </summary>
        public static Vector3D GetNormal(Triangle3D triangle)
        {
            //Vector3D edge1 = (b - a);
            //Vector3D edge2 = (c - a);
            //Vector3D normal = edge1 * edge2;
            //return normal;

            float x1 = triangle.B.I1 - triangle.A.I1;
            float x2 = triangle.B.I2 - triangle.A.I2;
            float x3 = triangle.B.I3 - triangle.A.I3;
            float y1 = triangle.C.I1 - triangle.A.I1;
            float y2 = triangle.C.I2 - triangle.A.I2;
            float y3 = triangle.C.I3 - triangle.A.I3;

            var normal = new Vector3D
            {
                I1 = x2 * y3 - x3 * y2,
                I2 = x3 * y1 - x1 * y3,
                I3 = x1 * y2 - x2 * y1,
                I4 = 1
            };

            return normal;
        }

        /// <summary>
        /// Нормаль к треугольнику
        /// </summary>
        public static Line3D GetNormalLine(Triangle3D triangle, int color)
        {
            Vector3D a = GetCenter(triangle);
            Vector3D b = (a + GetNormal(triangle));
            var result = new Line3D
            {
                A = a,
                B = b,
                Color = color,
                IsNormal = true
            };
            return result;
        }
    }
}