using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders.BizTalkHost
{
    public class DeleteReceiveHandlerBuilder : ICommandBuilder
    {
        private const string Command = "\t\t<BizTalk.BuildGenerator.Tasks.DeleteReceiveHandler ContinueOnError=\"true\"   HostName=\"{0}\" AdapterName=\"{1}\" />";
        private const string Key = "<!-- @DeleteReceiveHandlerCommand@ -->";


        #region ICommandBuilder Members
        /// <summary>
        /// Implements the Command Builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder sb = new StringBuilder();

            foreach (BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHost host in args.BizTalkHosts.Hosts)
            {
                foreach (BizTalk.MetaDataBuildGenerator.MetaData.BizTalkAdapterHandler handler in host.ReceiveHandlers)
                {
                    if (handler.Included)
                    {
                        string cmd = string.Format(Command, host.Name, handler.AdapterName);
                        sb.AppendLine(cmd);
                    }
                }                
            }

            fileBuilder.Replace(Key, sb.ToString());
        }

        #endregion
    }
}
