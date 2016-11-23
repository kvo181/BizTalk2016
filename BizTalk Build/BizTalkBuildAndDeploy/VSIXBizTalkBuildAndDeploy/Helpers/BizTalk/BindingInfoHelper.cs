using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk
{
    /// <summary>
    /// Binding Info Helper
    /// </summary>
    public sealed class BindingInfoHelper
    {
        private string _bindingInfoFilePath = string.Empty;
        private Microsoft.BizTalk.Deployment.Binding.BindingInfo _bindingInfo =
            new Microsoft.BizTalk.Deployment.Binding.BindingInfo();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bindingInfoFilePath"></param>
        public BindingInfoHelper(string bindingInfoFilePath)
        {
            _bindingInfoFilePath = bindingInfoFilePath;
        }

        /// <summary>
        /// Get the list of InProcess hosts out of a Binding file
        /// </summary>
        public List<string> BizTalkHosts
        {
            get
            {
                if (!File.Exists(_bindingInfoFilePath))
                    return new List<string>();
                return GetBizTalkHosts();
            }
        }

        private void Deserialize()
        {
            if (string.IsNullOrEmpty(_bindingInfoFilePath))
                return;
            if (null != _bindingInfo)
                return;

            try
            {
                _bindingInfo.LoadXml(_bindingInfoFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to load xml {0}", _bindingInfoFilePath), ex);
            }
        }
        private List<string> GetBizTalkHosts()
        {
            List<string> biztalkhosts = new List<string>();
            Deserialize();
            if (null != _bindingInfo.ModuleRefCollection)
            {
                foreach (Microsoft.BizTalk.Deployment.Binding.ModuleRef module in _bindingInfo.ModuleRefCollection)
                {
                    foreach (Microsoft.BizTalk.Deployment.Binding.ServiceRef service in module.Services)
                    {
                        if (null == service.Host) continue;
                        if (!biztalkhosts.Contains(service.Host.Name))
                            if (ValidateHost(service.Host.Name))
                                biztalkhosts.Add(service.Host.Name);
                    }
                }
            }
            if (null != _bindingInfo.SendPortCollection)
            {
                foreach (Microsoft.BizTalk.Deployment.Binding.SendPort sendPort in _bindingInfo.SendPortCollection)
                {
                    if (null == sendPort.PrimaryTransport) continue;
                    if (null != sendPort.PrimaryTransport.SendHandler)
                        if (!biztalkhosts.Contains(sendPort.PrimaryTransport.SendHandler.Name))
                            if (ValidateHost(sendPort.PrimaryTransport.SendHandler.Name))
                                biztalkhosts.Add(sendPort.PrimaryTransport.SendHandler.Name);
                    if (null != sendPort.SecondaryTransport)
                    {
                        if (null != sendPort.SecondaryTransport.SendHandler)
                            if (!biztalkhosts.Contains(sendPort.SecondaryTransport.SendHandler.Name))
                                if (ValidateHost(sendPort.SecondaryTransport.SendHandler.Name))
                                    biztalkhosts.Add(sendPort.SecondaryTransport.SendHandler.Name);
                    }
                }
            }
            if (null != _bindingInfo.ReceivePortCollection)
            {
                foreach (Microsoft.BizTalk.Deployment.Binding.ReceivePort receivePort in _bindingInfo.ReceivePortCollection)
                {
                    foreach (Microsoft.BizTalk.Deployment.Binding.ReceiveLocation receiveLocation in receivePort.ReceiveLocations)
                    {
                        if (null != receiveLocation.ReceiveHandler)
                            if (!biztalkhosts.Contains(receiveLocation.ReceiveHandler.Name))
                                if (ValidateHost(receiveLocation.ReceiveHandler.Name))
                                    biztalkhosts.Add(receiveLocation.ReceiveHandler.Name);
                    }
                }
            }
            return biztalkhosts;
        }
        private bool ValidateHost(string name)
        {
            if (!HostsHelper.Exists(name))
                return false;

            if ((uint)HostsHelper.GetHostObject(name).GetPropertyValue("HostType") == 1)
                return true;
            else
                return false;
        }
    }
}
