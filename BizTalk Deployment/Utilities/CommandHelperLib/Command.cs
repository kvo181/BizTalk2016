using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Diagnostics;

namespace bizilante.Tools.CommandLine
{
    public abstract class Command
    {
        protected CommandResult commandResult;
        protected NameValueCollection nameValueArgs;
        protected CommandLineArgDescriptorList parameterDescriptors;

        protected Command(NameValueCollection nameValueArgs)
        {
            this.nameValueArgs = nameValueArgs;
        }

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        protected abstract CommandLineArgDescriptorList GetParameterDescriptors();
        public string GetSummary()
        {
            return ConsoleHelper.Wrap(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { this.Name.PadRight(0x12, ' '), this.Description }), 0x15, Console.BufferWidth, -19);
        }

        protected void OnLog(object sender, LogEventArgs e)
        {
            if (e != null)
            {
                using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor((System.Diagnostics.TraceLevel) e.LogEntry.Type)))
                {
                    Console.WriteLine(e.LogEntry.ToString());
                }
            }
        }

        public void ShowError(Exception ex)
        {
            Exception innerException = ex;
            StringBuilder builder = new StringBuilder();
            while (innerException != null)
            {
                Trace.WriteLine(innerException.ToString());
                builder.Append(innerException.Message + Environment.NewLine);
                builder.Append(innerException.StackTrace + Environment.NewLine);
                innerException = innerException.InnerException;
            }
            this.WriteLogEntry(LogEntryType.Error, builder.ToString());
        }

        public virtual void Validate()
        {
            throw new NotImplementedException();
        }

        public virtual List<CommandLineArgumentException> ValidateArgs()
        {
            List<CommandLineArgumentException> list = new List<CommandLineArgumentException>();
            list.AddRange(CommandLineParser.Expand(ref this.nameValueArgs, this.ParameterDescriptors));
            list.AddRange(CommandLineParser.Validate(this.nameValueArgs, this.ParameterDescriptors));
            string[] values = this.nameValueArgs.GetValues((string) null);
            if ((values != null) && (values.Length > 1))
            {
                string[] strArray2 = new string[values.Length - 1];
                for (int i = 0; i < (values.Length - 1); i++)
                {
                    strArray2[i] = values[i + 1];
                }
                string formattedString = CommandResources.GetFormattedString(CommandResources.ResourceID.ExtraUnnamedArguments, new object[] { string.Join(", ", strArray2) });
                list.Add(new CommandLineArgumentException(formattedString, string.Empty, System.Diagnostics.TraceLevel.Error));
            }
            return list;
        }

        protected void WriteLogEntry(LogEntryType logEntryType, Exception exception)
        {
            while (exception != null)
            {
                LogEntry logEntry = new LogEntry(exception.Message, logEntryType);
                this.OnLog(this, new LogEventArgs(logEntry));
                exception = exception.InnerException;
            }
        }

        protected void WriteLogEntry(LogEntryType logEntryType, string message)
        {
            LogEntry logEntry = new LogEntry(message, logEntryType);
            this.OnLog(this, new LogEventArgs(logEntry));
        }

        public virtual void WriteUsage()
        {
            Console.WriteLine(ConsoleHelper.Wrap(this.Name + ": " + this.Description, 2, Console.BufferWidth, -2));
            Console.WriteLine(string.Empty);
            Console.WriteLine(this.Usage);
            Console.WriteLine(string.Empty);
            Console.WriteLine(this.Parameters);
            Console.WriteLine(string.Empty);
            Console.WriteLine(ConsoleHelper.Wrap(this.Example, 2, Console.BufferWidth, -2));
            Console.WriteLine(string.Empty);
            Console.WriteLine(ConsoleHelper.Wrap(this.Notes, 2, Console.BufferWidth, -2));
            Console.WriteLine(string.Empty);
        }

        public virtual void WriteUsageHint()
        {
            Console.WriteLine(CommandResources.GetFormattedString(CommandResources.ResourceID.CommandUsageHint, new object[] { this.Name }));
        }

        public NameValueCollection Args
        {
            get
            {
                return this.nameValueArgs;
            }
        }

        public virtual string Description
        {
            get
            {
                return CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Description", new object[] { this.Name }));
            }
        }

        public virtual string Example
        {
            get
            {
                string str = CommandResources.GetString("Label_Example");
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Example", new object[] { this.Name })), 0, Console.BufferWidth, 2);
                return (str + Environment.NewLine + str3);
            }
        }

        public abstract string Name { get; }

        public virtual string Notes
        {
            get
            {
                string str = CommandResources.GetString("Label_Notes");
                string str3 = ConsoleHelper.Wrap(CommandResources.GetString(string.Format(CultureInfo.InvariantCulture, "Command_{0}.Notes", new object[] { this.Name })), 0, Console.BufferWidth, 2);
                return (str + Environment.NewLine + str3);
            }
        }

        public CommandLineArgDescriptorList ParameterDescriptors
        {
            get
            {
                if (this.parameterDescriptors == null)
                {
                    this.parameterDescriptors = this.GetParameterDescriptors();
                }
                return this.parameterDescriptors;
            }
        }

        public virtual string Parameters
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

        public CommandResult Result
        {
            get
            {
                return this.commandResult;
            }
        }

        public virtual string Usage
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

