using System;
using System.Globalization;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Creates a command to remove vocabularies and policies included in this application
    /// </summary>
    public class RemoveRulesCommandBuilder : ICommandBuilder
    {
        private const string RemoveVocabularyCommand =
            "\t\t<bizilante.BuildGenerator.Policies.Tasks.RemoveVocabulary VocabularyName=\"{0}\" />";
        private const string RemovePolicyCommand =
            "\t\t<bizilante.BuildGenerator.Policies.Tasks.RemovePolicy PolicyName=\"{0}\" />";
        private const string RemoveRulesCommandFormat =
            "\t\t<Exec Command='BTSTask RemoveResource /ApplicationName:$(ProductName) /Luid=\"RULE/{0}/{1}\"' ContinueOnError=\"true\" />";

        private const string VocabularyCommandTag = "<!-- @RemoveVocabulariesCommand@ -->";
        private const string PolicyCommandTag = "<!-- @RemovePoliciesCommand@ -->";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            // first we need to remove the policies
            if (null != args.Rules.Policies)
            {
                foreach (BizTalk.MetaDataBuildGenerator.Policy policy in args.Rules.Policies)
                {
                    string command = string.Format(CultureInfo.InvariantCulture, RemoveRulesCommandFormat, policy.Name, policy.Version);
                    commandBuilder.Append(command);
                    commandBuilder.Append(Environment.NewLine);
                }
            }
            if (null != args.Rules.PolicyNames)
            {
                foreach (string policyName in args.Rules.PolicyNames)
                {
                    string command = string.Format(CultureInfo.InvariantCulture, RemovePolicyCommand, policyName);
                    commandBuilder.Append(command);
                    commandBuilder.Append(Environment.NewLine);
                }
            }
            fileBuilder.Replace(PolicyCommandTag, commandBuilder.ToString());

            commandBuilder.Clear();

            // secondly we can remove the vocabularies
            if (null != args.Rules.VocabularieNames)
            {
                foreach (string vocabularyName in args.Rules.VocabularieNames)
                {
                    string command = string.Format(CultureInfo.InvariantCulture, RemoveVocabularyCommand, vocabularyName);
                    commandBuilder.Append(command);
                    commandBuilder.Append(Environment.NewLine);
                }
            }
            fileBuilder.Replace(VocabularyCommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}