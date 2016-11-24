using System;
using Microsoft.BizTalk.ApplicationDeployment;
using Microsoft.BizTalk.ExplorerOM;

namespace bizilante.Helpers.BizTalkGroupHelper
{
    public class BizTalkGroup
    {
        private Group grp = new Group();

        public BizTalkGroup(string server, string database)
        {
            grp.DBServer = server;
            grp.DBName = database;
        }

        public string GetApplicationVersion(string application)
        {
            try
            {
                BtsCatalogExplorer btsExplorer = (BtsCatalogExplorer)grp.CatalogExplorer;
                Microsoft.BizTalk.ApplicationDeployment.ApplicationCollection apps = grp.Applications;
                Microsoft.BizTalk.ApplicationDeployment.Application app = apps[application];
                if (null != app)
                    return app.Description;
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("BizTalkGroup.GetApplicationVersion failed: {0}", exception.Message), exception);
            }
            finally
            {
            }
            return "0.0.0.0";
        }
    }
}
