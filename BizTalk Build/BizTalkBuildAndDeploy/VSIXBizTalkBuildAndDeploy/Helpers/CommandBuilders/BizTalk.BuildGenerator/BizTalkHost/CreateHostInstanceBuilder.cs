using System.Collections.Generic;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders.BizTalkHost
{
    public class CreateHostInstanceBuilder : ICommandBuilder
    {
        private const string Command = "\t\t<BizTalk.BuildGenerator.Tasks.CreateHostInstance HostName=\"{0}\" Username=\"{1}\" Password=\"{2}\" />";
        private const string Key = "<!-- @CreateHostInstanceCommand@ -->";


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
                    a.Add(host.Username);
                    a.Add(host.Password);

                    string cmd = string.Format(Command, a.ToArray());
                    sb.AppendLine(cmd);
                }
            }

            fileBuilder.Replace(Key, sb.ToString());
        }

        #endregion
    }
}
