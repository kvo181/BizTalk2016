using System;
using BizUnit.Xaml;
using Microsoft.BizTalk.ExplorerOM;

namespace BizUnit.TestSteps.BizTalk.Port
{
    public class ReceiveLocationCreateStep : TestStepBase
    {
        private static BtsCatalogExplorer _catalog;

        public string PortName { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public PipelineType Pipeline { get; set; }

        public ReceiveLocationCreateStep()
        {
            _catalog = BizTalkHelper.GetBtsCatalogExplorer();
        }

        public override void Execute(Context context)
        {
            _catalog.Refresh();

            // Create port
            ReceivePort port = _catalog.AddNewReceivePort(false);
            port.Name = PortName;
            context.LogInfo(string.Format("Created receive port with name: '{0}'", PortName));

            // Create location
            ReceiveLocation location = port.AddNewReceiveLocation();

            location.Name = LocationName;
            location.Address = Address;
            location.TransportType = _catalog.ProtocolTypes["FILE"];
            location.TransportTypeData = string.Format("<CustomProps><FilePath vt=\"8\">{0}</FilePath><BatchSize vt=\"19\">20</BatchSize><FileMask vt=\"8\">*.*</FileMask><RenameReceivedFiles vt=\"11\">-1</RenameReceivedFiles></CustomProps>", Address);
            switch (Pipeline)
            {
                case ReceiveLocationCreateStep.PipelineType.PassThru:
                    location.ReceivePipeline = _catalog.Pipelines["Microsoft.BizTalk.DefaultPipelines.PassThruReceive"];
                    break;
                case ReceiveLocationCreateStep.PipelineType.Xml:
                    location.ReceivePipeline = _catalog.Pipelines["Microsoft.BizTalk.DefaultPipelines.XMLReceive"];
                    break;
            }

            foreach (ReceiveHandler receiveHandler in _catalog.ReceiveHandlers)
            {
                if (receiveHandler.TransportType.Name == location.TransportType.Name)
                {
                    location.ReceiveHandler = receiveHandler;
                    break;
                }
            }

            context.LogInfo(string.Format("Created receive location with name: '{0}', TransportType: '{1}', Address:'{2}', Pipeline:'{3}', Receive handler:'{4}'", LocationName, location.TransportType == null ? string.Empty : location.TransportType.Name, location.Address, location.ReceivePipeline == null ? string.Empty : location.ReceivePipeline.FullName, location.ReceiveHandler == null ? string.Empty : location.ReceiveHandler.Name));

            _catalog.SaveChanges();

            context.Add(string.Format("ReceiveLocation-{0}", location.Name), location);
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(PortName) ||
                string.IsNullOrEmpty(LocationName))
                throw new Exception(string.Format("PortName: '{0}' and/or LocationName: '{1}' cannot be empty", PortName, LocationName));
        }

        public enum PipelineType
        {
            PassThru = 1,
            Xml
        }
    }
}
