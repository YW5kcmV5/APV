using System;

namespace APV.Pottle.DatabaseConsoleTest
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
                //GeoLocatorTests.Execute();
                //CacheTests.Execute();
                ProxyTests.Execute();
                //PerformanceTests.Execute();

                Console.WriteLine("Success.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}", ex.Message);
            }
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }
    }
}