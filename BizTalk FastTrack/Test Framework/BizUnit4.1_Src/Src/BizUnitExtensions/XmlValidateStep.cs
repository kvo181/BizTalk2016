//---------------------------------------------------------------------
// File: XmlValidateStep
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

using BizUnit.Extensions.Utilities;
using BizUnit.Xaml;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace BizUnit.Extensions
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    /// <TestStep AssemblyPath="BizUnit.Extensions.dll" TypeName="BizUnit.Extensions.XmlValidateStep">
    /// <InputFileName></InputFileName>
    /// <SchemaFiles>
    ///		<SchemaFile namespaceAlias="ns1" takeFromCtx="CINSPersonCreated"></SchemaFile>
    ///     <SchemaFile namespaceAlias="ns2" takeFromCtx="PersonNameDescriptives"></SchemaFiles>
    /// </SchemaFiles>
    /// <ValidationStep AssemblyPath="" TypeName="BizUnit.ContextValidationStep">
    ///		<Context keyName="XmlValidateStepErrorCount">0</Context>
    ///	</ValidationStep>
    /// </TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>InputFileName</term>
    ///			<description>full path of the xml file to validate</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    public class XmlValidateStep : TestStepBase
    {
        private XmlNode schemaFilesNode;
        private XmlNodeList schemaFilesNodeList;
        private XmlNode testConfig;
        private Context context;

        private string inputFileName;
        private XmlSchemaSet schemaSet;

        public string InputFileName
        {
            get { return inputFileName; }
            set { inputFileName = value; }
        }
        public XmlSchemaSet SchemaSet
        {
            get
            {
                return schemaSet;
            }
            set
            {
                schemaSet = value;
            }
        }

        #region ITestStepOM Members

        public override void Execute(Context context)
        {
            InitializeSchemaSet();

            ArrayList arr = new ArrayList();
            // Load XML document from input file
            try
            {
                SchemaValidator sv = new SchemaValidator(SchemaSet);
                arr = sv.ValidateIt(InputFileName);
                context.Add("XmlValidateStepErrorCount", arr.Count.ToString(), true);
                if (arr.Count > 0)
                {
                    foreach (string valerr in arr)
                    {
                        context.LogInfo(valerr);
                    }
                }

            }
            //catch any exceptions from the try block
            catch (Exception ex)
            {
                context.LogException(ex);
            }
        }

        public override void Validate(Context context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region "private methods"
        private void InitializeSchemaSet()
        {
            SchemaSet = new XmlSchemaSet();
            string filepathstr = ""; //holds the raw file path with no reference to the context (for wildcard and key substitution)
            string schemafilepath = ""; //holds the file path passed through the context
            string nsstr = ""; //holds the raw namespace with no reference to the context
            string schemans = "";  //holds the namespace passed through the context
            int rowIndex = 0;
            schemaFilesNodeList = schemaFilesNode.SelectNodes("./SchemaFile");

            //Since the schema files is a repeating element the apporoach taken here is to get the nodes by using
            //xpath expressions with indexes of the form
            ///*[local-name()='SchemaList']/*[local-name()='SchemaFile' and position() = 1]


            foreach (XmlNode schemaFileNode in schemaFilesNodeList)
            {
                //Get the file path string from the inner text of the node  (if available)
                filepathstr = string.Format("./*[local-name()='SchemaList']/*[local-name()='SchemaFile' and position() = {0}]", rowIndex + 1);
                schemafilepath = context.ReadConfigAsString(testConfig, filepathstr);

                nsstr = string.Format("./*[local-name()='SchemaList']/*[local-name()='SchemaFile' and position() = {0}]/@namespace", rowIndex + 1);
                schemans = context.ReadConfigAsString(testConfig, nsstr);

                //add these two elements to the enty in the schema set
                SchemaSet.Add(schemans, schemafilepath);

            }
        }

        #endregion
    }
}
