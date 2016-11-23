using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders.BizTalkHost
{
    public class StopHostCommandBuilder : ICommandBuilder
    {
        private const string Command = "\t\t<BizTalk.BuildGenerator.Tasks.StopHost ContinueOnError=\"true\"   HostName=\"{0}\" />";
        private const string Key = "<!-- @StopHostCommand@ -->";


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
                if (host.HostType == BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHostType.InProcess)
                {
                    string cmd = string.Format(Command, host.Name);
                    sb.AppendLine(cmd);
                }
            }

            fileBuilder.Replace(Key, sb.ToString());
        }

        #endregion
    }
}
