using EnvDTE;
using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers
{
    /// <summary>
    /// Allows the provider to update the UI as to progress of the generation
    /// </summary>
    /// <param name="percentageComplete"></param>
    public delegate void BuildProcessUpdate(OutputWindowPane owp, int percentageComplete, string buildFile);
    
    /// <summary>
    /// Interface to be implemented by build providers
    /// </summary>
    public interface IBuildProvider
    {
        /// <summary>
        /// Event to update the build process status
        /// </summary>
        event BuildProcessUpdate Update;
        /// <summary>
        /// Method to get provider to create build scripts
        /// </summary>
        /// <param name="args"></param>
        void CreateBuild(OutputWindowPane owp, GenerationArgs args);
    }
}
