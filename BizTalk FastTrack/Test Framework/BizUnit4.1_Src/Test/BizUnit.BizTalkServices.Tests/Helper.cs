using System;
using System.ServiceModel;

namespace BizUnit.BizTalkServices.Tests
{
    class Helper
    {
        public static BizUnitWcfServiceLibrary.IBizUnitService BizUnitService(string machineName)
        {
            // We need to call a Wcf service in order to be able to manipulate BizTalk artifacts on
            // a remote computer. The service has to be running on the remote computer, since a
            // only artifacts hosted by the group to which the server belongs can be manipulated.
            // The running service has to be available as:
            // net.tcp://<servername>:123/BizUnitServices/BizUnitService.svc
            var binding = new NetTcpBinding();
            var uri = new Uri(string.Format("net.tcp://{0}:123/BizUnitServices/BizUnitService.svc", machineName));
            var address = new EndpointAddress(uri, EndpointIdentity.CreateSpnIdentity(""));
            return ChannelFactory<BizUnitWcfServiceLibrary.IBizUnitService>.CreateChannel(binding, address);
        }
    }
}
