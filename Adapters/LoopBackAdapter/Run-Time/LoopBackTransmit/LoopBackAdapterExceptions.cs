//---------------------------------------------------------------------
// File: LoopBackAdapterExceptions.cs
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
using System.Runtime.Serialization;

namespace bizilante.BizTalk.Adapters.Runtime.Adapters.LoopBackAdapter
{
	internal class LoopBackAdapterException : ApplicationException
	{
		public static string UnhandledTransmitError = "The LoopBack Adapter encounted an error transmitting a batch of messages.";

		public LoopBackAdapterException () { }

		public LoopBackAdapterException (string msg) : base(msg) { }

		public LoopBackAdapterException (Exception inner) : base(String.Empty, inner) { }

		public LoopBackAdapterException (string msg, Exception e) : base(msg, e) { }

		protected LoopBackAdapterException (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}

