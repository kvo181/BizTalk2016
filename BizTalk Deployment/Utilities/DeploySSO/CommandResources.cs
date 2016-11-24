using System;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text;

namespace bizilante.Deployment.Apps.SSO
{
    static class CommandResources
    {
        private static ResourceManager resourceManager = new ResourceManager(typeof(CommandResources));

        public static string GetFormattedString(ResourceID name, params object[] args)
        {
            Exception exception = null;
            try
            {
                string format = resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
                return ((format != null) ? string.Format(CultureInfo.CurrentUICulture, format, args) : name.ToString());
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            catch (FormatException exception4)
            {
                exception = exception4;
            }
            if (exception == null)
            {
                return name.ToString();
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(name.ToString());
            foreach (object obj2 in ArrayList.Adapter(args))
            {
                builder.Append(" ");
                builder.Append(obj2.ToString());
            }
            return builder.ToString();
        }

        public static string GetString(ResourceID name)
        {
            Exception exception = null;
            try
            {
                return resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            return name.ToString();
        }

        public static string GetString(string name)
        {
            Exception exception = null;
            try
            {
                return resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            return name.ToString();
        }

        public enum ResourceID
        {
            None,
            Label_Usage,
            Label_Parameters,
            CommandUsageHint,
            ExtraUnnamedArguments,
            ProgramUsage,
            Label_Commands,
            ProgramUsageHint,
            NonEncryptedFileNotSpecified,
            NonEncryptedFileNotFound,
            CompanyNameNotSpecified,
            PropertyNameInvalid,
            PropertyExists,
            ParameterMissing,
            InvalidValue,
            Deploy,
            DeploySuccess,
            CommandSupportsNoParam,
            UnknownCommand,
            ParamDesc_NonEncryptedFile,
            ParamDesc_CompanyName,
            ParamDesc_Server,
            ParamDesc_Database,
            ParamDesc_Timeout,
            DatabaseNotSpecified,
            DatabaseInvalid,
            ServerInvalid,
            TimeoutInvalid
        }
    }
}

