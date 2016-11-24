using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using bizilante.Tools.CommandLine;
using System.Configuration;
using bizilante.Deployment.BTSDeployHost.Utils;

namespace bizilante.Deployment.BTSDeployHost.CommandLine
{
    sealed class BTSDeployAppCommand : Command
    {
        public BTSDeployAppCommand(NameValueCollection nameValueArgs)
            : base(nameValueArgs)
        {
        }

        public override void Execute()
        {
            try
            {
#if DEBUG
                System.Diagnostics.Debugger.Launch();
#endif
                this.Validate();

                string app = base.Args["Application"];
                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.DeployApp, new object[] { string.IsNullOrEmpty(app) ? "Adapter" : "Application", string.IsNullOrEmpty(app) ? base.Args["Adapter"] : base.Args["Application"], base.Args["Action"] });
                base.WriteLogEntry(LogEntryType.Information, formattedString);

                // Deploy the BizTalk application
                GUIBTSDeployPSHost.Run(base.Args);

                formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.DeployAppSuccess, new object[] { string.IsNullOrEmpty(app) ? "Adapter" : "Application", string.IsNullOrEmpty(app) ? base.Args["Adapter"] : base.Args["Application"], base.Args["Action"] });
                base.WriteLogEntry(LogEntryType.Information, formattedString);

                base.commandResult = new CommandResult();
            }
            catch (Exception exception2)
            {
                base.ShowError(exception2);
                base.commandResult = new CommandResult(exception2);
                if (((exception2 is OutOfMemoryException) || (exception2 is StackOverflowException)))
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
            string paramDesc_Type = CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Type);
            paramDesc_Type += "[" + ConfigurationManager.AppSettings["Type"] + "]";
            string paramDesc_Action = CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Action);
            paramDesc_Action += "[" + ConfigurationManager.AppSettings["Action"] + "]";
            CommandLineArgDescriptor[] collection = new CommandLineArgDescriptor[] { 
                new CommandLineArgDescriptor(true, "Type", paramDesc_Type, CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Environment", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Environment), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Application", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Application), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Version", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Version), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Adapter", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Adapter), CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Action", paramDesc_Action, CommandLineArgDescriptor.ArgumentType.String),
               new CommandLineArgDescriptor(true, "Log", CommandResources.GetString(CommandResources.ResourceID.ParamDesc_Log), CommandLineArgDescriptor.ArgumentType.Boolean)
            };
            CommandLineArgDescriptorList list = new CommandLineArgDescriptorList();
            list.AddRange(collection);
            return list;
        }

        public override void Validate()
        {
            ParameterHelper.ValidateAction(base.Args);
            ParameterHelper.ValidateType(base.Args);
            ParameterHelper.ValidateEnvironment(base.Args);
            ParameterHelper.ValidateApplicationOrAdapter(base.Args);
            ParameterHelper.ValidateLog(base.Args);
        }

        public override void WriteUsageHint()
        {
            System.Console.WriteLine(CommandResources.GetFormattedString(CommandResources.ResourceID.CommandUsageHint, new object[] { this.Name }));
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
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Example", new object[] { this.Name })), 0, System.Console.BufferWidth, 2);
                return (str + Environment.NewLine + str3);
            }
        }

        public override string Name
        {
            get
            {
                return "DeployApp";
            }
        }

        public override string Notes
        {
            get
            {
                string str = CommandResources.GetString("Label_Notes");
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Notes", new object[] { this.Name })), 0, System.Console.BufferWidth, 2);
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
                string str2 = ConsoleHelper.Wrap(this.Name + this.ParameterDescriptors.GetUsage(), 0, System.Console.BufferWidth, 2);
                return (str + Environment.NewLine + str2);
            }
        }
    }
}

