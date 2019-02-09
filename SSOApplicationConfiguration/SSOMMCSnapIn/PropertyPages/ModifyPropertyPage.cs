using bizilante.ManagementConsole.SSO.UserControls;
using Microsoft.ManagementConsole;

namespace bizilante.ManagementConsole.SSO.PropertyPages
{
    internal class ModifyPropertyPage : PropertyPage
    {
        private Modify_KeyValue_PropertiesControl modifyPropertiesControl;

        private string _scopeNode = "";

        private string _currentKeyName = "";

        public ModifyPropertyPage(string strScopeNode)
        {
            this._scopeNode = strScopeNode;
            base.Title = "Property Page";
            this.modifyPropertiesControl = new Modify_KeyValue_PropertiesControl(this);
            base.Control = this.modifyPropertiesControl;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ResultNode resultNode = (ResultNode)base.ParentSheet.SelectionObject;
            this._currentKeyName = resultNode.DisplayName;
            this.modifyPropertiesControl.RefreshData((ResultNode)base.ParentSheet.SelectionObject);
        }

        protected override bool OnApply()
        {
            if (base.Dirty)
            {
                if (!this.modifyPropertiesControl.CanApplyChanges())
                {
                    return false;
                }
                this.modifyPropertiesControl.UpdateData((ResultNode)base.ParentSheet.SelectionObject);
            }
            return true;
        }

        protected override bool OnOK()
        {
            string scopeNode = this._scopeNode;
            string text = "";
            string text2 = "";
            this.modifyPropertiesControl.GetKeyValue(out text, out text2);
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            string[] keys = sSO.GetKeys(scopeNode);
            string[] values = sSO.GetValues(scopeNode);
            string[] array = new string[keys.Length];
            string[] array2 = new string[values.Length];
            bool flag = false;
            if (!(text == "") && !(text2 == ""))
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == text)
                    {
                        flag = true;
                    }
                }
                for (int j = 0; j < keys.Length; j++)
                {
                    if (keys[j].ToUpper() == this._currentKeyName.ToUpper())
                    {
                        array[j] = text;
                        array2[j] = text2;
                    }
                    else
                    {
                        array[j] = keys[j];
                        array2[j] = values[j];
                    }
                }
                if (flag)
                {
                    if (!(text.ToUpper() == this._currentKeyName.ToUpper()))
                    {
                        this.modifyPropertiesControl.KeyExistException();
                        return true;
                    }
                    sSO.CreateApplicationFieldsValues(scopeNode, array, array2);
                }
                else
                {
                    sSO.CreateApplicationFieldsValues(scopeNode, array, array2);
                }
            }
            return this.OnApply();
        }

        protected override bool QueryCancel()
        {
            return true;
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnCancel()
        {
            this.modifyPropertiesControl.RefreshData((ResultNode)base.ParentSheet.SelectionObject);
        }
    }
}
