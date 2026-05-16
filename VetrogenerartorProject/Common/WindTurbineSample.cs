using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Common
{
    [DataContract]
    public class WindTurbineSample
    {
        [DataMember] public DateTime Timestamp { get; set; }
        [DataMember] public double WindSpeed { get; set; }
        [DataMember] public double WindDirection { get; set; }
        [DataMember] public double NacellePosition { get; set; }
        [DataMember] public double PowerKW { get; set; }
        [DataMember] public double PotentialPowerDefaultKW { get; set; }
        [DataMember] public double PowerFactor { get; set; }
        [DataMember] public double ReactivePowerKvar { get; set; }
        [DataMember] public double GridFrequencyHz { get; set; }
        [DataMember] public double GeneratorRpm { get; set; }
        [DataMember] public int RowIndex { get; set; }
        [DataMember] public string TurbineId { get; set; }
    }

    [ServiceContract]
    public interface IWindTurbineService
    {
        [OperationContract]
        void StartSession(string turbineId);

        [OperationContract]
        void PushSample(WindTurbineSample sample);

        [OperationContract]
        void EndSession();
    }
}
