//---------------------------------------------------------------------
// File: XPathValidator.cs
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

using System.Xml;
using System.Xml.XPath;

namespace BizUnit.Extensions.Utilities
{
    /// <summary>
    /// A utility class which applies XPath expressions to xml files and returns the value
    /// in a variety of data types
    /// </summary>
    public class XPathValidator
    {
        /// <summary>
        /// basic constructor
        /// </summary>
        public XPathValidator()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Evaluates the XPath expression and returns a string value
        /// </summary>
        /// <param name="InputXmlFile">full path of the xml file to parse</param>
        /// <param name="XPathString">XPath expression to apply</param>
        /// <returns>string</returns>
        public string GetStringValue(string InputXmlFile, string XPathString)
        {
            string retval;
            object obj = MakeXPathExpression(InputXmlFile, XPathString);
            retval = (string)obj;
            return (retval);
        }

        /// <summary>
        /// Evaluates the XPath expression and returns a integer value
        /// </summary>
        /// <param name="InputXmlFile">full path of the xml file to parse</param>
        /// <param name="XPathString">XPath expression to apply</param>
        /// <returns>int</returns>
        public int GetIntegerValue(string InputXmlFile, string XPathString)
        {
            int retval;
            object obj = MakeXPathExpression(InputXmlFile, XPathString);
            retval = System.Convert.ToInt32(obj);
            return (retval);
        }

        /// <summary>
        /// Evaluates the XPath expression and returns a bool value
        /// </summary>
        /// <param name="InputXmlFile">full path of the xml file to parse</param>
        /// <param name="XPathString">XPath expression to apply</param>
        /// <returns>bool</returns>
        public bool GetBooleanValue(string InputXmlFile, string XPathString)
        {
            bool retval;
            object obj = MakeXPathExpression(InputXmlFile, XPathString);
            retval = System.Convert.ToBoolean(obj);
            return (retval);
        }
        private object MakeXPathExpression(string inputXmlFile, string XPathString)
        {

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(inputXmlFile);
            XPathNavigator nav = xDoc.CreateNavigator();
            XPathExpression expr = nav.Compile(XPathString);

            return (nav.Evaluate(expr));
        }
    }
}
