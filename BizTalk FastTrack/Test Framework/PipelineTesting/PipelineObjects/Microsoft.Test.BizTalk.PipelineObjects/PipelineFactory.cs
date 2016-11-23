namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Xml;

    public class PipelineFactory
    {
        private const ExecuteMethod assembleStageExecuteMethod = ExecuteMethod.All;
        private static readonly Guid assembleStageId = new Guid("9d0e4107-4cce-4536-83fa-4a5040674ad6");
        private const string assembleStageName = "Assemble";
        private const ExecuteMethod decodeStageExecuteMethod = ExecuteMethod.All;
        private static readonly Guid decodeStageId = new Guid("9d0e4103-4cce-4536-83fa-4a5040674ad6");
        private const string decodeStageName = "Decode";
        private const ExecuteMethod disassembleStageExecuteMethod = ExecuteMethod.FirstMatch;
        private static readonly Guid disassembleStageId = new Guid("9d0e4105-4cce-4536-83fa-4a5040674ad6");
        private const string disassembleStageName = "Disassemble";
        private const ExecuteMethod encodeStageExecuteMethod = ExecuteMethod.All;
        private static readonly Guid encodeStageId = new Guid("9d0e4108-4cce-4536-83fa-4a5040674ad6");
        private const string encodeStageName = "Encode";
        private const ExecuteMethod preassembleStageExecuteMethod = ExecuteMethod.All;
        private static readonly Guid preassembleStageId = new Guid("9d0e4101-4cce-4536-83fa-4a5040674ad6");
        private const string preassembleStageName = "Pre-Assemble";
        private static readonly Guid ReceiveCategoryId = new Guid("f66b9f5e-43ff-4f5f-ba46-885348ae1b4e");
        private const ExecuteMethod resolvePartyStageExecuteMethod = ExecuteMethod.All;
        private static readonly Guid resolvePartyStageId = new Guid("9d0e410e-4cce-4536-83fa-4a5040674ad6");
        private const string resolvePartyStageName = "ResolveParty";
        private static readonly Guid SendCategoryId = new Guid("8c6b051c-0ff5-4fc2-9ae5-5016cb726282");
        private static Hashtable stageDescriptors = null;
        private static object syncRoot = new object();
        private const ExecuteMethod validateStageExecuteMethod = ExecuteMethod.All;
        private static readonly Guid validateStageId = new Guid("9d0e410d-4cce-4536-83fa-4a5040674ad6");
        private const string validateStageName = "Validate";
        private const string XsiNamespaceUri = "http://www.w3.org/2001/XMLSchema-instance";

        public Stage CreateAssembleStage(IPipeline pipeline)
        {
            Stage stage = new Stage("Assemble", ExecuteMethod.All, assembleStageId, pipeline);
            pipeline.Stages.Add(stage);
            return stage;
        }

        public Stage CreateDisassembleStage(IPipeline pipeline)
        {
            Stage stage = new Stage("Disassemble", ExecuteMethod.FirstMatch, disassembleStageId, pipeline);
            pipeline.Stages.Add(stage);
            return stage;
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IBaseComponent CreatePipelineComponent(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                IBaseComponent component = (IBaseComponent) Activator.CreateInstance(type);
                if (component != null)
                {
                    return component;
                }
            }
            string path = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0").GetValue("InstallPath").ToString() + "Pipeline Components";
            if (typeName.IndexOf(',') >= 0)
            {
                typeName = typeName.Substring(0, typeName.IndexOf(','));
            }
            foreach (FileInfo info in new DirectoryInfo(path).GetFiles("*.dll"))
            {
                try
                {
                    IBaseComponent component2 = (IBaseComponent) Assembly.LoadFrom(info.FullName).CreateInstance(typeName);
                    if (component2 != null)
                    {
                        return component2;
                    }
                }
                catch
                {
                }
            }
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Can't instantiate pipeline component component with type name {0}.", new object[] { typeName }));
        }

        private IBaseComponent CreatePipelineComponent(XmlNode componentNode)
        {
            XmlNode node = componentNode.SelectSingleNode("Name");
            if (node == null)
            {
                throw new InvalidOperationException("Component name node is not found");
            }
            IBaseComponent component = this.CreatePipelineComponent(node.InnerText);
            IPropertyBag propertyBag = (IPropertyBag) new BTMPropertyBag();
            foreach (XmlNode node2 in componentNode.SelectNodes("Properties/Property"))
            {
                if (node2.Attributes["Name"] == null)
                {
                    throw new InvalidOperationException("Name attribute is not found for a property node");
                }
                string innerText = node2.Attributes["Name"].InnerText;
                XmlNode valueNode = node2.SelectSingleNode("Value");
                if (valueNode != null)
                {
                    object typedPropertyValue = this.GetTypedPropertyValue(valueNode);
                    propertyBag.Write(innerText, ref typedPropertyValue);
                }
            }
            ((IPersistPropertyBag) component).Load(propertyBag, 0);
            return component;
        }

        public IPipeline CreatePipelineFromFile(string pipelineFileName)
        {
            XmlNode node3;
            Guid guid;
            StageDescriptor descriptor;
            Stage stage;
            IBaseComponent component;
            XmlDocument document = new XmlDocument();
            document.Load(pipelineFileName);
            XmlNode node = document.SelectSingleNode("/Document/@PolicyFilePath");
            if (node == null)
            {
                throw new InvalidOperationException("PolicyFilePath attribute can not be found in the pipeline XML content");
            }
            IPipeline pipeline = null;
            string innerText = node.InnerText;
            if (innerText != null)
            {
                if (!(innerText == "BTSTransmitPolicy.xml"))
                {
                    if (innerText == "BTSReceivePolicy.xml")
                    {
                        pipeline = new ReceivePipeline(ReceiveCategoryId);
                        goto Label_0077;
                    }
                }
                else
                {
                    pipeline = new SendPipeline(SendCategoryId);
                    goto Label_0077;
                }
            }
            throw new InvalidOperationException("Unknown policy file name");
        Label_0077:
        Label_016D:
            foreach (XmlNode node2 in document.SelectNodes("/Document/Stages/Stage"))
            {
                node3 = node2.Attributes["CategoryId"];
                if (node3 == null)
                {
                    throw new InvalidOperationException("CategoryId attribute is missing");
                }
                guid = new Guid(node3.InnerText);
                descriptor = (StageDescriptor) this.StageDescriptors[guid];
                if (descriptor == null)
                {
                    throw new InvalidOperationException("Unknown stage category Id");
                }
                stage = new Stage(descriptor.Name, descriptor.ExecuteMethod, guid, pipeline);
                pipeline.Stages.Add(stage);
                foreach (XmlNode node4 in node2.SelectNodes("Components/Component"))
                {
                    component = this.CreatePipelineComponent(node4);
                    stage.AddComponent(component);
                }
                goto Label_016D;
            }
            return pipeline;
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IPipeline CreatePipelineFromType(string pipelineTypeName)
        {
            if (pipelineTypeName == null)
            {
                throw new ArgumentNullException("pipelineTypeName");
            }
            Type pipelineType = Type.GetType(pipelineTypeName, true);
            return this.CreatePipelineFromType(pipelineType);
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IPipeline CreatePipelineFromType(Type pipelineType)
        {
            if (pipelineType == null)
            {
                throw new ArgumentNullException("pipelineType");
            }
            object target = Activator.CreateInstance(pipelineType);
            object obj3 = pipelineType.InvokeMember("XmlContent", BindingFlags.GetProperty, null, target, null, CultureInfo.InvariantCulture);
            if (obj3 == null)
            {
                throw new InvalidOperationException("XmlContent property of the pipeline object returned null");
            }
            XmlDocument document = new XmlDocument();
            document.LoadXml((string) obj3);
            XmlNode node = document.SelectSingleNode("/Document/CategoryId");
            if (node == null)
            {
                throw new InvalidOperationException("CategoryId node can not be found in the pipeline XML content");
            }
            IPipeline pipeline = null;
            Guid categoryId = new Guid(node.InnerText);
            if (categoryId == SendCategoryId)
            {
                pipeline = new SendPipeline(categoryId);
            }
            else
            {
                if (categoryId != ReceiveCategoryId)
                {
                    throw new InvalidOperationException("Unknown pipeline category Id");
                }
                pipeline = new ReceivePipeline(categoryId);
            }
            foreach (XmlNode node2 in document.SelectNodes("/Document/Stages/Stage"))
            {
                XmlNode node3 = node2.SelectSingleNode("PolicyFileStage");
                if (node3 == null)
                {
                    throw new InvalidOperationException("PolicyFileStage element is missing");
                }
                XmlNode node4 = node3.Attributes["Name"];
                if (node4 == null)
                {
                    throw new InvalidOperationException("Name attribute is missing");
                }
                string innerText = node4.InnerText;
                XmlNode node5 = node3.Attributes["execMethod"];
                if (node5 == null)
                {
                    throw new InvalidOperationException("execMethod attribute is missing");
                }
                ExecuteMethod executeMethod = (ExecuteMethod) Enum.Parse(typeof(ExecuteMethod), node5.InnerText, true);
                XmlNode node6 = node3.Attributes["stageId"];
                if (node6 == null)
                {
                    throw new InvalidOperationException("stageId attribute is missing");
                }
                Guid id = new Guid(node6.InnerText);
                Stage stage = new Stage(innerText, executeMethod, id, pipeline);
                pipeline.Stages.Add(stage);
                foreach (XmlNode node7 in node2.SelectNodes("Components/Component"))
                {
                    IBaseComponent component = this.CreatePipelineComponent(node7);
                    stage.AddComponent(component);
                }
            }
            return pipeline;
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IPipeline CreatePipelineFromType(string pipelineTypeName, string assemblyName)
        {
            if (pipelineTypeName == null)
            {
                throw new ArgumentNullException("pipelineTypeName");
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (FileNotFoundException)
            {
                assembly = Assembly.LoadFrom(assemblyName);
            }
            Type pipelineType = assembly.GetType(pipelineTypeName, true);
            return this.CreatePipelineFromType(pipelineType);
        }

        public IPipeline CreateReceivePipeline()
        {
            return new ReceivePipeline(ReceiveCategoryId);
        }

        public IPipeline CreateSendPipeline()
        {
            return new SendPipeline(SendCategoryId);
        }

        private object GetTypedPropertyValue(XmlNode valueNode)
        {
            if (valueNode == null)
            {
                throw new ArgumentNullException("valueNode");
            }
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            string innerText = valueNode.SelectSingleNode("@xsi:type", nsmgr).InnerText;
            string[] strArray = innerText.Split(new char[] { ':' });
            if (strArray.Length > 1)
            {
                innerText = strArray[1];
            }
            string str2 = valueNode.InnerText;
            if ("int" == innerText)
            {
                return Convert.ToInt32(str2, CultureInfo.InvariantCulture);
            }
            if ("boolean" == innerText)
            {
                return Convert.ToBoolean(str2, CultureInfo.InvariantCulture);
            }
            if ("char" == innerText)
            {
                return Convert.ToChar(int.Parse(str2, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            }
            return str2;
        }

        public Hashtable StageDescriptors
        {
            get
            {
                lock (syncRoot)
                {
                    if (stageDescriptors == null)
                    {
                        stageDescriptors = new Hashtable();
                        stageDescriptors.Add(decodeStageId, new StageDescriptor("Decode", ExecuteMethod.All));
                        stageDescriptors.Add(disassembleStageId, new StageDescriptor("Disassemble", ExecuteMethod.FirstMatch));
                        stageDescriptors.Add(validateStageId, new StageDescriptor("Validate", ExecuteMethod.All));
                        stageDescriptors.Add(resolvePartyStageId, new StageDescriptor("ResolveParty", ExecuteMethod.All));
                        stageDescriptors.Add(preassembleStageId, new StageDescriptor("Pre-Assemble", ExecuteMethod.All));
                        stageDescriptors.Add(assembleStageId, new StageDescriptor("Assemble", ExecuteMethod.All));
                        stageDescriptors.Add(encodeStageId, new StageDescriptor("Encode", ExecuteMethod.All));
                    }
                    return stageDescriptors;
                }
            }
        }

        internal class StageDescriptor
        {
            private Microsoft.Test.BizTalk.PipelineObjects.ExecuteMethod executeMethod;
            private string name;

            public StageDescriptor(string name, Microsoft.Test.BizTalk.PipelineObjects.ExecuteMethod executeMethod)
            {
                this.name = name;
                this.executeMethod = executeMethod;
            }

            public Microsoft.Test.BizTalk.PipelineObjects.ExecuteMethod ExecuteMethod
            {
                get
                {
                    return this.executeMethod;
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
}

