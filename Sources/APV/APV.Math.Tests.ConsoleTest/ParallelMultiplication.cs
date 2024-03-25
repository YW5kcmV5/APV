using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using APV.Common;

namespace APV.Math.Tests.ConsoleTest
{
    public static class ParallelMultiplication
    {
        public class Data
        {
            public double X;
            public double Y;
            public double Result;
        }

        private class ManagerMultiplication
        {
            private static readonly Stack<Data> _stack = new Stack<Data>(10000000);
            private static readonly object _lock = new object();

            public static void Invoker(object x)
            {
                do
                {
                    Data data = null;
                    lock (_lock)
                    {
                        data = (_stack.Count > 0) ? _stack.Pop() : null;
                    }
                    if (data != null)
                    {
                        data.Result = data.X*data.Y;
                    }
                } while (true);
            }

            public static void Add(Data data)
            {
                lock (_lock)
                {
                    _stack.Push(data);
                }
            }

            static ManagerMultiplication()
            {
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
                ThreadPool.QueueUserWorkItem(Invoker, null);
            }

            public static void Init()
            {
            }
        }

        private static List<Data> _queue = new List<Data>(100000);

        private static readonly Random Rnd = new Random();

        private static readonly object Lock = new object();

        private static void Initialize(Data[] data)
        {
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                data[i] = new Data
                    {
                        X = Rnd.Next(int.MaxValue)/1000.0,
                        Y = Rnd.Next(int.MaxValue)/1000.0,
                    };
            }
        }

        private static void MultiplySingle(Data[] data)
        {
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                Data item = data[i];
                data[i].Result = item.X * item.Y;
            }
        }

        private static void MultiplyParallel(Data[] data)
        {
            //var tuple = new Tuple<int, Data[]>(0, data);
            //Parallel.ForEach(data, item => item.Result = item.X * item.Y);

            //int length = data.Length;
            //for (int i = 0; i < length; i++)
            //{
            //    Data item = data[i];
            //    ManagerMultiplication.Add(item);
            //}
        }

        public static TimeSpan Calculate(Action<Data[]> multiply, int n)
        {
            var data = new Data[n];
            Initialize(data);
            const int count = 10;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                multiply(data);
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static void Execute()
        {
            try
            {
                ManagerMultiplication.Init();

                var counts = new[] {100000, 1000000, 10000000};

                for (int i = 0; i < counts.Length; i++)
                {
                    int count = counts[i];

                    TimeSpan singleParallel = Calculate(MultiplySingle, count);
                    TimeSpan parallell = Calculate(MultiplyParallel, count);

                    Console.WriteLine("MultiplySingle[{0}]:{1}", count, singleParallel);
                    Console.WriteLine("MultiplyParallel[{0}]:{1}", count, parallell);
                    Console.WriteLine();
                }
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
