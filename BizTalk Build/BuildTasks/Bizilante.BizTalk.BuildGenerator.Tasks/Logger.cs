using System;
using Microsoft.Build.Utilities;

namespace bizilante.BuildGenerator.Tasks
{
    /// <summary>
    /// This class is to abstract the msbuild logger so when unit testing we do not fail because
    /// msbuild didnt start the test
    /// </summary>
    internal sealed class Logger
    {
        /// <summary>
        /// Ctor
        /// </summary>
        private Logger()
        {
        }

        /// <summary>
        /// This allows a task to log messages
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        internal static void LogMessage(Task task, string message)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (task.BuildEngine != null)
                task.Log.LogMessage(message);
        }
    }
}