namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;

    public class Stage
    {
        private const string AssemblingStageId = "9d0e4107-4cce-4536-83fa-4a5040674ad6";
        private ArrayList components = new ArrayList();
        private int currentDisassembler;
        private const string DisassemblingStageId = "9d0e4105-4cce-4536-83fa-4a5040674ad6";
        private ExecuteMethod executeMethod;
        private Guid id;
        private string name;
        private IBaseMessage outputMessage;
        private IPipeline pipeline;

        public Stage(string name, ExecuteMethod executeMethod, Guid id, IPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }
            this.name = name;
            this.executeMethod = executeMethod;
            this.id = id;
            this.pipeline = pipeline;
        }

        public void AddComponent(IBaseComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            this.components.Add(component);
        }

        public void AddMessage(IPipelineContext pipelineContext, IBaseMessage message, bool severalMessages)
        {
            if (!this.IsAssemblingStage())
            {
                throw new NotSupportedException("This call is legal only for the assembling stage");
            }
            if (this.components.Count == 0)
            {
                if (severalMessages)
                {
                    throw new InvalidOperationException("Can't have more than one incoming message in pipeline without assembler component");
                }
                this.outputMessage = message;
            }
            else
            {
                IAssemblerComponent component = this.components[0] as IAssemblerComponent;
                IBaseComponent sender = this.components[0] as IBaseComponent;
                if (component != null)
                {
                    this.FireCalling(sender, "AddDocument");
                    component.AddDocument(pipelineContext, message);
                    this.FireCalled(sender, "AddDocument");
                }
                else
                {
                    if (severalMessages)
                    {
                        throw new InvalidOperationException("Can't have more than one incoming message with not an assembler component in send pipeline");
                    }
                    this.outputMessage = message;
                }
            }
        }

        public IBaseMessage Assemble(IPipelineContext pipelineContext)
        {
            if (!this.IsAssemblingStage())
            {
                throw new NotSupportedException("This call is legal only for the assembling stage");
            }
            if (this.components.Count != 0)
            {
                IAssemblerComponent component = this.components[0] as IAssemblerComponent;
                IBaseComponent sender = this.components[0] as IBaseComponent;
                int num = 0;
                if (component != null)
                {
                    this.FireCalling(sender, "Assemble");
                    this.outputMessage = component.Assemble(pipelineContext);
                    this.FireCalled(sender, "Assemble");
                    num = 1;
                }
                for (int i = num; i < this.components.Count; i++)
                {
                    sender = this.components[i] as IBaseComponent;
                    IComponent component3 = this.components[i] as IComponent;
                    if (component3 == null)
                    {
                        throw new InvalidOperationException("Component doesn't implement IComponent interface");
                    }
                    this.FireCalling(sender, "Execute");
                    this.outputMessage = component3.Execute(pipelineContext, this.outputMessage);
                    this.FireCalled(sender, "Execute");
                }
            }
            return this.outputMessage;
        }

        public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            IBaseMessage pInMsg = inputMessage;
            if (!this.IsDisassemblingStage())
            {
                if (this.executeMethod != ExecuteMethod.All)
                {
                    throw new NotSupportedException("Execution method is not supported");
                }
                foreach (IComponent component4 in this.components)
                {
                    this.FireCalling(component4 as IBaseComponent, "Execute");
                    pInMsg = component4.Execute(pipelineContext, pInMsg);
                    this.FireCalled(component4 as IBaseComponent, "Execute");
                }
                return pInMsg;
            }
            if (this.executeMethod != ExecuteMethod.FirstMatch)
            {
                throw new NotSupportedException("Execution method is invalid for the disassembling stage");
            }
            if (this.components.Count <= 0)
            {
                this.outputMessage = pInMsg;
                return pInMsg;
            }
            bool flag = false;
            for (int i = 0; i < this.components.Count; i++)
            {
                IBaseComponent sender = this.components[i] as IBaseComponent;
                IProbeMessage message2 = this.components[i] as IProbeMessage;
                if (message2 != null)
                {
                    this.FireCalling(sender, "Probe");
                    flag = message2.Probe(pipelineContext, pInMsg);
                    this.FireCalled(sender, "Probe");
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    this.currentDisassembler = i;
                    IDisassemblerComponent component2 = this.components[this.currentDisassembler] as IDisassemblerComponent;
                    if (component2 != null)
                    {
                        this.FireCalling(sender, "Disassemble");
                        component2.Disassemble(pipelineContext, pInMsg);
                        this.FireCalled(sender, "Disassemble");
                    }
                    else
                    {
                        IComponent component3 = this.components[this.currentDisassembler] as IComponent;
                        if (component3 == null)
                        {
                            throw new InvalidOperationException("Pipeline component doesn't implement IDisassemblerComponent nor IComponent interfaces");
                        }
                        this.FireCalling(sender, "Execute");
                        pInMsg = component3.Execute(pipelineContext, pInMsg);
                        this.FireCalled(sender, "Execute");
                    }
                    break;
                }
            }
            if (!flag)
            {
                throw new InvalidOperationException("None of the disassembler components could recognize a data");
            }
            return pInMsg;
        }

        protected void FireCalled(object sender, string message)
        {
            this.pipeline.FireCalled(sender, message);
        }

        protected void FireCalling(object sender, string message)
        {
            this.pipeline.FireCalling(sender, message);
        }

        public IEnumerator GetComponentEnumerator()
        {
            return this.components.GetEnumerator();
        }

        public IBaseMessage GetNextOutputMessage(IPipelineContext pipelineContext)
        {
            if (pipelineContext == null)
            {
                throw new ArgumentNullException("pipelineContext");
            }
            if (!this.IsDisassemblingStage())
            {
                throw new NotSupportedException("This call is legal only for the disassembling stage");
            }
            IBaseMessage outputMessage = null;
            if (this.components.Count == 0)
            {
                outputMessage = this.outputMessage;
                this.outputMessage = null;
                return outputMessage;
            }
            IBaseComponent sender = this.components[this.currentDisassembler] as IBaseComponent;
            IDisassemblerComponent component2 = this.components[this.currentDisassembler] as IDisassemblerComponent;
            this.FireCalling(sender, "GetNext");
            if (component2 != null)
            {
                outputMessage = component2.GetNext(pipelineContext);
            }
            this.FireCalled(sender, "GetNext");
            return outputMessage;
        }

        public bool IsAssemblingStage()
        {
            return (this.id == new Guid("9d0e4107-4cce-4536-83fa-4a5040674ad6"));
        }

        public bool IsDisassemblingStage()
        {
            return (this.id == new Guid("9d0e4105-4cce-4536-83fa-4a5040674ad6"));
        }

        public ExecuteMethod ExecutionMethod
        {
            get
            {
                return this.executeMethod;
            }
        }

        public Guid Id
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

