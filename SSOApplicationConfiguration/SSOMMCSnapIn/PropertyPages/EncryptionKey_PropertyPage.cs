using bizilante.ManagementConsole.SSO.UserControls;
using Microsoft.ManagementConsole;
using System;

namespace bizilante.ManagementConsole.SSO.PropertyPages
{
    internal class EncryptionKey_PropertyPage : PropertyPage
    {
        private EncryptionKey_PropertiesControl _encryptionKey_PropertiesControl;
        private string _scopeNodeAction = string.Empty;
        private string _encryptedText = string.Empty;
        private string _applicationFileName = string.Empty;

        public event EventHandler<EventArgs<bool, string>> EncryptionKeyEnteredForImport;

        public string ScopeNodeAction
        {
            get
            {
                return this._scopeNodeAction;
            }
            set
            {
                this._scopeNodeAction = value;
            }
        }

        public string EncryptedText
        {
            get
            {
                return this._encryptedText;
            }
            set
            {
                this._encryptedText = value;
            }
        }

        public string ApplicationFileName
        {
            get
            {
                return this._applicationFileName;
            }
            set
            {
                this._applicationFileName = value;
            }
        }

        public string EncryptionKey
        {
            get;
            set;
        }

        public EncryptionKey_PropertyPage(string action, string applicationName)
        {
            this._applicationFileName = applicationName;
            this._scopeNodeAction = action;
            base.Title = "Enter Encryption Key";
            this._encryptionKey_PropertiesControl = new EncryptionKey_PropertiesControl(this);
            base.Control = this._encryptionKey_PropertiesControl;
        }

        public EncryptionKey_PropertyPage(string action, string applicationName, string encryptedText) : this(action, applicationName)
        {
            this._encryptedText = encryptedText;
        }

        protected override void OnInitialize()
        {
            if (this._scopeNodeAction == "Import" && (this.EncryptedText.Length == 0 || this.ApplicationFileName.Length == 0))
            {
                throw new InvalidOperationException("Application FileName and EncryptedText must be provided prior to import");
            }
            base.OnInitialize();
        }

        protected override bool OnApply()
        {
            if (base.Dirty && !this._encryptionKey_PropertiesControl.CanApplyChanges())
            {
                return false;
            }
            if (this._encryptionKey_PropertiesControl.CanApplyChanges() && this._scopeNodeAction == "Import")
            {
                string encryptionKey;
                this._encryptionKey_PropertiesControl.GetKeyValue(out encryptionKey);
                bool flag = SSOHelper.ImportSSOApplication(encryptionKey, this.ApplicationFileName, this.EncryptedText);
                if (this.EncryptionKeyEnteredForImport != null)
                {
                    EventArgs<bool, string> e = new EventArgs<bool, string>(flag, this._scopeNodeAction);
                    this.EncryptionKeyEnteredForImport(this, e);
                }
                return flag;
            }
            if (this._encryptionKey_PropertiesControl.CanApplyChanges() && this._scopeNodeAction == "Export")
            {
                string encryptionKey2;
                this._encryptionKey_PropertiesControl.GetKeyValue(out encryptionKey2);
                this.EncryptionKey = encryptionKey2;
            }
            return true;
        }

        protected override bool OnOK()
        {
            return !this._encryptionKey_PropertiesControl.CheckText() && this.OnApply();
        }

        public bool Save()
        {
            return this.OnOK();
        }

        protected override bool QueryCancel()
        {
            return true;
        }

        protected override void OnCancel()
        {
        }

        protected override void OnDestroy()
        {
        }
    }
}
