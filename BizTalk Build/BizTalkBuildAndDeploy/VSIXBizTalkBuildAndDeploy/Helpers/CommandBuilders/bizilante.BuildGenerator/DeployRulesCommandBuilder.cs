using System;
using System.Globalization;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Creates a command to deploy vocabularies and policies included in this application
    /// </summary>
    public class DeployRulesCommandBuilder : ICommandBuilder
    {
        private const string DeployVocabularyCommand =
            "\t\t<bizilante.BuildGenerator.Policies.Tasks.DeployVocabulary VocabularyName=\"{0}\" VocabularyFileName=\"{1}\\{2}\" />";
        private const string DeployPolicyCommand =
            "\t\t<bizilante.BuildGenerator.Policies.Tasks.DeployPolicy PolicyName=\"{0}\" PolicyFileName=\"{1}\\{2}\" Deploy=\"{3}\" />";
        private const string AddRulesCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /ApplicationName:$(ProductName) /Type:System.BizTalk:Rules  /Name=\"{0}\" /Version=\"{1}\"' />";

        private const string VocabularyCommandTag = "<!-- @DeployVocabulariesCommand@ -->";
        private const string PolicyCommandTag = "<!-- @DeployPoliciesCommand@ -->";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            // first we need to deploy the vocabularies
            if (null != args.Rules.Vocabularies)
            {
                foreach (BizTalk.MetaDataBuildGenerator.Vocabulary vocabulary in args.Rules.Vocabularies)
                {
                    string command = string.Format(CultureInfo.InvariantCulture, DeployVocabularyCommand, vocabulary.Name, BizTalk.BizTalkHelper.GetRulesTargetsPath(args.ProjectStructureType), vocabulary.FullName);
                    commandBuilder.Insert(0, Environment.NewLine);
                    commandBuilder.Insert(0, command);
                }
            }
            fileBuilder.Replace(VocabularyCommandTag, commandBuilder.ToString());

            commandBuilder.Clear();

            // secondly we can deploy the policies
            if (null != args.Rules.Policies)
            {
                foreach (BizTalk.MetaDataBuildGenerator.Policy policy in args.Rules.Policies)
                {
                    string command = string.Format(CultureInfo.InvariantCulture, DeployPolicyCommand, policy.Name, BizTalk.BizTalkHelper.GetRulesTargetsPath(args.ProjectStructureType), policy.FullName, true);
                    commandBuilder.Append(command);
                    commandBuilder.Append(Environment.NewLine);
                    command = string.Format(CultureInfo.InvariantCulture, AddRulesCommandFormat, policy.Name, policy.Version);
                    commandBuilder.Append(command);
                    commandBuilder.Append(Environment.NewLine);
                }
            }
            fileBuilder.Replace(PolicyCommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}