using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Interface to be implemented by classes which build commands
    /// </summary>
    public interface ICommandBuilder
    {
        void Build(GenerationArgs args, StringBuilder fileBuilder);
    }
}