//---------------------------------------------------------------------
// File: StringFormatterStep.cs
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
    /// The StringformatterStep is used to format a string with a number of Parameters and then
    /// to load the formatted string into the value of a specified context key. This is useful when
    /// you have to build say, a connection string, with dynamic values for server, database etc
    /// and then need to store in the context for use in future steps that connect to the specified database
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="BizUnit.Extensions.dll" TypeName="BizUnit.Extensions.StringFormatterStep">
    ///	<BaseString>Update table set fld = '{0}' where fld1 = '{1}'</BaseString>
    ///	<Delimiter>|</Delimiter>
    ///	<NumParams>2</NumParams>
    ///	<ParamString>Smith|10192929239</ParamString>
    ///	<ContextKey>OutputString</ContextKey>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>BaseString</term>
    ///			<description>The base string with placeholders</description>
    ///		</item>
    ///		<item>
    ///			<term>Delimiter</term>
    ///			<description>The delimiter between the Parameters that should fit into the placeholder</description>
    ///		</item>
    ///		<item>
    ///			<term>NumParams</term>
    ///			<description>The number of Parameters</description>
    ///		</item>
    ///		<item>
    ///		<term>ParamString</term>
    ///		<description>The actual string with all the Parameters separated by the given delimiter</description>
    ///		</item>
    ///		<item>
    ///		<term>ContextKey</term>
    ///		<description>the name of the context key in which to store the output value</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class StringFormatterStep : TestStepBase
    {
        private string baseString;

        public string BaseString
        {
            get { return baseString; }
            set { baseString = value; }
        }
        private int numParams;

        public int NumParams
        {
            get { return numParams; }
            set { numParams = value; }
        }
        private string paramString;

        public string ParamString
        {
            get { return paramString; }
            set { paramString = value; }
        }
        private string contextKeyWithResult;

        public string ContextKeyWithResult
        {
            get { return contextKeyWithResult; }
            set { contextKeyWithResult = value; }
        }
        private string delimiter;

        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

        private string resultString;
        private string[] tokens;

        public override void Execute(Context context)
        {
            Validate(context);
            //setup the array of Parameters
            tokens = ParamString.Split(Delimiter.ToCharArray(), numParams);
            //now format the string
            resultString = String.Format(BaseString, tokens);

            context.LogInfo(string.Format("String Formatter created : {0}", resultString));
            context.Add(contextKeyWithResult, resultString, true);

        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(BaseString, "Base String");
            ArgumentValidation.CheckForEmptyString(NumParams.ToString(), "Num Params");
            ArgumentValidation.CheckForEmptyString(ParamString, "Parameter String");
            ArgumentValidation.CheckForEmptyString(Delimiter, "Delimiter");
            ArgumentValidation.CheckForEmptyString(ContextKeyWithResult, "Context Key");

        }
    }
}
