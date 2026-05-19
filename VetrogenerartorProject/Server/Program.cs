using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(WindTurbineService)))
            {
                host.Open();
                Console.WriteLine("Server je pokrenut na net.tcp://localhost:4000/WindTurbine");
                Console.WriteLine("Pritisnite [Enter] za zaustavljanje servera.");
                Console.ReadLine();
            }
        }
    }
}
