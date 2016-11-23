using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Gets the current date formatted appropriately
    /// </summary>
    public class GetDate : Task
    {
        /// <summary>
        /// The outputted date
        /// </summary>
        [Output]
        public string Date { get; set; }

        /// <summary>
        /// The required format
        /// </summary>
        [Required]
        public string Format { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var now = DateTime.Now;
            Date = now.ToString(Format, null);
            return true;
        }

    }

}


