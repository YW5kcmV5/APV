namespace APV.Pottle.Toolkit.Linguistics.KeywordManagers.Solarix
{
    public sealed class WordInfo
    {
        public string Name { get; set; }

        public long WordId { get; set; }

        public string[] PartsOfSpeech { get; set; }

        public string[] PrimaryLemmas { get; set; }

        public string[] SecondaryLemmas { get; set; }
    }
}