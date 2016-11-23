//------------------------------------------------------------------------------
// <copyright file="VSPackageBizTalkBuildAndDeploy.cs" company="bizilante">
//     Copyright (c) 2016 bizilante.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Reflection;
using System.IO;

namespace VSIXBizTalkBuildAndDeploy
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(VSPackageBizTalkBuildAndDeploy.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Helpers.Options.OptionPageGrid), "Bizilante BuildAndDeploy", "General", 0, 0, true)]
    public sealed class VSPackageBizTalkBuildAndDeploy : Package
    {
        /// <summary>
        /// VSPackageBizTalkBuildAndDeploy GUID string.
        /// </summary>
        public const string PackageGuidString = "ef40b34a-3b83-45b4-b198-197d51745f2f";

        #region Properties
        private static EnvDTE.DTE _DTE;
        internal static EnvDTE.DTE DTE
        {
            get { return _DTE; }
        }
        private static EnvDTE80.DTE2 _DTE2;
        internal static EnvDTE80.DTE2 DTE2
        {
            get { return _DTE2; }
        } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="VSPackageBizTalkBuildAndDeploy"/> class.
        /// </summary>
        public VSPackageBizTalkBuildAndDeploy()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _DTE = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            _DTE2 = (EnvDTE80.DTE2)GetService(typeof(EnvDTE.DTE));
            var ow = _DTE2.ToolWindows.OutputWindow;

            Assembly asm = Assembly.GetAssembly(typeof(VSPackageBizTalkBuildAndDeploy));
            string asmLocation = asm.Location;
            AssemblyName asmName = asm.GetName();
            string addinname = string.Format("{0} - Version {1}", asmName.Name, asmName.Version);
            var owp = GetBizTalkBuildAndDeployPane(ow);
            owp.OutputString(string.Format("Loaded from {0}", asmLocation));
            owp.OutputString(Environment.NewLine);
            owp.OutputString(string.Format("Hello BizTalk World! from {0}", addinname));
            owp.OutputString(Environment.NewLine);
            owp.OutputString(Environment.NewLine);

            Helpers.BizTalk.BizTalkHelper.Package = this;
            CreateBizTalkBuildAndDeployScript.Initialize(this, _DTE2, owp);
            ExecuteBizTalkBuildAndDeployScript.Initialize(this, _DTE2, owp);
            ImportSSO.Initialize(this, owp);
            ExportSSO.Initialize(this, owp);

            try
            {
                Helpers.Options.OptionPageGrid page = (Helpers.Options.OptionPageGrid)GetDialogPage(typeof(Helpers.Options.OptionPageGrid));
                if (string.IsNullOrWhiteSpace(page.TasksPath))
                {
                    FileInfo fi = new FileInfo(asmLocation);
                    page.TasksPath = fi.DirectoryName + @"\MSBuild";
                }
            }
            catch { }

            owp.OutputString(Environment.NewLine);
        }

        #endregion

        #region Private Methods
        private EnvDTE.OutputWindowPane GetBizTalkBuildAndDeployPane(EnvDTE.OutputWindow ow)
        {
            foreach (EnvDTE.OutputWindowPane p in ow.OutputWindowPanes)
                if (p.Name == "bizilante Build & Deploy")
                    return p;
            EnvDTE.OutputWindowPane owp = ow.OutputWindowPanes.Add("bizilante Build & Deploy");
            return owp;
        }
        #endregion
    }
}
