//---------------------------------------------------------------------
// File: FileMoveStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2016, bizilante. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Collections.ObjectModel;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Soap
{
    public class WebServiceStep : TestStepBase
    {
        private Stream _request;
        private Stream _response;
        private Collection<SoapHeader> _soapHeaders = new Collection<SoapHeader>();

        public DataLoaderBase RequestBody { get; set; }
        public string ServiceUrl { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool HasResponse { get; set; }

        public WebServiceStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }

        public Collection<SoapHeader> SoapHeaders
        {
            set
            {
                _soapHeaders = value;
            }
            get
            {
                return _soapHeaders;
            }
        }

        public override void Execute(Context context)
        {
            _request = RequestBody.Load(context);

            context.LogXmlData("Request", _request, true);

            if (HasResponse)
            {
                _response = CallWebMethod(
                    _request,
                    ServiceUrl,
                    Action,
                    Username,
                    Password,
                    context);

                var responseForPostProcessing = 
                    SubSteps.Aggregate(_response, (current, subStep) => subStep.Execute(current, context));
            }
            else
            {
                CallVoidWebMethod(
                    _request,
                    ServiceUrl,
                    Action,
                    Username,
                    Password,
                    context);
            }
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(ServiceUrl))
            {
                throw new StepValidationException("ServiceUrl may not be null or empty", this);
            }

            if (string.IsNullOrEmpty(Action))
            {
                throw new StepValidationException("Action may not be null or empty", this);
            }

            RequestBody.Validate(context);
        }

        private Stream CallWebMethod(
            Stream requestData,
            string serviceUrl,
            string action,
            string username,
            string password,
            Context ctx)
        {
            try
            {
                Stream responseData;
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.UseDefaultWebProxy = true;

                var epa = new EndpointAddress(new Uri(serviceUrl));

                ChannelFactory<IGenericContract> cf = null;
                IGenericContract channel;
                Message request;
                Message response;
                string responseString;

                try
                {
                    cf = new ChannelFactory<IGenericContract>(binding, epa);
                    if (cf.Credentials != null)
                    {
                        if (!UseDefaultCredentials)
                        {
                            cf.Credentials.UserName.UserName = username;
                            cf.Credentials.UserName.Password = password;
                        }
                        //else
                        //{
                        //    cf.Credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
                        //    cf.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Identification;
                        //}
                    }

                    cf.Open();
                    channel = cf.CreateChannel();
                    using (new OperationContextScope((IContextChannel)channel))
                    {
                        XmlReader r = new XmlTextReader(requestData);

                        request = Message.CreateMessage(MessageVersion.Soap11, action, r);

                        foreach (var header in _soapHeaders)
                        {
                            MessageHeader messageHeader = MessageHeader.CreateHeader(header.HeaderName, header.HeaderNameSpace, header.HeaderInstance);
                            OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader);
                        }

                        response = channel.Invoke(request);

                        string responseStr = response.GetReaderAtBodyContents().ReadOuterXml();
                        ctx.LogXmlData("Response", responseStr);
                        responseData = StreamHelper.LoadMemoryStream(responseStr);
                    }
                    request.Close();
                    response.Close();
                    cf.Close();
                }
                catch (CommunicationException ce)
                {
                    ctx.LogException(ce);
                    if (cf != null)
                    {
                        cf.Abort();
                    }
                    throw;
                }
                catch (TimeoutException te)
                {
                    ctx.LogException(te);
                    if (cf != null)
                    {
                        cf.Abort();
                    }
                    throw;
                }
                catch (Exception e)
                {
                    ctx.LogException(e);
                    if (cf != null)
                    {
                        cf.Abort();
                    }
                    throw;
                }

                return responseData;
            }
            catch (Exception ex)
            {
                ctx.LogException(ex);
                throw;
            }
        }
        private void CallVoidWebMethod(
            Stream requestData,
            string serviceUrl,
            string action,
            string username,
            string password,
            Context ctx)
        {
            try
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.UseDefaultWebProxy = true;

                var epa = new EndpointAddress(new Uri(serviceUrl));

                ChannelFactory<IGenericVoidContract> cf = null;
                IGenericVoidContract channel;
                Message request;

                try
                {
                    cf = new ChannelFactory<IGenericVoidContract>(binding, epa);
                    if (cf.Credentials != null)
                    {
                        if (!UseDefaultCredentials)
                        {
                            cf.Credentials.UserName.UserName = username;
                            cf.Credentials.UserName.Password = password;
                        }
                        //else
                        //{
                        //    cf.Credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
                        //    cf.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Identification;
                        //}
                    }

                    cf.Open();
                    channel = cf.CreateChannel();
                    using (new OperationContextScope((IContextChannel)channel))
                    {
                        XmlReader r = new XmlTextReader(requestData);

                        request = Message.CreateMessage(MessageVersion.Soap11, action, r);

                        foreach (var header in _soapHeaders)
                        {
                            MessageHeader messageHeader = MessageHeader.CreateHeader(header.HeaderName, header.HeaderNameSpace, header.HeaderInstance);
                            OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader);
                        }

                        channel.Invoke(request);
                    }
                    request.Close();
                    cf.Close();
                }
                catch (CommunicationException ce)
                {
                    ctx.LogException(ce);
                    if (cf != null)
                    {
                        cf.Abort();
                    }
                    throw;
                }
                catch (TimeoutException te)
                {
                    ctx.LogException(te);
                    if (cf != null)
                    {
                        cf.Abort();
                    }
                    throw;
                }
                catch (Exception e)
                {
                    ctx.LogException(e);
                    if (cf != null)
                    {
                        cf.Abort();
                    }
                    throw;
                }

            }
            catch (Exception ex)
            {
                ctx.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// A dummy WCF interface that will be manipulated by the CallWebMethod above
        /// </summary>
        [ServiceContract]
        interface IGenericContract
        {
            [OperationContract(Action = "*", ReplyAction = "*")]
            Message Invoke(Message msg);
        }
        /// <summary>
        /// A dummy WCF interface that will be manipulated by the CallWebMethod above
        /// </summary>
        [ServiceContract]
        interface IGenericVoidContract
        {
            [OperationContract(Action = "*", IsOneWay = true)]
            void Invoke(Message msg);
        }
    }
}
