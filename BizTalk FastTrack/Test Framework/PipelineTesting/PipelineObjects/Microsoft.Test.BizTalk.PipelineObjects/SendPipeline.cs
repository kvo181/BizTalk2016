namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;

    public class SendPipeline : GenericPipeline
    {
        private IBaseMessage outputMessage;

        internal SendPipeline(Guid categoryId) : base(categoryId)
        {
        }

        public override void Execute(IPipelineContext pipelineContext)
        {
            if (pipelineContext == null)
            {
                throw new ArgumentNullException("pipelineContext");
            }
            if (base.InputMessages.Count == 0)
            {
                throw new InvalidOperationException("There must be an input message for the send pipeline");
            }
            int assemblingStageIndex = this.GetAssemblingStageIndex();
            if (assemblingStageIndex == -1)
            {
                throw new InvalidOperationException("Assembling stage is not found");
            }
            Stage sender = base.Stages[assemblingStageIndex] as Stage;
            bool severalMessages = base.InputMessages.Count > 1;
            foreach (IBaseMessage message in base.InputMessages)
            {
                this.outputMessage = base.ExecuteSubPipeline(pipelineContext, message, 0, assemblingStageIndex - 1);
                base.FireCalling(sender, "AddMessage");
                sender.AddMessage(pipelineContext, this.outputMessage, severalMessages);
                base.FireCalled(sender, "AddMessage");
            }
            base.FireCalling(sender, "Assemble");
            this.outputMessage = sender.Assemble(pipelineContext);
            base.FireCalled(sender, "Assemble");
            this.outputMessage = base.ExecuteSubPipeline(pipelineContext, this.outputMessage, assemblingStageIndex + 1, base.Stages.Count - 1);
        }

        private int GetAssemblingStageIndex()
        {
            for (int i = 0; i < base.Stages.Count; i++)
            {
                Stage stage = base.Stages[i] as Stage;
                if (stage.IsAssemblingStage())
                {
                    return i;
                }
            }
            throw new InvalidOperationException("Assembling stage is not found");
        }

        public override IBaseMessage GetNextOutputMessage(IPipelineContext pipelineContext)
        {
            IBaseMessage outputMessage = this.outputMessage;
            this.outputMessage = null;
            return outputMessage;
        }
    }
}

