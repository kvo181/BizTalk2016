using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using bizilante.Tools.CommandLine;
using System.Globalization;

namespace bizilante.Deployment.BTSDeployHost.CommandLine
{
    internal sealed class HelpCommand : Command
    {
        public HelpCommand(NameValueCollection nameValueArgs) : base(nameValueArgs)
        {
        }

        public override void Execute()
        {
            try
            {
                string[] values = base.Args.GetValues((string) null);
                if ((values != null) && (values.Length > 0))
                {
                    string str = values[0];
                    throw new CommandLineArgumentException(CommandResources.GetFormattedString(CommandResources.ResourceID.UnknownCommand, new object[] { str }), null, TraceLevel.Error);
                }
                this.WriteUsage();
                base.commandResult = new CommandResult();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.ToString());
                base.WriteLogEntry(LogEntryType.Error, exception.Message);
                base.commandResult = new CommandResult(exception);
                if ((exception is OutOfMemoryException) || (exception is StackOverflowException))
                {
                    throw;
                }
            }
        }

        protected override CommandLineArgDescriptorList GetParameterDescriptors()
        {
            CommandLineArgDescriptor[] collection = new CommandLineArgDescriptor[0];
            CommandLineArgDescriptorList list = new CommandLineArgDescriptorList();
            list.AddRange(collection);
            return list;
        }

        public override void WriteUsage()
        {
            System.Console.WriteLine(this.Name + ": " + base.Description);
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(CommandResources.GetString(CommandResources.ResourceID.Label_Usage));
            System.Console.WriteLine("  " + CommandResources.GetString(CommandResources.ResourceID.ProgramUsage));
            System.Console.WriteLine(string.Empty);
            NameValueCollection nameValueArgs = new NameValueCollection();
            List<Command> list = new List<Command>();
            list.Add(new BTSDeployAppCommand(nameValueArgs));
            list.Add(new BTSDeployCommand(nameValueArgs));
            StringBuilder builder = new StringBuilder();
            string str4 = CommandResources.GetString(CommandResources.ResourceID.Label_Commands);
            builder.Append(str4);
            foreach (Command command in list)
            {
                builder.Append(Environment.NewLine);
                builder.Append(command.GetSummary());
            }
            System.Console.WriteLine(builder.ToString());
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(this.Example);
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(ConsoleHelper.Wrap(this.Notes, 2, System.Console.BufferWidth, -2));
            System.Console.WriteLine(string.Empty);
        }

        public override void WriteUsageHint()
        {
            System.Console.WriteLine(CommandResources.GetFormattedString(CommandResources.ResourceID.ProgramUsageHint, new object[] { this.Name }));
        }

        public override string Name
        {
            get
            {
                return "BTSDeployHost.exe";
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

        public override string Notes
        {
            get
            {
                string str = CommandResources.GetString("Label_Notes");
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Notes", new object[] { this.Name })), 0, System.Console.BufferWidth, 2);
                return (str + Environment.NewLine + str3);
            }
        }
    }
}

