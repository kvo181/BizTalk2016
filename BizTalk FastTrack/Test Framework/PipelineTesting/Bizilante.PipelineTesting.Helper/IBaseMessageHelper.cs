using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bizilante.PipelineTesting.Helper
{
    public class IBaseMessageHelper
    {
        public static void ShowMessageContext(IBaseMessage message, TestContext testContext)
        {
            testContext.WriteLine("-------------------");
            for (int i = 0; i < message.Context.CountProperties; i++)
            {
                string strName;
                string strNamespace;
                var value = message.Context.ReadAt(i, out strName, out strNamespace);
                testContext.WriteLine("{1} - {0} = {2}", strName, strNamespace, value);
            }
            testContext.WriteLine("-------------------");
        }
        public static void ShowMessage(IBaseMessage message, TestContext testContext, bool asXml = true)
        {
            testContext.WriteLine("-------------------");
            for (int i = 0; i < message.Context.CountProperties; i++)
            {
                string strName;
                string strNamespace;
                var value = message.Context.ReadAt(i, out strName, out strNamespace);
                testContext.WriteLine("{1} - {0} = {2}", strName, strNamespace, value);
            }
            testContext.WriteLine("-------------------");
            if (asXml)
            {
                // Show the intermediate result
                var reader = XmlReader.Create(message.BodyPart.Data);
                {
                    using (StringWriter writer = new StringWriter())
                    {
                        XmlTextWriter xmlWriter = new XmlTextWriter(writer)
                        {
                            //set formatting options
                            Formatting = Formatting.Indented,
                            Indentation = 1,
                            IndentChar = '\t'
                        };
                        //write the document formatted
                        xmlWriter.WriteNode(reader, true);
                        testContext.WriteLine(writer.ToString());
                    }
                    testContext.WriteLine("-------------------");
                }
            }
            else
            {
                var reader = new StreamReader(message.BodyPart.Data);
                testContext.WriteLine(reader.ReadToEnd());
                testContext.WriteLine("-------------------");
            }
        }
    }
}
