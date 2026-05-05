using Common;
using System;

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
                throw new System.ServiceModel.FaultException<ValidationFault>(
                    new ValidationFault("SessionMeta ne sme biti null."));
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
    }
}
