namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public abstract class GenericPipeline : IPipeline
    {
        private Guid categoryId;
        private bool executeMatchFirstAsAll;
        private ArrayList inputMessages = new ArrayList();
        private ArrayList stages = new ArrayList();

        public event PipelineComponentCallEventHandler Called;

        public event PipelineComponentCallEventHandler Calling;

        public GenericPipeline(Guid categoryId)
        {
            this.categoryId = categoryId;
        }

        public abstract void Execute(IPipelineContext pipelineContext);
        protected IBaseMessage ExecuteSubPipeline(IPipelineContext pipelineContext, IBaseMessage inputMessage, int startStageIndex, int endStageIndex)
        {
            IBaseMessage message = inputMessage;
            for (int i = startStageIndex; i <= endStageIndex; i++)
            {
                Stage sender = this.Stages[i] as Stage;
                IInitializePipelineContext ctx = pipelineContext as IInitializePipelineContext;
                ctx.SetStageId(sender.Id);
                ctx.SetStageIndex(i);
                this.FireCalling(sender, "Execute");
                message = sender.Execute(pipelineContext, message);
                this.FireCalled(sender, "Execute");
            }
            return message;
        }

        public void FireCalled(object sender, string message)
        {
            if (this.Called != null)
            {
                this.Called(sender, new CallEventArgs(message));
            }
        }

        public void FireCalling(object sender, string message)
        {
            if (this.Calling != null)
            {
                this.Calling(sender, new CallEventArgs(message));
            }
        }

        public abstract IBaseMessage GetNextOutputMessage(IPipelineContext pipelineContext);

        public Guid CategoryId
        {
            get
            {
                return this.categoryId;
            }
        }

        public bool ExecuteFirstMatchAsAll
        {
            get
            {
                return this.executeMatchFirstAsAll;
            }
            set
            {
                this.executeMatchFirstAsAll = value;
            }
        }

        public ArrayList InputMessages
        {
            get
            {
                return this.inputMessages;
            }
        }

        public ArrayList Stages
        {
            get
            {
                return this.stages;
            }
        }
    }
}

