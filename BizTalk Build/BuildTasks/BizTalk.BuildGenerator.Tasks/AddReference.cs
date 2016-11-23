using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// This is an MsBuild task which uses the BizTalk ExplorerOM to add a reference to a BizTalk application
    /// </summary>
    public class AddReference : BtsCatalogExplorerTask
    {
        /// <summary>
        /// The name of the referenced application
        /// </summary>
        [Required]
        public string ReferencedApplicationName { get; set; }
        /// <summary>
        /// This will add a reference from one BTS App to anotther
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Logger.LogMessage(this, string.Format(CultureInfo.CurrentCulture, "Executing task: {0}", MethodBase.GetCurrentMethod().ReflectedType.FullName));

            //Validate
            if (string.IsNullOrEmpty(MessageBoxConnection))
                throw new ApplicationException("The message box connection has not been set");

            if (string.IsNullOrEmpty(ApplicationName))
                throw new ApplicationException("The application name has not been provided");

            if (string.IsNullOrEmpty(ReferencedApplicationName))
                throw new ApplicationException("The referenced application name has not been provided");

            //Setup Catalog
            Catalog.ConnectionString = MessageBoxConnection;

            //Check if app exists
            if (BtsCatalogExplorerHelper.ApplicationExists(Catalog, ApplicationName))
            {
                //check that the refernced application also exists
                if (BtsCatalogExplorerHelper.ApplicationExists(Catalog, ReferencedApplicationName))
                {
                    Logger.LogMessage(this, "Adding reference to the application");
                    BtsCatalogExplorerHelper.AddReference(Catalog, ApplicationName, ReferencedApplicationName);
                    Logger.LogMessage(this, "Application reference added");
                }
                else
                {
                    Logger.LogMessage(this, "The referenced application does not exist in the BizTalk group, therefore it was not stopped");
                    throw new ApplicationException("The referenced application does not exist");
                }
            }
            else
            {
                Logger.LogMessage(this, "The source application does not exist in the BizTalk group, therefore it was not stopped");
                throw new ApplicationException("The referenced application does not exist");
            }

            return true;
        }       
    }
}