using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace bizilante.Helpers.LogDeployment
{
    public class Logging
    {
        #region Log Event Handling
        // A delegate type for hooking up change notifications.
        public delegate void LogEventHandler(LogEventArgs e);
        // An event that clients can use to be notified whenever 
        // powershell output needs to visualised
        public static event LogEventHandler Log;
        // Invoke the Log event; called whenever logging has to be done
        protected static void OnLog(LogEventArgs e)
        {
            if (Log != null)
                Log(e);
        }
        #endregion

        /// <summary>
        /// Method used to store the deployment log info into the DeploymentDb database.
        /// </summary>
        /// <param name="application">name of the application</param>
        /// <param name="version">version of the application</param>
        /// <param name="note">why this deployment action</param>
        /// <param name="msifile">msi file</param>
        /// <param name="logfile">log file</param>
        public static void Execute(string action, string application, string version, string note, string msifile, string logfile, string logInfo, bool failed, string error)
        {
#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            OnLog(new LogEventArgs { Message = string.Empty });

            // We have 4 arguments:
            // 1. Deployment action
            // 2. Name of the application
            // 3. Version being installed
            // 4. msi file (optional)
            // 5. installation log file
            // We need all!
            if ((string.IsNullOrEmpty(action) && (action.ToLower() != "recycle")) ||
                string.IsNullOrEmpty(logfile) ||
                (string.IsNullOrEmpty(application) && (action.ToLower() != "recycle")) ||
                (string.IsNullOrEmpty(version) && (action.ToLower() != "recycle")))
            {
                string message = "We need the action, application, version and log file, before we can do any logging.";
                if (string.IsNullOrEmpty(action))
                    message += " action is missing.";
                if (string.IsNullOrEmpty(application))
                    message += " application is missing.";
                if (string.IsNullOrEmpty(version))
                    message += " version is missing.";
                if (string.IsNullOrEmpty(logfile))
                    message += " LOG file is missing";
                throw new Exception(message);
            }

            if (string.IsNullOrEmpty(application) || (action.ToLower() == "recycle")) application = "N/A";
            if (string.IsNullOrEmpty(version) || (action.ToLower() == "recycle")) version = "N/A";

            // Validate msi file
            if (!string.IsNullOrEmpty(msifile))
            {
                FileInfo msifi = new FileInfo(msifile);
                if (!msifi.Exists)
                    throw new Exception(string.Format("File '{0}' does not exist!", msifile));
            }

            // Validate deployment log file
            FileInfo fi = new FileInfo(logfile);
            if (!fi.Exists)
                throw new Exception(string.Format("File '{0}' does not exist!", logfile));

            string deploymentDb = LogDeployment.Utils.GetDeploymentDb();

            OnLog(new LogEventArgs { Message = string.Format("Uploading log file '{0}' to DeploymentDb '{1}' ...", logfile, deploymentDb) });

            string group = string.Empty;
            string environment = string.Empty;
            string user = string.Empty;
            DateTime dtDate = DateTime.MinValue;
            string step = string.Empty;
            string description = string.Empty;
            string date = string.Empty;
            long id = -1;

            bool packageDeploymentFailed = failed;
            string packageDeploymentError = error;
            int l = 0;
            StreamReader rdr = fi.OpenText();
            try
            {
                bool bSuccess = false;
                while (!rdr.EndOfStream)
                {
                    l++;
                    string line = rdr.ReadLine();
                    string[] items = line.Split(new string[] { ";" }, StringSplitOptions.None);
                    int i = 0;
                    foreach (string item in items)
                    {
                        switch (i++)
                        {
                            case 0: // application
                                break;
                            case 1: // msi
                                break;
                            case 2: // user
                                user = item;
                                break;
                            case 3: // environment
                                environment = item;
                                break;
                            case 4: // date
                                date = item;
                                dtDate = DateTime.ParseExact(date, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
                                break;
                            case 5: // step
                                step = item;
                                break;
                            case 6: // description
                                description = item;
                                break;
                        }
                    }
                    if (l == 1)
                    {
                        id = Utils.InsertIntoDeployment(GetGroupName(environment), environment, user, dtDate, application, version, action, note);
                        if (id <= 0) break;
                    }
                    else
                    {
                        bSuccess = Utils.InsertIntoLog(id, step, dtDate, description);
                        if (!bSuccess) break;
                        // For a step 8 or 11 we must update the DeploymentDb for an uninstalled app.
                        switch (step)
                        {
                            case "8":
                            case "11":
                                string app = description.Replace("uninstalled", string.Empty).Trim();
                                Utils.SetUnInstallPackage(GetGroupName(environment), application, app, user, dtDate);
                                break;
                            case "99":
                                packageDeploymentFailed = true;
                                packageDeploymentError = description;
                                break;
                        }
                    }
                }
                rdr.Close();

                OnLog(new LogEventArgs { Message = string.Format("Uploaded {0} lines", l) });

                if (id > 0)
                {
                    OnLog(new LogEventArgs { Message = string.Format("Uploading log info") });
                    bSuccess = Utils.InsertIntoLogFile(id, logInfo);
                }

                if ((id > 0) & bSuccess)
                {
                    // When finished we set the deployment end date
                    Utils.SetEndDeployment(id, dtDate, packageDeploymentFailed, packageDeploymentError);

                    // When finished we set the new application version
                    Utils.SetApplicationVersion(application, version);

                    OnLog(new LogEventArgs { Message = string.Format("Uploading LOG finished") });

                    if (!string.IsNullOrEmpty(msifile))
                    {
                        OnLog(new LogEventArgs { Message = string.Format("Uploading msi file '{0}' to DeploymentDb '{1}' ...", msifile, deploymentDb) });

                        // Get the info to log
                        string[] lines = ListPackageHelper.Helper.ListPackageContent(msifile);
                        string title = string.Empty;
                        string author = string.Empty;
                        string subject = string.Empty;
                        string comments = string.Empty;
                        string keywords = string.Empty;
                        string createtime = string.Empty;
                        string packagecode = string.Empty;
                        string productcode = string.Empty;

                        l = 0;
                        bSuccess = false;
                        foreach (string line in lines)
                        {
                            l++;
                            string resourcetype = string.Empty;
                            string luid = string.Empty;
                            string filename = string.Empty;
                            version = string.Empty;

                            switch (l)
                            {
                                case 1:
                                    title = line;
                                    break;
                                case 2:
                                    author = line;
                                    break;
                                case 3:
                                    subject = line;
                                    break;
                                case 4:
                                    comments = line;
                                    break;
                                case 5:
                                    keywords = line;
                                    break;
                                case 6:
                                    createtime = line;
                                    break;
                                case 7:
                                    packagecode = line.Split(new char[] { ':' })[1].Trim();
                                    break;
                                case 8:
                                    productcode = line.Split(new char[] { ':' })[1].Trim();
                                    break;
                                case 9:
                                    break;
                                default:
                                    string[] items = line.Split(new string[] { ";" }, StringSplitOptions.None);
                                    int i = 0;
                                    foreach (string item in items)
                                    {
                                        switch (i++)
                                        {
                                            case 0:
                                                resourcetype = item;
                                                break;
                                            case 1:
                                                luid = item;
                                                break;
                                            case 2:
                                                filename = item;
                                                break;
                                            case 3:
                                                version = item;
                                                break;
                                        }
                                    }
                                    break;
                            }

                            if (l == 9)
                            {
                                id = Utils.InsertIntoPackage(id, title, author, subject, comments, keywords, createtime, packagecode, productcode, application);
                                if (id <= 0) break;
                            }
                            else if (l > 9)
                            {
                                bSuccess = Utils.InsertIntoFiles(id, resourcetype, luid, filename, version);
                                if (!bSuccess) break;
                            }
                        }

                        OnLog(new LogEventArgs { Message = string.Format("Uploaded {0} lines", l) });


                        if ((id > 0) & bSuccess)
                            OnLog(new LogEventArgs { Message = string.Format("Uploading MSI finished") });
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog(new LogEventArgs { Message = string.Format("Uploaded {0} lines, then failure", l) });
                OnLog(new LogEventArgs { Message = string.Format("Logging failed: {0}", ex.Message) });
            }
            finally
            {
                if (rdr != null)
                    rdr.Dispose();
            }

        }

        private static string GetGroupName(string env)
        {
            string groupname = string.Empty;
            switch (env.Trim().ToLower())
            {
                case "loc":
                case "dev":
                case "tst":
                case "edu":
                case "hfx":
                case "prd":
                    groupname = "BizTalk " + env.Trim();
                    break;
            }
            return groupname;
        }
        private static string ToValidDateTime(string date)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(date);

            byte b = 32;
            byte p = 46;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte c = bytes[i];
                if ((c < 47) || (c > 58))
                {
                    if (c == 44)
                        bytes[i] = p;
                    else
                        bytes[i] = b;
                }
            }

            string newDate = ASCIIEncoding.ASCII.GetString(bytes);
            return newDate.Trim();
        }
    }
}
