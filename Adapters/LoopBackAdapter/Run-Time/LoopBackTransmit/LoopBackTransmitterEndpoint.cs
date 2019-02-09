//---------------------------------------------------------------------
// File: LoopBackAdapterWorkItem.cs
// 
// Summary: Implementation of an adapter framework sample adapter. 
//
// Sample: LoopBack Transmit Adapter, demonstrating solicit-response.
//
//
//---------------------------------------------------------------------
// This file is part of the Microsoft BizTalk Server 2006 SDK
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// This source code is intended only as a supplement to Microsoft BizTalk
// Server 2006 release and/or on-line documentation. See these other
// materials for detailed information regarding Microsoft code samples.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;

using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;
using Microsoft.BizTalk.CAT.BestPractices.Framework.Instrumentation;


namespace bizilante.BizTalk.Adapters.Runtime.Adapters.LoopBackAdapter
{
    /// <summary>
    /// There is one instance of LoopBackTransmitterEndpoint class for each every static send port.
    /// Messages will be forwarded to this class by AsyncTransmitterBatch (via LoopBackTransmitAdapterBatch)
    /// </summary>
    internal class LoopBackTransmitterEndpoint : AsyncTransmitterEndpoint
    {
        private readonly IBTTransportProxy _transportProxy;
        private AsyncTransmitter _asyncTransmitter = null;
        private string _propertyNamespace;

        public LoopBackTransmitterEndpoint(AsyncTransmitter asyncTransmitter)
            : base(asyncTransmitter)
        {
            _asyncTransmitter = asyncTransmitter;
            _transportProxy = asyncTransmitter.TransportProxy;
        }

        public override void Open(EndpointParameters endpointParameters, IPropertyBag handlerPropertyBag, string propertyNamespace)
        {
            _propertyNamespace = propertyNamespace;
        }


        /// <summary>
        /// Implementation for AsyncTransmitterEndpoint::ProcessMessage
        /// Transmit the message and optionally return the response message
        /// </summary>
        public override IBaseMessage ProcessMessage(IBaseMessage message)
        {
            Guid callToken = TraceManager.CustomComponent.TraceIn();
            long startScope = TraceManager.CustomComponent.TraceStartScope("ProcessMessage", callToken);

            LoopBackTransmitProperties props = new LoopBackTransmitProperties(message, _propertyNamespace);
            IBaseMessage responseMsg = null;

            TraceManager.CustomComponent.TraceInfo("TwoWay: {0}", props.IsTwoWay);
            if (props.IsTwoWay)
            {
                responseMsg = BuildResponseMessage(message, message.Context, props);
            }

            TraceManager.CustomComponent.TraceEndScope("ProcessMessage", startScope, callToken);
            TraceManager.CustomComponent.TraceOut(callToken);

            return responseMsg;
        }

        private IBaseMessage BuildResponseMessage(IBaseMessage message, IBaseMessageContext context, LoopBackTransmitProperties props)
        {
            Guid callToken = TraceManager.CustomComponent.TraceIn();
            long startScope = TraceManager.CustomComponent.TraceStartScope("BuildResponseMessage", callToken);

            IBaseMessageFactory messageFactory = _transportProxy.GetMessageFactory();
            IBaseMessage btsResponse = messageFactory.CreateMessage();
            TraceManager.CustomComponent.TraceInfo("PropertyCopy: {0}", props.PropertyCopy);
            if (props.PropertyCopy)
            {
                btsResponse.Context = PipelineUtil.CloneMessageContext(context);
            }
            TraceManager.CustomComponent.TraceInfo("CustomPropertyCopy: {0}", props.CustomPropertyCopy);
            if (props.CustomPropertyCopy)
            {
                btsResponse.Context = messageFactory.CreateMessageContext();
                for (int i = 0; i < context.CountProperties; i++)
                {
                    string strName;
                    string strNamespace;
                    object oValue = context.ReadAt(i, out strName, out strNamespace);
                    if (!strNamespace.StartsWith("http://schemas.microsoft.com/BizTalk"))
                    {
                        if (context.IsPromoted(strName, strNamespace))
                        {
                            TraceManager.CustomComponent.TraceInfo("Promoted into context: {1}#{0}={2}", strName, strNamespace, oValue);
                            btsResponse.Context.Promote(strName, strNamespace, oValue);
                        }
                        else
                        {
                            TraceManager.CustomComponent.TraceInfo("Copied into context: {1}#{0}={2}", strName, strNamespace, oValue);
                            btsResponse.Context.Write(strName, strNamespace, oValue);
                        }
                    }
                }
            }
            TraceManager.CustomComponent.TraceInfo("PartCount: {0}", message.PartCount);
            for (int i = 0; i < message.PartCount; i++)
            {
                string str;
                VirtualStream stream = new VirtualStream();
                StreamReader rdr = new StreamReader(message.GetPartByIndex(i, out str).GetOriginalDataStream(), true);
                StreamWriter wrtr = new StreamWriter(stream, rdr.CurrentEncoding);
                wrtr.Write(rdr.ReadToEnd());
                rdr.Close();
                wrtr.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                IBaseMessagePart part = messageFactory.CreateMessagePart();
                if (props.PropertyCopy)
                {
                    part.Charset = message.GetPart(str).Charset;
                    part.ContentType = message.GetPart(str).ContentType;
                    part.PartProperties = PipelineUtil.CopyPropertyBag(message.GetPart(str).PartProperties, messageFactory);
                }
                btsResponse.AddPart(str, part, message.GetPart(str).PartID.Equals(message.BodyPart.PartID));
                btsResponse.GetPart(str).Data = stream;
            }

            TraceManager.CustomComponent.TraceEndScope("BuildResponseMessage", startScope, callToken);
            TraceManager.CustomComponent.TraceOut(callToken);

            return btsResponse;
        }
    }
}
