using System;
using System.Runtime.Serialization;

namespace APV.CloudSync.Common
{
    [Serializable]
    [DataContract(Namespace = Constants.NamespaceData)]
    public enum FileSystemOperation
    {
        [EnumMember]
        FileAdd,

        [EnumMember]
        FileDelete,

        [EnumMember]
        FileRename,

        [EnumMember]
        FileModify,

        [EnumMember]
        FolderAdd,

        [EnumMember]
        FolderDelete,

        [EnumMember]
        FolderRename
    }

    [Serializable]
    [DataContract(Namespace = Constants.NamespaceData)]
    public enum SyncOperationState
    {
        [EnumMember]
        Initilialized,

        [EnumMember]
        InProcess,

        [EnumMember]
        Error,

        [EnumMember]
        Completed
    }

    [Serializable]
    [DataContract(Namespace = Constants.NamespaceData)]
    public enum SyncOperationType
    {
        [EnumMember]
        CopyFile,

        [EnumMember]
        DeleteFile,

        [EnumMember]
        DeleteFolder,

        [EnumMember]
        CreateFolder,

        [EnumMember]
        RenameFile,

        [EnumMember]
        RenameFolder
    }
}