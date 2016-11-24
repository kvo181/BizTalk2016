using bizilante.Deployment.BTSDeployHost.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using System.Text;

namespace bizilante.Deployment.BTSDeployHost.Host
{
    public class HostUI : PSHostUserInterface
    {
        private PSHostRawUserInterface _rawUI;
        private BTSDeployForm.DeployForm _gui;

        public HostUI(BTSDeployForm.DeployForm gui)
        {
            _gui = gui;
        }

        public override PSHostRawUserInterface RawUI
        {
            get 
            {
                if (null == _rawUI)
                    _rawUI = new HostRawUI(_gui.OutputTextBox);
                return _rawUI; 
            }
        }

        public override string ReadLine()
        {
            return null;
        }

        public override void Write(string value)
        {
            Log(value);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            Log(backgroundColor, value);
        }

        public override void WriteLine(string value)
        {
            LogLine(value);
        }

        public override void WriteErrorLine(string message)
        {
            LogLine(ConsoleColor.Red, message);
        }

        public override void WriteDebugLine(string value)
        {
            LogLine(ConsoleColor.Blue, value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            return;
        }

        public override void WriteVerboseLine(string message)
        {
            LogLine("<Info> " + message);
        }

        public override void WriteWarningLine(string message)
        {
            LogLine("<Warning> " + message);
        }

        /// <summary>
        /// Prompts the user for input. 
        /// <param name="caption">The caption or title of the prompt.</param>
        /// <param name="message">The text of the prompt.</param>
        /// <param name="descriptions">A collection of FieldDescription objects that 
        /// describe each field of the prompt.</param>
        /// <returns>A dictionary object that contains the results of the user 
        /// prompts.</returns>
        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            return null;
        }

        /// <summary>
        /// Prompts the user for credentials with a specified prompt window caption, 
        /// prompt message, user name, and target name. In this example this 
        /// functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption for the message window.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="userName">The user name whose credential is to be prompted for.</param>
        /// <param name="targetName">The name of the target for which the credential is collected.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            return PromptForCredential(caption, message, userName, targetName, PSCredentialTypes.Default, PSCredentialUIOptions.Default);
        }

        /// <summary>
        /// Prompts the user for credentials by using a specified prompt window caption, 
        /// prompt message, user name and target name, credential types allowed to be 
        /// returned, and UI behavior options. In this example this functionality 
        /// is not needed so the method throws a NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption for the message window.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="userName">The user name whose credential is to be prompted for.</param>
        /// <param name="targetName">The name of the target for which the credential is collected.</param>
        /// <param name="allowedCredentialTypes">A PSCredentialTypes constant that 
        /// identifies the type of credentials that can be returned.</param>
        /// <param name="options">A PSCredentialUIOptions constant that identifies the UI 
        /// behavior when it gathers the credentials.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            IntPtr handle = _gui.GetSafeWindowHandle();
            return NativeUtils.CredUIPromptForCredential(
                caption,
                message,
                userName,
                targetName,
                allowedCredentialTypes,
                options,
                handle);
        }

        /// Provides a set of choices that enable the user to choose a 
        /// single option from a set of options. 
        /// </summary>
        /// <param name="caption">Text that proceeds (a title) the choices.</param>
        /// <param name="message">A message that describes the choice.</param>
        /// <param name="choices">A collection of ChoiceDescription objects that describe 
        /// each choice.</param>
        /// <param name="defaultChoice">The index of the label in the Choices parameter 
        /// collection. To indicate no default choice, set to -1.</param>
        /// <returns>The index of the Choices parameter collection element that corresponds 
        /// to the option that is selected by the user.</returns>
        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            IntPtr handle = _gui.GetSafeWindowHandle();
            return Form.PromptForChoice.GetConfirmChoice(handle, caption, message);
        }

        public override SecureString ReadLineAsSecureString()
        {
            return null;
        }

        private void Log(ConsoleColor color, string value)
        {
            BTSDeployForm.DeployForm.SetOutputTextBoxColorDelegate optDelegate =
                new BTSDeployForm.DeployForm.SetOutputTextBoxColorDelegate(_gui.SetOutputTextBoxColor);
            _gui.OutputTextBox.Invoke(optDelegate, new object[] { color.ToColor(), value });
        }
        private void Log(string value)
        {
            BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate optDelegate =
                new BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate(_gui.SetOutputTextBoxContent);
            _gui.OutputTextBox.Invoke(optDelegate, new object[] { value });
        }
        private void LogLine(ConsoleColor color, string value)
        {
            BTSDeployForm.DeployForm.SetOutputTextBoxColorDelegate optDelegate =
                new BTSDeployForm.DeployForm.SetOutputTextBoxColorDelegate(_gui.SetOutputTextBoxColor);
            _gui.OutputTextBox.Invoke(optDelegate, new object[] { color.ToColor(), value + Environment.NewLine });
        }
        private void LogLine(string value)
        {
            BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate optDelegate =
                new BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate(_gui.SetOutputTextBoxContent);
            _gui.OutputTextBox.Invoke(optDelegate, new object[] { value + Environment.NewLine });
        }
    }
}