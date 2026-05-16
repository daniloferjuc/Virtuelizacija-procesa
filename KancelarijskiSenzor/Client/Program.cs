using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = ConfigurationManager.AppSettings["DatasetPath"];
            string logPath = ConfigurationManager.AppSettings["LogPath"];
            int rowLimit = int.Parse(ConfigurationManager.AppSettings["RowLimit"]);
          
            int sendDelayMs = 0;
            string delayCfg = ConfigurationManager.AppSettings["SendDelayMs"];
            if (!string.IsNullOrWhiteSpace(delayCfg))
            {
                int.TryParse(delayCfg, out sendDelayMs);
            }

            Console.WriteLine("==========================================");
            Console.WriteLine(" Kancelarijski senzor - Klijent");
            Console.WriteLine("==========================================");
            Console.WriteLine($"CSV: {csvPath}");
            Console.WriteLine($"Log: {logPath}");
            Console.WriteLine($"Row limit: {rowLimit}");
            Console.WriteLine();

            CsvSampleReader csvReader = new CsvSampleReader(csvPath, logPath, rowLimit);
            List<SensorSample> samples = csvReader.LoadSamples();

            Console.WriteLine();
            Console.WriteLine($"Ucitano {samples.Count} sample-ova iz CSV-a.");
            Console.WriteLine();

            ChannelFactory<ISensorService> factory =
                new ChannelFactory<ISensorService>("SensorService");
            ISensorService proxy = factory.CreateChannel();

            try
            {
                SessionMeta meta = new SessionMeta(
                    sessionId: $"S-{DateTime.Now:yyyyMMdd-HHmmss}",
                    volume: "Volume [mV]",
                    co: "Carbon_Monoxide [Ohms]",
                    no2: "Nitrogen_Dioxide [Ohms]",
                    pressure: "Pressure [Hectopascal]",
                    dateTime: DateTime.Now);

                Console.WriteLine($"-> StartSession {meta.SessionId}");
                Ack startAck = proxy.StartSession(meta);
                Console.WriteLine($"<- {startAck}");
                Console.WriteLine();

                int accepted = 0;
                int rejected = 0;

                for (int i = 0; i < samples.Count; i++)
                {
                    SensorSample sample = samples[i];
                    try
                    {
                        Ack ack = proxy.PushSample(sample);
                        Console.WriteLine($"[{i + 1,3}] {ack}");
                        accepted++;
                    }
                    catch (FaultException<ValidationFault> ex)
                    {
                        Console.WriteLine($"[{i + 1,3}] [ValidationFault] {ex.Detail.Message}");
                        rejected++;
                    }
                    catch (FaultException<DataFormatFault> ex)
                    {
                        Console.WriteLine($"[{i + 1,3}] [DataFormatFault] {ex.Detail.Message}");
                        rejected++;
                    }

                    if (sendDelayMs > 0)
                    {
                        Thread.Sleep(sendDelayMs);
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"-> EndSession");
                Ack endAck = proxy.EndSession();
                Console.WriteLine($"<- {endAck}");
                Console.WriteLine();
                Console.WriteLine($"Ukupno: {accepted} prihvaceno, {rejected} odbijeno.");
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