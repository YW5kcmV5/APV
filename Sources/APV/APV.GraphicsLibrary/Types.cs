using System;
using System.Runtime.Serialization;

namespace APV.GraphicsLibrary
{
    /// <summary>
    /// Формат картинки
    /// </summary>
    [Serializable]
    [DataContract]
    public enum ImageFormat
    {
        [EnumMember]
        Png,

        [EnumMember]
        Bmp,

        [EnumMember]
        Emf,

        [EnumMember]
        Exif,

        [EnumMember]
        Gif,

        [EnumMember]
        Icon,

        [EnumMember]
        Jpeg,

        [EnumMember]
        Tiff,

        [EnumMember]
        Wmf,

        [EnumMember]
        MemoryBmp,
    }

}