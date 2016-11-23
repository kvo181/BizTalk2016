namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;

    public class ReceivePipeline : GenericPipeline
    {
        private IBaseMessage outputMessage;

        internal ReceivePipeline(Guid categoryId) : base(categoryId)
        {
        }

        public override void Execute(IPipelineContext pipelineContext)
        {
            if (pipelineContext == null)
            {
                throw new ArgumentNullException("pipelineContext");
            }
            if (base.InputMessages.Count > 1)
            {
                throw new InvalidOperationException("There must be only one input message for the receive pipeline");
            }
            if (base.InputMessages.Count == 0)
            {
                throw new InvalidOperationException("There must be an input message for the receive pipeline");
            }
            this.outputMessage = base.InputMessages[0] as IBaseMessage;
            int disassemblingStageIndex = this.GetDisassemblingStageIndex();
            if (disassemblingStageIndex == -1)
            {
                throw new InvalidOperationException("Disassembling stage is not found");
            }
            this.outputMessage = base.ExecuteSubPipeline(pipelineContext, this.outputMessage, 0, disassemblingStageIndex);
        }

        private int GetDisassemblingStageIndex()
        {
            for (int i = 0; i < base.Stages.Count; i++)
            {
                Stage stage = base.Stages[i] as Stage;
                if (stage.IsDisassemblingStage())
                {
                    return i;
                }
            }
            throw new InvalidOperationException("Disassembling stage is not found");
        }

        public override IBaseMessage GetNextOutputMessage(IPipelineContext pipelineContext)
        {
            int disassemblingStageIndex = this.GetDisassemblingStageIndex();
            if (disassemblingStageIndex == -1)
            {
                throw new InvalidOperationException("Disassembling stage is not found");
            }
            Stage sender = base.Stages[disassemblingStageIndex] as Stage;
            base.FireCalling(sender, "GetNextOutputMessage");
            this.outputMessage = sender.GetNextOutputMessage(pipelineContext);
            base.FireCalled(sender, "GetNextOutputMessage");
            if (this.outputMessage != null)
            {
                this.outputMessage = base.ExecuteSubPipeline(pipelineContext, this.outputMessage, disassemblingStageIndex + 1, base.Stages.Count - 1);
            }
            return this.outputMessage;
        }
    }
}

