using System;
using System.Globalization;
using System.Reflection;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// This is an msbuild task to start an application
    /// </summary>
    public class StartApplication : BtsCatalogExplorerTask
    {
        /// <summary>
        /// This will start an application
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Logger.LogMessage(this,
                              string.Format(CultureInfo.CurrentCulture, "Executing task: {0}",
                                            MethodBase.GetCurrentMethod().ReflectedType.FullName));

            //Validate
            if (string.IsNullOrEmpty(ApplicationName))
                throw new ApplicationException("The application name has not been provided");
            if (string.IsNullOrEmpty(MessageBoxConnection))
                throw new ApplicationException("The message box connection has not been provided");

            //Setup Catalog
            Catalog.ConnectionString = MessageBoxConnection;

            //Check if application exists
            if (BtsCatalogExplorerHelper.ApplicationExists(Catalog, ApplicationName))
            {                
                BtsCatalogExplorerHelper.StartApplication(Catalog, ApplicationName);                
            }
            else
                Log.LogMessage("The application does not exist so task will exit", null);

            return true;
        }

        
    }
}