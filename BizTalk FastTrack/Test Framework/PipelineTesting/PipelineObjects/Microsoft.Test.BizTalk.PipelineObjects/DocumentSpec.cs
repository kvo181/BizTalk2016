namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.ParsingEngine;
    using Microsoft.BizTalk.SerializingEngine;
    using Microsoft.XLANGs.RuntimeTypes;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    public class DocumentSpec : IFFDocumentSpec, IDocumentSpec, IInitializeDocumentSpec
    {
        private ArrayList annotationList = new ArrayList();
        private string bodyXPath;
        private string docSpecName;
        private string docType;
        private DocumentSchema documentSchema;
        private string rootTargetNamespace;
        private XmlSchemaCollection schemaCollection;
        private Hashtable schemaList;
        private object thisLock = new object();

        public void AddAnnotation(IPropertyAnnotation annotation)
        {
            this.annotationList.Add(annotation);
        }

        private void CreateDocSchema()
        {
            if (this.schemaCollection == null)
            {
                throw new ArgumentNullException("schemaCollection");
            }
            string docType = this.DocType;
            if (docType == null)
            {
                throw new ApplicationException("DocumentSpec does not contain a doctype.");
            }
            string[] strArray = docType.Split(new char[] { '#' });
            if (strArray.Length == 1)
            {
                this.documentSchema = new DocumentSchema(this.schemaCollection, "", new XmlQualifiedName(strArray[0]));
            }
            else
            {
                if (strArray.Length != 2)
                {
                    throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "Invalid doctype format: {0}", new object[] { docType }));
                }
                this.documentSchema = new DocumentSchema(this.schemaCollection, strArray[0], new XmlQualifiedName(strArray[1], strArray[0]));
            }
        }

        private void CreateSchemaCollection()
        {
            this.schemaCollection = new XmlSchemaCollection();
            foreach (Stream stream in this.schemaList.Values)
            {
                stream.Position = 0L;
                XmlSchema schema = XmlSchema.Read(stream, new ValidationEventHandler(this.ValidationHandler));
                if (schema == null)
                {
                    throw new ApplicationException("Cannot read from schema");
                }
                this.schemaCollection.Add(schema);
            }
        }

        public Stream CreateXmlInstance(TextWriter tw)
        {
            this.GetDocSchema();
            return this.documentSchema.CreateXmlInstance();
        }

        public string GetBodyPath()
        {
            return this.bodyXPath;
        }

        public IDictionaryEnumerator GetDistinguishedPropertyAnnotationEnumerator()
        {
            return null;
        }

        public DocumentSchema GetDocSchema()
        {
            this.GetSchemaCollection();
            if (this.documentSchema == null)
            {
                lock (this.thisLock)
                {
                    if (this.documentSchema == null)
                    {
                        this.CreateDocSchema();
                    }
                }
            }
            if (this.documentSchema == null)
            {
                throw new ApplicationException("Cannot create DocSchema!");
            }
            return this.documentSchema;
        }

        public IEnumerator GetPropertyAnnotationEnumerator()
        {
            return this.annotationList.GetEnumerator();
        }

        public XmlSchemaCollection GetSchemaCollection()
        {
            if (this.schemaCollection == null)
            {
                lock (this.thisLock)
                {
                    if (this.schemaCollection == null)
                    {
                        this.CreateSchemaCollection();
                    }
                }
            }
            if (this.schemaCollection == null)
            {
                throw new ApplicationException("Cannot build schema collection!");
            }
            return this.schemaCollection;
        }

        public void Initialize(string rootTargetNamespace, string docSpecName, string docType, string bodyXPath, Hashtable schemaList)
        {
            this.rootTargetNamespace = rootTargetNamespace;
            this.docSpecName = docSpecName;
            this.docType = docType;
            this.bodyXPath = bodyXPath;
            this.schemaList = schemaList;
        }

        public XmlReader Parse(IDataReader dr)
        {
            this.GetDocSchema();
            return new FFReader(this.documentSchema, dr);
        }

        public bool Probe(IDataReader dr)
        {
            return this.GetDocSchema().Probe(dr);
        }

        public Stream Serialize(XmlReader inputReader, Encoding encoding)
        {
            this.GetDocSchema();
            return new FFStream(this.documentSchema, inputReader, encoding);
        }

        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            Console.WriteLine("Validation failed: {0}", args);
        }

        public string BodyXPath
        {
            get
            {
                return this.bodyXPath;
            }
        }

        public string DocSpecName
        {
            get
            {
                return this.docSpecName;
            }
        }

        public string DocSpecStrongName
        {
            get
            {
                return this.docSpecName;
            }
        }

        public string DocType
        {
            get
            {
                return this.docType;
            }
        }

        public string DocumentSpecName
        {
            get
            {
                return this.docSpecName;
            }
        }

        public string DocumentType
        {
            get
            {
                return this.docType;
            }
        }

        public string RootTargetNamespace
        {
            get
            {
                return this.rootTargetNamespace;
            }
        }

        public Hashtable SchemaList
        {
            get
            {
                return this.schemaList;
            }
        }
    }
}

