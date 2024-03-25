using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace APV.Avtoliga.Common
{
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
    /// Тип обратной связи пользователя
    /// </summary>
    [Serializable]
    [DataContract]
    public enum FeedbackType
    {
        /// <summary>
        /// Заявка на запччасть (заказ)
        /// </summary>
        Order,

        /// <summary>
        /// Отзыв
        /// </summary>
        Feedback,

        /// <summary>
        /// Сотрудничество
        /// </summary>
        Cooperation
    }

    [Serializable]
    [DataContract]
    public enum HelpType
    {
        None,

        /// <summary>
        /// Как заказать?
        /// </summary>
        HowToOrder,

        /// <summary>
        /// Оплата и гарантии
        /// </summary>
        Guarantee,

        /// <summary>
        /// Доставка заказа
        /// </summary>
        Delivery,

        /// <summary>
        /// Возврат деталей
        /// </summary>
        Back
    }
}