using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class DataFormatFault
    {
        public DataFormatFault() { }
        public DataFormatFault(string message) { this.Message = message; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class ValidationFault
    {
        public ValidationFault() { }
        public ValidationFault(string message) { this.Message = message; }

        [DataMember]
        public string Message { get; set; }
    }
}
