using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using Microsoft.BizTalk.CAT.BestPractices.Framework.Instrumentation;

namespace BizUnitService
{
    partial class BizUnitWindowsService : ServiceBase
    {
        public ServiceHost ServiceHost;

        //Default values
        public const int DefaultRetrynumberauthorized = 3;

        //Internal parameters
        private int _iRestartRequested;
        private DateTime _dtLastRetry = DateTime.Now.Date;

        //Properties
        private int _retryNumberAuthorized;
        public int RetryNumberAuthorized
        {
            get { return _retryNumberAuthorized; }
            set { _retryNumberAuthorized = value; }
        }

        public BizUnitWindowsService()
        {
            InitializeComponent();
            UpdateConf();
        }

        public static void Main()
        {
            Run(new BizUnitWindowsService());
        }

        protected override void OnStart(string[] args)
        {
            TraceManager.ServiceComponent.TraceInfo("Starting the service...");
            if (ServiceHost != null)
            {
                ServiceHost.Close();
            }
            StartHosting();
        }

        protected override void OnStop()
        {
            TraceManager.ServiceComponent.TraceInfo("Stopping the service...");
            StopHosting();
        }

        /// <summary>
        /// In case of failure, 'Faulted' event, we need to restart the service host.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServiceHostFaulted(object sender, EventArgs e)
        {
            //Tracing error
            TraceManager.ServiceComponent.TraceWarning("Object {0} entered Faulted state.\r\nerrInfo : {1}",
                sender.ToString(), e.ToString());
            //Restart the service host
            TryAutoRestart();
        }

        private void StopHosting()
        {
            TraceManager.ServiceComponent.TraceInfo("StopHosting");
            //Checking host behaviour
            if (ServiceHost == null) return;
            //Forcing host closure :
            if (ServiceHost.State != CommunicationState.Closed)
            {
                if (ServiceHost.State == CommunicationState.Faulted)
                    ServiceHost.Abort();
                else
                    ServiceHost.Close();
            }
            //Backing it up to null
            ServiceHost = null;
        }
        private void StartHosting()
        {
#if DEBUG
            Debugger.Launch();
#endif
            TraceManager.ServiceComponent.TraceInfo("StartHosting");
            //Checking host current status
            if ((ServiceHost == null) ||
                (ServiceHost.State != CommunicationState.Opened))
            {
                //Checking other unrecognized status (like closed or closing) :
                if (ServiceHost != null) StopHosting();

                //Startup
                try
                {
                    ServiceHost = new ServiceHost(typeof(BizUnitWcfServiceLibrary.BizUnitService));
                    var endPointAddresses = ServiceHost.Description.Endpoints.Aggregate(string.Empty, (current, sep) => current + (string.IsNullOrEmpty(current) ? sep.Address.Uri.ToString() : ", " + sep.Address.Uri.ToString()));
                    TraceManager.ServiceComponent.TraceInfo("Name:{0}, ConfigurationName:{1}, EndPoint addresses:{2}", ServiceHost.Description.Name, ServiceHost.Description.ConfigurationName, endPointAddresses);
                    EventLog.WriteEntry(string.Format("Name:{0}, ConfigurationName:{1}, EndPoint addresses:{2}", ServiceHost.Description.Name, ServiceHost.Description.ConfigurationName, endPointAddresses), EventLogEntryType.Information);
                    ServiceHost.Open();
                    ServiceHost.Faulted += ServiceHostFaulted;
                }
                catch (Exception ex)
                {
                    //Setting up null
                    ServiceHost = null;
                    TraceManager.ServiceComponent.TraceError(ex.InnerException ?? ex);
                    EventLog.WriteEntry(ex.InnerException == null ? ex.Message : ex.InnerException.Message, EventLogEntryType.Error);

                    //Trying to start once again...
                    TryAutoRestart();
                }
            }
        }
        /// <summary>
        /// Restarting the service (just a stop and start activity)
        /// </summary>
        public virtual void ReStartHosting()
        {
            //Tracing information :
            TraceManager.ServiceComponent.TraceInfo("ReStartHosting");
            StopHosting();
            StartHosting();
        }
        /// <summary>
        /// Auto restart function
        /// </summary>
        private void TryAutoRestart()
        {
            //If the date is older than today,
            //forget the retry-number
            if (_dtLastRetry < DateTime.Now.Date)
            {
                lock (this)
                {
                    _dtLastRetry = DateTime.Now.Date;
                    _iRestartRequested = 0;
                }
            }

            //Checking the number of retry
            if (_iRestartRequested < RetryNumberAuthorized)
            {
                //Increment the retry number
                _iRestartRequested += 1;

                //Tracing information :
                TraceManager.ServiceComponent.TraceInfo("restarting WCF service #{0}",
                    _iRestartRequested);

                //Restarting
                ReStartHosting();
            }
            else
            {
                TraceManager.ServiceComponent.TraceInfo("Max threshold of restarts overcome :\r\n" +
                    "To change it, please modify the 'retryNumberAuthorized' key " +
                    "in your configuration file");

                //Exporting failure
                //this is because we want the service to fail
                //so that MSCS or other server custering services
                //can detect it to use the passive node instead
                throw new Exception("Cannot reconnect the server, check log for mode details");
            }
        }

        /// <summary>
        /// Loading parameters
        /// </summary>
        public void UpdateConf()
        {
            ////////////
            //Allocating default values :
            //
            RetryNumberAuthorized = DefaultRetrynumberauthorized;

            ////////////
            //Reallocating new values :
            //
            foreach (string aValue in ConfigurationManager.AppSettings)
            {
                switch (aValue)
                {
                    case "retryNumberAuthorized":
                        //Number of connexion retry before giving up.
                        var confValue = ConfigurationManager.AppSettings[aValue];
                        if (String.IsNullOrEmpty(confValue) == false)
                        {
                            if (int.TryParse(confValue, out _retryNumberAuthorized) == false)
                                RetryNumberAuthorized = DefaultRetrynumberauthorized;
                        }
                        break;
                }
            }
        }

    }
}
