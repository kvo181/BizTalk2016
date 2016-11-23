using System;
using VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator;
using VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData;

namespace VSIXBizTalkBuildAndDeploy.Helpers
{
    /// <summary>
    /// These are the args for generating the build
    /// </summary>
    [Serializable]
    public class GenerationArgs
    {
        private ApplicationSetup _applicationSetup = new ApplicationSetup();
        private UnitTesting _unitTesting = new UnitTesting();
        private ApplicationDeployment _applicationDeployment = new ApplicationDeployment();
        private ApplicationBindings _applicationBindings = new ApplicationBindings();
        private ApplicationDescription _applicationDescription = new ApplicationDescription();
        private BuildReferences _buildReferences = new BuildReferences();
        private SSOApplications _ssoApps = new SSOApplications();
        private string _outputFolder;
        private string _solutionPath;
        private BizTalkHosts _bizTalkHosts = new BizTalkHosts();
        private string _generationProvider;
        private BuildProperties _buildProperties = new BuildProperties();
        private Rules _rules = new Rules();
        private string _majorVersion;
        private string _minorVersion;
        private string _release;
        private string _build;
        private Options.ProjectStructureTypeEnum _projectStructureType;
        private Options.AssemblyVersionEnum _assemblyVersionType;

        ///<summary>
        ///</summary>
        public BuildProperties BuildProperties
        {
            get { return _buildProperties; }
            set { _buildProperties = value; }
        }

        ///<summary>
        ///</summary>
        public string GenerationProvider
        {
            get { return _generationProvider; }
            set { _generationProvider = value; }
        }


        ///<summary>
        ///</summary>
        public string SolutionPath
        {
            get { return _solutionPath; }
            set { _solutionPath = value; }
        }

        ///<summary>
        ///</summary>
        public BizTalkHosts BizTalkHosts
        {
            get { return _bizTalkHosts; }
            set { _bizTalkHosts = value; }
        }

        ///<summary>
        ///</summary>
        public BuildReferences BuildReferences
        {
            get { return _buildReferences; }
            set { _buildReferences = value; }
        }

        ///<summary>
        ///</summary>
        public string OutputFolder
        {
            get { return _outputFolder; }
            set { _outputFolder = value; }
        }

        ///<summary>
        ///</summary>
        public ApplicationDescription ApplicationDescription
        {
            get { return _applicationDescription; }
            set { _applicationDescription = value; }
        }


        ///<summary>
        ///</summary>
        public ApplicationBindings ApplicationBindings
        {
            get { return _applicationBindings; }
            set { _applicationBindings = value; }
        }


        ///<summary>
        ///</summary>
        public ApplicationDeployment ApplicationDeployment
        {
            get { return _applicationDeployment; }
            set { _applicationDeployment = value; }
        }


        ///<summary>
        ///</summary>
        public UnitTesting UnitTesting
        {
            get { return _unitTesting; }
            set { _unitTesting = value; }
        }


        ///<summary>
        ///</summary>
        public ApplicationSetup ApplicationSetup
        {
            get { return _applicationSetup; }
            set { _applicationSetup = value; }
        }

        ///<summary>
        ///</summary>
        public SSOApplications SsoApplications
        {
            get { return _ssoApps; }
            set { _ssoApps = value; }
        }

        ///<summary>
        ///</summary>
        public Rules Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }

        ///<summary>
        /// Build major version
        ///</summary>
        public string MajorVersion
        {
            get { return _majorVersion; }
            set { _majorVersion = value; }
        }

        ///<summary>
        /// Build minor version
        ///</summary>
        public string MinorVersion
        {
            get { return _minorVersion; }
            set { _minorVersion = value; }
        }

        ///<summary>
        /// Build release version
        ///</summary>
        public string Release
        {
            get { return _release; }
            set { _release = value; }
        }

        ///<summary>
        /// Build build version
        ///</summary>
        public string Build
        {
            get { return _build; }
            set { _build = value; }
        }

        /// <summary>
        /// The project structure type:
        /// - ACV-CSC
        /// - Default
        /// </summary>
        public Options.ProjectStructureTypeEnum ProjectStructureType
        {
            get { return _projectStructureType; }
            set { _projectStructureType = value; }
        }
        /// <summary>
        /// The assembly version type:
        /// - Default (Major.Minor.Date.Revision)
        /// - Normal (Major.Minor.Build.Revision)
        /// </summary>
        public Options.AssemblyVersionEnum AssemblyVersionType
        {
            get { return _assemblyVersionType; }
            set { _assemblyVersionType = value; }
        }

        public ApplicationSetup ApplicationSetup1
        {
            get
            {
                return _applicationSetup;
            }

            set
            {
                _applicationSetup = value;
            }
        }
    }
}