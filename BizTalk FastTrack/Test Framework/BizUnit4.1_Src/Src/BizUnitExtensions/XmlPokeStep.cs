//---------------------------------------------------------------------
// File: XmlPokeStep.cs
// 
// Summary: Provides Nant style XmlPoke functionality
//
// Copyright (c) http://bizunitextensions.codeplex.com. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml;
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.Extensions
{
    /// <summary>
    /// This step is modelled on the lines of the NAnt XmlPoke task
    /// The XmlPokeStep is used to update data in an XML file with values from the context
    /// This will enable the user to write tests which can use the output of one step to modify the input of another step
    /// </summary>
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="C:\Projects\BizUnit.Extensions.dll" TypeName="BizUnit.Extensions.XmlPokeStep">
    ///		<InputFileName>Name of input file</InputFileName>
    ///		<XPathExpressions>
    ///			<Expression>
    ///				<XPath></XPath>
    ///				<NewValue></NewValue>
    ///			</Expression>
    ///			<Expression>
    ///				<XPath></XPath>
    ///				<NewValue takeFromCtx="somekey"></NewValue>
    ///			</Expression>
    ///		</XPathExpressions>
    ///	</TestStep>
    ///	</code>

    public class XmlPokeStep : TestStepBase
    {
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private List<string> expressions = new List<string>();

        /// <summary>
        /// A list of formatted strings containing the XPath expression and the new value
        /// The XPath expression and the new value for the node are separated by a || token
        /// For example "/Customer/Name||Microsoft"
        /// </summary>
        public List<string> Expressions
        {
            get { return expressions; }
            set { expressions = value; }
        }

        private void PokeXmlDocument(Context context)
        {
            string xPathExpr;
            string newValue;
            string[] tokens;
            try
            {
                // Load XML document from input file
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(fileName);

                foreach (string expression in expressions)
                {
                    //Split the expression into its constituent parts
                    tokens = expression.Split("||".ToCharArray());
                    xPathExpr = tokens[0];
                    newValue = tokens[2]; //tokens[1] is a blank string

                    //check that they have been set up properly
                    ArgumentValidation.CheckForEmptyString(xPathExpr, "XPath Expression");
                    ArgumentValidation.CheckForEmptyString(newValue, "New Value");

                    //check what the old value was and log it
                    context.LogInfo("Old value is " + xDoc.SelectSingleNode(xPathExpr).InnerXml);
                    context.LogInfo("New value is " + newValue);
                    //now set the new value into the node corresponding to the xPath expression
                    xDoc.SelectSingleNode(xPathExpr).InnerXml = newValue;
                }

                //Save the Xml document after updating the nodes with new values
                xDoc.Save(fileName);


            }
            catch (XmlException ex)
            {
                context.LogException(ex);
            }

        }

        public override void Execute(Context context)
        {
            Validate(context);
            PokeXmlDocument(context);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(fileName, "FileName");
            ArgumentValidation.CheckForNullReference(Expressions, "Expressions");
        }

    }
}
