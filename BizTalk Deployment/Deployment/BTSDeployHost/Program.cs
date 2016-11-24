using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using bizilante.Tools.CommandLine;
using System.Collections.Specialized;
using System.Globalization;
using bizilante.Deployment.BTSDeployHost.Utils;
using bizilante.Deployment.BTSDeployHost.CommandLine;

namespace bizilante.Deployment.BTSDeployHost
{
    class Program
    {
        private int errors;
        private int warnings;

        public Program()
        {
            System.Console.CancelKeyPress += new ConsoleCancelEventHandler(Program.Console_CancelKeyPress);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            using (new MethodTracer(MethodBase.GetCurrentMethod()))
            {
                string format = StringResources.GetString(StringResources.ResourceID.CancelKeyPress);
                using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Warning)))
                {
                    Trace.WriteLine(format);
                    System.Console.WriteLine(format);
                }
            }
        }

        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.Program_UnhandledException);
            using (new MethodTracer(MethodBase.GetCurrentMethod()))
            {
                MethodTracer.TraceAssembly();
                Program program = new Program();
                return program.Run(args);
            }
        }

        private static void Program_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.IsTerminating)
            {
                Exception exceptionObject = e.ExceptionObject as Exception;
                if (exceptionObject != null)
                {
                    string format = "FATAL ERROR: An unhandled exception occurred in a thread pool or finalizer thread.";
                    Trace.WriteLine(format);
                    System.Console.WriteLine(format);
                    Trace.WriteLine(exceptionObject.ToString());
                    System.Console.WriteLine(exceptionObject.ToString());
                }
            }
            else
            {
                string str2 = "FATAL ERROR: An unhandled exception occurred in a managed thread.";
                Trace.WriteLine(str2);
                System.Console.WriteLine(str2);
            }
        }

        private int Run(string[] args)
        {
            try
            {
                NameValueCollection nameValueArgs = CommandLineParser.Parse(args);
                WriteHeader();
                CommandLineHelper.TraceWriteArguments(args, nameValueArgs);
                if (CommandLineHelper.ArgSpecified(nameValueArgs, "Debug"))
                {
                    nameValueArgs.Remove("Debug");
                    CommandLineHelper.ConsoleWriteArguments(nameValueArgs);
                }
                Command command = CommandFactory.Create(nameValueArgs);
                bool flag2 = CommandLineHelper.ArgSpecified(nameValueArgs, "?") || CommandLineHelper.ArgSpecified(nameValueArgs, "Help");
                if ((nameValueArgs.Count == 0) || flag2)
                {
                    using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
                    {
                        command.WriteUsage();
                        goto Label_02F4;
                    }
                }
                if (command is HelpCommand)
                {
                    command.Execute();
                    this.errors += command.Result.ErrorCount;
                    this.warnings += command.Result.WarningCount;
                    this.WriteCommandSummary();
                    return this.errors;
                }
                List<CommandLineArgumentException> list = command.ValidateArgs();
                if (list.Count > 0)
                {
                    foreach (CommandLineArgumentException exception in list)
                    {
                        this.errors += (exception.Severity == TraceLevel.Error) ? 1 : 0;
                        this.warnings += (exception.Severity == TraceLevel.Warning) ? 1 : 0;
                        using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(exception.Severity)))
                        {
                            System.Console.WriteLine(exception.Severity.ToString() + ": " + exception.Message);
                            System.Console.WriteLine(exception.StackTrace);
                            System.Console.WriteLine(string.Empty);
                            continue;
                        }
                    }
                }
                if (this.errors == 0)
                {
                    command.Execute();
                    this.errors += command.Result.ErrorCount;
                    this.warnings += command.Result.WarningCount;
                    this.WriteCommandSummary();
                }
                else
                {
                    using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
                    {
                        command.WriteUsageHint();
                    }
                }
            }
            catch (Exception exception2)
            {
                Trace.WriteLine(exception2.ToString());
                this.errors++;
                using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Error)))
                {
                    System.Console.WriteLine(exception2.Message);
                    System.Console.WriteLine(exception2.StackTrace);
                }
                if (((exception2 is OutOfMemoryException) || (exception2 is StackOverflowException)))
                {
                    throw;
                }
            }
            finally
            {
                string formattedString = StringResources.GetFormattedString(StringResources.ResourceID.CommandCompleted, new object[] { this.errors.ToString(CultureInfo.InvariantCulture), this.warnings.ToString(CultureInfo.InvariantCulture) });
                Trace.WriteLine(string.Empty);
                Trace.WriteLine(formattedString);
            }
        Label_02F4:
            return this.errors;
        }

        private void WriteCommandSummary()
        {
            string formattedString;
            if (this.errors > 0)
            {
                formattedString = StringResources.GetFormattedString(StringResources.ResourceID.CommandResultFailed, new object[] { this.errors.ToString(CultureInfo.InvariantCulture), this.warnings.ToString(CultureInfo.InvariantCulture) });
            }
            else
            {
                formattedString = StringResources.GetFormattedString(StringResources.ResourceID.CommandResultSucceeded, new object[] { this.errors.ToString(CultureInfo.InvariantCulture), this.warnings.ToString(CultureInfo.InvariantCulture) });
            }
            using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
            {
                System.Console.WriteLine(string.Empty);
                System.Console.WriteLine(formattedString);
                System.Console.WriteLine(string.Empty);
            }
        }

        private static void WriteHeader()
        {
            using (new ConsoleColorChanger(ConsoleColorManager.GetInstance().GetColor(TraceLevel.Info)))
            {
                string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                string formattedString = StringResources.GetFormattedString(StringResources.ResourceID.HeaderDescription, new object[] { fileVersion });
                string str3 = StringResources.GetString(StringResources.ResourceID.HeaderCopyright);
                System.Console.WriteLine(formattedString);
                System.Console.WriteLine(str3);
                System.Console.WriteLine(string.Empty);
            }
        }
    }
}
