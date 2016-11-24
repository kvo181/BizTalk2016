using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using bizilante.Tools.CommandLine;
using System.Collections.Generic;

namespace bizilante.Deployment.BTSDeployHost.Utils
{
    static class ParameterHelper
    {
        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            if (bytes != null)
            {
                foreach (byte num in bytes)
                {
                    builder.Append(num.ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            return builder.ToString();
        }

        public static void ValidatePackage(NameValueCollection nameValueArgs)
        {
            if ((nameValueArgs["Package"] != null) && (nameValueArgs["Package"].Length > 0))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(nameValueArgs["Package"]);
                if (!fi.Exists)
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.PackageNotFound, new object[] { fi.FullName }), "Package", System.Diagnostics.TraceLevel.Error);
            }
            else
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.PackageNotSpecified), "Package", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateApplicationOrAdapter(NameValueCollection nameValueArgs)
        {
            if (nameValueArgs["Application"] != null)
            {
                nameValueArgs.Set("Application", nameValueArgs["Application"].Trim());
            }
            if (nameValueArgs["Adapter"] != null)
            {
                nameValueArgs.Set("Adapter", nameValueArgs["Adapter"].Trim());
            }
            if (((nameValueArgs["Application"] == null) || (nameValueArgs["Application"].Length == 0)) &&
                ((nameValueArgs["Adapter"] == null) || (nameValueArgs["Adapter"].Length == 0)))
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.ApplicationAdapterNotSpecified, new object[] { }), "Application/Adapter", System.Diagnostics.TraceLevel.Error);
            }
            if (nameValueArgs["Application"] != null)
            {
                // Can we deploy this application?
                List<string> validApplications = Helper.GetApplications();
                if (!validApplications.Contains(nameValueArgs["Application"]))
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.InvalidApplication, new object[] { nameValueArgs["Application"] }), "Application", System.Diagnostics.TraceLevel.Error);

                // Did they specify a version? and is it a valid version?
                if ((nameValueArgs["Version"] == null) || (nameValueArgs["Version"].Length == 0))
                {
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.VersionNotSpecified), "Version", System.Diagnostics.TraceLevel.Error);
                }
                bool patch = Helper.IsPatch(nameValueArgs);
                List<string> validVersions = Helper.GetVersions(nameValueArgs["Application"], patch);
                if (!validVersions.Contains(nameValueArgs["Version"]))
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.InvalidVersion, new object[] { nameValueArgs["Version"], nameValueArgs["Application"] }), "Version", System.Diagnostics.TraceLevel.Error);
            }
            else
            {
                // Can we deploy this adapter?
                List<string> validAdapters = Helper.GetAdapters();
                if (!validAdapters.Contains(nameValueArgs["Adapter"]))
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.InvalidAdapter, new object[] { nameValueArgs["Adapter"] }), "Adapter", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateTimeout(NameValueCollection nameValueArgs)
        {
            int timeout;
            if (nameValueArgs["Timeout"] != null)
            {
                nameValueArgs.Set("Timeout", nameValueArgs["Timeout"].Trim());
            }
            if ((nameValueArgs["Timeout"] != null) && (!int.TryParse(nameValueArgs["Timeout"], out timeout)))
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.TimeoutInvalid, new object[] { }), "Timeout", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateDatabase(string database)
        {
            Regex regex = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.]){0,62}$");
            if (!regex.Match(database).Success)
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.DatabaseInvalid, new object[] { database }), "Database", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateServer(string server)
        {
            IPAddress address;
            if (!IPAddress.TryParse(server, out address))
            {
                Regex regex = new Regex(@"^((np|tcp|spx|adsp|rpc|vines):)?([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.\\]){0,256}((,[0-9]{1,5})|(,(ncacn_np|ncacn_ip_tcp|ncacn_nb_nb|ncacn_spx|ncacn_vns_spp|ncadg_ip_udp|ncadg_ipx|ncalrpc)))?(\\([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.\\]){0,256})?$");
                if (!regex.Match(server).Success)
                {
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.ServerInvalid, new object[] { server }), "Server", System.Diagnostics.TraceLevel.Error);
                }
            }
        }

        public static void ValidateServerDatabase(NameValueCollection nameValueArgs)
        {
            Trace.WriteLine("Validating Server and Database parameters...");
            if ((nameValueArgs["Server"] != null) && (nameValueArgs["Server"].Length > 0))
            {
                if ((nameValueArgs["Database"] == null) || (nameValueArgs["Database"].Length == 0))
                {
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.DatabaseNotSpecified), "Database", System.Diagnostics.TraceLevel.Error);
                }
            }
            else
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.ServerNotSpecified), "Server", System.Diagnostics.TraceLevel.Error);
            }
            ValidateServer(nameValueArgs["Server"]);
            ValidateDatabase(nameValueArgs["Database"]);
        }

        public static void ValidateType(NameValueCollection nameValueArgs)
        {
            if ((nameValueArgs["Type"] == null) || (nameValueArgs["Type"].Length == 0))
            {
                // Is this a valid type for the given action
                if (!Helper.IsValidType(nameValueArgs))
                    throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.TypeNotSpecified), "Type", System.Diagnostics.TraceLevel.Error);
                // The type can be optional for certain actions
                return;
            }
            List<string> validTypes = Helper.GetTypes();
            if (!validTypes.Contains(nameValueArgs["Type"]))
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.InvalidType, new object[] { nameValueArgs["Type"] }), "Type", System.Diagnostics.TraceLevel.Error);
            // Is this a valid type for the given action
            if (!Helper.IsValidType(nameValueArgs))
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.TypeNotPossible, new object[] { nameValueArgs["Action"], nameValueArgs["Type"] }), "Type", System.Diagnostics.TraceLevel.Error);

            // Is this a valid action for the given type
            if (!Helper.IsValidAction(nameValueArgs))
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.ActionNotPossible, new object[] { nameValueArgs["Action"], nameValueArgs["Type"] }), "Action", System.Diagnostics.TraceLevel.Error);
        }

        public static void ValidateEnvironment(NameValueCollection nameValueArgs)
        {
            if ((nameValueArgs["Environment"] == null) || (nameValueArgs["Environment"].Length == 0))
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.EnvironmentNotSpecified), "Environment", System.Diagnostics.TraceLevel.Error);
            }
            Dictionary<string, string> validEnvironments = Helper.GetBizTalkDbServers();
            if (!validEnvironments.ContainsKey(nameValueArgs["Environment"]))
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.InvalidEnvironment, new object[] { nameValueArgs["Environment"] }), "Environment", System.Diagnostics.TraceLevel.Error);
        }

        public static void ValidateAction(NameValueCollection nameValueArgs)
        {
            if ((nameValueArgs["Action"] == null) || (nameValueArgs["Action"].Length == 0))
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.ActionNotSpecified), "Action", System.Diagnostics.TraceLevel.Error);
            }
            List<string> validActions = Helper.GetActions();
            if (!validActions.Contains(nameValueArgs["Action"]))
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetFormattedString(CommandLine.CommandResources.ResourceID.InvalidAction, new object[] { nameValueArgs["Action"] }), "Action", System.Diagnostics.TraceLevel.Error);
        }

        public static void ValidateLog(NameValueCollection nameValueArgs)
        {
            if (string.IsNullOrEmpty(nameValueArgs["Log"])) return;
            bool bLog;
            if (!bool.TryParse(nameValueArgs["Log"], out bLog))
            {
                throw new CommandLineArgumentException(CommandLine.CommandResources.GetString(CommandLine.CommandResources.ResourceID.InvalidLogValue), "Log", System.Diagnostics.TraceLevel.Error);
            }
        }

    }
}

