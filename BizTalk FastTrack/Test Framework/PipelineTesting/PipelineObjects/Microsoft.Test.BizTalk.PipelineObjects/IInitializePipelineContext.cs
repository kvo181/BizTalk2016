namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using System;

    public interface IInitializePipelineContext
    {
        void AddDocSpecByName(string name, IDocumentSpec documentSpec);
        void AddDocSpecByType(string type, IDocumentSpec documentSpec);
        bool HasDocumentSpecByName(string name);
        bool HasDocumentSpecByType(string type);
        void SetStageId(Guid id);
        void SetStageIndex(int index);
    }
}

