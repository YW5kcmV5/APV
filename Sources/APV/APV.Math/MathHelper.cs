
namespace APV.Math
{
    public static class ЕrigonometricHelper
    {
        #region Trigonometric Functions

        // Secant 
        public static double Sec(this double x)
        {
            return 1 / System.Math.Cos(x);
        }

        // Cosecant
        public static double Cosec(this double x)
        {
            return 1 / System.Math.Sin(x);
        }

        // Cotangent 
        public static double Cotan(this double x)
        {
            return 1 / System.Math.Tan(x);
        }

        // Inverse Sine 
        public static double Arcsin(this double x)
        {
            return System.Math.Atan(x / System.Math.Sqrt(-x * x + 1));
        }

        // Inverse Cosine 
        public static double Arccos(this double x)
        {
            return System.Math.Atan(-x / System.Math.Sqrt(-x * x + 1)) + 2 * System.Math.Atan(1);
        }

        // Inverse Secant 
        public static double Arcsec(this double x)
        {
            return 2 * System.Math.Atan(1) - System.Math.Atan(System.Math.Sign(x) / System.Math.Sqrt(x * x - 1));
        }

        // Inverse Cosecant 
        public static double Arccosec(this double x)
        {
            return System.Math.Atan(System.Math.Sign(x) / System.Math.Sqrt(x * x - 1));
        }

        // Inverse Cotangent 
        public static double Arccotan(this double x)
        {
            return 2 * System.Math.Atan(1) - System.Math.Atan(x);
        }

        // Hyperbolic Sine 
        public static double HSin(this double x)
        {
            return (System.Math.Exp(x) - System.Math.Exp(-x)) / 2;
        }

        // Hyperbolic Cosine 
        public static double HCos(this double x)
        {
            return (System.Math.Exp(x) + System.Math.Exp(-x)) / 2;
        }

        // Hyperbolic Tangent 
        public static double HTan(this double x)
        {
            return (System.Math.Exp(x) - System.Math.Exp(-x)) / (System.Math.Exp(x) + System.Math.Exp(-x));
        }

        // Hyperbolic Secant 
        public static double HSec(this double x)
        {
            return 2 / (System.Math.Exp(x) + System.Math.Exp(-x));
        }

        // Hyperbolic Cosecant 
        public static double HCosec(this double x)
        {
            return 2 / (System.Math.Exp(x) - System.Math.Exp(-x));
        }

        // Hyperbolic Cotangent 
        public static double HCotan(this double x)
        {
            return (System.Math.Exp(x) + System.Math.Exp(-x)) / (System.Math.Exp(x) - System.Math.Exp(-x));
        }

        // Inverse Hyperbolic Sine 
        public static double HArcsin(this double x)
        {
            return System.Math.Log(x + System.Math.Sqrt(x * x + 1));
        }

        // Inverse Hyperbolic Cosine 
        public static double HArccos(this double x)
        {
            return System.Math.Log(x + System.Math.Sqrt(x * x - 1));
        }

        // Inverse Hyperbolic Tangent 
        public static double HArctan(this double x)
        {
            return System.Math.Log((1 + x) / (1 - x)) / 2;
        }

        // Inverse Hyperbolic Secant 
        public static double HArcsec(this double x)
        {
            return System.Math.Log((System.Math.Sqrt(-x * x + 1) + 1) / x);
        }

        // Inverse Hyperbolic Cosecant 
        public static double HArccosec(this double x)
        {
            return System.Math.Log((System.Math.Sign(x) * System.Math.Sqrt(x * x + 1) + 1) / x);
        }

        // Inverse Hyperbolic Cotangent 
        public static double HArccotan(this double x)
        {
            return System.Math.Log((x + 1) / (x - 1)) / 2;
        }

        // Logarithm to base N 
        public static double LogN(double x, double n)
        {
            return System.Math.Log(x) / System.Math.Log(n);
        }

        #endregion

        #region Sigma/Logit

        /// <summary>
        /// Сигмоидальная функция (сигмоид). Логистическая функция.
        /// Монотонно возрастающая всюду дифференцируемая S-образная нелинейная функция с насыщением.
        /// Сигмоид позволяет усиливать слабые сигналы и не насыщаться от сильных сигналов.
        /// Гроссберг (1973 год) обнаружил, что подобная нелинейная функция активации решает поставленную им дилемму шумового насыщения.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a">alpha – параметр наклона сигмоидальной функции активации. Изменяя этот параметр, можно построить функции с различной крутизной.</param>
        public static double Sigma(double x, double a = 1.0f)
        {
            return 1.0f / (1.0f + (float)System.Math.Exp(-a * x));
        }

        /// <summary>
        /// Сигмоидальная функция в диапазоне x[0..1] y[0..1]
        /// </summary>
        public static double NormilizedSigma(double x, double a = 1.0f)
        {
            return Sigma(2.0f * x - 1.0f, a);
        }

        /// <summary>
        /// Функция Логит. Инверсия "сигмоидальной", или "логистической" функции.
        /// </summary>
        public static double Logit(double x)
        {
            return System.Math.Log(x / (1 - x));
        }

        /// <summary>
        /// Функция Логит в диапазоне x[0..1] y[0..1]
        /// </summary>
        public static double NormilizedLogit(double x)
        {
            return (Logit(0.00001 + x * 0.99998) + 11.5129156) / 23.0244741;
        }

        #endregion
    }
}