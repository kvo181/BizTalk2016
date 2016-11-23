namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.XLANGs.RuntimeTypes;
    using System;

    public class PropertyAnnotation : IPropertyAnnotation
    {
        private string _namespace;
        private string name;
        private Guid propID = Guid.Empty;
        private int trackable;
        private string xpath;
        private string xsdType;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Namespace
        {
            get
            {
                return this._namespace;
            }
            set
            {
                this._namespace = value;
            }
        }

        public Guid PropID
        {
            get
            {
                return this.propID;
            }
            set
            {
                this.propID = value;
            }
        }

        public int Trackable
        {
            get
            {
                return this.trackable;
            }
            set
            {
                this.trackable = value;
            }
        }

        public string XPath
        {
            get
            {
                return this.xpath;
            }
            set
            {
                this.xpath = value;
            }
        }

        public string XSDType
        {
            get
            {
                return this.xsdType;
            }
            set
            {
                this.xsdType = value;
            }
        }
    }
}

