//---------------------------------------------------------------------
// File: Echo.cs
// 
// Summary: 
//
// Copyright (c) http://bizunitextensions.codeplex.com. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Xml;
using BizUnit.Xaml;

namespace BizUnit.Extensions
{
    /// <summary>
    /// This step echoes a given string to the console output (but through the context logInfo method. 
    /// It is extremely useful when tracing through large test scripts and we need to know at what stage we are at. 
    /// Its a sort of Debug.WriteLine functionality. 
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="" TypeName="BizUnit.Extensions.EchoStep">
    ///		<Message>Completed processing of first stage</Message>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Message</term>
    ///			<description>The message to be echoed to the output</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class EchoStep : TestStepBase
    {

        private string msgToDisplay;

        public string Message
        {
            get { return msgToDisplay; }
            set { msgToDisplay = value; }
        }

        public override void Execute(Context context)
        {
            Validate(context);
            context.LogInfo(String.Format("**** STAGE *** {0}", msgToDisplay));
        }

        public override void Validate(Context context)
        {
            //there is nothing to check here. If nothing is provided, nothing will be displayed
            if (msgToDisplay.Length == 0)
                context.LogWarning("Message to echo has been left blank");
            return;
        }
    }
}
