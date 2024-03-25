namespace APV.Pottle.Toolkit.Linguistics.Interfaces
{
    public interface ITranslitManager
    {
        string[] Translit(string languageCodeFrom, string languageCodeTo, string word, int limit);

        string[] GetTranslits(string languageCodeFrom);
    }
}