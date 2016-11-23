//---------------------------------------------------------------------
// File: ContextVerifyStep.cs
// 
// Summary: 
//
// Copyright (c) Santosh Benjamin
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using BizUnit.Common;
using BizUnit.Xaml;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace BizUnit.Extensions
{
    /// <summary>
    /// This step verifies whether the context has the specified keys and values. 
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// In this example, the data has already been loaded
    /// 
    /// <code escaped="true">
    ///	<ValidationStep AssemblyPath="" TypeName="BizUnit.Extensions.ContextVerifyStep">
    ///		<Verify contextKey="Name" keyValue="Microsoft" />
    ///		<Verify contextKey="Age" keyValue="25"  />
    ///	</ValidationStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Verify</term>
    ///			<description> The assertion to be made <para>(repeating)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>Verify/contextKey</term>
    ///			<description>The key name of the item in the context to be checked</description>
    ///		</item>
    ///		<item>
    ///			<term>Verify/keyValue</term>
    ///			<description>The value corresponding to the key</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	

    public class ContextVerifyStep : SubStepBase
    {

        private string contextKey;

        public string ContextKey
        {
            get { return contextKey; }
            set { contextKey = value; }
        }
        private string keyValue;

        public string KeyValue
        {
            get { return keyValue; }
            set { keyValue = value; }
        }

        private StringBuilder keysThatFailed = new StringBuilder();

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='data'>The stream cintaining the data to be validated.</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override Stream Execute(Stream data, Context context)
        {
            context.LogInfo("Checking key :" + ContextKey);
            if (context.GetValue(ContextKey) != KeyValue)
            {
                keysThatFailed.Append(ContextKey);
                keysThatFailed.Append("|");
                string errMsg = "Keys that failed check are : " + keysThatFailed.ToString();
                Exception e = new Exception(errMsg);
                context.LogException(e);
            }
            return data;
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(contextKey, "ContextKey");
            ArgumentValidation.CheckForEmptyString(keyValue, "KeyValue");
        }
    }
}
