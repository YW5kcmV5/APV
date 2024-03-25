using System.Collections.Generic;
using APV.Math.MathObjects.Vectors;

namespace APV.Math
{
    public static class MathFilters
    {
        #region Median Filter

        /// <summary>
        /// Медианный фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="windowSize">Размер окна</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static T[] MedianFilter<T>(this T[] x, int windowSize = 3)
        {
            if (windowSize % 2 == 0)
            {
                windowSize++;
            }
            if (x.Length <= windowSize)
            {
                return x;
            }

            int median = windowSize / 2;
            var y = new T[x.Length];
            for (int i = 0; i < y.Length; i++)
            {
                if ((i < median) || (i > x.Length - median - 1))
                {
                    y[i] = x[i];
                }
                else
                {
                    var window = new List<T>(windowSize);
                    for (int j = 0; j < windowSize; j++)
                    {
                        window.Add(x[i + j - median]);
                    }
                    window.Sort();
                    y[i] = window[median];
                }
            }

            for (int i = 0; i < median; i++)
            {
                y[i] = y[median];
                y[x.Length - i - 1] = y[x.Length - median - 1];
            }

            return y;
        }

        /// <summary>
        /// Медианный фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="windowSize">Размер окна</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static float[] MedianFilter(this float[] x, int windowSize = 3)
        {
            return MedianFilter<float>(x, windowSize);
        }

        /// <summary>
        /// Медианный фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="windowSize">Размер окна</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static int[] MedianFilter(this int[] x, int windowSize = 3)
        {
            return MedianFilter<int>(x, windowSize);
        }

        /// <summary>
        /// Медианный фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="windowSize">Размер окна</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static IntVector MedianFilter(this IntVector x, int windowSize = 3)
        {
            if (windowSize % 2 == 0)
            {
                windowSize++;
            }
            if (x.Length <= windowSize)
            {
                return x;
            }

            int median = windowSize / 2;
            var y = new IntVector(x.Length);
            for (int i = 0; i < y.Length; i++)
            {
                if ((i < median) || (i > x.Length - median - 1))
                {
                    y[i] = x[i];
                }
                else
                {
                    var window = new List<int>(windowSize);
                    for (int j = 0; j < windowSize; j++)
                    {
                        window.Add(x[i + j - median]);
                    }
                    window.Sort();
                    y[i] = window[median];
                }
            }

            for (int i = 0; i < median; i++)
            {
                y[i] = y[median];
                y[x.Length - i - 1] = y[x.Length - median - 1];
            }

            return y;
        }

        #endregion

        #region Aperiodic Smoothing

        /// <summary>
        /// Апериодическое сглаживание (фильтр первого порядка)
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="k">Коэффициент сглаживания (0..1, 1 - нет сглаживания)</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static float[] AperiodicSmoothingForward(this float[] x, float k = 0.3f)
        {
            var y = new float[x.Length];
            if (x.Length > 0)
            {
                y[0] = x[0];
                for (int i = 1; i < y.Length; i++)
                {
                    y[i] = k * x[i] + (1 - k) * y[i - 1];
                }
            }
            return y;
        }

        /// <summary>
        /// Апериодическое сглаживание (фильтр первого порядка, движение назад)
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="k">Коэффициент сглаживания (0..1, 1 - нет сглаживания)</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static float[] AperiodicSmoothingBackward(this float[] x, float k = 0.3f)
        {
            var y = new float[x.Length];
            if (x.Length > 0)
            {
                y[x.Length - 1] = x[x.Length - 1];
                for (int i = y.Length - 2; i >= 0; i--)
                {
                    y[i] = k * x[i] + (1 - k) * y[i + 1];
                }
            }
            return y;
        }

        /// <summary>
        /// Апериодическое сглаживание (фильтр первого порядка, движение вперед, затем назад)
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="k">Коэффициент сглаживания (0..1, 1 - нет сглаживания)</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static float[] AperiodicSmoothing(this float[] x, float k = 0.3f)
        {
            return AperiodicSmoothingBackward(AperiodicSmoothingForward(x, k), k);
        }

        #endregion

        #region Average

        public static float[] Average(this float[] x, int windowSize = 7)
        {
            int length = x.Length;
            if (windowSize % 2 == 0)
            {
                windowSize++;
            }
            int median = windowSize / 2;
            var result = new float[length];
            for (int i = median; i < length - median; i++)
            {
                float value = 0.0f;
                for (int j = -median; j <= median; j++)
                {
                    float k = (j == 0) ? 1.0f : 1.0f / (2.0f * System.Math.Abs(j));
                    value += k * x[i + j];
                }
                result[i] = value / windowSize;
            }

            for (int i = 0; i < median; i++)
            {
                result[i] = result[median];
                result[length - i - 1] = result[length - median - 1];
            }

            return result;
        }

        #endregion

        #region Digital Filter

        /// <summary>
        /// Цифровой (пороговый) фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="dxPositive">Размер порога при увеличении функции</param>
        /// <param name="dxNegative">Размер порога при уменьшении функции</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static float[] DigitalFilter(this float[] x, float dxPositive, float dxNegative)
        {
            var y = new float[x.Length];
            y[0] = x[0];
            for (int i = 1; i < y.Length; i++)
            {
                float dx = (x[i] - y[i - 1]);
                bool use = ((dx > 0) && (dx >= dxPositive)) || ((dx < 0) && (dx <= -dxNegative));
                y[i] = use ? x[i] : y[i - 1];
            }
            return y;
        }

        public static float[] DigitalFilter(this float[] x, float dx)
        {
            return DigitalFilter(x, dx, dx);
        }

        #endregion
    }
}