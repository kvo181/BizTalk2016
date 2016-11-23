namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.XLANGs.RuntimeTypes;
    using System;
    using System.Collections;

    public interface IInitializeDocumentSpec
    {
        void AddAnnotation(IPropertyAnnotation annotation);
        void Initialize(string rootTargetNamespace, string docSpecName, string docType, string bodyXPath, Hashtable schemaList);

        string BodyXPath { get; }

        string DocumentSpecName { get; }

        string DocumentType { get; }

        string RootTargetNamespace { get; }

        Hashtable SchemaList { get; }
    }
}

