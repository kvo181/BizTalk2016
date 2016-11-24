using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using bizilante.Tools.CommandLine;

namespace bizilante.Deployment.BTSDeployHost.CommandLine
{
    sealed class BTSDeployCommand : Command
    {
        public BTSDeployCommand(NameValueCollection nameValueArgs)
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
                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.Deploy, new object[] {  });
                base.WriteLogEntry(LogEntryType.Information, formattedString);

                // Deploy the BizTalk application
                GUIBTSDeployPSHost.Run();

                formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.DeploySuccess, new object[] { });
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
            CommandLineArgDescriptor[] collection = new CommandLineArgDescriptor[] { };
            CommandLineArgDescriptorList list = new CommandLineArgDescriptorList();
            list.AddRange(collection);
            return list;
        }

        public override void Validate()
        {
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
                return "Deploy";
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

