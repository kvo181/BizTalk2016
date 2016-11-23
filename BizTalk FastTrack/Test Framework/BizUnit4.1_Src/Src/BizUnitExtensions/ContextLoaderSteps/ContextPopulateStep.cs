//---------------------------------------------------------------------
// File: ContextPopulateStep.cs
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
    /// The ContextPopulateStep takes each key and value and adds it to the context. This is a standalone
    /// step and should not be nested (like the other ContextLoader steps)
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// In this example, the data is loaded directly into the context and doesnt need to come
    /// from an external source file. 
    /// 
    /// Each of these Context Load elements will result in the equivalent Context.Add(key,object) being executed
    /// 
    /// Currently the step supports the following data types, Int32,Int64,String (NOT case sensitive). It should be easy enough to extend this
    /// code to add support for other data types that you wish to use (or even to load regular expressions)
    /// Also note that when supplying the value in the Xml you need to enclose even integer types within quotes
    /// The system will apply the appropriate casts internally. If the system does not recognise the data type it will
    /// convert it into string by default
    /// 
    /// <code escaped="true">
    ///	<ContextLoaderStep AssemblyPath="" TypeName="BizUnit.Extensions.ContextPopulateStep">
    ///		<ContextLoad contextKey="Name" keyValue="Microsoft" dataType="String" />
    ///		<ContextLoad contextKey="Age" keyValue="25" dataType="Int32" />
    ///	</ContextLoaderStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>ContextLoad</term>
    ///			<description> The item you wish to add to the context <para>(repeating)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>ContextLoad/contextKey</term>
    ///			<description>The name of context key which will be used when adding the new context item</description>
    ///		</item>
    ///		<item>
    ///			<term>ContextLoad/keyValue</term>
    ///			<description>The value that you wish to put into the key</description>
    ///		</item>
    ///		<item>
    ///			<term>ContextLoad/dataType</term>
    ///			<description>The dataType you wish the object to be saved as</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	

    public class ContextPopulateStep : TestStepBase
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
        private string dataType;

        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        #region ITestStepOM Members

        public override void Execute(Context context)
        {
            context.LogInfo("ContextPopulator loading key:{0} with value:\"{1}\"", contextKey, keyValue);
            switch (dataType.ToUpper())
            {
                case "INT32":
                    context.Add(contextKey, Convert.ToInt32(keyValue));
                    break;

                case "INT64":
                    context.Add(contextKey, Convert.ToInt64(keyValue));
                    break;

                case "STRING":
                    context.Add(contextKey, Convert.ToString(keyValue));
                    break;

                default:
                    context.LogWarning("cannot find the data type.Applying default cast to string");
                    context.Add(contextKey, Convert.ToString(keyValue));
                    break;
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(contextKey, "ContextKey");
            ArgumentValidation.CheckForEmptyString(keyValue, "KeyValue");
            ArgumentValidation.CheckForEmptyString(dataType, "DataType");
        }

        #endregion
    }
}
