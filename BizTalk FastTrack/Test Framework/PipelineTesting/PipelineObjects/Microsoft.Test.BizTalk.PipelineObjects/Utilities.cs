namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.Win32;
    using Microsoft.XLANGs.RuntimeTypes;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    public sealed class Utilities
    {
        private const string BtsNamespace = "http://schemas.microsoft.com/BizTalk/2003";
        private const string ErrorInformationFormat = "Error\r\n\tSource:\t\t{0}\r\n\tMessage:\t{1}\r\n\tHRESULT:\t{2}";
        private const string MessagePartNameFormat = "part{0}";
        private static PromotingMap promotingMap = new PromotingMap();
        private const string XsdNamespace = "http://www.w3.org/2001/XMLSchema";

        private Utilities()
        {
        }

        public static IBaseMessage CreateMessage(IBaseMessageFactory factory, string fileName)
        {
            IBaseMessage message = factory.CreateMessage();
            message.Context = factory.CreateMessageContext();
            IBaseMessagePart part = factory.CreateMessagePart();
            part.Data = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            message.AddPart("body", part, true);
            return message;
        }

        public static IBaseMessage CreateMessage(IBaseMessageFactory factory, string fileName, StringCollection parts)
        {
            IBaseMessage message = CreateMessage(factory, fileName);
            for (int i = 0; i < parts.Count; i++)
            {
                IBaseMessagePart part = factory.CreateMessagePart();
                part.Data = new FileStream(parts[i], FileMode.Open, FileAccess.Read);
                message.AddPart(string.Format(CultureInfo.CurrentCulture, "part{0}", new object[] { i }), part, false);
            }
            return message;
        }

        public static void DumpMessageContextProperties(MessageContext context)
        {
            foreach (DictionaryEntry entry in context)
            {
                string propertyName = entry.Key.ToString().Split(new char[] { '@' })[0];
                string propertyNamespace = entry.Key.ToString().Split(new char[] { '@' })[1];
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Name = '{0}' namespace = '{1}' value = '{2}' promoted = '{3}' type = '{4}'", new object[] { propertyName, propertyNamespace, (entry.Value != null) ? entry.Value : "<Not set>", context.IsPromoted(propertyName, propertyNamespace) ? "Yes" : "No", (entry.Value != null) ? entry.Value.GetType().Name : "N/A" }));
            }
        }

        public static void ExpandFileMask(string fileMask, StringCollection collection)
        {
            string directoryName = Path.GetDirectoryName(fileMask);
            if (directoryName.Length == 0)
            {
                directoryName = ".";
            }
            DirectoryInfo info = new DirectoryInfo(directoryName);
            foreach (FileInfo info2 in info.GetFiles(Path.GetFileName(fileMask)))
            {
                collection.Add(info2.FullName);
            }
        }

        public static string GetBodyXPath(XmlDocument xmlDocument)
        {
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            nsmgr.AddNamespace("b", "http://schemas.microsoft.com/BizTalk/2003");
            XmlNode node = xmlDocument.SelectSingleNode("/xs:schema/xs:element/xs:annotation/xs:appinfo/b:recordInfo/@*[local-name() = 'body_xpath']", nsmgr);
            if (node != null)
            {
                return node.InnerText;
            }
            return "";
        }

        public static string GetDocumentType(XmlDocument xmlDocument)
        {
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            nsmgr.AddNamespace("b", "http://schemas.microsoft.com/BizTalk/2003");
            string xpath = string.Format(CultureInfo.InvariantCulture, "/xs:schema/xs:annotation/xs:appinfo/*[(local-name() = 'schemaInfo' or local-name() = 'SchemaInfo') and namespace-uri() = '{0}']/@*[local-name() = 'root_reference']", new object[] { "http://schemas.microsoft.com/BizTalk/2003" });
            XmlNode node = xmlDocument.SelectSingleNode(xpath, nsmgr);
            if (node == null)
            {
                node = xmlDocument.SelectSingleNode("/xs:schema/xs:element/@name", nsmgr);
            }
            if (node == null)
            {
                throw new ApplicationException("Document root reference cannot be determined");
            }
            string innerText = node.InnerText;
            string targetNamespace = GetTargetNamespace(xmlDocument);
            if (targetNamespace.Length <= 0)
            {
                return innerText;
            }
            return (targetNamespace + "#" + innerText);
        }

        public static Encoding GetEncodingFromString(string encodingOrCodePage)
        {
            try
            {
                return Encoding.GetEncoding(int.Parse(encodingOrCodePage, CultureInfo.CurrentCulture));
            }
            catch (FormatException)
            {
                return Encoding.GetEncoding(encodingOrCodePage);
            }
        }

        public static string GetEncodingOrCodePageFromSchema(string schemaFileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(schemaFileName);
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            nsmgr.AddNamespace("b", "http://schemas.microsoft.com/BizTalk/2003");
            string xpath = string.Format(CultureInfo.InvariantCulture, "/xs:schema/xs:annotation/xs:appinfo/*[(local-name() = 'schemaInfo' or local-name() = 'SchemaInfo') and namespace-uri() = '{0}']/@*[local-name() = 'codepage']", new object[] { "http://schemas.microsoft.com/BizTalk/2003" });
            XmlNode node = document.SelectSingleNode(xpath, nsmgr);
            if (node != null)
            {
                return node.InnerText;
            }
            return null;
        }

        public static string GetErrorMessage(Exception e)
        {
            string source = e.Source;
            if ((source == null) || (source.Length == 0))
            {
                source = "<no source is available>";
            }
            string message = e.Message;
            string str3 = null;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = Microsoft.Test.BizTalk.PipelineObjects.NativeMethods.LoadLibraryEx(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0\NTService").GetValue("EventMessageFile").ToString(), IntPtr.Zero, 2);
                if (zero != IntPtr.Zero)
                {
                    IntPtr[] arguments = null;
                    IntPtr[] ptrArray2 = null;
                    if (e is BTSException)
                    {
                        BTSException exception = (BTSException) e;
                        if (exception.ArgumentCount > 0)
                        {
                            arguments = new IntPtr[exception.ArgumentCount];
                            ptrArray2 = new IntPtr[exception.ArgumentCount];
                            int index = 0;
                            try
                            {
                                index = 0;
                                while (index < exception.ArgumentCount)
                                {
                                    arguments[index] = Marshal.StringToHGlobalAuto(exception.GetArgument(index));
                                    ptrArray2[index] = arguments[index];
                                    index++;
                                }
                            }
                            catch
                            {
                                index--;
                                while (index >= 0)
                                {
                                    Marshal.FreeCoTaskMem(arguments[index]);
                                    index--;
                                }
                                throw;
                            }
                        }
                    }
                    try
                    {
                        StringBuilder lpBuffer = new StringBuilder(0x2800);
                        int num2 = 0x7a;
                        int num3 = 0;
                        while ((num3 == 0) && (num2 == 0x7a))
                        {
                            num3 = Microsoft.Test.BizTalk.PipelineObjects.NativeMethods.FormatMessage(0x1800, new HandleRef(null, zero), Marshal.GetHRForException(e), 0, lpBuffer, lpBuffer.Capacity, ref arguments);
                            if (num3 == 0)
                            {
                                num2 = Marshal.GetLastWin32Error();
                                if (num2 == 0x7a)
                                {
                                    lpBuffer.Capacity *= 2;
                                }
                            }
                        }
                        if (num3 > 0)
                        {
                            str3 = lpBuffer.ToString();
                        }
                    }
                    finally
                    {
                        if (arguments != null)
                        {
                            for (int i = 0; i < ptrArray2.Length; i++)
                            {
                                Marshal.FreeCoTaskMem(ptrArray2[i]);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Microsoft.Test.BizTalk.PipelineObjects.NativeMethods.FreeLibrary(zero);
                }
            }
            if (str3 == null)
            {
                str3 = message;
            }
            if ((str3 == null) || (str3.Length == 0))
            {
                str3 = "<no message is available>";
            }
            return string.Format(CultureInfo.CurrentCulture, "Error\r\n\tSource:\t\t{0}\r\n\tMessage:\t{1}\r\n\tHRESULT:\t{2}", new object[] { source, str3, Marshal.GetHRForException(e).ToString("x", CultureInfo.InvariantCulture) });
        }

        private static object GetPropertyValue(IPropertyAnnotation annotation)
        {
            if (((string.Equals(annotation.XSDType, "date", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "time", StringComparison.OrdinalIgnoreCase)) || (string.Equals(annotation.XSDType, "dateTime", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "gMonthDay", StringComparison.OrdinalIgnoreCase))) || ((string.Equals(annotation.XSDType, "gDay", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "gYear", StringComparison.OrdinalIgnoreCase)) || (string.Equals(annotation.XSDType, "gYearMonth", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "gMonth", StringComparison.OrdinalIgnoreCase))))
            {
                return DateTime.Now;
            }
            if (((string.Equals(annotation.XSDType, "integer", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "byte", StringComparison.OrdinalIgnoreCase)) || (string.Equals(annotation.XSDType, "unsignedByte", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "positiveInteger", StringComparison.OrdinalIgnoreCase))) || (((string.Equals(annotation.XSDType, "nonNegativeInteger", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "int", StringComparison.OrdinalIgnoreCase)) || (string.Equals(annotation.XSDType, "unsignedInt", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "long", StringComparison.OrdinalIgnoreCase))) || ((string.Equals(annotation.XSDType, "unsignedLong", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "short", StringComparison.OrdinalIgnoreCase)) || string.Equals(annotation.XSDType, "unsignedShort", StringComparison.OrdinalIgnoreCase))))
            {
                return 1;
            }
            if ((string.Equals(annotation.XSDType, "decimal", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "float", StringComparison.OrdinalIgnoreCase)) || string.Equals(annotation.XSDType, "double", StringComparison.OrdinalIgnoreCase))
            {
                return 1.0;
            }
            if (string.Equals(annotation.XSDType, "boolean", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (string.Equals(annotation.XSDType, "base64Binary", StringComparison.OrdinalIgnoreCase))
            {
                return "GpM7";
            }
            if (string.Equals(annotation.XSDType, "negativeInteger", StringComparison.OrdinalIgnoreCase) || string.Equals(annotation.XSDType, "nonPositiveInteger", StringComparison.OrdinalIgnoreCase))
            {
                return -1;
            }
            if (string.Equals(annotation.XSDType, "hexBinary", StringComparison.OrdinalIgnoreCase))
            {
                return "0FEE";
            }
            if (string.Equals(annotation.XSDType, "duration", StringComparison.OrdinalIgnoreCase))
            {
                return TimeSpan.FromSeconds(1.0);
            }
            if (string.Equals(annotation.XSDType, "language", StringComparison.OrdinalIgnoreCase))
            {
                return "en-US";
            }
            if (string.Equals(annotation.XSDType, "anyURI", StringComparison.OrdinalIgnoreCase))
            {
                return "http://www.example.com";
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { annotation.Name, annotation.XSDType });
        }

        public static string GetTargetNamespace(XmlDocument xmlDocument)
        {
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            XmlNode node = xmlDocument.SelectSingleNode("/xs:schema/@targetNamespace", nsmgr);
            if (node != null)
            {
                return node.InnerText;
            }
            return "";
        }

        public static IInitializeDocumentSpec InitializeDocumentSpec(IDocumentSpec documentSpec, string documentSpecName, string schemaFileName, ArrayList propertyAnnotations)
        {
            IInitializeDocumentSpec spec = (IInitializeDocumentSpec) documentSpec;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(new XmlTextReader(schemaFileName));
            string documentType = GetDocumentType(xmlDocument);
            string targetNamespace = GetTargetNamespace(xmlDocument);
            string bodyXPath = GetBodyXPath(xmlDocument);
            Hashtable schemaList = new Hashtable();
            schemaList.Add(targetNamespace, new FileStream(schemaFileName, FileMode.Open, FileAccess.Read));
            spec.Initialize(targetNamespace, documentSpecName, documentType, bodyXPath, schemaList);
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            nsmgr.AddNamespace("b", "http://schemas.microsoft.com/BizTalk/2003");
            Hashtable hashtable2 = new Hashtable();
            Hashtable hashtable3 = new Hashtable();
            foreach (XmlNode node in xmlDocument.SelectNodes("/xs:schema/xs:annotation/xs:appinfo/b:imports/b:namespace", nsmgr))
            {
                hashtable2.Add(node.Attributes["prefix"].InnerText, node.Attributes["uri"].InnerText);
                string innerText = node.Attributes["location"].InnerText;
                if (new FileInfo(innerText).Exists)
                {
                    XmlDocument document2 = new XmlDocument();
                    document2.Load(innerText);
                    hashtable3.Add(node.Attributes["uri"].InnerText, document2);
                }
            }
            foreach (XmlNode node2 in xmlDocument.SelectNodes("/xs:schema//xs:annotation/xs:appinfo/b:properties/b:property[not(@distinguished='true')]", nsmgr))
            {
                PropertyAnnotation annotation = new PropertyAnnotation();
                string str5 = node2.Attributes["name"].InnerText;
                annotation.Name = str5.Split(new char[] { ':' })[1];
                annotation.Namespace = (string) hashtable2[str5.Split(new char[] { ':' })[0]];
                annotation.XPath = node2.Attributes["xpath"].InnerText;
                annotation.XSDType = null;
                XmlDocument document3 = (XmlDocument) hashtable3[annotation.Namespace];
                if (document3 != null)
                {
                    XmlNode node3 = document3.SelectSingleNode(string.Format(CultureInfo.CurrentCulture, "/xs:schema/xs:element[@name='{0}']", new object[] { annotation.Name }), nsmgr);
                    if (node3 != null)
                    {
                        XmlNode node4 = node3.Attributes["type"];
                        if (node4 != null)
                        {
                            string[] strArray = node4.InnerText.Split(new char[] { ':' });
                            if (2 == strArray.Length)
                            {
                                annotation.XSDType = strArray[1];
                            }
                            else if (1 == strArray.Length)
                            {
                                annotation.XSDType = strArray[0];
                            }
                        }
                    }
                }
                if (annotation.XSDType == null)
                {
                    annotation.XSDType = "string";
                }
                if (propertyAnnotations != null)
                {
                    propertyAnnotations.Add(annotation);
                }
                spec.AddAnnotation(annotation);
            }
            return spec;
        }

        public static void ProcessPropertyPromotionFile(IBaseMessage message, string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            foreach (XmlNode node in document.SelectNodes("/properties/property"))
            {
                if (node.Attributes["name"] == null)
                {
                    throw new ApplicationException("Attribute 'name' is missing in a property promotion file.");
                }
                string innerText = node.Attributes["name"].InnerText;
                string strNameSpace = null;
                if (node.Attributes["namespaceURI"] != null)
                {
                    strNameSpace = node.Attributes["namespaceURI"].InnerText;
                }
                if (node.Attributes["value"] == null)
                {
                    throw new ApplicationException("Attribute 'value' is missing in a property promotion file.");
                }
                string str3 = node.Attributes["value"].InnerText;
                string xSDType = "string";
                if (node.Attributes["type"] != null)
                {
                    xSDType = node.Attributes["type"].InnerText;
                }
                string strA = "promote";
                if (node.Attributes["method"] != null)
                {
                    strA = node.Attributes["method"].InnerText;
                }
                object obj2 = promotingMap.MapValue(str3, xSDType);
                if (string.Compare(strA, "promote", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (string.Compare(strA, "write", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "Not supported property promotion method is specified: '{0}'.", new object[] { strA }));
                    }
                    message.Context.Write(innerText, strNameSpace, obj2);
                }
                else
                {
                    message.Context.Promote(innerText, strNameSpace, obj2);
                    continue;
                }
            }
        }

        public static void PromoteProperties(IBaseMessage message, ArrayList propertyAnnotations)
        {
            foreach (IPropertyAnnotation annotation in propertyAnnotations)
            {
                object propertyValue = GetPropertyValue(annotation);
                if ((propertyValue != null) && (propertyValue is TimeSpan))
                {
                    propertyValue = propertyValue.ToString();
                }
                message.Context.Promote(annotation.Name, annotation.Namespace, propertyValue);
            }
        }
    }
}

