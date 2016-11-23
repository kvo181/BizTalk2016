using System.ServiceModel;

namespace BizUnitWcfServiceLibrary
{
    [ServiceContract(Namespace = "http://bizunit.servicecontracts/2011/09/")]
    public interface IBizUnitService
    {
        [OperationContract]
        void HostConductorStep(HostConductorStep step);

        [OperationContract]
        void OrchestrationConductorStep(OrchestrationConductorStep step);

        [OperationContract]
        void ReceiveLocationEnabledStep(ReceiveLocationEnabledStep step);

        [OperationContract]
        void ReceivePortConductorStep(ReceivePortConductorStep step);

        [OperationContract]
        void SendPortConductorStep(SendPortConductorStep step);

        [OperationContract]
        void SendPortGroupConductorStep(SendPortGroupConductorStep step);

        [OperationContract]
        string GetData(int value);

    }
}
