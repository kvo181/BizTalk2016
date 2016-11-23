//------------------------------------------------------------------------------
// <copyright file="CreateBizTalkBuildAndDeployScript.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace VSIXBizTalkBuildAndDeploy
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreateBizTalkBuildAndDeployScript
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4ebe6718-b157-4617-94f7-b555b3bfcfd0");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Top-level object in Visual Studio Automation object model
        /// </summary>
        private EnvDTE80.DTE2 _applicationObject;

        /// <summary>
        /// BizTalk Build and Deploy output window pane
        /// </summary>
        private EnvDTE.OutputWindowPane _owp;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBizTalkBuildAndDeployScript"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CreateBizTalkBuildAndDeployScript(Package package)
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
                //var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                OleMenuCommand menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += CommandQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CreateBizTalkBuildAndDeployScript Instance
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
        /// <param name="applicationObject">Top-level Visual Studio Automation object</param>
        public static void Initialize(Package package, EnvDTE80.DTE2 applicationObject, EnvDTE.OutputWindowPane owp)
        {
            Instance = new CreateBizTalkBuildAndDeployScript(package);
            Instance._applicationObject = applicationObject;
            Instance._owp = owp;
            Helpers.BizTalkBuildAndDeployHelper.Owp = owp;
            owp.OutputString(string.Format("Create BizTalk Build and Deploy script command initialized"));
            owp.OutputString(Environment.NewLine);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Create the msbuild script
            DateTime started = DateTime.Now;
            try
            {
                Helpers.BizTalkBuildAndDeployHelper.CreateBizTalkBuildAndDeployScript(package, (EnvDTE100.Solution4)_applicationObject.Solution, _owp);
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(string.Format("{0}: Create Build & Deploy script completed. It took {1} seconds", DateTime.Now, DateTime.Now.Subtract(started).Seconds));
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(Environment.NewLine);
            }
            catch (Exception ex)
            {
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(string.Format("{0}: Create Build & Deploy failed. It took {1} seconds", DateTime.Now, DateTime.Now.Subtract(started).Seconds));
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(string.Format("ErrorMessage : {0}", ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(string.Format("StackTrace : {0}", ex.InnerException != null ? ex.InnerException.Message : ex.StackTrace));
                _owp.OutputString(Environment.NewLine);
                _owp.OutputString(Environment.NewLine);
            }
        }

        /// <summary>
        /// Request the status of the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand oCommand = (OleMenuCommand)sender;
            oCommand.Visible = CommandItemToHandle();
            _owp.OutputString(string.Format("Create BizTalk Build and Deploy script command {0}", oCommand.Visible ? "visible" : "not visible"));
            _owp.OutputString(Environment.NewLine);
        }
        /// <summary>
        /// Can we use the commands in this package 
        /// </summary>
        /// <returns>true or false</returns>
        private bool CommandItemToHandle()
        {
            if (null == _applicationObject) return false;
            return Helpers.BizTalkBuildAndDeployHelper.IsBizTalkSolution((EnvDTE100.Solution4)_applicationObject.Solution);
        }
    }
}
