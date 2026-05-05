using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface ISensorService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Ack StartSession(SessionMeta meta);

        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        [FaultContract(typeof(ValidationFault))]
        Ack PushSample(SensorSample sample);

        [OperationContract]
        Ack EndSession();
    }
}
