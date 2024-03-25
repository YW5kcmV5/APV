
using System;

namespace APV.Math.Primitive3D.Entities
{
    /// <summary>
    /// [11, 12, 13, 14]
    /// [21, 22, 23, 24]
    /// [31, 32, 33, 34]
    /// [41, 42, 43, 44]
    /// </summary>
    public struct Matrix3D
    {
        public float I11;
        public float I12;
        public float I13;
        public float I14;
        public float I21;
        public float I22;
        public float I23;
        public float I24;
        public float I31;
        public float I32;
        public float I33;
        public float I34;
        public float I41;
        public float I42;
        public float I43;
        public float I44;

        public float this[int y, int x]
        {
            get
            {
                switch (y)
                {
                    case 0:
                        switch (x)
                        {
                            case 0:
                                return I11;
                            case 1:
                                return I12;
                            case 2:
                                return I13;
                            case 3:
                                return I14;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    case 1:
                        switch (x)
                        {
                            case 0:
                                return I21;
                            case 1:
                                return I22;
                            case 2:
                                return I23;
                            case 3:
                                return I24;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    case 2:
                        switch (x)
                        {
                            case 0:
                                return I31;
                            case 1:
                                return I32;
                            case 2:
                                return I33;
                            case 3:
                                return I34;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    case 3:
                        switch (x)
                        {
                            case 0:
                                return I41;
                            case 1:
                                return I42;
                            case 2:
                                return I43;
                            case 3:
                                return I44;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(y));
                }
            }
            set
            {
                switch (y)
                {
                    case 0:
                        switch (x)
                        {
                            case 0:
                                I11 = value;
                                return;
                            case 1:
                                I12 = value;
                                return;
                            case 2:
                                I13 = value;
                                return;
                            case 3:
                                I14 = value;
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    case 1:
                        switch (x)
                        {
                            case 0:
                                I21 = value;
                                return;
                            case 1:
                                I22 = value;
                                return;
                            case 2:
                                I23 = value;
                                return;
                            case 3:
                                I24 = value;
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    case 2:
                        switch (x)
                        {
                            case 0:
                                I31 = value;
                                return;
                            case 1:
                                I32 = value;
                                return;
                            case 2:
                                I33 = value;
                                return;
                            case 3:
                                I34 = value;
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    case 3:
                        switch (x)
                        {
                            case 0:
                                I41 = value;
                                return;
                            case 1:
                                I42 = value;
                                return;
                            case 2:
                                I43 = value;
                                return;
                            case 3:
                                I44 = value;
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x));
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(y));
                }
            }
        }

        public static float GetDet(Matrix3D x)
        {
            float det = x.I11*x.I22*x.I33 - x.I11*x.I23*x.I32 - x.I12*x.I21*x.I33 + x.I12*x.I23*x.I31 + x.I13*x.I21*x.I32 - x.I13*x.I22*x.I31;
            return det;
        }

        public static Matrix3D GetInvertible(Matrix3D x, out bool exists)
        {
            var result = new Matrix3D();

            float i1122 = x.I11*x.I22;
            float i1123 = x.I11*x.I23;
            float i2133 = x.I21*x.I33;
            float i1223 = x.I12*x.I23;
            float i2132 = x.I21*x.I32;
            float det = i1122 * x.I33 - i1123 * x.I32 - x.I12 * i2133 + i1223 * x.I31 + x.I13 * i2132 - x.I13 * x.I22 * x.I31;

            if (det == 0)
            {
                exists = false;
                return result;
            }

            exists = true;
            result.I11 = (x.I22*x.I33 - x.I32*x.I23)/det;
            result.I21 = (x.I31*x.I23 - i2133)/det;
            result.I31 = (i2132 - x.I31*x.I22)/det;
            result.I12 = (x.I32*x.I13 - x.I12*x.I33)/det;
            result.I22 = (x.I11*x.I33 - x.I31*x.I13)/det;
            result.I32 = (x.I31*x.I12 - x.I11*x.I32)/det;
            result.I13 = (i1223 - x.I22*x.I13)/det;
            result.I23 = (x.I21*x.I13 - i1123)/det;
            result.I33 = (i1122 - x.I21*x.I12)/det;

            result.I44 = x.I44;

            return result;
        }

        public static Matrix3D Add(Matrix3D x, Matrix3D y)
        {
            var result = new Matrix3D();
            result.I11 = x.I11 + y.I11;
            result.I12 = x.I12 + y.I12;
            result.I13 = x.I13 + y.I13;
            result.I14 = x.I14 + y.I14;
            result.I21 = x.I21 + y.I21;
            result.I22 = x.I22 + y.I22;
            result.I23 = x.I23 + y.I23;
            result.I24 = x.I24 + y.I24;
            result.I31 = x.I31 + y.I31;
            result.I32 = x.I32 + y.I32;
            result.I33 = x.I33 + y.I33;
            result.I34 = x.I34 + y.I34;
            result.I41 = x.I41 + y.I41;
            result.I42 = x.I42 + y.I42;
            result.I43 = x.I43 + y.I43;
            result.I44 = x.I44 + y.I44;
            return result;
        }

        public static Matrix3D Mul(Matrix3D x, Matrix3D y)
        {
            var result = new Matrix3D();
            result.I11 = x.I11 * y.I11 + x.I12 * y.I21 + x.I13 * y.I31 + x.I14 * y.I41;
            result.I12 = x.I11 * y.I12 + x.I12 * y.I22 + x.I13 * y.I32 + x.I14 * y.I42;
            result.I13 = x.I11 * y.I13 + x.I12 * y.I23 + x.I13 * y.I33 + x.I14 * y.I43;
            result.I14 = x.I11 * y.I14 + x.I12 * y.I24 + x.I13 * y.I34 + x.I14 * y.I44;
            result.I21 = x.I21 * y.I11 + x.I22 * y.I21 + x.I23 * y.I31 + x.I24 * y.I41;
            result.I22 = x.I21 * y.I12 + x.I22 * y.I22 + x.I23 * y.I32 + x.I24 * y.I42;
            result.I23 = x.I21 * y.I13 + x.I22 * y.I23 + x.I23 * y.I33 + x.I24 * y.I43;
            result.I24 = x.I21 * y.I14 + x.I22 * y.I24 + x.I23 * y.I34 + x.I24 * y.I44;
            result.I31 = x.I31 * y.I11 + x.I32 * y.I21 + x.I33 * y.I31 + x.I34 * y.I41;
            result.I32 = x.I31 * y.I12 + x.I32 * y.I22 + x.I33 * y.I32 + x.I34 * y.I42;
            result.I33 = x.I31 * y.I13 + x.I32 * y.I23 + x.I33 * y.I33 + x.I34 * y.I43;
            result.I34 = x.I31 * y.I14 + x.I32 * y.I24 + x.I33 * y.I34 + x.I34 * y.I44;
            result.I41 = x.I41 * y.I11 + x.I42 * y.I21 + x.I43 * y.I31 + x.I44 * y.I41;
            result.I42 = x.I41 * y.I12 + x.I42 * y.I22 + x.I43 * y.I32 + x.I44 * y.I42;
            result.I43 = x.I41 * y.I13 + x.I42 * y.I23 + x.I43 * y.I33 + x.I44 * y.I43;
            result.I44 = x.I41 * y.I14 + x.I42 * y.I24 + x.I43 * y.I34 + x.I44 * y.I44;
            return result;
        }

        public static Vector3D Mul(Matrix3D x, Vector3D y)
        {
            var result = new Vector3D();
            result.I1 = x.I11 * y.I1 + x.I12 * y.I2 + x.I13 * y.I3 + x.I14 * y.I4;
            result.I2 = x.I21 * y.I1 + x.I22 * y.I2 + x.I23 * y.I3 + x.I24 * y.I4;
            result.I3 = x.I31 * y.I1 + x.I32 * y.I2 + x.I33 * y.I3 + x.I34 * y.I4;
            result.I4 = x.I41 * y.I1 + x.I42 * y.I2 + x.I43 * y.I3 + x.I44 * y.I4;
            return result;
        }

        /// <summary>
        /// Матрица параллельного переноса
        /// </summary>
        public static Matrix3D Transfer(float dx, float dy, float dz)
        {
            var result = new Matrix3D();
            result.I11 = 1;
            result.I22 = 1;
            result.I33 = 1;
            result.I44 = 1;
            result.I14 = dx;
            result.I24 = dy;
            result.I34 = dz;
            return result;
        }

        /// <summary>
        /// Матрица масштабирования
        /// </summary>
        public static Matrix3D Scale(float kx, float ky, float kz)
        {
            var result = new Matrix3D();
            result.I11 = kx;
            result.I22 = ky;
            result.I33 = kz;
            result.I14 = 1;
            return result;
        }

        /// <summary>
        /// Матрица масштабирования по X
        /// </summary>
        public static Matrix3D ScaleX(float kx)
        {
            var result = new Matrix3D();
            result.I11 = kx;
            result.I22 = 1;
            result.I33 = 1;
            result.I14 = 1;
            return result;
        }

        /// <summary>
        /// Матрица масштабирования по y
        /// </summary>
        public static Matrix3D ScaleY(float ky)
        {
            var result = new Matrix3D();
            result.I11 = 1;
            result.I22 = ky;
            result.I33 = 1;
            result.I14 = 1;
            return result;
        }

        /// <summary>
        /// Матрицу масштабирования по z
        /// </summary>
        public static Matrix3D ScaleZ(float kz)
        {
            var result = new Matrix3D();
            result.I11 = 1;
            result.I22 = 1;
            result.I33 = kz;
            result.I14 = 1;
            return result;
        }

        /// <summary>
        /// Матрица поворота по X
        /// </summary>
        public static Matrix3D RotateX(float cos, float sin)
        {
            var result = new Matrix3D();
            result.I11 = 1;
            result.I44 = 1;
            result.I22 = cos;
            result.I23 = sin;
            result.I32 = -sin;
            result.I33 = cos;
            return result;
        }

        /// <summary>
        /// Матрица поворота по Y
        /// </summary>
        public static Matrix3D RotateY(float cos, float sin)
        {
            var result = new Matrix3D();
            result.I22 = 1;
            result.I44 = 1;
            result.I11 = cos;
            result.I13 = -sin;
            result.I31 = sin;
            result.I33 = cos;
            return result;
        }

        /// <summary>
        /// Матрица поворота по Z
        /// </summary>
        public static Matrix3D RotateZ(float cos, float sin)
        {
            var result = new Matrix3D();
            result.I33 = 1;
            result.I44 = 1;
            result.I11 = cos;
            result.I22 = cos;
            result.I12 = sin;
            result.I21 = -sin;
            return result;
        }

        /// <summary>
        /// Матрица перспективной проекции
        /// </summary>
        /// <param name="fov">Угол обзора камеры в играх выбирают 60°С 90°С 120°С</param>
        /// <param name="aspect">Соотношение сторон экрана x'/ y', обычно 4/3; 5/4; 16/9 просто берем разрешения экрана и делим ширину на высоту. </param>
        /// <param name="nearPlane">Передняя плоскость ее Z координата </param>
        /// <param name="farPlane">Задняя плоскость ее Z координата</param>
        /// <returns></returns>
        public static Matrix3D Perspective(float fov, float aspect, float nearPlane, float farPlane)
        {
            float h = 1f / (float)System.Math.Tan(fov / 2f);
            float dPlane = farPlane - nearPlane;
            float a = farPlane / dPlane;

            var result = new Matrix3D();
            result.I11 = h;
            result.I22 = aspect * h;
            result.I33 = a;
            result.I44 = -nearPlane * a;
            return result;
        }

        public static Matrix3D LookAt(Vector3D eye, Vector3D center, Vector3D up)
        {
            Vector3D z = Vector3D.Normilize(Vector3D.Sub(eye, center));
            Vector3D x = Vector3D.Normilize(Vector3D.Mul(up, z));
            Vector3D y = Vector3D.Normilize(Vector3D.Mul(z, x));
            var minv = new Matrix3D();
            var tr = new Matrix3D();
            for (int i = 0; i < 3; i++)
            {
                minv[0, i] = x[i];
                minv[1, i] = y[i];
                minv[2, i] = z[i];
                tr[i, 3] = -center[i];
            }
            Matrix3D modelView = Mul(minv, tr);
            return modelView;
        }

        /// <summary>
        /// Матрица перехода к экранным координатам.
        /// Куб мировых координат [-1,1]*[-1,1]*[-1,1] отображается в куб экранных координат (куб, т.к. у нас z-буфер) [x, x + width][y, y + height][0, d],
        /// где d — это разрешение z-буфера (255).
        /// </summary>
        public static Matrix3D Viewport(int x, int y, int width, int height, int depth)
        {
            var result = new Matrix3D
            {
                I14 = x + width / 2f,
                I24 = y + height / 2f,
                I34 = depth / 2f,
                I11 = width / 2f,
                I22 = height / 2f,
                I33 = depth / 2f,
                I43 = -depth
            };
            return result;
        }

        public static Matrix3D operator *(Matrix3D x, Matrix3D y)
        {
            return Mul(x, y);
        }

        public static Vector3D operator *(Matrix3D x, Vector3D y)
        {
            return Mul(x, y);
        }

        public static Matrix3D operator +(Matrix3D x, Matrix3D y)
        {
            return Add(x, y);
        }

        /// <summary>
        /// Единичная матрица
        /// </summary>
        public static readonly Matrix3D E = new Matrix3D { I11 = 1, I22 = 1, I33 = 1, I44 = 1 };
    }
}