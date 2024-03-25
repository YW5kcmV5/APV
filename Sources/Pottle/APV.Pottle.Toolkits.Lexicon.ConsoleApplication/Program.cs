using System;
using APV.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication
{
    class Program
    {
        public const string AdminUsername = @"Admin";

        public const string AdminPasswod = @"123";

        public const string VocabularyPath = @"C:\Program Files (x86)\RussianGrammaticalDictionary\Vocabularies\vocabulary_ru_153452_(1457257).xml";

        static void Main(string[] args)
        {
            Console.WindowWidth *= 2;
            Console.WindowHeight *= 2;

            Console.WriteLine("Begin.");
            Console.WriteLine();
            try
            {
                UserManagement.Instance.Login(AdminUsername, AdminPasswod);

                VocabularyInfo vocabulary = VocabularyManager.LoadVocabulary(VocabularyPath);
                VocabularyManager.ExportToDb(vocabulary);
                VocabularyManager.ExportReferencesToDb(vocabulary);

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
