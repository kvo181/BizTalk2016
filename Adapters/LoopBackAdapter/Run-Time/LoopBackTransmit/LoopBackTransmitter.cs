//---------------------------------------------------------------------
// File: LoopBackTransmitter.cs
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
using System.Xml;

using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;

using System.Reflection;
using Microsoft.Samples.BizTalk.Adapter.Common;

namespace bizilante.BizTalk.Adapters.Runtime.Adapters.LoopBackAdapter
{
	/// <summary>
	/// This is a singleton class for the LoopBack send adapter. All the messages, going to various
	/// send ports of this adapter type, will go through this class.
	/// 
	/// Messages will be delivered to this adapter in batches. The batch implementation is provided
	/// by the LoopBackTransmitAdapterBatch class.
	/// </summary>
	sealed public class LoopBackTransmitAdapter : AsyncTransmitter
	{
		private static Assembly _asm = null;
		private const string LoopBackTitle = "LoopBack Adapter";
        //private const string LoopBackVersion = "1.0.0.0";
        private static string LoopBackVersion = GetAssemblyAttribute(typeof(AssemblyVersionAttribute));
        private const string LoopBackDescription = "Return the transmitted message back to the MessageBox";
		private const string LoopBackNamespace = "http://bizilante.BizTalk.Adapters.LoopBack";


		public LoopBackTransmitAdapter() : base(
				LoopBackTitle,
				LoopBackVersion,
				LoopBackDescription,
				"LoopBack",
				new Guid("{01F122B8-A093-4C81-886A-0B986A94CF39}"),
				LoopBackNamespace, 
				typeof(LoopBackTransmitterEndpoint), 
				10)
		{
		}

		private static string GetAssemblyAttribute(Type tp)
		{
			if (_asm == null)
			{
				_asm = Assembly.GetExecutingAssembly();
			}
			object[] attrs = _asm.GetCustomAttributes(tp, false);
			if (tp == typeof(AssemblyVersionAttribute))
			{
				return _asm.GetName().Version.ToString();
			}
		    if (attrs.Length > 0)
		    {
		        return ((AssemblyTitleAttribute)attrs[0]).Title;
		    }
		    return "";
		}

		protected override IBTTransmitterBatch CreateAsyncTransmitterBatch()
		{
			return new LoopBackTransmitAdapterBatch(MaxBatchSize, LoopBackNamespace, TransportProxy, this);		
		}

		public ConfigProperties CreateProperties(string uri)
		{
			ConfigProperties properties = new LoopBackTransmitProperties(uri);
			return properties;
		}

		protected override void HandlerPropertyBagLoaded()
		{
			IPropertyBag config = HandlerPropertyBag;
			if (null != config)
			{
				XmlDocument handlerConfigDom = ConfigProperties.IfExistsExtractConfigDom(config);
				if (null != handlerConfigDom)
				{
					LoopBackTransmitProperties.TransmitHandlerConfiguration(handlerConfigDom);
				}
			}
		}
	}
}
