using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData
{
    /// <summary>
    /// Represents the BizTalk hosts required for this Build
    /// </summary>
    [Serializable]
    public class BizTalkHosts
    {
        private List<BizTalkHost> _Hosts = new List<BizTalkHost>();

        /// <summary>
        /// The hosts
        /// </summary>
        public List<BizTalkHost> Hosts
        {
            get { return _Hosts; }
            set { _Hosts = value; }
        }
    }
    /// <summary>
    /// Represents a BizTalk host
    /// </summary>
    [Serializable]
    public class BizTalkHost
    {
        private string _Name;
        private BizTalkHostType _HostType = BizTalkHostType.InProcess;
        private string _Username;
        private string _Password;
        private List<BizTalkAdapterHandler> _SendHandlers = new List<BizTalkAdapterHandler>();
        private List<BizTalkAdapterHandler> _ReceiveHandlers = new List<BizTalkAdapterHandler>();
        private string _WindowsGroupName = "BizTalk Application Users";
        private bool _TrackingEnabled = false;
        private bool _HostTrusted = true;
       
        /// <summary>
        /// Ctor
        /// </summary>
        public BizTalkHost()
        {
            
        }
        /// <summary>
        /// Overrides to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_Name))
                return base.ToString();
            else
                return _Name;
        }
        /// <summary>
        /// The windows group associated with the host
        /// </summary>                
        [Description("The windows group associated with this host")]
        public string WindowsGroupName
        {
            get { return _WindowsGroupName; }
            set { _WindowsGroupName = value; }
        }
        /// <summary>
        /// The receive handlers associated with the host
        /// </summary>
        [Description("The list of receive handlers for this host")]
        public List<BizTalkAdapterHandler> ReceiveHandlers
        {
            get { return _ReceiveHandlers; }
            set { _ReceiveHandlers = value; }
        }
        /// <summary>
        /// The send handlers associated with the host
        /// </summary>
        [Description("The list of send handlers for this host")]
        public List<BizTalkAdapterHandler> SendHandlers
        {
            get { return _SendHandlers; }
            set { _SendHandlers = value; }
        }
        /// <summary>
        /// The password for the hosts credentials
        /// </summary>
        [Description("The password for the instance of this host")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        /// <summary>
        /// The user name of the hosts credentials
        /// </summary>
        [Description("The username for the instance of this host")]
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        /// <summary>
        /// The type of the host
        /// </summary>
        [Description("The type of this host")]
        public BizTalkHostType HostType
        {
            get { return _HostType; }
            set { _HostType = value; }
        }
        /// <summary>
        /// The name of the host
        /// </summary>
        [Description("The name of the host")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }        
        /// <summary>
        /// Trusted
        /// </summary>
        [Description("Indicates if the host is trusted")]
        public bool HostTrusted
        {
            get { return _HostTrusted; }
            set { _HostTrusted = value; }
        }
        /// <summary>
        /// Tracking enabled
        /// </summary>
        [Description("Indicates if the host tracking is enabled")]
        public bool TrackingEnabled
        {
            get { return _TrackingEnabled; }
            set { _TrackingEnabled = value; }
        }
        /// <summary>
        /// Can we create this host during the build process
        /// </summary>
        [Description("Can we create this host during the build process?")]
        public bool CanCreate { get; set; }

        /// <summary>
        /// Checks if a handler already exists for this adapter
        /// </summary>
        /// <param name="adapterName"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public static bool ContainsHandler(string adapterName, List<BizTalkAdapterHandler> handlers)
        {
            foreach (BizTalkAdapterHandler handler in handlers)
            {
                if (adapterName == handler.AdapterName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Adds any missing adapters for example if any were added since last run
        /// </summary>
        public void RefreshAdapters()
        {
            List<string> adapters = BizTalkHelper.GetAdapters();

            //Add missing adapters                
            foreach (string adapter in adapters)
            {
                if (!BizTalkHost.ContainsHandler(adapter, this.SendHandlers))
                    this.SendHandlers.Add(new BizTalkAdapterHandler(adapter));

                if (!BizTalkHost.ContainsHandler(adapter, this.ReceiveHandlers))
                    this.ReceiveHandlers.Add(new BizTalkAdapterHandler(adapter));
            }
        }
    }
    /// <summary>
    /// The types of BizTalk Host
    /// </summary>
    public enum BizTalkHostType
    {
        /// <summary>
        /// Represents an in process host
        /// </summary>
        InProcess,
        /// <summary>
        /// Represets an isolated host
        /// </summary>
        Isolated,
    }

    public class BizTalkAdapterHandler
    {
        private string _AdapterName;
        private bool _Included = false;

        public BizTalkAdapterHandler()
        {

        }

        public BizTalkAdapterHandler(string adapter)
        {
            _AdapterName = adapter;
        }
        public bool Included
        {
            get { return _Included; }
            set { _Included = value; }
        }

        /// <summary>
        /// The name of the adapter
        /// </summary>
        public string AdapterName
        {
            get { return _AdapterName; }
            set { _AdapterName = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_AdapterName))
                return base.ToString();
            else
                return _AdapterName;
        }        
    }
}
