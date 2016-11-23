
namespace BizUnit.TestSteps.i8c.Msmq
{
    ///<summary>
    /// Definition of a QueuePath.
    ///</summary>
    public class QueuePathDefinition
    {
        ///<summary>
        /// Queue Path name
        ///</summary>
        public string QueuePath { get; set; }
        ///<summary>
        /// Is the queue a transactional queue?
        ///</summary>
        public bool Transactional { get; set; }
        /// <summary>
        /// Indicates if the queue should exist
        /// </summary>
        public bool ShouldExist { get; set; }
    }
}
