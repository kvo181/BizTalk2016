using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.BizTalk.TestTools.Schema;
using System.Diagnostics;
using Microsoft.XLANGs.BaseTypes;

namespace Bizilante.PipelineTesting.Helper
{
    public class XmlValidationHelper
    {
        private static bool _IsValid = true;
        private static string validationMessage = string.Empty;

        public static ValidationResult ValidateSchema(TestableSchemaBase schemaObject, string xmlInstancePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaObject.Schema);
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaReaderSettingsValidationEventHandler);

            using (XmlReader reader = XmlReader.Create(xmlInstancePath, settings))
            {
                _IsValid = true;
                while (reader.Read())
                {
                    if (!_IsValid) break;
                }
            }

            return new ValidationResult() { IsValid = _IsValid, ValidationMessage = validationMessage };
        }
        public static ValidationResult ValidateSchema(SchemaBase schemaObject, Stream xmlInstanceStream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaObject.Schema);
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaReaderSettingsValidationEventHandler);

            using (XmlReader reader = XmlReader.Create(xmlInstanceStream, settings))
            {
                _IsValid = true;
                while (reader.Read())
                {
                    //if (!_IsValid) break;
                }
            }

            return new ValidationResult() { IsValid = _IsValid, ValidationMessage = validationMessage };
        }

        public static void SchemaReaderSettingsValidationEventHandler(object sender, ValidationEventArgs arguments)
        {
            if (arguments.Severity == XmlSeverityType.Error)
            {
                _IsValid = false;
                validationMessage = string.IsNullOrEmpty(validationMessage) ? arguments.Message :
                    Environment.NewLine + arguments.Message;
                Trace.WriteLine(arguments.Message);
            }
        }
    }
    /// <summary> 
    /// Class for returning the validation results. 
    /// Return true if the schema is valid else false. 
    /// </summary> 
    public class ValidationResult
    {
        /// <summary> 
        /// Return true if the schema is valid else false. 
        /// </summary> 
        public bool IsValid { get; set; }
        /// <summary> 
        /// Returns the error message if the schema validation is failed. 
        /// </summary> 
        public string ValidationMessage { get; set; }
    }
}
