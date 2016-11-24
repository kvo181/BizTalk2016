using System;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text;

namespace bizilante.Deployment.BTSDeployHost.CommandLine
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
            PackageNotSpecified,
            PackageNotFound,
            ApplicationAdapterNotSpecified,
            ApplicationNotSpecified,
            PropertyNameInvalid,
            PropertyExists,
            ParameterMissing,
            InvalidValue,
            Deploy,
            DeploySuccess,
            DeployApp,
            DeployAppSuccess,
            CommandSupportsNoParam,
            UnknownCommand,
            ParamDesc_Type,
            ParamDesc_Environment,
            ParamDesc_Application,
            ParamDesc_Version,
            ParamDesc_Adapter,
            ParamDesc_Action,
            ParamDesc_Log,
            ServerNotSpecified,
            DatabaseNotSpecified,
            DatabaseInvalid,
            ServerInvalid,
            TimeoutInvalid,
            TypeNotSpecified,
            InvalidType,
            ActionNotSpecified,
            InvalidAction,
            ActionNotPossible,
            TypeNotPossible,
            EnvironmentNotSpecified,
            InvalidEnvironment,
            InvalidApplication,
            InvalidAdapter,
            VersionNotSpecified,
            InvalidVersion,
            InvalidLogValue
        }
    }
}

