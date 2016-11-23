namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Bam.EventObservation;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class PipelineContext : IPipelineContext, IInitializePipelineContext, IPipelineContextEx
    {
        private int componentIndex;
        private Guid m_stageID = Guid.Empty;
        private IBaseMessageFactory messageFactory = new MessageFactory();
        private Hashtable nameToDocSpecMap = new Hashtable();
        private Guid pipelineId = Guid.Empty;
        private int stageIndex;
        private Hashtable typeToDocSpecMap = new Hashtable();

        public void AddDocSpecByName(string name, IDocumentSpec docSpec)
        {
            this.nameToDocSpecMap.Add(name, docSpec);
        }

        public void AddDocSpecByType(string type, IDocumentSpec docSpec)
        {
            this.typeToDocSpecMap.Add(type, docSpec);
        }

        public IDocumentSpec GetDocumentSpecByName(string docspecName)
        {
            IDocumentSpec spec = (IDocumentSpec) this.nameToDocSpecMap[docspecName];
            if (spec == null)
            {
                throw new COMException();
            }
            return spec;
        }

        public IDocumentSpec GetDocumentSpecByType(string docType)
        {
            IDocumentSpec spec = (IDocumentSpec) this.typeToDocSpecMap[docType];
            if (spec == null)
            {
                throw new COMException();
            }
            return spec;
        }

        public EventStream GetEventStream()
        {
            return null;
        }

        public string GetGroupSigningCertificate()
        {
            return null;
        }

        public IBaseMessageFactory GetMessageFactory()
        {
            return this.messageFactory;
        }

        public object GetTransaction()
        {
            return null;
        }

        public bool HasDocumentSpecByName(string name)
        {
            return (this.nameToDocSpecMap[name] != null);
        }

        public bool HasDocumentSpecByType(string type)
        {
            return (this.typeToDocSpecMap[type] != null);
        }

        public bool AuthenticationRequiredOnReceivePort
        {
            get
            {
                return false;
            }
        }

        public int ComponentIndex
        {
            get
            {
                return this.componentIndex;
            }
        }

        public Guid PipelineID
        {
            get
            {
                return this.pipelineId;
            }
        }

        public string PipelineName
        {
            get
            {
                return "Pipeline";
            }
        }

        public IResourceTracker ResourceTracker
        {
            get
            {
                return new Microsoft.BizTalk.Component.Interop.ResourceTracker();
            }
        }

        public Guid StageID
        {
            get
            {
                return this.m_stageID;
            }
        }

        public int StageIndex
        {
            get
            {
                return this.stageIndex;
            }
        }

        #region IInitializePipelineContext Members


        public void SetStageId(Guid id)
        {
            if (this.m_stageID == Guid.Empty)
                this.m_stageID = id;
        }

        public void SetStageIndex(int index)
        {
            this.stageIndex = index;
        }

        #endregion
    }
}

