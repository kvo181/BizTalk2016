using System;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Remote.Common
{
    ///<summary>
    /// When invoking BizUnit steps on a remote server, this step has to be used to initialize
    /// the server name in the test context.
    ///</summary>
    public class RemoteServerHostStep : TestStepBase
    {
        ///<summary>
        /// Remote server hosting the BizUnit services.
        ///</summary>
        public string RemoteServer { get; set; }

        ///<summary>
        /// The name for the SPN identity.
        ///</summary>
        public string Spn { get; set; }

        /// <summary>
        /// Add the remote server to the test context
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(Context context)
        {
            if (string.IsNullOrEmpty(RemoteServer)) return;
            context.Add("BizUnitRemoteServer", RemoteServer);
            if (string.IsNullOrEmpty(Spn)) return;
            context.Add("BizUnitSPN", Spn);
        }

        /// <summary>
        /// Validate the class instance.
        /// </summary>
        /// <param name="context"></param>
        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(RemoteServer))
            {
                throw new ArgumentNullException("RemoteServer is either null or an empty string");
            }
        }
    }
}
