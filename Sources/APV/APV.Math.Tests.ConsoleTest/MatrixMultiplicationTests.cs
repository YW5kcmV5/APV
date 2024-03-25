using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using APV.Common;

namespace APV.Math.Tests.ConsoleTest
{
    /// <summary>
    /// http://codepyre.com/2010/03/parallel-matrix-multiplication-with-the-task-parallel-library-tpl/
    /// Summary:
    /// 1) Array of array ([][]) is faster then array ([]) and faster that dimension array ([,]);
    /// 2) Parrallel is good but only when matrix dimension more then 50x50
    /// 3) What is faster float or double:
    /// -  64-bit double faster for 64 bit CPU (double is 64-bit floating-point)
    /// </summary>
    public static class MatrixMultiplicationTests
    {
        private static void Initialize(int n, double[][] a, double[][] b, double[][] c)
        {
            int index = 0;
            do
            {
                c[index] = new double[n];
                b[index] = new double[n];
                a[index] = new double[n];
                int column = 0;
                int value = 0;
                do
                {
                    c[index][column] = 0;
                    double num7 = value; // used so we only cast once
                    b[index][column] = num7;
                    a[index][column] = num7;
                    column++;
                    value = index + value;
                } while (column < n);
                index++;
            } while (index < n);
        }

        private static void Multiply(Tuple<int, double[][]> a, double[][] b, double[][] c)
        {
            int size = a.Item2.GetLength(0);
            int cols = b[0].Length;
            double[][] ai = a.Item2;

            int i = 0;
            int offset = a.Item1;
            do
            {
                int k = 0;
                do
                {
                    int j = 0;
                    do
                    {
                        double[] ci = c[offset];
                        ci[j] = (ai[i][k] * b[k][j]) + ci[j];
                        j++;
                    } while (j < cols);
                    k++;
                } while (k < cols);
                i++;
                offset++;
            } while (i < size);
        }

        private static IEnumerable<Tuple<int, double[][]>> PartitionData(int n, double[][] a, int chunkFactor)
        {
            int remaining = n;
            int currentRow = 0;

            while (remaining > 0)
            {
                if (remaining < chunkFactor)
                {
                    chunkFactor = remaining;
                }

                remaining = remaining - chunkFactor;
                var ai = new double[chunkFactor][];
                for (int i = 0; i < chunkFactor; i++)
                {
                    ai[i] = a[currentRow + i];
                }

                int oldRow = currentRow;
                currentRow += chunkFactor;
                yield return new Tuple<int, double[][]>(oldRow, ai);
            }
        }

        public static void MultiplySimpleParallel(double[][] a, double[][] b, double[][] c, int n)
        {
            Parallel.For(0, n, i =>
            {
                for (int k = 0; k < n; k++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double[] ci = c[i];
                        ci[j] = (a[i][k] * b[k][j]) + ci[j];
                    }
                }
            });
        }

        public static void MultiplyOptimizedParallel(double[][] a, double[][] b, double[][] c, int n)
        {
            int chunkFactor = Environment.ProcessorCount;
            IEnumerable<Tuple<int, double[][]>> data = PartitionData(n, a, chunkFactor);
            Parallel.ForEach(data, item => Multiply(item, b, c));
        }

        public static void MultiplyJaggedFromCpp(double[][] a, double[][] b, double[][] c, int n)
        {
            int i = 0;
            if (0 < n)
            {
                do
                {
                    int k = 0;
                    do
                    {
                        int j = 0;
                        do
                        {
                            double[] ci = c[i];
                            ci[j] = (a[i][k]*b[k][j]) + ci[j];
                            j++;
                        } while (j < n);
                        k++;
                    } while (k < n);
                    i++;
                } while (i < n);
            }
        }

        public static TimeSpan Calculate(Action<double[][], double[][], double[][], int> multiply, int n)
        {
            var c = new double[n][];
            var a = new double[n][];
            var b = new double[n][];
            Initialize(n, a, b, c);
            const int count = 10;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                multiply(a, b, c, n);
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static void Execute()
        {
            try
            {
                TimeSpan jaggedFromCpp4 = Calculate(MultiplyJaggedFromCpp, 4);
                TimeSpan simpleParallel4 = Calculate(MultiplySimpleParallel, 4);
                TimeSpan multiplyOptimizedParallel4 = Calculate(MultiplyOptimizedParallel, 4);

                TimeSpan jaggedFromCpp50 = Calculate(MultiplyJaggedFromCpp, 50);
                TimeSpan simpleParallel50 = Calculate(MultiplySimpleParallel, 50);
                TimeSpan multiplyOptimizedParallel50 = Calculate(MultiplyOptimizedParallel, 50);

                TimeSpan jaggedFromCpp100 = Calculate(MultiplyJaggedFromCpp, 100);
                TimeSpan simpleParallel100 = Calculate(MultiplySimpleParallel, 100);
                TimeSpan multiplyOptimizedParallel100 = Calculate(MultiplyOptimizedParallel, 100);

                TimeSpan jaggedFromCpp400 = Calculate(MultiplyJaggedFromCpp, 400);
                TimeSpan simpleParallel400 = Calculate(MultiplySimpleParallel, 400);
                TimeSpan multiplyOptimizedParallel400 = Calculate(MultiplyOptimizedParallel, 400);

                TimeSpan jaggedFromCpp1000 = Calculate(MultiplyJaggedFromCpp, 1000);
                TimeSpan simpleParallel1000 = Calculate(MultiplySimpleParallel, 1000);
                TimeSpan multiplyOptimizedParallel1000 = Calculate(MultiplyOptimizedParallel, 1000);

                Console.WriteLine("MultiplyJaggedFromCpp[4]:{0}", jaggedFromCpp4);
                Console.WriteLine("MultiplySimpleParallel[4]:{0}", simpleParallel4);
                Console.WriteLine("MultiplyOptimizedParallel[4]:{0}", multiplyOptimizedParallel4);

                Console.WriteLine("MultiplyJaggedFromCpp[50]:{0}", jaggedFromCpp50);
                Console.WriteLine("MultiplySimpleParallel[50]:{0}", simpleParallel50);
                Console.WriteLine("MultiplyOptimizedParallel[50]:{0}", multiplyOptimizedParallel50);

                Console.WriteLine("MultiplyJaggedFromCpp[100]:{0}", jaggedFromCpp100);
                Console.WriteLine("MultiplySimpleParallel[100]:{0}", simpleParallel100);
                Console.WriteLine("MultiplyOptimizedParallel[100]:{0}", multiplyOptimizedParallel100);

                Console.WriteLine("MultiplyJaggedFromCpp[400]:{0}", jaggedFromCpp400);
                Console.WriteLine("MultiplySimpleParallel[400]:{0}", simpleParallel400);
                Console.WriteLine("MultiplyOptimizedParallel[400]:{0}", multiplyOptimizedParallel400);

                Console.WriteLine("MultiplyJaggedFromCpp[1000]:{0}", jaggedFromCpp1000);
                Console.WriteLine("MultiplySimpleParallel[1000]:{0}", simpleParallel1000);
                Console.WriteLine("MultiplyOptimizedParallel[1000]:{0}", multiplyOptimizedParallel1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}", ex.ToTraceString());
                throw;
            }
        }
    }
}
