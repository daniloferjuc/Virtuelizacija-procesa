using Common;
using System;
using System.Globalization;
using System.IO;

namespace Service
{
    class SessionFileWriter : IDisposable
    {
        private StreamWriter measurementsWriter;
        private StreamWriter rejectsWriter;
        private bool disposed = false;
        private string sessionId;
        private string dataPath;

        public string SessionId { get => sessionId; }
        public string DataPath { get => dataPath; }

        public SessionFileWriter(string sessionId, string dataPath)
        {
            this.sessionId = sessionId;
            this.dataPath = dataPath;

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            string measurementsFile = Path.Combine(dataPath, $"measurements_{sessionId}.csv");
            string rejectsFile = Path.Combine(dataPath, $"rejects_{sessionId}.csv");

            measurementsWriter = new StreamWriter(new FileStream(measurementsFile, FileMode.Create, FileAccess.Write));
            measurementsWriter.WriteLine("DateTime,Volume,CO,NO2,Pressure");

            rejectsWriter = new StreamWriter(new FileStream(rejectsFile, FileMode.Create, FileAccess.Write));
            rejectsWriter.WriteLine("DateTime,Volume,CO,NO2,Pressure,Reason");
        }

        ~SessionFileWriter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (measurementsWriter != null)
                    {
                        measurementsWriter.Dispose();
                        measurementsWriter = null;
                    }
                    if (rejectsWriter != null)
                    {
                        rejectsWriter.Dispose();
                        rejectsWriter = null;
                    }
                }
                disposed = true;
            }
        }

        public void WriteMeasurement(SensorSample sample)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(SessionFileWriter));
            }

            string line = string.Format(CultureInfo.InvariantCulture,
                "{0:yyyy-MM-dd HH:mm:ss},{1},{2},{3},{4}",
                sample.DateTime, sample.Volume, sample.CO, sample.NO2, sample.Pressure);

            measurementsWriter.WriteLine(line);
            measurementsWriter.Flush();
        }

        public void WriteReject(SensorSample sample, string reason)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(SessionFileWriter));
            }

            string dt = sample != null
                ? sample.DateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                : "";
            string vol = sample != null ? sample.Volume.ToString(CultureInfo.InvariantCulture) : "";
            string co = sample != null ? sample.CO.ToString(CultureInfo.InvariantCulture) : "";
            string no2 = sample != null ? sample.NO2.ToString(CultureInfo.InvariantCulture) : "";
            string pr = sample != null ? sample.Pressure.ToString(CultureInfo.InvariantCulture) : "";

            string line = $"{dt},{vol},{co},{no2},{pr},{reason}";
            rejectsWriter.WriteLine(line);
            rejectsWriter.Flush();
        }
    }
}
