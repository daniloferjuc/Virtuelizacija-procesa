using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class SessionMeta
    {
        public SessionMeta() { }

        public SessionMeta(string sessionId, string volume, string co,
                           string no2, string pressure, DateTime dateTime)
        {
            this.SessionId = sessionId;
            this.Volume = volume;
            this.CO = co;
            this.NO2 = no2;
            this.Pressure = pressure;
            this.DateTime = dateTime;
        }

        [DataMember]
        public string SessionId { get; set; }

        [DataMember]
        public string Volume { get; set; }

        [DataMember]
        public string CO { get; set; }

        [DataMember]
        public string NO2 { get; set; }

        [DataMember]
        public string Pressure { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return $"SessionMeta {{ Id={SessionId}, Volume={Volume}, CO={CO}, " +
                   $"NO2={NO2}, Pressure={Pressure}, Started={DateTime:yyyy-MM-dd HH:mm:ss} }}";
        }
    }
}
