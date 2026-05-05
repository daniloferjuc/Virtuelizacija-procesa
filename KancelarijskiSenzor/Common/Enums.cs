using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public enum AckStatus
    {
        [EnumMember] IN_PROGRESS,
        [EnumMember] COMPLETED,
        [EnumMember] REJECTED
    }
}
