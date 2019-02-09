using bizilante.ManagementConsole.SSO.Forms;
using bizilante.ManagementConsole.SSO.PropertyPages;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using System;
using System.Configuration;
using System.Security.Principal;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO
{
    internal class ApplicationScopeNode : ScopeNode
    {
        public const string EXPORT_ACTION = "Export";
        public const string IMPORT_ACTION = "Import";

        private bool _hasKeyValuePropertyPage;
        private bool _ssoAdminResult;
        private string _applicationFileName = string.Empty;
        private string _encryptedText = string.Empty;

        public event EventHandler ApplicationImported;
        public ApplicationScopeNode(bool isRootNode) : 
            this(ConfigurationManager.AppSettings["CompanyName"] + " SSO Application Configuration", isRootNode)
        {
        }

        public ApplicationScopeNode(string applicationName) : this(applicationName, false)
        {
        }

        public ApplicationScopeNode(string applicationName, bool isRootNode)
        {
            if (!ActionsSnapIn.HasSecurityRights)
            {
                bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
                sSO.GetSecretServerName();
                WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                _ssoAdminResult = windowsPrincipal.IsInRole(sSO.SSOAdminGroup);
                ActionsSnapIn.HasSecurityRights = _ssoAdminResult;
            }
            if (ActionsSnapIn.HasSecurityRights)
            {
                DisplayName = applicationName;
                if (!isRootNode)
                {
                    MmcListViewDescription mmcListViewDescription = new MmcListViewDescription();
                    mmcListViewDescription.DisplayName = applicationName;
                    mmcListViewDescription.ViewType = typeof(UserListView);
                    mmcListViewDescription.Options = MmcListViewOptions.SingleSelect;
                    ViewDescriptions.Add(mmcListViewDescription);
                    ViewDescriptions.DefaultIndex = 0;
                }
                ImageIndex = 0;
                SelectedImageIndex = 0;
                if (DisplayName == "_NewApplication")
                {
                    EnabledStandardVerbs = StandardVerbs.Rename;
                }
                Microsoft.ManagementConsole.Action actionExport = 
                    new Microsoft.ManagementConsole.Action("Export Application", "Exports the Key/Value Pairs for this application", -1, "Export");
                Microsoft.ManagementConsole.Action actionAdd = 
                    new Microsoft.ManagementConsole.Action("Add Application", "Adds a new Application", -1, "Add_Application");
                Microsoft.ManagementConsole.Action actionAddKvp = 
                    new Microsoft.ManagementConsole.Action("Add Key Value Pair", "Adds Key Value Pair", -1, "Add_KeyValue");
                Microsoft.ManagementConsole.Action actionDelete = 
                    new Microsoft.ManagementConsole.Action("Delete Application", "Deletes the Application", -1, "Delete");
                Microsoft.ManagementConsole.Action actionImport = 
                    new Microsoft.ManagementConsole.Action("Import Application", "Imports the Key/Value Pairs for an Application", -1, "Import");
                ActionsPaneItems.Add(actionAdd);
                if (isRootNode)
                {
                    actionExport.Enabled = false;
                }
                else
                {
                    actionAdd.Enabled = false;
                    ActionsPaneItems.Add(actionAddKvp);
                    ActionsPaneItems.Add(actionDelete);
                }
                ActionsPaneItems.Add(actionImport);
                ActionsPaneItems.Add(actionExport);
                return;
            }
            DisplayName = applicationName + " - No Access Rights!";
            if (!isRootNode)
            {
                MmcListViewDescription mmcListViewDescription2 = new MmcListViewDescription();
                mmcListViewDescription2.DisplayName = applicationName + " - No Access Rights!";
                mmcListViewDescription2.Options = MmcListViewOptions.HideSelection;
                ViewDescriptions.Add(mmcListViewDescription2);
            }
        }

        protected override void OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        {
            string a;
            if ((a = (string)action.Tag) != null)
            {
                if (a == "Delete")
                {
                    ScopeNode scopeNode = new ScopeNode();
                    scopeNode.DisplayName = base.DisplayName;
                    bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
                    sSO.GetApplications();
                    int index = 0;
                    for (int i = 0; i < base.Parent.Children.Count; i++)
                    {
                        if (Parent.Children[i].DisplayName.ToUpper() == DisplayName.ToUpper())
                        {
                            index = i;
                        }
                    }
                    sSO.DeleteApplication(base.DisplayName);
                    Parent.Children.RemoveAt(index);
                    return;
                }
                if (a == "Add_KeyValue")
                {
                    if (DisplayName.ToUpper() == "_NewApplication".ToUpper())
                    {
                        EnabledStandardVerbs = StandardVerbs.None;
                    }
                    _hasKeyValuePropertyPage = true;
                    ShowPropertySheet("Add Key/Value Pair");
                    OnRefresh(status);
                    base.OnRefresh(status);
                    return;
                }
                if (a == "Add_Application")
                {
                    string[] array = new string[1];
                    string[] array2 = new string[1];
                    array[0] = "";
                    array2[0] = "";
                    string text = "_NewApplication";
                    bool flag = false;
                    bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
                    string[] applications = sSO.GetApplications();
                    for (int j = 0; j < applications.Length; j++)
                    {
                        if (applications[j].ToUpper() == text.ToUpper())
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
                        messageBoxParameters.Text = "This application already exists";
                        messageBoxParameters.Caption = "Add New Application";
                        messageBoxParameters.Icon = MessageBoxIcon.Exclamation;
                        SnapIn.Console.ShowDialog(messageBoxParameters);
                    }
                    else
                    {
                        ScopeNode scopeNode2 = new ScopeNode();
                        scopeNode2.DisplayName = "_NewApplication";
                        ScopeNode scopeNode3 = new ScopeNode();
                        Children.Add(scopeNode2);
                        int count = Children.Count;
                        sSO.CreateApplicationFieldsValues("_NewApplication", array, array2);
                        Children[count - 1] = new ApplicationScopeNode("_NewApplication");
                    }
                    base.OnRefresh(status);
                    return;
                }
                if (!(a == "Export"))
                {
                    if (!(a == "Import"))
                    {
                        return;
                    }
                    DialogResult dialogResult = SSOHelper.OpenSSOImportFile(
                        out _applicationFileName, 
                        out _encryptedText, 
                        this);
                    if (dialogResult != DialogResult.Cancel)
                    {
                        EncryptionKey_PropertyPage encryptionKey_PropertyPage = 
                            new EncryptionKey_PropertyPage("Import", _applicationFileName, _encryptedText);
                        encryptionKey_PropertyPage.EncryptionKeyEnteredForImport += 
                            new EventHandler<EventArgs<bool, string>>(propertyPage_EncryptionKeyEnteredForImport);
                        AcceptKeyForImportForm form = 
                            new AcceptKeyForImportForm(encryptionKey_PropertyPage);
                        base.SnapIn.Console.ShowDialog(form);
                    }
                    base.OnRefresh(status);
                }
                else
                {
                    EncryptionKey_PropertyPage encryptionKey_PropertyPage2 = 
                        new EncryptionKey_PropertyPage("Export", base.DisplayName);
                    AcceptKeyForImportForm form2 = new AcceptKeyForImportForm(encryptionKey_PropertyPage2);
                    base.SnapIn.Console.ShowDialog(form2);
                    if (encryptionKey_PropertyPage2.EncryptionKey != null && encryptionKey_PropertyPage2.EncryptionKey.Length != 0)
                    {
                        if (!SSOHelper.ExportSSOApplication(base.DisplayName, encryptionKey_PropertyPage2.EncryptionKey, this))
                        {
                            MessageBoxParameters messageBoxParameters2 = new MessageBoxParameters();
                            messageBoxParameters2.Text = "Error exporting the application.  Please check the event log for further information";
                            messageBoxParameters2.Caption = "Export Application Error";
                            messageBoxParameters2.Icon = MessageBoxIcon.Hand;
                            base.SnapIn.Console.ShowDialog(messageBoxParameters2);
                            return;
                        }
                    }
                }
            }
        }

        protected override void OnRename(string newText, SyncStatus status)
        {
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            string[] applications = sSO.GetApplications();
            bool flag = false;
            for (int i = 0; i < applications.Length; i++)
            {
                if (applications[i].ToUpper() == newText.ToUpper())
                {
                    flag = true;
                }
            }
            if (flag)
            {
                MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
                messageBoxParameters.Text = "Application Already Exists: " + newText;
                messageBoxParameters.Caption = "Add New Application";
                messageBoxParameters.Icon = MessageBoxIcon.Exclamation;
                base.SnapIn.Console.ShowDialog(messageBoxParameters);
            }
            else
            {
                int count = base.Parent.Children.Count;
                int index = 0;
                for (int j = 0; j < count; j++)
                {
                    if (base.Parent.Children[j].DisplayName == "_NewApplication")
                    {
                        index = j;
                    }
                }
                sSO.DeleteApplication(base.Parent.Children[index].DisplayName);
                base.Parent.Children[index].DisplayName = newText;
                base.Parent.Children[index].LanguageIndependentName = newText;
                base.Parent.Children[index].EnabledStandardVerbs = StandardVerbs.None;
                string[] array = new string[1];
                string[] array2 = new string[1];
                array[0] = "";
                array2[0] = "";
                sSO.CreateApplicationFieldsValues(newText, array, array2);
            }
            base.OnRename(newText, status);
        }

        protected override void OnRefresh(AsyncStatus status)
        {
        }

        protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        {
            if (_hasKeyValuePropertyPage)
            {
                Add_KeyValue_PropertyPage add_KeyValue_PropertyPage = new Add_KeyValue_PropertyPage(base.DisplayName);
                add_KeyValue_PropertyPage.KeyValueAdded += new EventHandler(propertyPage_KeyValueAdded);
                propertyPageCollection.Add(add_KeyValue_PropertyPage);
            }
        }

        protected virtual void OnApplicationImported(EventArgs e)
        {
            ApplicationImported?.Invoke(this, e);
        }


        private void propertyPage_EncryptionKeyEnteredForImport(object sender, EventArgs<bool, string> args)
        {
            if (args.Value) OnApplicationImported(new EventArgs());
        }

        private void propertyPage_KeyValueAdded(object sender, EventArgs e)
        {
            UserListView.OnListViewChanged(base.DisplayName);
        }
    }
}
