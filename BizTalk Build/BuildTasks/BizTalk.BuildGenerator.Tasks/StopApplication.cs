using System;
using System.Globalization;
using System.Reflection;
using Microsoft.BizTalk.ExplorerOM;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// This is an MsBuild task which uses the BizTalk ExplorerOM to stop a BizTalk application
    /// </summary>
    public class StopApplication : BtsCatalogExplorerTask
    {
        /// <summary>
        /// This will stop an application.  It also performs clean up exercises so only use within dev environment
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Logger.LogMessage(this,
                              string.Format(CultureInfo.CurrentCulture, @"Executing task: {0}",
                                            MethodBase.GetCurrentMethod().ReflectedType.FullName));

            //Validate
            if (string.IsNullOrEmpty(MessageBoxConnection))
                throw new ApplicationException(@"The message box connection has not been set");

            if (string.IsNullOrEmpty(ApplicationName))
                throw new ApplicationException(@"The application name has not been provided");

            //Setup Catalog
            Catalog.ConnectionString = MessageBoxConnection;

            //Check if app exists
            if (BtsCatalogExplorerHelper.ApplicationExists(Catalog, ApplicationName))
            {
                //Stop Application
                var app = Catalog.Applications[ApplicationName];
                if (app.Status == Status.Stopped)
                    Logger.LogMessage(this, "The application is already stopped");
                else
                {
                    CleanOrchestrations(app);
                    Logger.LogMessage(this, "Stopping the application");
                    BtsCatalogExplorerHelper.StopApplication(Catalog, ApplicationName);
                    Logger.LogMessage(this, "The application is stopped");
                }
            }
            else
            {
                Logger.LogMessage(this,
                                  "The application does not exist in the BizTalk group, therefore it was not stopped");
            }

            return true;
        }

        /// <summary>
        /// This will clean all running orchestration instances
        /// </summary>
        /// <param name="app"></param>
        private void CleanOrchestrations(Application app)
        {
            Log.LogMessage("Cleaning orchestrations", null);
            foreach (BtsOrchestration orc in app.Orchestrations)
            {
                Log.LogMessage("Cleaning orchestration: " + orc.FullName, null);
                orc.AutoSuspendRunningInstances = true;
                orc.AutoTerminateInstances = true;
            }
            Log.LogMessage("Finished cleaning orchestrations", null);
        }
    }
}