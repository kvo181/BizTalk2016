using BizUnitWcfServiceLibrary;
using System;
using System.Configuration;
using System.Security.Principal;
using System.ServiceModel;

namespace BizUnit.TestSteps.BizTalk.Remote
{
    ///<summary>
    ///</summary>
    public class ServiceHelper
    {
        ///<summary>
        /// Create a WCF channel for the BizUnitService
        ///</summary>
        ///<returns></returns>
        public static IBizUnitService GetBizUnitService(Context context)
        {
            return GetBizUnitService(context, false);
        }

        ///<summary>
        /// Create a WCF channel for the BizUnitService
        ///</summary>
        ///<param name="context"></param>
        ///<param name="impersonate"></param>
        ///<returns></returns>
        public static IBizUnitService GetBizUnitService(Context context, bool impersonate)
        {
            var factory = new ChannelFactory<IBizUnitService>("bizUnitTcpEndPoint");
            if (factory.Credentials != null && impersonate)
            {
                factory.Credentials.Windows.AllowedImpersonationLevel =
                    TokenImpersonationLevel.Impersonation;
                //factory.Credentials.Windows.ClientCredential =
                //    System.Net.CredentialCache.DefaultNetworkCredentials;
                context.LogInfo("AllowedImpersonationLevel:{0}, Username:{1}", factory.Credentials.Windows.AllowedImpersonationLevel, factory.Credentials.Windows.ClientCredential.UserName);
            }

            // We may need to override the default endpoint uri
            if (context.ContainsKey("BizUnitRemoteServer"))
            {
                var remoteServer = context.GetValue("BizUnitRemoteServer");
                var bizUnitEndPointFormat = ConfigurationManager.AppSettings["BizUnitEndPoint"];
                if (!string.IsNullOrEmpty(bizUnitEndPointFormat))
                {
                    if (factory.Endpoint != null)
                    {
                        var bizUnitEndPoint = string.Format(bizUnitEndPointFormat, remoteServer);
                        var uri = new Uri(bizUnitEndPoint);
                        var spn = string.Empty;
                        if (context.ContainsKey("BizUnitSPN"))
                            spn = context.GetValue("BizUnitSPN");
                        var address = new EndpointAddress(uri, EndpointIdentity.CreateSpnIdentity(spn));
                        factory.Endpoint.Address = address;
                    }
                }
            }

            if (factory.Endpoint != null)
                context.LogInfo("Uri:{0}", factory.Endpoint.Address.Uri.AbsoluteUri);

            return factory.CreateChannel();
        }
    }
}
