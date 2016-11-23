using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace VSIXBizTalkBuildAndDeploy.Helpers.Options
{
    public class OptionPageGrid : DialogPage
    {
        private string companyName = string.Empty;
        private string tasksPath = string.Empty;
        private ProjectStructureTypeEnum projectStructureType = ProjectStructureTypeEnum.Default;
        private AssemblyVersionEnum assemblyVersionType = AssemblyVersionEnum.Default;
        private bool generateBindings = false;
        private string bizTalkBindingToolDbServer = string.Empty;
        private string bizTalkBindingToolDbDatabase;

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("SSO Company Name")]
        [Description("The company name for the SSO import task")]
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("Tasks Path")]
        [Description("The file path for the MSBUILD tasks")]
        public string TasksPath
        {
            get { return tasksPath; }
            set { tasksPath = value; }
        }

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("Project structure type")]
        [Description("The project structure folder structure")]
        public ProjectStructureTypeEnum ProjectStructureType
        {
            get { return projectStructureType; }
            set { projectStructureType = value; }
        }

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("Assembly Version type")]
        [Description("The assembly version structure")]
        public AssemblyVersionEnum AssemblyVersionType
        {
            get { return assemblyVersionType; }
            set { assemblyVersionType = value; }
        }

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("Generate Bindings")]
        [Description("Allow the use of the Generate Bindings tool")]
        public bool GenerateBindings
        {
            get { return generateBindings = false; }
            set { generateBindings = value; }
        }

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("BizTalkBindingToolDb Server")]
        [Description("BizTalkBindingToolDb database server instance name")]
        public string BizTalkBindingToolDbServer
        {
            get { return bizTalkBindingToolDbServer; }
            set { bizTalkBindingToolDbServer = value; }
        }

        [Category("BizTalk BuildAndDeploy")]
        [DisplayName("BizTalkBindingToolDb Database")]
        [Description("BizTalkBindingToolDb database name")]
        public string BizTalkBindingToolDbDatabase
        {
            get { return bizTalkBindingToolDbDatabase; }
            set { bizTalkBindingToolDbDatabase = value; }
        }

    }
}
