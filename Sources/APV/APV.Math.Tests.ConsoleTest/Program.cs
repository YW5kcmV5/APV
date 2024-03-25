using System;
using APV.Common;

namespace APV.Math.Tests.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth *= 2;
            Console.WindowHeight *= 2;

            Console.WriteLine("Begin.");
            Console.WriteLine();
            try
            {
                //MathExpressionTest.Execute();
                //MatrixMultiplicationTests.Execute();
                //ParallelMultiplication.Execute();
                MathCalculationTests.Execute();

                Console.WriteLine("Success.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}", ex.ToTraceString());
            }
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }
    }
}