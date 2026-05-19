using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
            if (sample.WindSpeed < 0 || sample.GridFrequencyHz < 0)
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Vrednosti ne mogu biti negativne!"));
            }

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
