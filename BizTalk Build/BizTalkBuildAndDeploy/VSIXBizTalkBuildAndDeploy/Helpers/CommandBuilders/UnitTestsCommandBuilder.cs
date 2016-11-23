using System.Globalization;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Builds the command for calling the unit tests
    /// </summary>
    public class UnitTestsCommandBuilder : ICommandBuilder
    {
        private const string UnitTestsCommand =
            "\t\t<Exec Command='\"$(VS140COMNTOOLS)..\\IDE\\mstest.exe\" {0} /testsettings:\"{1}\"' />";

        private const string CommandTag = "<!-- @UnitTestCommand@ -->";
        private const string TestContainerTag = " /testcontainer:\"{0}\" ";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder containerBuilder = new StringBuilder();

            if (args.UnitTesting.TestContainers.Count > 0)
            {
                foreach (BizTalk.MetaDataBuildGenerator.UnitTestContainer container in args.UnitTesting.TestContainers)
                {
                    if (!string.IsNullOrEmpty(container.Location))
                    {
                        containerBuilder.Append(
                            string.Format(CultureInfo.InvariantCulture, TestContainerTag,
                                          PathHelper.MakeConfigurable(container.Location)));
                    }
                }

                string command =
                    string.Format(CultureInfo.InvariantCulture, UnitTestsCommand, containerBuilder.ToString(),
                                  args.UnitTesting.TestRunConfigPath);
                fileBuilder.Replace(CommandTag, command);
            }
            else
                fileBuilder.Replace(CommandTag, string.Empty);

        }

        #endregion
    }
}