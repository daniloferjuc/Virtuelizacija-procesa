using Common;
using System;
using System.ServiceModel;

namespace Service
{
    public class SensorService : ISensorService
    {
        private SessionMeta currentSession;

        public Ack StartSession(SessionMeta meta)
        {
            if (currentSession != null)
            {
                return new Ack(AckStatus.REJECTED,
                    $"Sesija {currentSession.SessionId} je vec otvorena.");
            }

            if (meta == null)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("SessionMeta ne sme biti null."));
            }

            if (string.IsNullOrWhiteSpace(meta.SessionId))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("SessionId je obavezan."));
            }

            currentSession = meta;
            Console.WriteLine($"[Service] Otvorena sesija: {meta}");
            return new Ack(AckStatus.IN_PROGRESS, $"Sesija {meta.SessionId} otvorena.");
        }

        public Ack PushSample(SensorSample sample)
        {
            if (currentSession == null)
            {
                return new Ack(AckStatus.REJECTED, "Sesija nije otvorena.");
            }

            ValidateSample(sample);

            Console.WriteLine($"[Service] Prijem uzorka: {sample}");
            return new Ack(AckStatus.IN_PROGRESS, "Sample primljen.");
        }

        public Ack EndSession()
        {
            if (currentSession == null)
            {
                return new Ack(AckStatus.REJECTED, "Nema otvorene sesije.");
            }

            string sessionId = currentSession.SessionId;
            currentSession = null;
            Console.WriteLine($"[Service] Sesija {sessionId} zavrsena.");
            return new Ack(AckStatus.COMPLETED, $"Sesija {sessionId} zavrsena.");
        }

        private void ValidateSample(SensorSample sample)
        {
            if (sample == null)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Sample ne sme biti null."));
            }

            if (double.IsNaN(sample.Volume) || double.IsInfinity(sample.Volume))
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("Volume nije validan broj."));
            }

            if (double.IsNaN(sample.CO) || double.IsInfinity(sample.CO))
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("CO nije validan broj."));
            }

            if (double.IsNaN(sample.NO2) || double.IsInfinity(sample.NO2))
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("NO2 nije validan broj."));
            }

            if (double.IsNaN(sample.Pressure) || double.IsInfinity(sample.Pressure))
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("Pressure nije validan broj."));
            }

            if (sample.DateTime == default(DateTime))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("DateTime polje je obavezno."));
            }

            if (sample.Pressure <= 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault($"Pressure mora biti > 0, dobijeno: {sample.Pressure}"));
            }

            if (sample.Volume < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault($"Volume mora biti >= 0, dobijeno: {sample.Volume}"));
            }

            if (sample.CO < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault($"CO mora biti >= 0, dobijeno: {sample.CO}"));
            }

            if (sample.NO2 < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault($"NO2 mora biti >= 0, dobijeno: {sample.NO2}"));
            }
        }
    }
}
