using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using PTL = Winterdom.BizTalk.PipelineTesting;

namespace Bizilante.PipelineTesting.Helper
{
    /// <summary>
    /// Helper class for messages
    /// </summary>
    public class MessageHelper
    {
        /// <summary>
        /// Saves a message to disk
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void SaveMessage(string path, Stream data)
        {
            char[] buffer = new char[4096];
            int read;
            data.Seek(0, SeekOrigin.Begin);
            StreamReader rdr = new StreamReader(data);
            StreamWriter writer = new StreamWriter(path, false);
            while ((read = rdr.Read(buffer, 0, buffer.Length)) > 0)
                writer.Write(buffer, 0, read);
            writer.Flush();
            data.Seek(0, SeekOrigin.Begin);
            writer.Close();
        }
        /// <summary>
        /// Loads a message from disk
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Stream LoadMessage(string path)
        {
            StreamReader rdr = new StreamReader(path);
            return rdr.BaseStream;
        }
        /// <summary>
        /// Show the stream content
        /// (We assume the content is XML)
        /// </summary>
        /// <param name="data"></param>
        public static void ShowMessage(Stream data, TestContext testContext)
        {
            XmlReader reader = XmlReader.Create(data);
            {
                testContext.WriteLine("-------------------");
                var stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineHandling = NewLineHandling.Entitize;
                settings.NewLineOnAttributes = true;
                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    //write the document formatted
                    while (reader.Read())
                        WriteShallowNode(reader, xmlWriter);
                }
                //xmlWriter.WriteNode(reader, true);
                testContext.WriteLine(stringBuilder.ToString());
                testContext.WriteLine("");
                testContext.WriteLine("-------------------");
            }
            data.Seek(0, SeekOrigin.Begin);
        }

        public static void WriteShallowNode(XmlReader reader, XmlWriter writer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, true);
                    if (reader.IsEmptyElement)
                    {
                        writer.WriteEndElement();
                    }
                    break;
                case XmlNodeType.Text:
                    writer.WriteString(reader.Value);
                    break;
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    writer.WriteWhitespace(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    writer.WriteCData(reader.Value);
                    break;
                case XmlNodeType.EntityReference:
                    writer.WriteEntityRef(reader.Name);
                    break;
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.ProcessingInstruction:
                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                case XmlNodeType.DocumentType:
                    writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    break;
                case XmlNodeType.Comment:
                    writer.WriteComment(reader.Value);
                    break;
                case XmlNodeType.EndElement:
                    writer.WriteFullEndElement();
                    break;
            }
        }

        public static Microsoft.BizTalk.Message.Interop.IBaseMessage Test_FF_Assembler_Pipeline<T1>(Microsoft.BizTalk.Message.Interop.IBaseMessage iBaseMsg)
        {
            Microsoft.BizTalk.Component.Interop.IBaseComponent comp = PTL.Simple.Assembler
                .FlatFile()
                .WithDocumentSpec<T1>().End();

            PTL.SendPipelineWrapper pipeline = PTL.Simple.Pipelines.Send()
                .WithAssembler(comp)
                .WithSpec<T1>();
            // Execute the send pipeline
            Microsoft.BizTalk.Message.Interop.IBaseMessage outputMessage = pipeline.Execute(iBaseMsg);

            return outputMessage;
        }

        public static PTL.MessageCollection Test_FF_Disassembler_Pipeline<T1>(Microsoft.BizTalk.Message.Interop.IBaseMessage iBaseMsg)
        {
            Microsoft.BizTalk.Component.Interop.IBaseComponent comp = PTL.Simple.Disassembler
                .FlatFile().WithValidation(false)
                .WithDocumentSpec<T1>().End();

            PTL.ReceivePipelineWrapper pipeline = PTL.Simple.Pipelines.Receive()
                .WithDisassembler(comp)
                .WithSpec<T1>();

            // Execute the send pipeline
            PTL.MessageCollection outputMessage = pipeline.Execute(iBaseMsg);

            return outputMessage;
        }
    }
}
