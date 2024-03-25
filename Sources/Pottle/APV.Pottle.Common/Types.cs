using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using APV.Common.Attributes.EnumAttributes;

namespace APV.Pottle.Common
{
    /// <summary>
    /// Состояние закупки
    /// </summary>
    [Serializable]
    [DataContract]
    public enum PurchaseStatus
    {
        /// <summary>
        /// Активная.
        /// Закупки в которые можно добавлять, удалять товар или менять его количество.
        /// </summary>
        [Description("Активная. Закупки в которые можно добавлять, удалять товар или менять его количество.")]
        [EnumMember]
        Active,

        /// <summary>
        /// В обработке.
        /// Закупки, которые находяться в обработке у продавца - ожидание подтвержедния.
        /// </summary>
        [EnumMember]
        [Description("В обработке. Закупки, которые находяться в обработке у продавца - ожидание подтвержедния.")]
        Process,

        /// <summary>
        /// К оплате.
        /// Подтвержденные закупки с выставленными счетами - необходимо оплатить!
        /// </summary>
        [EnumMember]
        [Description("К оплате. Подтвержденные закупки с выставленными счетами - необходимо оплатить!")]
        Accept,

        /// <summary>
        /// В доставке на пункт выдачи.
        /// Оплаченные закупки, осталось дождаться доставки в пункт выдачи.
        /// </summary>
        [EnumMember]
        [Description("В доставке на пункт выдачи. Оплаченные закупки, осталось дождаться доставки в пункт выдачи.")]
        Delivery,

        /// <summary>
        /// Готовые.
        /// Необходимо забрать из пункта выдачи.
        /// </summary>
        [EnumMember]
        [Description("Готовые. Необходимо забрать из пункта выдачи.")]
        Ready,

        /// <summary>
        /// Завершённые закупки.
        /// Всё завершённые закупки.
        /// </summary>
        [EnumMember]
        [Description("Завершённые закупки. Всё завершённые закупки.")]
        Completed,
    }

    /// <summary>
    /// Роль (должность) участника (работника) магазина
    /// </summary>
    [Serializable]
    public enum StoreEmployeeRole
    {
        /// <summary>
        /// Владелец
        /// </summary>
        [EnumMember]
        [Description("Владелец")]
        Owner,

        /// <summary>
        /// Продавец
        /// </summary>
        [EnumMember]
        [Description("Продавец")]
        Seller,

        /// <summary>
        /// Пользователь, оказывающий услуги точки выдачи
        /// </summary>
        [EnumMember]
        [Description("Пользователь, оказывающий услуги точки выдачи")]
        Outpost,

        /// <summary>
        /// Пользователь, оказывающий помощь по учету и контролю
        /// </summary>
        [EnumMember]
        [Description("Пользователь, оказывающий помошь по учету и контролю")]
        Checkman,
    }

    /// <summary>
    /// Место хранения данных
    /// </summary>
    [Serializable]
    [DataContract]
    public enum DataStorage
    {
        /// <summary>
        /// База данных
        /// </summary>
        [EnumMember]
        [Description("База данных.")]
        Database,

        /// <summary>
        /// Файловая система
        /// </summary>
        [EnumMember]
        [Description("Файловая система.")]
        FileSystem,
    }

    /// <summary>
    /// Часть речи
    /// </summary>
    [Serializable]
    [DataContract]
    public enum PartOfSpeech
    {
        /// <summary>
        /// Неизвестный
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Имя существительное
        /// </summary>
        [EnumMember]
        Noun = 1,

        /// <summary>
        /// Имя собственное
        /// </summary>
        [EnumMember]
        ProperNoun = 2,

        /// <summary>
        /// Местоимение ("я", "ты", "он", "она", "меня", "мне", ...)
        /// </summary>
        [EnumMember]
        Pronoun = 4,

        /// <summary>
        /// Местоимение-существительное
        /// </summary>
        [EnumMember]
        PronounNoun = 8,

        /// <summary>
        /// Имя прилагательное
        /// </summary>
        [EnumMember]
        Adjective = 16,

        /// <summary>
        /// Имя числительное
        /// </summary>
        [EnumMember]
        Numeral = 32,

        /// <summary>
        /// Глагол
        /// </summary>
        [EnumMember]
        Verb = 64,

        /// <summary>
        /// Инфинитив
        /// </summary>
        [EnumMember]
        Infinitive = 128,

        /// <summary>
        /// Безличный глагол ("надо", "можно", "лучше", "стало" ...)
        /// </summary>
        [EnumMember]
        ImpersonalVerb = 256,

        /// <summary>
        /// Воcклицательный глагол ("стоп")
        /// </summary>
        [EnumMember]
        ExclamationVerb = 512,

        /// <summary>
        /// Наречие
        /// </summary>
        [EnumMember]
        Adverb = 1024,

        /// <summary>
        /// Предлог ("в", "без", "до", "из", "к", "на", "по", "о", "от", "перед", "при", "через", "с", "у", "за", "над", "об", "под", "про", "для", ...)
        /// </summary>
        [EnumMember]
        Excuse = 2048,

        /// <summary>
        /// Союз ("как", "словно", "и", "а", "что", "или", "но" ...)
        /// </summary>
        [EnumMember]
        Union = 4096,

        /// <summary>
        /// Частица ("ужель", "вот то-то", "ух", "фу", "ай", "ура" ...)
        /// </summary>
        [EnumMember]
        Particle = 8192,

        /// <summary>
        /// Притяжательная частица ("её", "его", "их")
        /// </summary>
        [EnumMember]
        PossessiveParticle = 16384,

        /// <summary>
        /// Междометие ("о", "ох", "ах", "ну", "ух", "ай" ...)
        /// </summary>
        [EnumMember]
        Interjection = 32768,

        /// <summary>
        /// Причастие
        /// </summary>
        [EnumMember]
        Communion = 65536,

        /// <summary>
        /// Деепричастие ("скользя", "намереваясь", "краснея", "извиняясь", "изучая", "приоткрыв" ...)
        /// </summary>
        [EnumMember]
        Participle = 131072,

        /// <summary>
        /// Вводное слово ("вообще", "наконец", "вроде", "наверное", "таким образом", "между прочим", "по-моему", "в частности" ...)
        /// </summary>
        [EnumMember]
        Parenthesis = 262144,

        /// <summary>
        /// Послеслог ("позже")
        /// </summary>
        [EnumMember]
        Postposition = 524288,
    }

    /// <summary>
    /// Множество частей речи
    /// </summary>
    [Flags]
    [Serializable]
    [DataContract]
    public enum PartsOfSpeech
    {
        [EnumMember]
        Unknown = PartOfSpeech.Unknown,

        [EnumMember]
        Noun = PartOfSpeech.Noun,

        [EnumMember]
        ProperNoun = PartOfSpeech.ProperNoun,

        [EnumMember]
        Pronoun = PartOfSpeech.Pronoun,

        [EnumMember]
        PronounNoun = PartOfSpeech.PronounNoun,

        [EnumMember]
        Adjective = PartOfSpeech.Adjective,

        [EnumMember]
        Numeral = PartOfSpeech.Numeral,

        [EnumMember]
        Verb = PartOfSpeech.Verb,

        [EnumMember]
        Infinitive = PartOfSpeech.Infinitive,

        [EnumMember]
        ImpersonalVerb = PartOfSpeech.ImpersonalVerb,

        [EnumMember]
        ExclamationVerb = PartOfSpeech.ExclamationVerb,

        [EnumMember]
        Adverb = PartOfSpeech.Adverb,

        [EnumMember]
        Excuse = PartOfSpeech.Excuse,

        [EnumMember]
        Union = PartOfSpeech.Union,

        [EnumMember]
        Particle = PartOfSpeech.Particle,

        [EnumMember]
        PossessiveParticle = PartOfSpeech.PossessiveParticle,

        [EnumMember]
        Interjection = PartOfSpeech.Interjection,

        [EnumMember]
        Communion = PartOfSpeech.Communion,

        [EnumMember]
        Participle = PartOfSpeech.Participle,

        [EnumMember]
        Parenthesis = PartOfSpeech.Parenthesis,

        [EnumMember]
        Postposition = PartOfSpeech.Postposition,
    }

    [Serializable]
    [DataContract]
    public enum WordReferenceType
    {
        Original = 0, 
        
        /// <summary>
        /// Связь родитель-ребёнок (текущее "слово", например "ели" образовано от слова-ссылки "ель" или "есть") 
        /// </summary>
        Parent = 1,

        /// <summary>
        /// Текущее слово является синонимом
        /// </summary>
        Synonym = 2,

        /// <summary>
        /// Текущее слово является синонимо родителя
        /// </summary>
        ParentSynonym = 3,

        /// <summary>
        /// Связь между именем собственным и производными от него (альтернативное нмписание имени, транслитерация)
        /// </summary>
        ParentName = 4,
    }

    /// <summary>
    /// Уточнение типа позиции - номер квартиры, офиса или строения
    /// </summary>
    [Serializable]
    [DataContract]
    public enum AddressPositionType
    {
        /// <summary>
        /// Квартира (кв., к.)
        /// </summary>
        [EnumMember]
        [Prefix("квартира")]
        Apartments,

        /// <summary>
        /// Помещение (пом., п.)
        /// </summary>
        [EnumMember]
        [Prefix("помещение")]
        Placement,

        /// <summary>
        /// Офис (оф.)
        /// </summary>
        [EnumMember]
        [Prefix("офис")]
        Office,

        /// <summary>
        /// Строение (ст., с.)
        /// </summary>
        [EnumMember]
        [Prefix("строение")]
        Building,

        /// <summary>
        /// Нет уточнения (возможно весь дом)
        /// </summary>
        [EnumMember]
        None,

        /// <summary>
        /// Другое
        /// </summary>
        [EnumMember]
        Other,
    }

    /// <summary>
    /// Тип ключевого слова (в зависисимости от типа ключевого слова используются разные коэффициенты для рассчета очков)
    /// </summary>
    [Serializable]
    [DataContract]
    public enum KeywordType
    {
        /// <summary>
        /// Имя
        /// </summary>
        [EnumMember]
        Name,

        /// <summary>
        /// Альтернативное имя (например ник, или синоним)
        /// </summary>
        [EnumMember]
        AlternativeName,

        /// <summary>
        /// Характеристика
        /// </summary>
        [EnumMember]
        Characteristic,

        /// <summary>
        /// Описание
        /// </summary>
        [EnumMember]
        Description,
    }

    [Serializable]
    [DataContract]
    public enum SettingType
    {
        [EnumMember]
        String,

        [EnumMember]
        Xml,

        [EnumMember]
        Boolean,

        [EnumMember]
        Long,

        [EnumMember]
        Decimal,

        [EnumMember]
        Enum
    }

    /// <summary>
    /// Тип cвойства товара ("описание", "комплектация", "иснтрукция", "характеристика"...)
    /// </summary>
    [Serializable]
    [DataContract]
    public enum ProductOptionType
    {
        /// <summary>
        /// Описание
        /// </summary>
        [EnumMember]
        Description,

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        [EnumMember]
        AdditionalInfo,

        /// <summary>
        /// Комплектация
        /// </summary>
        [EnumMember]
        Complectation,

        /// <summary>
        /// Инструкция
        /// </summary>
        [EnumMember]
        Instruction,

        /// <summary>
        /// Внимание
        /// </summary>
        [EnumMember]
        Warning,

        /// <summary>
        /// Характеристика
        /// </summary>
        [EnumMember]
        Characteristic,
    }

    /// <summary>
    /// Тип модификатора товара ("цвет", "размер").
    /// При заказе товара дополнительно небходимо указывать значение модификатора
    /// </summary>
    [Serializable]
    [DataContract]
    public enum ProductCharacteristicModifier
    {
        /// <summary>
        /// Модификатор отсутсвует
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Цвет
        /// </summary>
        [EnumMember]
        Color = 2,

        /// <summary>
        /// Размер
        /// </summary>
        [EnumMember]
        Size = 4,
    }

    /// <summary>
    /// Множество модификаторов
    /// </summary>
    [Serializable]
    [DataContract]
    [Flags]
    public enum ProductCharacteristicModifiers
    {
        [EnumMember]
        None = ProductCharacteristicModifier.None,

        [EnumMember]
        Color = ProductCharacteristicModifier.Color,

        [EnumMember]
        Size = ProductCharacteristicModifier.Size,
    }
}