// XmlValidtation (For .NET 2.0)

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace BizUnit.Extensions.Utilities
{
    /// <summary>
    /// SchemaValidator DotNet 2.0
    /// This class validates an XmlFile or XmlStream or XmlDocument against either a specified XsdFile 
    /// or a collection of Schemas and returns the validation results in an arraylist
    /// </summary>
    public class SchemaValidator
    {
        #region Private members
        private string xsdFileName;
        private XmlReaderSettings settings;
        private XmlSchemaSet schemaSet;
        private XmlReader Reader;
        private ArrayList ValidationResults = new ArrayList();
        #endregion
        #region "Properties"
        public bool IsValidFile
        {
            get
            {
                if (this.ValidationResults.Count == 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion
        #region constructor
        public SchemaValidator(string XsdFileName)
        {
            xsdFileName = XsdFileName;
            settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += ValidatingReader_ValidationEventHandler;
            schemaSet = new XmlSchemaSet();
            schemaSet.Add("", XsdFileName);
            settings.Schemas = schemaSet;
            Reader = XmlReader.Create(xsdFileName, settings);


        }

        public SchemaValidator(XmlSchemaSet schemaSet)
        {
            settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += ValidatingReader_ValidationEventHandler;
            settings.Schemas = schemaSet;


        }




        #endregion
        #region Public methods
        /// <summary>
        /// This public method tries to return all errors
        /// of validating the XML file against XSD Schema.
        /// </summary>
        /// <param name="XmlFileName">The physical path of XML file.</param>
        /// <returns>An ArrayList of string values of all errors</returns>
        public ArrayList ValidateIt(string XmlFileName)
        {
            Reader = XmlReader.Create(XmlFileName, settings);
            PerformValidation();
            return this.ValidationResults;
        }


        public ArrayList ValidateIt(Stream XmlStream)
        {
            Reader = XmlReader.Create(XmlStream, settings);
            PerformValidation();
            return this.ValidationResults;
        }
        #endregion
        #region private methods
        private void PerformValidation()
        {
            try
            {


                while (Reader.Read())
                {
                    /*Empty loop*/
                }

            }// try
            //Handle exceptions if you want
            catch (UnauthorizedAccessException AccessEx)
            {
                throw AccessEx;
            }// catch
            catch (Exception Ex)
            {
                throw Ex;
            }// catch
        }// PerformValidation
        /// <summary>
        /// This handler simply adds all erros with their line and
        /// position number to the result.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="args">ValidationEventArgs</param>
        private void ValidatingReader_ValidationEventHandler(object sender,
            ValidationEventArgs args)
        {

            string strTemp;
            strTemp = "Line: " + args.Exception.LineNumber + " - Position: " +
                args.Exception.LinePosition + " - " + args.Message;

            this.ValidationResults.Add(strTemp);
        }// ValidatingReader_ValidationEventHandler
        #endregion


    }
}





