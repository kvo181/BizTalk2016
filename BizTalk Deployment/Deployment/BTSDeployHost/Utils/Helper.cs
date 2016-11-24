using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;

namespace bizilante.Deployment.BTSDeployHost.Utils
{
    class Helper
    {
        private static Dictionary<string, string> BTSEnvironments = new Dictionary<string, string>();
        public static Dictionary<string, string> GetBizTalkDbServers()
        {
            if (BTSEnvironments.Count != 0) return BTSEnvironments;
            for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
                if (ConfigurationManager.AppSettings.GetKey(i).StartsWith("BizTalk_"))
                    BTSEnvironments.Add(ConfigurationManager.AppSettings.GetKey(i).Replace("BizTalk_", string.Empty), ConfigurationManager.AppSettings[i]);
            return BTSEnvironments;
        }

        private static readonly string Root = ConfigurationManager.AppSettings["MsiRootPath"];
        public static List<string> GetApplications()
        {
            var di = new DirectoryInfo(Root);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", Root));
            var dis = di.GetDirectories();
            return dis.Where(d => d.Name != "Adapters").Select(d => d.Name).ToList();
        }
        public static List<string> GetVersions(string appName, bool patch)
        {
            var di = new DirectoryInfo(Root + "\\" + appName);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", di.FullName));

            FileInfo[] files;
            if (!patch)
            {
                files = di.GetFiles(string.Format("{0}_*-FULL.msi", appName));
                return files.Select(f => f.Name.Substring(f.Name.LastIndexOf("_") + 1, f.Name.IndexOf("-FULL") - f.Name.LastIndexOf("_") - 1)).ToList();
            }

            files = di.GetFiles(string.Format("{0}_*-Patch.msi", appName));
            return files.Select(f => f.Name.Substring(f.Name.LastIndexOf("_") + 1, f.Name.IndexOf("-Patch") - f.Name.LastIndexOf("_") - 1)).ToList();

        }
        public static List<string> GetAdapters()
        {
            var adapterRoot = Root + "\\Adapters";
            var di = new DirectoryInfo(adapterRoot);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", adapterRoot));

            var dis = di.GetDirectories();
            return dis.Select(d => d.Name).ToList();
        }

        public static List<string> GetTypes()
        {
            string[] types = ConfigurationManager.AppSettings["Type"].Split(new string[] { "," }, StringSplitOptions.None);
            List<string> listOfTypes = new List<string>();
            foreach (string type in types)
                listOfTypes.Add(type.Trim());
            return listOfTypes;
        }
        public static List<string> GetActions()
        {
            string[] actions = ConfigurationManager.AppSettings["Action"].Split(new string[] { "," }, StringSplitOptions.None);
            List<string> listOfActions = new List<string>();
            foreach (string action in actions)
                listOfActions.Add(action.Trim());
            return listOfActions;
        }

        public static bool IsPatch(NameValueCollection nameValueArgs)
        {
            return (string.Compare(nameValueArgs["Type"], "Patch", true) == 0);
        }
        public static bool IsValidAction(NameValueCollection nameValueArgs)
        {
            string validActionList = string.Format("Type_{0}", nameValueArgs["Type"]);
            string[] actions = ConfigurationManager.AppSettings[validActionList].Split(new string[] { "," }, StringSplitOptions.None);
            List<string> validActions = new List<string>();
            foreach (string action in actions)
                validActions.Add(action.Trim());
            return validActions.Contains(nameValueArgs["Action"]);
        }
        public static bool IsValidType(NameValueCollection nameValueArgs)
        {
            string invalidTypeList = string.Format("Action_{0}", Convert.ToString(nameValueArgs["Action"]));
            string[] types = ConfigurationManager.AppSettings[invalidTypeList].Split(new string[] { "," }, StringSplitOptions.None);
            List<string> invalidTypes = new List<string>();
            foreach (string type in types)
                invalidTypes.Add(type.Trim());
            return !invalidTypes.Contains(nameValueArgs["Type"]);
        }

    }
}
