
namespace APV.Math.Primitive3D.Entities
{
    public struct Matrix33
    {
        public float I11;
        public float I12;
        public float I13;
        public float I21;
        public float I22;
        public float I23;
        public float I31;
        public float I32;
        public float I33;

        public static float GetDet(Matrix33 x)
        {
            float det = x.I11 * x.I22 * x.I33 - x.I11 * x.I23 * x.I32 - x.I12 * x.I21 * x.I33 + x.I12 * x.I23 * x.I31 + x.I13 * x.I21 * x.I32 - x.I13 * x.I22 * x.I31;
            return det;
        }

        public static Matrix33 GetInvertible(Matrix33 x, out float det)
        {
            var result = new Matrix33();

            float i1122 = x.I11 * x.I22;
            float i1123 = x.I11 * x.I23;
            float i2133 = x.I21 * x.I33;
            float i1223 = x.I12 * x.I23;
            float i2132 = x.I21 * x.I32;
            det = i1122 * x.I33 - i1123 * x.I32 - x.I12 * i2133 + i1223 * x.I31 + x.I13 * i2132 - x.I13 * x.I22 * x.I31;

            if (det == 0)
            {
                return result;
            }

            //result.I11 = (x.I22 * x.I33 - x.I32 * x.I23) / det;
            //result.I21 = (x.I31 * x.I23 - i2133) / det;
            //result.I31 = (i2132 - x.I31 * x.I22) / det;
            //result.I12 = (x.I32 * x.I13 - x.I12 * x.I33) / det;
            //result.I22 = (x.I11 * x.I33 - x.I31 * x.I13) / det;
            //result.I32 = (x.I31 * x.I12 - x.I11 * x.I32) / det;
            //result.I13 = (i1223 - x.I22 * x.I13) / det;
            //result.I23 = (x.I21 * x.I13 - i1123) / det;
            //result.I33 = (i1122 - x.I21 * x.I12) / det;

            result.I11 = (x.I22 * x.I33 - x.I32 * x.I23) / det;
            result.I12 = (x.I31 * x.I23 - i2133) / det;
            result.I13 = (i2132 - x.I31 * x.I22) / det;
            result.I21 = (x.I32 * x.I13 - x.I12 * x.I33) / det;
            result.I22 = (x.I11 * x.I33 - x.I31 * x.I13) / det;
            result.I23 = (x.I31 * x.I12 - x.I11 * x.I32) / det;
            result.I31 = (i1223 - x.I22 * x.I13) / det;
            result.I32 = (x.I21 * x.I13 - i1123) / det;
            result.I33 = (i1122 - x.I21 * x.I12) / det;

            return result;
        }

        public static Matrix33 Add(Matrix33 x, Matrix33 y)
        {
            var result = new Matrix33
            {
                I11 = x.I11 + y.I11,
                I12 = x.I12 + y.I12,
                I13 = x.I13 + y.I13,
                I21 = x.I21 + y.I21,
                I22 = x.I22 + y.I22,
                I23 = x.I23 + y.I23,
                I31 = x.I31 + y.I31,
                I32 = x.I32 + y.I32,
                I33 = x.I33 + y.I33
            };
            return result;
        }

        public static Matrix33 Mul(Matrix33 x, Matrix33 y)
        {
            var result = new Matrix33
            {
                I11 = x.I11*y.I11 + x.I12*y.I21 + x.I13*y.I31,
                I12 = x.I11*y.I12 + x.I12*y.I22 + x.I13*y.I32,
                I13 = x.I11*y.I13 + x.I12*y.I23 + x.I13*y.I33,
                I21 = x.I21*y.I11 + x.I22*y.I21 + x.I23*y.I31,
                I22 = x.I21*y.I12 + x.I22*y.I22 + x.I23*y.I32,
                I23 = x.I21*y.I13 + x.I22*y.I23 + x.I23*y.I33,
                I31 = x.I31*y.I11 + x.I32*y.I21 + x.I33*y.I31,
                I32 = x.I31*y.I12 + x.I32*y.I22 + x.I33*y.I32,
                I33 = x.I31*y.I13 + x.I32*y.I23 + x.I33*y.I33
            };
            return result;
        }

        public static Vector3 Mul(Matrix33 x, Vector3 y)
        {
            var result = new Vector3
            {
                I1 = x.I11*y.I1 + x.I12*y.I2 + x.I13*y.I3,
                I2 = x.I21*y.I1 + x.I22*y.I2 + x.I23*y.I3,
                I3 = x.I31*y.I1 + x.I32*y.I2 + x.I33*y.I3
            };
            return result;
        }

        public static Matrix33 operator *(Matrix33 x, Matrix33 y)
        {
            return Mul(x, y);
        }

        public static Vector3 operator *(Matrix33 x, Vector3 y)
        {
            return Mul(x, y);
        }

        public static Matrix33 operator +(Matrix33 x, Matrix33 y)
        {
            return Add(x, y);
        }

        /// <summary>
        /// Единичная матрица
        /// </summary>
        public static readonly Matrix33 E = new Matrix33 { I11 = 1, I22 = 1, I33 = 1 };
    }
}
