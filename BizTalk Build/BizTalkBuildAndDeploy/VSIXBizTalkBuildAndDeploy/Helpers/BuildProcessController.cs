using System;
using System.IO;
using EnvDTE;

namespace VSIXBizTalkBuildAndDeploy.Helpers
{
    /// <summary>
    /// Default build process controller which creates build scripts as per the build generator defaults
    /// </summary>
    public class DefaultBuildProcessController : IBuildProvider
    {
        #region Private methods
        public static void CopyResourceFiles(string destinationPath)
        {
            string SourcePath = AppDomain.CurrentDomain.BaseDirectory + @"\Resources";

            CopyFolder(SourcePath, destinationPath);
        }
        private static void CopyFolder(string sourcePath, string destinationPath)
        {
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);
            CopyFiles(sourcePath, destinationPath);
            foreach (string dir in Directory.GetDirectories(sourcePath))
            {
                string[] folders = Path.GetFullPath(dir).Split(Path.DirectorySeparatorChar);
                string folderName = folders[folders.GetUpperBound(0)];
                DirectoryInfo subDirectory = destinationDirectory.CreateSubdirectory(folderName);
                CopyFolder(dir, subDirectory.FullName);
            }
        }
        private static void CopyFiles(string sourceDirectory, string destinationDirectory)
        {
            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(file));
                if (!File.Exists(destinationFilePath))
                    File.Copy(file, destinationFilePath);
            }
        }
        #endregion

        #region IBuildProvider Members
        /// <summary>
        /// Implements the update event
        /// </summary>
        public event BuildProcessUpdate Update;
        /// <summary>
        /// Implements the create build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="owp"></param>
        public void CreateBuild(OutputWindowPane owp, GenerationArgs args)
        {
            FileBuilders.IFileBuilder fileBuilder;

            //This is no longer required as a path to them will be provided
            //Copy all of the files which dont need to be built
            //CopyResourceFiles(args.OutputFolder); 

            double totalSteps = 11;
            double step = 0;

            fileBuilder = new FileBuilders.ApplicationPropertiesFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100/totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.CustomTargetsFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.CustomPropertiesFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.RemoveFromGacFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.RulesTargetsFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.BuildProjectFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.CommonApplicationTargetsFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.DebugCmdFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.QuickBuildDebugCmdFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.ReleaseCmdFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);            

            fileBuilder = new FileBuilders.GenerateBindingsFileBuilder();
            fileBuilder.Build(args);
            step++;
            Update(owp, Convert.ToInt32(Math.Round(step * 100 / totalSteps)), fileBuilder.Name);
        
        }

        #endregion        
    }
}