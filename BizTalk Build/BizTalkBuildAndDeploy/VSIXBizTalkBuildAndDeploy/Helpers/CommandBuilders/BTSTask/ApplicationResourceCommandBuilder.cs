using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData;
using VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    public class ApplicationResourceCommandBuilder : ICommandBuilder
    {
        private const string AddWebDirectoryCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:{0} /ApplicationName:$(ProductName) /Type:System.BizTalk:WebDirectory /Overwrite /Destination:{1}'/>";

        private const string AddBindingCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /Property:TargetEnvironment=\"{1}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:BizTalkBinding' />";

        private const string AddBizTalkAssemblyCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:BizTalkAssembly /Overwrite /Destination:\"{1}\" /Options:{2}' />";

        private const string AddAssemblyCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:Assembly /Overwrite /Destination:\"{1}\" /Options:{2}' />";

        private const string AddBamCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:Bam /Overwrite /Destination:\"{1}\"' />";

        private const string AddFileCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:File /Overwrite /Destination:\"{1}\"' />";

        private const string AddPreProcessingScriptCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:PreProcessingScript /Overwrite /Destination:\"{1}\"' />";

        private const string AddPostProcessingScriptCommandFormat =
            "\t\t<Exec Command='BTSTask AddResource /Source:\"{0}\" /ApplicationName:\"$(ProductName)\" /Type:System.BizTalk:PostProcessingScript /Overwrite /Destination:\"{1}\"' />";

        private const string CommandTag = "<!-- @DeployResourcesCommand@ -->";

        #region ICommandBuilder Members

        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            foreach (BizTalk.MetaDataBuildGenerator.ApplicationResource resource in args.ApplicationDescription.Resources)
            {
                string command = string.Empty;
                List<string> commandArgs = new List<string>();

                switch (resource.Type)
                {
                    case ResourceTypes.BizTalkBinding:
                        command = AddBindingCommandFormat;
                        BindingResourceAdapter bindingAdapter = BindingResourceAdapter.Create(resource);
                        commandArgs.Add(bindingAdapter.SourceLocation);
                        commandArgs.Add(bindingAdapter.TargetEnvironment);
                        commandArgs.Add(resource.Type);

                        break;
                    case ResourceTypes.BizTalkAssembly:
                        command = AddBizTalkAssemblyCommandFormat;
                        BizTalkAssemblyResourceAdapter bizTalkAssemblyAdapter =
                            BizTalkAssemblyResourceAdapter.Create(resource);
                        commandArgs.Add(BaseResourceAdapter.FormatResourcePath(bizTalkAssemblyAdapter.SourceLocation));
                        commandArgs.Add(bizTalkAssemblyAdapter.DestinationLocation);
                        commandArgs.Add(bizTalkAssemblyAdapter.Options);

                        break;
                    case ResourceTypes.Assembly:
                        command = AddAssemblyCommandFormat;
                        AssemblyResourceAdapter assemblyAdapter = AssemblyResourceAdapter.Create(resource);
                        commandArgs.Add(BaseResourceAdapter.FormatResourcePath(assemblyAdapter.SourceLocation));
                        commandArgs.Add(assemblyAdapter.DestinationLocation);
                        commandArgs.Add(assemblyAdapter.Options);

                        break;
                    case ResourceTypes.WebDirectory:
                        command = AddWebDirectoryCommandFormat;
                        WebDirectoryResourceAdapter webDirectoryAdapter = WebDirectoryResourceAdapter.Create(resource);
                        commandArgs.Add(webDirectoryAdapter.SourceLocation);
                        commandArgs.Add(webDirectoryAdapter.DestinationLocation);

                        break;
                    case ResourceTypes.Bam:
                        command = AddBamCommandFormat;
                        BamResourceAdapter bamAdapter = BamResourceAdapter.Create(resource);
                        commandArgs.Add(BaseResourceAdapter.FormatResourcePath(bamAdapter.SourceLocation));
                        commandArgs.Add(bamAdapter.DestinationLocation);
                        break;

                    case ResourceTypes.File:
                        command = AddFileCommandFormat;
                        FileResourceAdapter fileAdapter = FileResourceAdapter.Create(resource);
                        commandArgs.Add(BaseResourceAdapter.FormatResourcePath(fileAdapter.SourceLocation));
                        commandArgs.Add(fileAdapter.DestinationLocation);
                        break;

                    case ResourceTypes.PostProcessingScript:
                        command = AddPostProcessingScriptCommandFormat;
                        ScriptResourceAdapter scriptAdapter = ScriptResourceAdapter.Create(resource);
                        commandArgs.Add(BaseResourceAdapter.FormatResourcePath(scriptAdapter.SourceLocation));
                        commandArgs.Add(scriptAdapter.DestinationLocation);
                        break;

                    case ResourceTypes.PreProcessingScript:
                        command = AddPreProcessingScriptCommandFormat;
                        ScriptResourceAdapter scriptAdapter1 = ScriptResourceAdapter.Create(resource);
                        commandArgs.Add(BaseResourceAdapter.FormatResourcePath(scriptAdapter1.SourceLocation));
                        commandArgs.Add(scriptAdapter1.DestinationLocation);
                        break;
                    default:
                        break;
                }

                command = string.Format(CultureInfo.InvariantCulture, command, commandArgs.ToArray());
                commandBuilder.Append(command);
                commandBuilder.Append(Environment.NewLine);
            }

            fileBuilder.Replace(CommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}