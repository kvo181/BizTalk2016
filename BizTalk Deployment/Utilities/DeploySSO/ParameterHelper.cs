using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using bizilante.Tools.CommandLine;

namespace bizilante.Deployment.Apps.SSO
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

        public static void ValidateEncryptedFile(NameValueCollection nameValueArgs)
        {
            if ((nameValueArgs["NonEncryptedFile"] != null) && (nameValueArgs["NonEncryptedFile"].Length > 0))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(nameValueArgs["NonEncryptedFile"]);
                if (!fi.Exists)
                    throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.NonEncryptedFileNotFound, new object[] { fi.FullName }), "NonEncryptedFile", System.Diagnostics.TraceLevel.Error);
            }
            else
            {
                throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.NonEncryptedFileNotSpecified), "NonEncryptedFile", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateEncryptionKey(NameValueCollection nameValueArgs)
        {
            if (nameValueArgs["CompanyName"] != null)
            {
                nameValueArgs.Set("CompanyName", nameValueArgs["CompanyName"].Trim());
            }
            if ((nameValueArgs["CompanyName"] == null) || (nameValueArgs["CompanyName"].Length == 0))
            {
                throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.CompanyNameNotSpecified, new object[] { }), "CompanyName", System.Diagnostics.TraceLevel.Error);
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
                throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.TimeoutInvalid, new object[] { }), "Timeout", System.Diagnostics.TraceLevel.Error);
            }
        }

        public static void ValidateDatabase(string database)
        {
            Regex regex = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9]|[!@#\$%\^&'\)\(\.\-_\{\}~\.]){0,62}$");
            if (!regex.Match(database).Success)
            {
                throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.DatabaseInvalid, new object[] { database }), "Database", System.Diagnostics.TraceLevel.Error);
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
                    throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.ServerInvalid, new object[] { server }), "Server", System.Diagnostics.TraceLevel.Error);
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
                    throw new CommandLineArgumentException(CommandResources.GetString(CommandResources.ResourceID.DatabaseNotSpecified), "Database", System.Diagnostics.TraceLevel.Error);
                }
            }
            else
            {
                bizilante.SSO.Helper.SSO sso = new bizilante.SSO.Helper.SSO();
                sso.GetSecretServerName();
                nameValueArgs.Set("Server", sso.DBServer);
                if ((nameValueArgs["Database"] == null) || (nameValueArgs["Database"].Length == 0))
                    nameValueArgs.Set("Database", sso.DB);
            }
            ValidateServer(nameValueArgs["Server"]);
            ValidateDatabase(nameValueArgs["Database"]);
        }
    }
}

