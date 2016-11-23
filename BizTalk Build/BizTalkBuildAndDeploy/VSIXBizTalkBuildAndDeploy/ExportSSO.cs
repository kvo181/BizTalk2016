//------------------------------------------------------------------------------
// <copyright file="ExportSSO.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;
using EnvDTE;
using System.Runtime.InteropServices;
using bizilante.SSO.Helper;
using System.Text;
using System.Web;

namespace VSIXBizTalkBuildAndDeploy
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ExportSSO
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 257;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("ddfb7f48-c55e-4cab-9f46-96ca03a0389c");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// BizTalk Build and Deploy output window pane
        /// </summary>
        private OutputWindowPane _owp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportSSO"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ExportSSO(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(OnExportSSOCommand, menuCommandID);
                menuItem.BeforeQueryStatus += menuCommand_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ExportSSO Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="owp">OutputWindowPane</param>
        public static void Initialize(Package package, OutputWindowPane owp)
        {
            Instance = new ExportSSO(package);
            Instance._owp = owp;
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void OnExportSSOCommand(object sender, EventArgs e)
        {
            IVsHierarchy hierarchy = null;
            uint itemid = VSConstants.VSITEMID_NIL;

            if (!ImportSSO.IsSingleProjectItemSelection(out hierarchy, out itemid)) return;

            var vsProject = (IVsProject)hierarchy;

            // get the name of the item
            string itemFullPath = null;
            if (ErrorHandler.Failed(vsProject.GetMkDocument(itemid, out itemFullPath))) return;

            // Save the project file
            var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            int hr = solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, hierarchy, 0);
            if (hr < 0)
            {
                throw new COMException(string.Format("Failed to add project item", itemFullPath, ImportSSO.GetErrorInfo()), hr);
            }

            var selectedProjectItem = ImportSSO.GetProjectItemFromHierarchy(hierarchy, itemid);
            if (selectedProjectItem != null)
            {
                string itemFolder = Path.GetDirectoryName(itemFullPath);
                string itemFilename = Path.GetFileNameWithoutExtension(itemFullPath);
                string itemExtension = Path.GetExtension(itemFullPath);

                var dialog = new Dialogs.ExportSSO(itemFilename, itemFullPath);
                dialog.ShowDialog();
                if (!dialog.OK) return;

                // Export the SSO application 
                ExportSSOApplication(itemFilename, itemFullPath);
            }
        }
        private void SSO_Update(object sender, SSOEventArgs e)
        {
            if (null == _owp) return;
            _owp.OutputString(e.Source + ": " + e.Message);
        }
        private void menuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                // start by assuming that the menu will not be shown
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;
                if (!ImportSSO.IsSingleProjectItemSelection(out hierarchy, out itemid)) return;

                // Get the file path
                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);
                var transformFileInfo = new FileInfo(itemFullPath);

                // then check if the file is a SSO file
                // if not leave the menu hidden
                if (transformFileInfo.Extension == ".sso")
                {
                    menuCommand.Visible = true;
                    menuCommand.Enabled = true;
                    return;
                }
                if (transformFileInfo.Extension != ".xml") return;
                if (!ImportSSO.IsSSOXmlFile(transformFileInfo)) return;

                menuCommand.Visible = true;
                menuCommand.Enabled = true;
            }
        }
        public bool ExportSSOApplication(string appName, string filename)
        {
            string itemExtension = Path.GetExtension(filename);
            if (itemExtension == ".sso")
            {
                // TODO We only allow to export to a XML file
                return false;
            }

            SSO sSO = new SSO(Helpers.BizTalk.BizTalkHelper.CompanyName);
            sSO.SsoEvent += new EventHandler<SSOEventArgs>(SSO_Update);
            string[] keys = sSO.GetKeys(appName);
            string[] values = sSO.GetValues(appName);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?><SSOApplicationExport><applicationData>");
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] != null && !(keys[i] == ""))
                {
                    stringBuilder.Append(string.Concat(new string[]
                    {
                        "<add key=\"",
                        keys[i],
                        "\" value=\"",
                        HttpUtility.HtmlEncode(values[i]),
                        "\" />"
                    }));
                }
            }
            stringBuilder.Append("</applicationData></SSOApplicationExport>");
            bool result = true;
            StreamWriter streamWriter = new StreamWriter(filename, false);
            try
            {
                streamWriter.Write(stringBuilder.ToString());
                streamWriter.Flush();
                SSO_Update(this, new SSOEventArgs("ExportSSOApplication", string.Format("{0} exported.", filename), false));
            }
            catch (Exception ex)
            {
                SSO_Update(this, new SSOEventArgs("ExportSSOApplication", ex.Message, true));
                result = false;
                return result;
            }
            finally
            {
                streamWriter.Close();
                streamWriter.Dispose();
            }
            return result;
        }
    }
}
