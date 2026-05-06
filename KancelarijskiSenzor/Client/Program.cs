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
                    volume: "Volume [mV]",
                    co: "Carbon_Monoxide [Ohms]",
                    no2: "Nitrogen_Dioxide [Ohms]",
                    pressure: "Pressure [Hectopascal]",
                    dateTime: DateTime.Now);

                Console.WriteLine($"-> StartSession {meta}");
                Ack startAck = proxy.StartSession(meta);
                Console.WriteLine($"<- {startAck}");
                Console.WriteLine();

                // Test 1: validan sample
                Console.WriteLine("--- Test 1: validan sample ---");
                TrySend(proxy, new SensorSample(
                    volume: 133.06,
                    co: 283808.18,
                    no2: 14849.59,
                    pressure: 1024.43,
                    dateTime: new DateTime(2016, 11, 27, 17, 22, 45)));
                Console.WriteLine();

                // Test 2: Pressure <= 0 (ValidationFault)
                Console.WriteLine("--- Test 2: Pressure <= 0 ---");
                TrySend(proxy, new SensorSample(
                    volume: 100.0,
                    co: 50000.0,
                    no2: 1000.0,
                    pressure: 0.0,
                    dateTime: DateTime.Now));
                Console.WriteLine();

                // Test 3: NaN vrednost (DataFormatFault)
                Console.WriteLine("--- Test 3: CO = NaN ---");
                TrySend(proxy, new SensorSample(
                    volume: 100.0,
                    co: double.NaN,
                    no2: 1000.0,
                    pressure: 1024.0,
                    dateTime: DateTime.Now));
                Console.WriteLine();

                // Test 4: nedostaje DateTime (ValidationFault)
                Console.WriteLine("--- Test 4: DateTime nije postavljen ---");
                TrySend(proxy, new SensorSample(
                    volume: 100.0,
                    co: 50000.0,
                    no2: 1000.0,
                    pressure: 1024.0,
                    dateTime: default(DateTime)));
                Console.WriteLine();

                // Test 5: negativan NO2 (ValidationFault)
                Console.WriteLine("--- Test 5: NO2 < 0 ---");
                TrySend(proxy, new SensorSample(
                    volume: 100.0,
                    co: 50000.0,
                    no2: -500.0,
                    pressure: 1024.0,
                    dateTime: DateTime.Now));
                Console.WriteLine();

                Console.WriteLine($"-> EndSession");
                Ack endAck = proxy.EndSession();
                Console.WriteLine($"<- {endAck}");
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

        static void TrySend(ISensorService proxy, SensorSample sample)
        {
            try
            {
                Console.WriteLine($"-> PushSample {sample}");
                Ack ack = proxy.PushSample(sample);
                Console.WriteLine($"<- {ack}");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Console.WriteLine($"<- [ValidationFault] {ex.Detail.Message}");
            }
            catch (FaultException<DataFormatFault> ex)
            {
                Console.WriteLine($"<- [DataFormatFault] {ex.Detail.Message}");
            }
        }
    }
}
