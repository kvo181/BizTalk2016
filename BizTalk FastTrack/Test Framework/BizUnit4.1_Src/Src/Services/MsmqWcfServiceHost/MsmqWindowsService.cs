using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.ServiceModel;
using Microsoft.BizTalk.CAT.BestPractices.Framework.Instrumentation;

namespace MsmqWcfServiceHost
{
    public class MsmqWindowsService : ServiceBase
    {
        public ServiceHost ServiceHost = null;

        //Default values
        public const int DefaultRetrynumberauthorized = 3;

        //Internal parameters
        private int _iRestartRequested = 0;
        private DateTime _dtLastRetry = DateTime.Now.Date;

        //Properties
        private int _retryNumberAuthorized;
        public int RetryNumberAuthorized
        {
            get { return _retryNumberAuthorized; }
            set { _retryNumberAuthorized = value; }
        }

        public MsmqWindowsService()
        {
            InitializeComponent();
            UpdateConf();
        }
        public static void Main()
        {
            Run(new MsmqWindowsService());
        }

        /// <summary>
        /// Start the Windows service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            TraceManager.ServiceComponent.TraceInfo("Starting the service...");
            if (ServiceHost != null)
            {
                ServiceHost.Close();
            }
            StartHosting();
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

        /// <summary>
        /// Stop the Windows service.
        /// </summary>
        protected override void OnStop()
        {
            TraceManager.ServiceComponent.TraceInfo("Stopping the service...");
            StopHosting();
        }

        private void InitializeComponent()
        {
            // 
            // MsmqWindowsService
            // 
            ServiceName = "MsmqWCFWindowsService";
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
            System.Diagnostics.Debugger.Launch();
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
                    ServiceHost = new ServiceHost(typeof(MSMQWcfServiceLibrary.MsmqService));
                    var EndPointAddresses = ServiceHost.Description.Endpoints.Aggregate(string.Empty, (current, sep) => current + (string.IsNullOrEmpty(current) ? sep.Address.Uri.ToString() : ", " + sep.Address.Uri.ToString()));
                    TraceManager.ServiceComponent.TraceInfo("Name:{0}, ConfigurationName:{1}, EndPoint addresses:{2}", ServiceHost.Description.Name, ServiceHost.Description.ConfigurationName, EndPointAddresses);
                    EventLog.WriteEntry(string.Format("Name:{0}, ConfigurationName:{1}, EndPoint addresses:{2}", ServiceHost.Description.Name, ServiceHost.Description.ConfigurationName, EndPointAddresses), EventLogEntryType.Information);
                    ServiceHost.Open();
                    ServiceHost.Faulted += new EventHandler(ServiceHostFaulted);
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
                this._iRestartRequested += 1;

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
            var confValue = String.Empty;
            foreach (string aValue in ConfigurationManager.AppSettings)
            {
                switch (aValue)
                {
                    case "retryNumberAuthorized":
                        //Number of connexion retry before giving up.
                        confValue = ConfigurationManager.AppSettings[aValue];
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
