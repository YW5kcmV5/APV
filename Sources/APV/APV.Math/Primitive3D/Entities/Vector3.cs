using System;

namespace APV.Math.Primitive3D.Entities
{
    public struct Vector3
    {
        public float I1;
        public float I2;
        public float I3;

        public Vector3(float i1, float i2, float i3)
        {
            I1 = i1;
            I2 = i2;
            I3 = i3;
        }

        public Vector3(Vector3 vector)
        {
            I1 = vector.I1;
            I2 = vector.I2;
            I3 = vector.I3;
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
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public static Vector3 Normilize(Vector3D x)
        {
            var length = (float)System.Math.Sqrt(x.I1*x.I1 + x.I2*x.I2 + x.I3*x.I3);
            var result = new Vector3
            {
                I1 = x.I1/length,
                I2 = x.I2/length,
                I3 = x.I3/length,
            };
            return result;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static Vector3 Sub(Vector3 x, Vector3 y)
        {
            var result = new Vector3
            {
                I1 = x.I1 - y.I1,
                I2 = x.I2 - y.I2,
                I3 = x.I3 - y.I3,
            };
            return result;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static Vector3 Add(Vector3 x, Vector3 y)
        {
            var result = new Vector3
            {
                I1 = x.I1 + y.I1,
                I2 = x.I2 + y.I2,
                I3 = x.I3 + y.I3,
            };
            return result;
        }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        public static float ScalarMul(Vector3 x, Vector3 y)
        {
            float result = x.I1 * y.I1 + x.I2 * y.I2 + x.I3 * y.I3;
            return result;
        }

        public static Vector3 Mul(Vector3 x, Vector3 y)
        {
            var result = new Vector3
            {
                I1 = x.I2*y.I3 - x.I3*y.I2,
                I2 = x.I3*y.I1 - x.I1*y.I3,
                I3 = x.I1*y.I2 - x.I2*y.I1,
            };
            return result;
        }

        public static Vector3 Mul(Vector3 x, float y)
        {
            var result = new Vector3
            {
                I1 = x.I2 * y,
                I2 = x.I3 * y,
                I3 = x.I3 * y,
            };
            return result;
        }

        #region Operators

        public static Vector3 operator *(Vector3 x, Vector3 y)
        {
            return Mul(x, y);
        }

        public static Vector3 operator *(Vector3 x, float y)
        {
            return Mul(x, y);
        }

        public static Vector3 operator +(Vector3 x, Vector3 y)
        {
            return Add(x, y);
        }

        public static Vector3 operator -(Vector3 x, Vector3 y)
        {
            return Sub(x, y);
        }

        #endregion
    }
}