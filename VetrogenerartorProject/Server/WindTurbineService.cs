using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server
{
    internal class WindTurbineService : IWindTurbineService
    {
        public void StartSession(string TurbineId)
        {
            Console.WriteLine($"[SESSION] Zapoceta sesija za turbinu: {TurbineId}");
        }

        public void PushSample(WindTurbineSample sample)
        {
            Console.WriteLine($"[DATA] Primljen uzorak br. {sample.RowIndex} za {sample.TurbineId}");

            if (sample.GeneratorRpm > 1500)
            {
                Console.WriteLine($"[WARNING] Overspeed detektovan: {sample.GeneratorRpm} RPM!");
            }
        }

        public void EndSession()
        {
            Console.WriteLine("[SESSION] Sesija je zavrsena.");
        }
    }
}
