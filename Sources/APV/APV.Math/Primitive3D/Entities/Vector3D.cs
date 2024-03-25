using System;

namespace APV.Math.Primitive3D.Entities
{
    public struct Vector3D
    {
        public float I1;
        public float I2;
        public float I3;
        public float I4;

        public Vector3D(float i1, float i2, float i3, float i4 = 1)
        {
            I1 = i1;
            I2 = i2;
            I3 = i3;
            I4 = i4;
        }

        public Vector3D(Vector3D vector)
        {
            I1 = vector.I1;
            I2 = vector.I2;
            I3 = vector.I3;
            I4 = vector.I4;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return I1;
                    case 1:
                        return I2;
                    case 2:
                        return I3;
                    case 3:
                        return I4;
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            set
            {
                switch (index)
                {
                    case 0:
                        I1 = value;
                        return;
                    case 1:
                        I2 = value;
                        return;
                    case 2:
                        I3 = value;
                        return;
                    case 3:
                        I4 = value;
                        return;
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public static Vector3D Normilize(Vector3D x)
        {
            var length = (float)System.Math.Sqrt(x.I1*x.I1 + x.I2*x.I2 + x.I3*x.I3);
            var result = new Vector3D
            {
                I1 = x.I1/length,
                I2 = x.I2/length,
                I3 = x.I3/length,
                I4 = x.I4
            };
            return result;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static Vector3D Sub(Vector3D x, Vector3D y)
        {
            var result = new Vector3D
            {
                I1 = x.I1 - y.I1,
                I2 = x.I2 - y.I2,
                I3 = x.I3 - y.I3,
                I4 = 1,
            };
            return result;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static Vector3D Add(Vector3D x, Vector3D y)
        {
            var result = new Vector3D
            {
                I1 = x.I1 + y.I1,
                I2 = x.I2 + y.I2,
                I3 = x.I3 + y.I3,
                I4 = 1,
            };
            return result;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static float ScalarMul(Vector3D x, Vector3D y)
        {
            float result = x.I1 * y.I1 + x.I2 * y.I2 + x.I3 * y.I3;
            return result;
        }

        public static Vector3D Mul(Vector3D x, Vector3D y)
        {
            var result = new Vector3D
            {
                I1 = x.I2*y.I3 - x.I3*y.I2,
                I2 = x.I3*y.I1 - x.I1*y.I3,
                I3 = x.I1*y.I2 - x.I2*y.I1,
                I4 = 1,
            };
            return result;
        }

        public static Vector3D Mul(Vector3D x, float y)
        {
            //var result = new Vector3D
            //{
            //    I1 = x.I2 * y - x.I3 * y,
            //    I2 = x.I3 * y - x.I1 * y,
            //    I3 = x.I1 * y - x.I2 * y,
            //    I4 = 1,
            //};
            var result = new Vector3D
            {
                I1 = x.I2 * y,
                I2 = x.I3 * y,
                I3 = x.I3 * y,
                I4 = 1,
            };
            return result;
        }

        #region Operators

        public static Vector3D operator *(Vector3D x, Vector3D y)
        {
            return Mul(x, y);
        }

        public static Vector3D operator *(Vector3D x, float y)
        {
            return Mul(x, y);
        }

        public static Vector3D operator +(Vector3D x, Vector3D y)
        {
            return Add(x, y);
        }

        public static Vector3D operator -(Vector3D x, Vector3D y)
        {
            return Sub(x, y);
        }

        #endregion
    }
}