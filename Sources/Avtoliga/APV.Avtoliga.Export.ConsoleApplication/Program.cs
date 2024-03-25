using System;
using APV.Avtoliga.Core.Application;

namespace APV.Avtoliga.Export.ConsoleApplication
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
                ContextManager.LoginAsAdmin();

                //Exporter.ExportTrademarks(@"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Автоберг\Trademarks");
                Exporter.ExportProducts(@"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Автоберг\Products");

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