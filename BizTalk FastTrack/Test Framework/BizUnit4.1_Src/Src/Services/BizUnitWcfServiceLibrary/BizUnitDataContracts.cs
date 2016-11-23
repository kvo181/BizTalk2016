using System.Runtime.Serialization;

namespace BizUnitWcfServiceLibrary
{
    [DataContract(Namespace = "http://bizunit.datacontracts/2011/09/")]
    public class HostConductorStep
    {
        ///<summary>
        /// "start" or "stop" the host instance
        ///</summary>
        [DataMember]
        public string Action { get; set; }

        ///<summary>
        /// Name of the host instance
        ///</summary>
        [DataMember]
        public string HostInstanceName { get; set; }

        ///<summary>
        /// Server on which to start/stop the host instance
        ///</summary>
        [DataMember]
        public string Servers { get; set; }

        ///<summary>
        /// Username to use when starting/stopping via WMI
        ///</summary>
        [DataMember]
        public string Logon { get; set; }

        ///<summary>
        /// Password
        ///</summary>
        [DataMember]
        public string PassWord { get; set; }

        ///<summary>
        /// Grant the user LogOnAsService
        ///</summary>
        [DataMember]
        public bool GrantLogOnAsService { get; set; }

        public override string ToString()
        {
            return string.Format("{0} on instance {1}, servers:{2}, with user:{3}, GrantLogOn:{4}", Action,
                                 HostInstanceName, Servers, Logon, GrantLogOnAsService);
        }
    }

    [DataContract(Namespace = "http://bizunit.datacontracts/2011/09/")]
    public class OrchestrationConductorStep
    {
        ///<summary>
        /// Name of the BizTalk assembly
        ///</summary>
        [DataMember]
        public string AssemblyName { get; set; }
        ///<summary>
        /// Name of the Orchestration
        ///</summary>
        [DataMember]
        public string OrchestrationName { get; set; }
        ///<summary>
        /// We only allow you to stop/start an orchestration
        ///</summary>
        [DataMember]
        public OrchestrationAction Action { get; set; }
        ///<summary>
        /// Time to wait in seconds after having stopped/started the orchestration
        ///</summary>
        [DataMember]
        public int DelayForCompletion { get; set; }

        public override string ToString()
        {
            return string.Format("{0} on orchestration {1}, assembly:{2}, delay:{3}", Action, OrchestrationName,
                                 AssemblyName, DelayForCompletion);
        }
    }

    [DataContract(Namespace = "http://bizunit.datacontracts/2011/09/")]
    public class ReceiveLocationEnabledStep
    {
        ///<summary>
        /// The name of the receive location to check
        ///</summary>
        [DataMember]
        public string ReceiveLocationName { get; set; }
        ///<summary>
        /// If true is specified then the test step will check to see that the receive location is disabled, if false, the step will check it is enabled
        ///</summary>
        [DataMember]
        public bool IsDisabled { get; set; }

        public override string ToString()
        {
            return string.Format("{1} {0}?", IsDisabled ? "Is disabled" : "Is enabled", ReceiveLocationName);
        }
    }

    [DataContract(Namespace = "http://bizunit.datacontracts/2011/09/")]
    public class ReceivePortConductorStep
    {
        ///<summary>
        /// The name of the receive port to containing the receive location to enable/dissable
        ///</summary>
        [DataMember]
        public string ReceivePortName { get; set; }
        ///<summary>
        /// The name of the receive location to enable/dissable
        ///</summary>
        [DataMember]
        public string ReceiveLocationName { get; set; }
        ///<summary>
        /// The action to perform on the receive location: Enable|Disable
        ///</summary>
        [DataMember]
        public ReceivePortAction Action { get; set; }
        ///<summary>
        /// The number of seconds to deplay for this step to complete
        ///</summary>
        [DataMember]
        public int DelayForCompletion { get; set; }

        public override string ToString()
        {
            return string.Format("{0} on receive port {1}, location:{2}, delay:{3}", Action, ReceivePortName,
                                 ReceiveLocationName, DelayForCompletion);
        }
    }

    [DataContract(Namespace = "http://bizunit.datacontracts/2011/10/")]
    public class SendPortConductorStep
    {
        ///<summary>
        /// The name of the receive port to containing the receive location to enable/dissable
        ///</summary>
        [DataMember]
        public string SendPortName { get; set; }
        ///<summary>
        /// The action to perform on the receive location: Enable|Disable
        ///</summary>
        [DataMember]
        public SendPortAction Action { get; set; }
        ///<summary>
        /// The number of seconds to deplay for this step to complete
        ///</summary>
        [DataMember]
        public int DelayForCompletion { get; set; }

        public override string ToString()
        {
            return string.Format("{0} on send port {1}, delay:{2}", Action, SendPortName, DelayForCompletion);
        }
    }

    [DataContract(Namespace = "http://bizunit.datacontracts/2011/10/")]
    public class SendPortGroupConductorStep
    {
        ///<summary>
        /// The name of the send port group to start/stop
        ///</summary>
        [DataMember]
        public string SendPortGroupName { get; set; }
        ///<summary>
        /// The action to perform on the send port group
        ///</summary>
        [DataMember]
        public SendPortGroupAction Action { get; set; }
        ///<summary>
        /// The number of seconds to deplay for this step to complete
        ///</summary>
        [DataMember]
        public int DelayForCompletion { get; set; }

        public override string ToString()
        {
            return string.Format("{0} on send port group {1}, delay:{2}", Action, SendPortGroupName, DelayForCompletion);
        }
    }

    ///<summary>
    /// Possible actions on the orchestration
    ///</summary>
    public enum OrchestrationAction
    {
        ///<summary>
        /// Start the orchestration
        ///</summary>
        Start,
        ///<summary>
        /// Stop the orchestration
        ///</summary>
        Stop
    }
    ///<summary>
    /// The action to perform on the receive port
    ///</summary>
    public enum ReceivePortAction
    {
        ///<summary>
        /// Action to perform
        ///</summary>
        Enable,
        ///<summary>
        /// Action to perform
        ///</summary>
        Disable,
    }
    ///<summary>
    /// The action to perform on the send port
    ///</summary>
    public enum SendPortAction
    {
        ///<summary>
        /// Start the orchestration
        ///</summary>
        Start,
        ///<summary>
        /// Stop the orchestration
        ///</summary>
        Stop,
        ///<summary>
        /// Unenlist the orchestration
        ///</summary>
        Unenlist
    }
    ///<summary>
    /// The action to perform on the send port group
    ///</summary>
    public enum SendPortGroupAction
    {
        ///<summary>
        /// Start the Send Port Group
        ///</summary>
        Start,
        ///<summary>
        /// Stop the Send Port Group
        ///</summary>
        Stop
    }
}
