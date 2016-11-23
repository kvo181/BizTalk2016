using Microsoft.Build.Utilities;
using Microsoft.BizTalk.Adapter.Wcf.Publishing.Description;
using Microsoft.Build.Framework;
using Microsoft.BizTalk.Adapter.Wcf.Publishing;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Publishes a wcf service based on the description file
    /// </summary>
    public class WcfServiceGenerator : Task
    {
        /// <summary>
        /// The path to the description file
        /// </summary>
        [Required]
        public string DescriptionPath { get; set; }

        /// <summary>
        /// Overrides the execute to implement the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var description = WcfServiceDescription.LoadXml(DescriptionPath);
            var publisher = new Publisher();
            publisher.Publish(description);
            return true;
        }
    }
}
