using Common;
using System;
using System.ServiceModel;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISensorService> factory =
                new ChannelFactory<ISensorService>("SensorService");
            ISensorService proxy = factory.CreateChannel();

            Console.WriteLine("==========================================");
            Console.WriteLine(" Kancelarijski senzor - Klijent");
            Console.WriteLine("==========================================");

            try
            {
                SessionMeta meta = new SessionMeta(
                    sessionId: $"S-{DateTime.Now:yyyyMMdd-HHmmss}",
                    volume:   "Volume [mV]",
                    co:       "Carbon_Monoxide [Ohms]",
                    no2:      "Nitrogen_Dioxide [Ohms]",
                    pressure: "Pressure [Hectopascal]",
                    dateTime: DateTime.Now);

                Console.WriteLine($"-> StartSession {meta}");
                Ack startAck = proxy.StartSession(meta);
                Console.WriteLine($"<- {startAck}");

                SensorSample sample = new SensorSample(
                    volume:   133.06,
                    co:       283808.18,
                    no2:      14849.59,
                    pressure: 1024.43,
                    dateTime: new DateTime(2016, 11, 27, 17, 22, 45));

                Console.WriteLine($"-> PushSample {sample}");
                Ack pushAck = proxy.PushSample(sample);
                Console.WriteLine($"<- {pushAck}");

                Console.WriteLine($"-> EndSession");
                Ack endAck = proxy.EndSession();
                Console.WriteLine($"<- {endAck}");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Console.WriteLine($"[ValidationFault] {ex.Detail.Message}");
            }
            catch (FaultException<DataFormatFault> ex)
            {
                Console.WriteLine($"[DataFormatFault] {ex.Detail.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
            finally
            {
                try
                {
                    if (factory.State == CommunicationState.Faulted ||
                        factory.State == CommunicationState.Closed)
                    {
                        factory.Abort();
                    }
                    else
                    {
                        factory.Close();
                    }
                }
                catch (Exception)
                {
                    factory.Abort();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Pritisnite bilo koji taster za izlazak.");
            Console.ReadKey();
        }
    }
}
