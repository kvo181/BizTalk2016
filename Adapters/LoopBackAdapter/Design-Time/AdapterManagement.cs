//---------------------------------------------------------------------
// File: AdapterManagement.cs
// 
// Summary: Implementation of adapter framework interfaces for sample
// adapters.
//
// Sample: Adapter framework adapter.
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
using System.Reflection;
using System.Resources;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Adapter.Framework;
using Microsoft.Samples.BizTalk.Adapter.Common;

namespace bizilante.BizTalk.Adapters.LoopBackDesignTime
{
	/// <summary>
	/// Class StaticAdapterManagement implements
	/// IAdapterConfig and IStaticAdapterConfig interfaces for
	/// management to illustrate a static adapter that uses the
	/// adapter framework
	/// </summary>
	public class AdapterManagement : AdapterManagementBase ,IAdapterConfig, IStaticAdapterConfig, IAdapterConfigValidation 
	{
		private static readonly ResourceManager resourceManager = 
			new ResourceManager("bizilante.BizTalk.Adapters.LoopBackDesignTime.LoopBackAdapterResource", Assembly.GetExecutingAssembly());


		#region IAdapterConfig Members
		/// <summary>
		/// Returns the configuration schema as a string.
		/// (Implements IAdapterConfig)
		/// </summary>
		/// <param name="type">Configuration schema to return</param>
		/// <returns>Selected xsd schema as a string</returns>
		public string GetConfigSchema(ConfigType type) 
		{
			switch (type) 
			{
				case ConfigType.TransmitHandler:
					throw new NotSupportedException("Configuration not supported.  Cannot fetch config schema for transmithandler.");

				case ConfigType.TransmitLocation:
					return LocalizeSchema(GetResource("bizilante.BizTalk.Adapters.LoopBackDesignTime.TransmitLocation.xsd"), resourceManager);

				default:
					throw new NotSupportedException("Configuration not supported.  Cannot fetch config schema for receival.");
			}
		}
		/// <summary>
		/// Acquire externally referenced xsd's
		/// </summary>
		/// <param name="xsdLocation">Location of schema</param>
		/// <param name="xsdNamespace">Namespace</param>
		/// <param name="xsdSchema">Schema file name (return)</param>
		/// <returns>Outcome of acquisition</returns>
		public Result GetSchema(string xsdLocation,
								string xsdNamespace,
								out string xsdSchema)
		{
			xsdSchema = null;
			return Result.Continue;
		}
		#endregion


		#region IStaticAdapterConfig Members
		/// <summary>
		/// Get the WSDL file name for the selected WSDL in GetServiceOrganization.
		/// Dummy implementation with a file open dialog.
		/// </summary>
		/// <param name="wsdls">place holder</param>
		/// <returns>The selected WSDL or an empty string[]</returns>
		public string[] GetServiceDescription(string[] wsdls) 
		{
			return null;
		}

		/// <summary>
		/// This function is called by the add generated items functionality in 
		/// BizTalk (rightclick BizTalk project in VS, Add, etc.) 
		/// Gets the XML instance of TreeView that needs to be rendered
		/// Dummy implementation with a file open dialog.
		/// </summary>
		/// <param name="endPointConfiguration"></param>
		/// <param name="nodeIdentifier"></param>
		/// <returns>TreeView xml instance</returns>
		public string GetServiceOrganization(IPropertyBag endPointConfiguration,
											 string nodeIdentifier)
		{
			return null;
		}
		#endregion


		#region IAdapterConfigValidation Members
		/// <summary>
		/// Validate xmlInstance against configuration.  In this example it does nothing.
		/// </summary>
		/// <param name="configType">Type of port or location being configured</param>
		/// <param name="xmlInstance">Instance value to be validated</param>
		/// <returns>Validated configuration.</returns>
		public string ValidateConfiguration(ConfigType configType,
			string xmlInstance) 
		{
			string validXml = String.Empty;

			switch (configType) 
			{
				case ConfigType.ReceiveHandler:
					break;
				case ConfigType.ReceiveLocation:
					break;
				case ConfigType.TransmitHandler:
					break;

				case ConfigType.TransmitLocation:
					validXml = ValidateTransmitLocation(xmlInstance); 
					break;
			}

			return validXml;
		}
		#endregion

		/// <summary>
		/// Helper to get resource from manifest. Replace with 
		/// ResourceManager.GetString if .resources or
		/// .resx files are used for managing this assemblies resources.
		/// </summary>
		/// <param name="resource">Full resource name</param>
		/// <returns>Resource value</returns>
		private string GetResource(string resource) 
		{
			string value = null;
			if (null != resource) 
			{
				Assembly assem = GetType().Assembly;
				Stream stream = assem.GetManifestResourceStream(resource);
				StreamReader reader;

				if (stream != null)
					using (reader = new StreamReader(stream)) 
					{
						value = reader.ReadToEnd();
					}
			}

			return value;
		}

		/// <summary>
		/// Generate uri entry based on transmit location configuration values
		/// </summary>
		/// <param name="xmlInstance">Instance value to be validated</param>
		/// <returns>Validated configuration.</returns>
		private string ValidateTransmitLocation(string xmlInstance) 
		{
			// Load up document
			XmlDocument document = new XmlDocument();
			document.LoadXml(xmlInstance);
			
			XmlNode uri = document.SelectSingleNode("Config/uri");
			if (null == uri) 
			{
				uri = document.CreateElement("uri");
				document.DocumentElement.AppendChild(uri);
			}
			uri.InnerText = "LoopBack://" + Guid.NewGuid().ToString();
						   
			return document.OuterXml;
		}

	} 
}
