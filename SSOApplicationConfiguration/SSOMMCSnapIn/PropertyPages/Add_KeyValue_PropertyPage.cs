using bizilante.ManagementConsole.SSO.UserControls;
using Microsoft.ManagementConsole;
using System;

namespace bizilante.ManagementConsole.SSO.PropertyPages
{
    internal class Add_KeyValue_PropertyPage : PropertyPage
    {
        private string _scopenode = string.Empty;

        private Add_KeyValue_PropertiesControl _add_KeyValue_PropertiesControl;

        public event EventHandler KeyValueAdded;

        public Add_KeyValue_PropertyPage(string strScopeNode)
        {
            this._scopenode = strScopeNode;
            base.Title = "Add Key ";
            this._add_KeyValue_PropertiesControl = new Add_KeyValue_PropertiesControl(this);
            base.Control = this._add_KeyValue_PropertiesControl;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override bool OnApply()
        {
            if (base.Dirty && !this._add_KeyValue_PropertiesControl.CanApplyChanges())
            {
                return false;
            }
            this.KeyValueAdded?.Invoke(this, new EventArgs());
            return true;
        }

        protected override bool OnOK()
        {
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            string[] keys = sSO.GetKeys(this._scopenode);
            string[] values = sSO.GetValues(this._scopenode);
            string[] array = new string[keys.Length + 1];
            string[] array2 = new string[values.Length + 1];
            string text;
            string text2;
            this._add_KeyValue_PropertiesControl.GetKeyValue(out text, out text2);
            if (!this._add_KeyValue_PropertiesControl.CheckText())
            {
                bool flag = false;
                for (int i = 0; i < values.Length; i++)
                {
                    if (keys[i].ToUpper() == text.ToUpper())
                    {
                        flag = true;
                    }
                }
                for (int j = 0; j < values.Length; j++)
                {
                    array[j] = keys[j];
                    array2[j] = values[j];
                }
                array[keys.Length] = text;
                array2[values.Length] = text2;
                if (flag)
                {
                    this._add_KeyValue_PropertiesControl.KeyExist(text);
                }
                else
                {
                    sSO.CreateApplicationFieldsValues(this._scopenode, array, array2);
                }
            }
            return this.OnApply();
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
