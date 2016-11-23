using System;
using BizUnit.Xaml;
using Microsoft.BizTalk.ExplorerOM;

namespace BizUnit.TestSteps.BizTalk.Port
{
    public class ReceiveLocationDeleteStep : TestStepBase
    {
        private static BtsCatalogExplorer _catalog;

        public string LocationName { get; set; }

        public ReceiveLocationDeleteStep()
        {
            _catalog = BizTalkHelper.GetBtsCatalogExplorer();
        }

        public override void Execute(Context context)
        {
            _catalog.Refresh();

            string locationName = string.Format("ReceiveLocation-{0}", LocationName);
            // Location to remove is found in the test context
            if (!context.ContainsKey(locationName))
                throw new Exception(string.Format("{0} not found in the test context. You can only remove locations created by the ReceiveLocationCreateStep test step!", locationName));

            ReceiveLocation location = (ReceiveLocation)context.GetObject(locationName);

            context.LogInfo(string.Format("Remove receive location with name: '{0}', TransportType: '{1}', Address:'{2}', Pipeline:'{3}', Receive handler:'{4}'", location.Name, location.TransportType == null ? string.Empty : location.TransportType.Name, location.Address, location.ReceivePipeline == null ? string.Empty : location.ReceivePipeline.FullName, location.ReceiveHandler == null ? string.Empty : location.ReceiveHandler.Name));

            ReceivePort port = _catalog.ReceivePorts[location.ReceivePort.Name];
            _catalog.RemoveReceivePort(port);

            _catalog.SaveChanges();
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(LocationName))
                throw new Exception(string.Format("LocationName: '{0}' cannot be empty", LocationName));
        }

        public enum PipelineType
        {
            PassThru = 1,
            Xml
        }
    }
}
