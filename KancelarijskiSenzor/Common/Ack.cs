using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class Ack
    {
        public Ack() { }

        public Ack(AckStatus status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        [DataMember]
        public AckStatus Status { get; set; }

        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{Status}] {Message}";
        }
    }
}
