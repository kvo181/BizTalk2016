//---------------------------------------------------------------------
// File: StringFormatterMultipleStep.cs
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
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.Extensions
{
    /// <summary>
    /// The StringFormatter multiple step is used to format a base with multiple Parameters
    /// and then to load the result into a specified key in the context
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    /// <TestStep AssemblyPath="BizUnit.Extensions.dll" TypeName="BizUnit.Extensions.StringFormatterMultipleStep">
    /// <BaseString>exec NotifyNewCRM '{0}','1'</BaseString>
    /// <ParamStringCollection>
    ///		<ParamString takeFromCtx="InputPartyID"></ParamString>
    /// </ParamStringCollection>
    /// <ContextKey>OutputString</ContextKey>
    /// </TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>BaseString</term>
    ///			<description>The base string with placeholders for the new Parameters</description>
    ///		</item>
    ///		<item>
    ///			<term>ParamStringCollection</term>
    ///			<description>A collection of parameter strings</description>
    ///		</item>
    ///		<item>
    ///			<term>ParamString</term>
    ///			<description>Each parameter string to pick up and this parameter may already exist in the context</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class StringFormatterMultipleStep : TestStepBase
    {

        private string baseString;

        public string BaseString
        {
            get { return baseString; }
            set { baseString = value; }
        }
        private string[] tokens;

        public string[] Tokens
        {
            get { return tokens; }
            set { tokens = value; }
        }

        private string contextKeyWithResult;

        public string ContextKeyWithResult
        {
            get { return contextKeyWithResult; }
            set { contextKeyWithResult = value; }
        }

        private string resultString;

        public string ResultString
        {
            get { return resultString; }
            set { resultString = value; }
        }

        public override void Execute(Context context)
        {
            Validate(context);
            resultString = String.Format(baseString, tokens);

            context.LogInfo(string.Format("String Formatter created : {0}", resultString));
            if (contextKeyWithResult.Length > 0)
            {
                context.Add(contextKeyWithResult, resultString, true);
            }

        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(baseString, "Base String");
            ArgumentValidation.CheckForNullReference(tokens, "Tokens");

        }
    }
}
