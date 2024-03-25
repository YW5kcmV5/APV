using System;
using APV.Pottle.WebParsers.Avtoberg;

namespace APV.Pottle.Export.ConsoleApplication
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
                new AvtobergWebParser().Export();
                //new VotoniaWebParser().Export();

                //new CountryExporter().Export(@"F:\MyProjects\Pottle\Data\Product database\Country");
                //new CompanyExporter().Export(@"F:\MyProjects\Pottle\Data\Product database\Company");
                //new TrademarkExporter().Export(@"F:\MyProjects\Pottle\Data\Product database\Trademark");
                //new SupplierExporter().Export(@"F:\MyProjects\Pottle\Data\Product database\Supplier");

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