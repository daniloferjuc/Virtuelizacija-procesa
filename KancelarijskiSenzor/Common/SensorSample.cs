using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class SensorSample
    {
        public SensorSample() { }

        public SensorSample(double volume, double co, double no2, double pressure, DateTime dateTime)
        {
            this.Volume = volume;
            this.CO = co;
            this.NO2 = no2;
            this.Pressure = pressure;
            this.DateTime = dateTime;
        }

        [DataMember]
        public double Volume { get; set; }

        [DataMember]
        public double CO { get; set; }

        [DataMember]
        public double NO2 { get; set; }

        [DataMember]
        public double Pressure { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[{0:yyyy-MM-dd HH:mm:ss}] Volume={1:F2}, CO={2:F2}, NO2={3:F2}, Pressure={4:F2}",
                DateTime, Volume, CO, NO2, Pressure);
        }
    }
}
