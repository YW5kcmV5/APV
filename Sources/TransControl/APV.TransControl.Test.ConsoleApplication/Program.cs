using System;
using APV.Common;

namespace APV.Vodokanal.Test.ConsoleApplication
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
                SimpleOracleConnector.Execute();

                Console.WriteLine("Success.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}\r\nTrace={1}", ex.Message, ex.ToTraceString());
            }
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }
    }
}