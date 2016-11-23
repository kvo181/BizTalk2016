using System.ServiceModel;

namespace MSMQWcfServiceLibrary
{
    ///<summary>
    /// MSMQ Service contract
    ///</summary>
    [ServiceContract]
    public interface IMsmqService
    {
        ///<summary>
        /// Create a MSMQ queue on the machine hosting this service.
        /// (Can only be created locally)
        ///</summary>
        ///<param name="queueName"></param>
        ///<param name="label"></param>
        ///<param name="transactional"></param>
        /// <returns>Path of the created queue.</returns>
        [OperationContract]
        string CreateQueue(string queueName, string label, bool transactional);
        ///<summary>
        /// Delete a MSMQ queue on the machine hosting this service.
        /// (Can only be deleted locally)
        ///</summary>
        ///<param name="queueName"></param>
        [OperationContract]
        void DeleteQueue(string queueName);
    }
}
