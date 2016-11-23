using System;
using System.Globalization;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Creates a command to Import the SSO Configuration Application Script
    /// (the *.sso file exported from the SSO Snap in MMC)
    /// </summary>
    public class ImportSSOCommandBuilder : ICommandBuilder
    {
        private const string ImportSSOCommand =
            "\t\t<bizilante.BuildGenerator.SSO.Tasks.Import CompanyName=\"{0}\" NonEncryptedFile=\"{1}\" />";
        private const string AddFileCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:File /Overwrite /Destination:\"{1}\"' />";

        private const string CommandTag = "<!-- @ImportSSOCommand@ -->";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            foreach (BizTalk.MetaDataBuildGenerator.SSOApplication app in args.SsoApplications.SSOApps)
            {
                string command = string.Format(CultureInfo.InvariantCulture, AddFileCommandFormat, app.ToString(), app.Destination);
                commandBuilder.Append(command);
                commandBuilder.Append(Environment.NewLine);
                command = string.Format(CultureInfo.InvariantCulture, ImportSSOCommand, app.CompanyName, app.ToString());
                commandBuilder.Append(command);
                commandBuilder.Append(Environment.NewLine);
            }

            fileBuilder.Replace(CommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}