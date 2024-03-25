using System;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [Serializable]
    [DataContract(Namespace = Constants.DictionaryNamespace)]
    public enum CoordType
    {
        [EnumMember]
        Enumeration,

        [EnumMember]
        Boolean,
    }

    [Serializable]
    [DataContract(Namespace = Constants.DictionaryNamespace)]
    public enum CoordValueType
    {
        /// <summary>
        /// Словоизменительная категория, например падеж или число для существительного.
        /// </summary>
        [EnumMember]
        Inflectional = 0,

        /// <summary>
        /// Постоянный морфологический признак словарной статьи, например род для существительного, вид для русского глагола.
        /// </summary>
        [EnumMember]
        Permanent = 1,

        /// <summary>
        /// Необязательный постоянный морфологический признак словарной статьи, 
        /// который может отсутствовать у большинства словарных статей данной части речи, например модальность для глагола.
        /// </summary>
        [EnumMember]
        Optional = 2,
    }

    [Serializable]
    [DataContract(Namespace = Constants.DictionaryNamespace)]
    public enum KeywordType
    {
        /// <summary>
        /// Оригинальное слово
        /// </summary>
        Original,

        /// <summary>
        /// Лемма (включая лемму леммы)
        /// </summary>
        Lemma,

        /// <summary>
        /// Синоним оригинально слова
        /// </summary>
        Synonym,

        /// <summary>
        /// Синоним лемм (синонимы всех лемм вплоть до леммы верхнего уровня)
        /// </summary>
        LemmaSynonym,
    }
}