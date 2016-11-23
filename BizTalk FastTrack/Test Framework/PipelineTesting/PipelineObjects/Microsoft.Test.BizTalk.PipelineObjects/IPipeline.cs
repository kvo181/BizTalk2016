namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public interface IPipeline
    {
        event PipelineComponentCallEventHandler Called;

        event PipelineComponentCallEventHandler Calling;

        void Execute(IPipelineContext pipelineContext);
        void FireCalled(object sender, string message);
        void FireCalling(object sender, string message);
        IBaseMessage GetNextOutputMessage(IPipelineContext pipelineContext);

        Guid CategoryId { get; }

        ArrayList InputMessages { get; }

        ArrayList Stages { get; }
    }
}

