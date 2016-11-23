using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Creates a command to remove vocabularies and policies included in this application
    /// </summary>
    public class AssemblyInfoCommandBuilder : ICommandBuilder
    {
        // TODO make the AssemblyFileBuildNumberFormat configurable, independent of ProjectStructureType
        private const string AssemblyInfoCommand1 =
            "\t\t<MSBuild.ExtensionPack.Framework.AssemblyInfo AssemblyInfoFiles=\"$(AssemblyInfoFiles)\" AssemblyBuildNumberFormat=\"DirectSet\" AssemblyFileBuildNumberFormat=\"yyMMdd\" AssemblyFileBuildNumberType=\"DateString\" AssemblyFileRevisionType=\"AutoIncrement\" AssemblyFileRevisionFormat=\"00\" AssemblyFileMajorVersion=\"$(MajorVersion)\" AssemblyFileMinorVersion=\"$(MinorVersion)\" AssemblyVersion=\"$(MajorVersion).$(MinorVersion).0.0\">\r\n\t\t\t<Output TaskParameter=\"MaxAssemblyFileVersion\" PropertyName=\"ProductDescription\"/>\r\n\t\t</MSBuild.ExtensionPack.Framework.AssemblyInfo>";
        private const string AssemblyInfoCommand2 =
            "\t\t<bizilante.BuildGenerator.Tasks.AssemblyInfo AssemblyInfoFiles=\"$(AssemblyInfoFiles)\" AssemblyBuildNumberFormat=\"DirectSet\" AssemblyFileMinorVersionType=\"AutoIncrement\" AssemblyFileBuildNumberType=\"AutoIncrement\" AssemblyFileRevisionType=\"AutoIncrement\" AssemblyFileMajorVersion=\"$(MajorVersion)\" AssemblyVersion=\"$(MajorVersion).$(MinorVersion).0.0\">\r\n\t\t\t<Output TaskParameter=\"MaxAssemblyFileVersion\" PropertyName=\"ProductDescription\"/>\r\n\t\t</bizilante.BuildGenerator.Tasks.AssemblyInfo>";

        private const string AssemblyInfoCommandTag = "<!-- @AssemblyInfoCommand@ -->";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
         public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            if (args.AssemblyVersionType == Options.AssemblyVersionEnum.Normal)
            {
                string command = AssemblyInfoCommand2;
                commandBuilder.Append(command);
            }
            else
            {
                string command = AssemblyInfoCommand1;
                commandBuilder.Append(command);
            }

            fileBuilder.Replace(AssemblyInfoCommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}