using System;
using System.Globalization;
using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData;
using VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    public class RemoveFromGacCommandBuilder : ICommandBuilder
    {
        private const string CommandTag = "<!-- @RemoveFromGacCommand@ -->";

        private const string RemoveFromGacFormat =
            "\t\t<Exec Command='\"$(GacUtilPath)gacutil\" /u {0}' ContinueOnError=\"true\"/>";

        #region ICommandBuilder Members

        /// <summary>
        /// Adds the command to the file builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            foreach (BizTalk.MetaDataBuildGenerator.ApplicationResource resource in args.ApplicationDescription.Resources)
            {
                if (resource.Type == ResourceTypes.Assembly)
                {
                    AssemblyResourceAdapter assemblyAdapter = AssemblyResourceAdapter.Create(resource);
                    commandBuilder.Append(
                        string.Format(CultureInfo.InvariantCulture, RemoveFromGacFormat,
                                      assemblyAdapter.AssemblyNameWithoutExtension));
                    commandBuilder.Append(Environment.NewLine);
                }
                else if (resource.Type == ResourceTypes.BizTalkAssembly)
                {
                    BizTalkAssemblyResourceAdapter btsAdapter = BizTalkAssemblyResourceAdapter.Create(resource);
                    commandBuilder.Append(
                        string.Format(CultureInfo.InvariantCulture, RemoveFromGacFormat,
                                      btsAdapter.AssemblyNameWithoutExtension));
                    commandBuilder.Append(Environment.NewLine);
                }
            }

            fileBuilder.Replace(CommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}