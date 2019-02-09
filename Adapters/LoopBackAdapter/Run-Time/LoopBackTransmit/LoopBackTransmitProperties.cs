//---------------------------------------------------------------------
// File: LoopBackTransmitProperties.cs
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

using Microsoft.XLANGs.BaseTypes;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;

namespace bizilante.BizTalk.Adapters.Runtime.Adapters.LoopBackAdapter
{
	/// <summary>
	/// This class maintains send port properties associated with a message. These properties
	/// will be extracted from the message context for static send ports.
	/// </summary>
	public class LoopBackTransmitProperties : ConfigProperties
	{
		private const string ProtocolAlias = "LoopBack://";

		// Handler members...
		private const int BatchSizeValue = 50;

		// Endpoint members...
		private readonly bool _isTwoWay;		
		private string _uri;
		private bool _propertyCopy;
        private bool _customPropertyCopy;

		//Handler properties
		public static int BatchSize { get { return BatchSizeValue; } }

		//Endpoint properties
		public bool IsTwoWay { get { return _isTwoWay; } }
		public string Uri { get { return _uri; } }
		public bool PropertyCopy { get { return _propertyCopy; } }
        public bool CustomPropertyCopy { get { return _customPropertyCopy; } }

		private static readonly PropertyBase IsSolicitResponseProp = new BTS.IsSolicitResponse();

		// If we needed to use SSO we will need this extra property. It is set in the
		// LocationConfiguration method below.
		// Additionally:
		//   TransmitLocation.xsd in the design-time project must also be edited to
		//   expose the necessary SSO properties.
		//   DotNetFileAsyncTransmitterBatch.cs within the run-time project must be
		//   edited to retrieve and populate the SSOResult class.
		//private string ssoAffiliateApplication;
		//public string AffiliateApplication { get { return ssoAffiliateApplication; } }

		public LoopBackTransmitProperties(IBaseMessage message, string propertyNamespace)
		{
			XmlDocument locationConfigDom;

			//  get the adapter configuration off the message
			string config = (string) message.Context.Read("AdapterConfig", propertyNamespace);
			_isTwoWay = (bool) message.Context.Read(IsSolicitResponseProp.Name.Name, IsSolicitResponseProp.Name.Namespace);

			//  the config can be null all that means is that we are doing a dynamic send
			if (null != config)
			{
				locationConfigDom = new XmlDocument();
				locationConfigDom.LoadXml(config);
				//  For Dynamic Sends the Location config can be null
				//  Location properties - possibly override some handler properties
				LocationConfiguration(locationConfigDom);
			}
		}

		public LoopBackTransmitProperties(string uri)
		{
			_uri = uri;
			UpdateUriForDynamicSend();
		}


		public static void TransmitHandlerConfiguration(XmlDocument configDom)
		{

		}

		public void LocationConfiguration (XmlDocument configDom)
		{
			// If we needed to use SSO we will need this extra property
			//this.ssoAffiliateApplication = IfExistsExtract(configDOM, "/Config/ssoAffiliateApplication");

			if (!Boolean.TryParse(configDom.SelectSingleNode("/Config/PropertyCopy").InnerText, out _propertyCopy))
			{
				_propertyCopy = false;
			}
            if (!Boolean.TryParse(configDom.SelectSingleNode("/Config/CustomPropertyCopy").InnerText, out _customPropertyCopy))
            {
                _customPropertyCopy = false;
            }
        }


		public void UpdateUriForDynamicSend()
		{
			// Strip off the adapters alias
			if ( _uri.StartsWith(ProtocolAlias, StringComparison.OrdinalIgnoreCase))
			{
				string newUri = _uri.Substring(ProtocolAlias.Length);
				_uri = newUri;
			}
		}
	}
}
