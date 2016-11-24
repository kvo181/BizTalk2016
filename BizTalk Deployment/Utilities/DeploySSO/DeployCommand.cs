using System;
using System.Collections.Specialized;
using bizilante.Tools.CommandLine;
using System.Globalization;
using System.Text;

namespace bizilante.Deployment.Apps.SSO
{
    sealed class DeployCommand : Command
    {
        public DeployCommand(NameValueCollection nameValueArgs)
            : base(nameValueArgs)
        {
        }

        public override void Execute()
        {
            try
            {
                this.Validate();
                string key = base.Args["CompanyName"];
                string file = base.Args["NonEncryptedFile"];
                string timeout = base.Args["Timeout"];

                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.Deploy, new object[] { key, file });
                base.WriteLogEntry(LogEntryType.Information, formattedString);

                // We need to show the identity used to deploy to SSO
                System.Security.Principal.WindowsIdentity identity =
                    System.Security.Principal.WindowsIdentity.GetCurrent();
                if (null != identity)
                {
                    WriteLogEntry(LogEntryType.Verbose,
                        string.Format("Current WindowsIdentity: AuthenticationType:{0} - IsAuthenticated:{1} - Name:{2}",
                                      identity.AuthenticationType,
                                      identity.IsAuthenticated,
                                      identity.Name));
                    using (System.Security.Principal.WindowsImpersonationContext context =
                        identity.Impersonate())
                    {
                        identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                        if (null != identity)
                            WriteLogEntry(LogEntryType.Verbose,
                            string.Format("Impersonated WindowsIdentity: AuthenticationType:{0} - IsAuthenticated:{1} - Name:{2}",
                                          identity.AuthenticationType,
                                          identity.IsAuthenticated,
                                          identity.Name));

                        // Deploy the non encrypted SSO XML file.
                        //System.Diagnostics.Debugger.Launch();
                        string title;
                        DeploySSO deploy = new DeploySSO() { NonEncryptedFile = file, CompanyName = key };
                        deploy.Overwrite = true; // We always overwrite the current SSO application settings
                        deploy.Log += new DeploySSO.LogHandler(deploy_Log);
                        deploy.Execute(out title);

                        formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.DeploySuccess, new object[] { title });
                        base.WriteLogEntry(LogEntryType.Information, formattedString);
                    }
                }

                identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                if (null != identity)
                    WriteLogEntry(LogEntryType.Verbose,
    string.Format("After impersonation WindowsIdentity: AuthenticationType:{0} - IsAuthenticated:{1} - Name:{2}",
                  identity.AuthenticationType,
                  identity.IsAuthenticated,
                  identity.Name));

                base.commandResult = new CommandResult();
            }
            catch (Exception exception2)
            {
                base.ShowError(exception2);
                base.commandResult = new CommandResult(exception2);
                if ((exception2 is OutOfMemoryException) || (exception2 is StackOverflowException))
                {
                    throw;
                }
            }
        }

        void deploy_Log(string message)
        {
            base.WriteLogEntry(LogEntryType.Verbose, DateTime.Now.ToString("yyyyMMdd-HHmmss") + " - " + message);
        }

        protected override CommandLineArgDescriptorList GetParameterDescriptors()
        {
            CommandLineArgDescriptor[] collection = new CommandLineArgDescriptor[] { 
                new CommandLineArgDescriptor(true, "NonEncryptedFile", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_NonEncryptedFile), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "CompanyName", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_CompanyName), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Server", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Server), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Database", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Database), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Timeout", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Timeout), CommandLineArgDescriptor.ArgumentType.String)
            };
            CommandLineArgDescriptorList list = new CommandLineArgDescriptorList();
            list.AddRange(collection);
            return list;
        }

        public override void Validate()
        {
            ParameterHelper.ValidateEncryptionKey(base.Args);
            ParameterHelper.ValidateEncryptedFile(base.Args);
            ParameterHelper.ValidateTimeout(base.Args);
        }

        public override void WriteUsageHint()
        {
            Console.WriteLine(CommandResources.GetFormattedString(CommandResources.ResourceID.CommandUsageHint, new object[] { this.Name }));
        }

        public override string Description
        {
            get
            {
                return CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Description", new object[] { this.Name }));
            }
        }

        public override string Example
        {
            get
            {
                string str = CommandResources.GetString("Label_Example");
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Example", new object[] { this.Name })), 0, Console.BufferWidth, 2);
                return (str + Environment.NewLine + str3);
            }
        }

        public override string Name
        {
            get
            {
                return "Deploy";
            }
        }

        public override string Notes
        {
            get
            {
                string str = CommandResources.GetString("Label_Notes");
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Notes", new object[] { this.Name })), 0, Console.BufferWidth, 2);
                return (str + Environment.NewLine + str3);
            }
        }

        public override string Parameters
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                string str = CommandResources.GetString(CommandResources.ResourceID.Label_Parameters);
                builder.Append(str);
                foreach (CommandLineArgDescriptor descriptor in this.ParameterDescriptors)
                {
                    string option = descriptor.GetOption();
                    builder.Append(Environment.NewLine);
                    builder.Append(option);
                }
                return builder.ToString();
            }
        }

        public override string Usage
        {
            get
            {
                string str = CommandResources.GetString(CommandResources.ResourceID.Label_Usage);
                string str2 = ConsoleHelper.Wrap(this.Name + this.ParameterDescriptors.GetUsage(), 0, Console.BufferWidth, 2);
                return (str + Environment.NewLine + str2);
            }
        }
    }
}

