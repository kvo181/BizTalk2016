//---------------------------------------------------------------------
// File: LoopBackTransmitAdapterBatch.cs
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


using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;

namespace bizilante.BizTalk.Adapters.Runtime.Adapters.LoopBackAdapter
{
    /// <summary>
    /// Summary description for LoopBackTransmitAdapterBatch.
    /// </summary>
    public class LoopBackTransmitAdapterBatch : AsyncTransmitterBatch
	{
		public LoopBackTransmitAdapterBatch(int maxBatchSize, string propertyNamespace, IBTTransportProxy transportProxy,
					AsyncTransmitter asyncTransmitter)
			:
			   base(maxBatchSize, typeof(LoopBackTransmitterEndpoint), propertyNamespace, null, transportProxy, asyncTransmitter)
		{
		}
	}
}
