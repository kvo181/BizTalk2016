using System.Collections.Generic;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders.BizTalkHost
{

    public class CreateHostBuilder : ICommandBuilder
    {
        private const string Command = "\t\t<BizTalk.BuildGenerator.Tasks.CreateHost HostName=\"{0}\" Trusted=\"{1}\" HostTracking=\"{2}\" HostType=\"{3}\" GroupName=\"{4}\" />";
        private const string Key = "<!-- @CreateHostCommand@ -->";


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
                if (host.CanCreate)
                {
                    List<string> a = new List<string>();
                    a.Add(host.Name);
                    a.Add(host.HostTrusted.ToString());
                    a.Add(host.TrackingEnabled.ToString());
                    a.Add(host.HostType.ToString());
                    a.Add(host.WindowsGroupName);
                    string cmd = string.Format(Command, a.ToArray());
                    sb.AppendLine(cmd);
                }
            }

            fileBuilder.Replace(Key, sb.ToString());
        }

        #endregion
    }
}
