using System;
using System.ServiceModel;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(SensorService)))
            {
                host.Open();

                Console.WriteLine("==========================================");
                Console.WriteLine(" Kancelarijski senzor - WCF servis");
                Console.WriteLine("==========================================");
                Console.WriteLine("Servis je otvoren. Pritisnite bilo koji taster za zatvaranje.");
                Console.WriteLine();

                Console.ReadKey();

                host.Close();
                Console.WriteLine("Servis je zatvoren.");
            }
        }
    }
}
