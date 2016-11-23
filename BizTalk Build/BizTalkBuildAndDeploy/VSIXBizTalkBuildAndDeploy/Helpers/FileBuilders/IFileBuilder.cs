namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public interface IFileBuilder
    {
        string Name { get; }
        void Build(GenerationArgs args);
    }
}