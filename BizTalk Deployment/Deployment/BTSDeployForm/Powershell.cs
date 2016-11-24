using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace bizilante.Deployment.BTSDeployForm
{
    // A delegate type for hooking up change notifications.
    public delegate void LogEventHandler(object sender, LogEventArgs e);

    public class Powershell
    {
        // An event that clients can use to be notified whenever 
        // powershell output needs to visualised
        public event LogEventHandler Log;

        // Invoke the Log event; called whenever logging has to be done
        protected virtual void OnLog(LogEventArgs e)
        {
            if (Log != null)
                Log(this, e);
        }

        internal void LogMessage(string message)
        {
            OnLog(new LogEventArgs() { Message = message, Type = EventLogEntryType.Information });
        }
        internal void LogMessage(string format, params object[] args)
        {
            OnLog(new LogEventArgs() { Message = string.Format(format, args), Type = EventLogEntryType.Information });
        }
        internal void LogError(string message)
        {
            OnLog(new LogEventArgs() { Message = message, Type = EventLogEntryType.Error });
        }
        internal void LogError(string format, params object[] args)
        {
            OnLog(new LogEventArgs() { Message = string.Format(format, args), Type = EventLogEntryType.Error });
        }

        public void GetProcess()
        {
            LogMessage("Process                 Id");
            LogMessage("----------------------------");

            RunspaceInvoke invoker = new RunspaceInvoke();
            Collection<PSObject> results = invoker.Invoke("get-process");
            foreach (PSObject thisResult in invoker.Invoke("get-process"))
            {
                LogMessage(
                        "{0,-24}{1}",
                        thisResult.Members["ProcessName"].Value,
                        thisResult.Members["Id"].Value);
            }
        }

        public void TestOutputAndError()
        {
            RunspaceInvoke invoker = new RunspaceInvoke();
            string[] input = { "system", "security1", "software" };
            IList errors;
            string scriptBlock =
            "$input | foreach {get-item hklm:\\$_}";
            foreach (PSObject thisResult in
            invoker.Invoke(scriptBlock, input, out errors))
            {
                LogMessage("Output: {0}", thisResult);
            }
            foreach (object thisError in errors)
            {
                LogError("Error: {0}", thisError);
            }
            LogMessage("Test Finished");
        }

        public void TestWithPipeline()
        {
            LogMessage("Invoking TestWithPipeline...");
            // Create and open a runspace that uses the default host
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            // Create a pipeline that runs a script block
            Pipeline pipeline = runspace.CreatePipeline("dir c:\\");
            // Invoke the pipeline in a try...catch and save the
            // collection returned by Invoke()
            Collection<PSObject> results = null;
            try
            {
                results = pipeline.Invoke();
            }
            catch (RuntimeException e)
            {
                // Display a message and exit if a RuntimeException is thrown
                LogError("Exception during Invoke(): {0}, {1}",
                e.GetType().Name, e.Message);
                return;
            }
            // Display the BaseObject of every PSObject returned by Invoke()
            foreach (PSObject thisResult in results)
            {
                LogMessage("Result is: {0}", thisResult.BaseObject);
            }
            LogMessage("TestWithPipeline Finished.");
        }
        public void TestWithPipelineWithInput()
        {
            LogMessage("Invoking {0}...", MethodBase.GetCurrentMethod().Name);
            // Create and open a runspace that uses the default host
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            // Create a pipeline that runs a script block
            Pipeline pipeline = runspace.CreatePipeline("$input | sort-object");
            int[] numbers = { 3, 2, 1 };
            // Invoke the pipeline in a try...catch and save the
            // collection returned by Invoke()
            Collection<PSObject> results = null;
            try
            {
                pipeline.Input.Write(numbers, true);
                results = pipeline.Invoke();
            }
            catch (RuntimeException e)
            {
                // Display a message and exit if a RuntimeException is thrown
                LogError("Exception during Invoke(): {0}, {1}",
                e.GetType().Name, e.Message);
                return;
            }
            // Display the BaseObject of every PSObject returned by Invoke()
            foreach (PSObject thisResult in results)
            {
                LogMessage("Result is: {0}", thisResult.BaseObject);
            }
            LogMessage("{0} Finished.", MethodBase.GetCurrentMethod().Name);
        }
        public void TestDeployScript()
        {
            LogMessage("Invoking {0}...", MethodBase.GetCurrentMethod().Name);
            // Create and open a runspace that uses the default host
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            // Create a pipeline that runs a script block
            Pipeline pipeline = runspace.CreatePipeline("$input | d:\\biztalk\\powershell\\deployapplication.ps1");
            int[] numbers = { 3, 2, 1 };
            // Invoke the pipeline in a try...catch and save the
            // collection returned by Invoke()
            Collection<PSObject> results = null;
            try
            {
                //pipeline.Input.Write(numbers, true);
                results = pipeline.Invoke();
            }
            catch (RuntimeException e)
            {
                // Display a message and exit if a RuntimeException is thrown
                LogError("Exception during Invoke(): {0}, {1}",
                e.GetType().Name, e.Message);
                return;
            }
            // Display the BaseObject of every PSObject returned by Invoke()
            foreach (PSObject thisResult in results)
            {
                LogMessage("Result is: {0}", thisResult.BaseObject);
            }
            LogMessage("{0} Finished.", MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="scriptFile"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string RunScript(DeployForm host, string scriptFile, Dictionary<string, string> parameters)
        {
            //this.myHost.UI.Write(ConsoleColor.Cyan, ConsoleColor.Black, "\nRun Script: ");
            //this.Execute(scriptFile);
            /*
            // Create the host instance to use.
            var myHost = new MyPSHost(this);

            // create Powershell runspace 
            var runspace = RunspaceFactory.CreateRunspace(myHost);

            // open it 
            runspace.Open();

            // Create a PowerShell object to run the script.
            using (var powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;

                powershell.AddScript(scriptFile);
                var command = new Command(scriptFile);
                foreach (var p in parameters)
                    powershell.AddParameter(null, p.Value);
                powershell.Invoke(scriptFile);
            }

            // close the runspace 
            runspace.Close();

             */
            return null;
        }

        /// <summary>
        /// helper method that takes your script path, loads up the script 
        /// into a variable, and passes the variable to the RunScript method 
        /// that will then execute the contents 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string LoadScript(string filename)
        {
            try
            {
                // Create an instance of StreamReader to read from our file. 
                // The using statement also closes the StreamReader. 
                using (var sr = new StreamReader(filename))
                {

                    // use a string builder to get all our lines from the file 
                    var fileContents = new StringBuilder();

                    // string to hold the current line 
                    string curLine;

                    // loop through our file and read each line into our 
                    // stringbuilder as we go along 
                    while ((curLine = sr.ReadLine()) != null)
                    {
                        // read each line and MAKE SURE YOU ADD BACK THE 
                        // LINEFEED THAT IT THE ReadLine() METHOD STRIPS OFF 
                        fileContents.Append(curLine + "\n");
                    }

                    // call RunScript and pass in our file contents 
                    // converted to a string 
                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong. 
                var errorText = "The file could not be read:";
                errorText += e.Message + "\n";
                return errorText;
            }

        }

    }
}
