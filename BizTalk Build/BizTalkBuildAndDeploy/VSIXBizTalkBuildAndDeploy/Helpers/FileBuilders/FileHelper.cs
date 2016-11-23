using System.IO;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public class FileHelper
    {
        /// <summary>
        /// Writes the text to a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sb"></param>
        public static void WriteFile(string path, StringBuilder sb)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(sb.ToString());
                writer.Flush();
                fs.Close();
            }
        }
        /// <summary>
        /// Configures a file to remove the full paths and replace with parameter for source code root
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="args"></param>
        public static void ConfigureForSourceCodeRoot(StringBuilder sb, GenerationArgs args)
        {
            string directory ;
            switch (BizTalk.BizTalkHelper.ProjectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    directory = Path.GetDirectoryName(args.SolutionPath + "\\..\\");
                    break;
                default:
                    directory = Path.GetDirectoryName(args.SolutionPath);
                    break;
            }
            if (directory != null) sb.Replace(new DirectoryInfo(directory).FullName, "$(SourceCodeRootFolder)");
        }
        /// <summary>
        /// Replace the parameter for source code root with the full name
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="args"></param>
        public static void ReplaceSourceCodeRoot(StringBuilder sb, GenerationArgs args)
        {
            string directory;
            switch (BizTalk.BizTalkHelper.ProjectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    directory = Path.GetDirectoryName(args.SolutionPath + "\\..\\");
                    break;
                default:
                    directory = Path.GetDirectoryName(args.SolutionPath);
                    break;
            }
            if (directory != null) sb.Replace("$(SourceCodeRootFolder)",new DirectoryInfo(directory).FullName.TrimEnd(new char[] {'\\'}));
        }
        /// <summary>
        /// Configures a file to set the %BTAD_InstallDir% correctly for the build script
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="args"></param>
        public static void ConfigureForBizTalkInstallVariable(StringBuilder sb)
        {
            sb.Replace("%BTAD_InstallDir%", "%%BTAD_InstallDir%%");
        }
        /// <summary>
        /// Configures a file to set the ReadMe source file name
        /// </summary>
        /// <param name="sb"></param>
        public static void ConfigureForReadMe(StringBuilder sb)
        {
            string readMeFileName;
            string releaseNoteFileName;
            switch (BizTalk.BizTalkHelper.ProjectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    readMeFileName = "$(SourceCodeRootFolder)\\ReadMe.htm";
                    releaseNoteFileName = "$(SourceCodeRootFolder)\\ReleaseNote.txt";
                    break;
                default:
                    readMeFileName = "$(SourceCodeRootFolder)\\Setup\\Resources\\ReadMe.htm";
                    releaseNoteFileName = "$(SourceCodeRootFolder)\\ReleaseNotes.txt";
                    break;
            }
            sb.Replace("@ReadMePath@", readMeFileName);
            sb.Replace("@ReleaseNotesPath@", releaseNoteFileName);
        }
    }
}