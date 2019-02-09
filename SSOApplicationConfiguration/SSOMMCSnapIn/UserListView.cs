using bizilante.ManagementConsole.SSO.Properties;
using bizilante.ManagementConsole.SSO.PropertyPages;
using Microsoft.ManagementConsole;
using System;
using System.Collections;
using System.Text;

namespace bizilante.ManagementConsole.SSO
{
    internal class UserListView : MmcListView
    {
        private string _applicationName;

        private static event EventHandler<EventArgs<string>> ListViewChanged;

        protected override void OnShow()
        {
            Refresh();
        }

        protected override void OnInitialize(AsyncStatus status)
        {
            ListViewChanged += UserListView_ListViewChanged;
            this._applicationName = base.ScopeNode.LanguageIndependentName;
            base.Columns[0].Title = "Key";
            base.Columns[0].SetWidth(300);
            base.Columns.Add(new MmcListViewColumn("Value", 350));
            base.Mode = MmcListViewMode.Report;
            base.SnapIn.SmallImages.Add(Resources.add_scope);
            base.OnInitialize(status);
            base.ActionsPaneItems.Add(new Microsoft.ManagementConsole.Action("Refresh", "refresh", -1, "Refresh"));
        }

        protected override void OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        {
            string a;
            if ((a = (string)action.Tag) != null)
            {
                if (!(a == "Refresh"))
                {
                    return;
                }
                this.Refresh();
            }
        }

        protected override void OnSelectionChanged(SyncStatus status)
        {
            this._applicationName = base.ScopeNode.LanguageIndependentName;
            int count = base.SelectedNodes.Count;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ResultNode resultNode in ((IEnumerable)base.SelectedNodes))
            {
                stringBuilder.Append(resultNode.DisplayName + " AND " + resultNode.SubItemDisplayNames[0]);
            }
            if (count == 0)
            {
                base.SelectionData.Clear();
                base.SelectionData.ActionsPaneItems.Clear();
                return;
            }
            base.SelectionData.Update((ResultNode)base.SelectedNodes[0], count > 1, null, null);
            base.SelectionData.ActionsPaneItems.Clear();
            base.SelectionData.ActionsPaneItems.Add(
                new Microsoft.ManagementConsole.Action("Properties", "Properties", -1, "Properties"));
            base.SelectionData.ActionsPaneItems.Add(
                new Microsoft.ManagementConsole.Action("Delete", "Deletes the key/value pair from the application", -1, "Delete"));
        }

        protected override void OnSelectionAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        {
            string a;
            if ((a = (string)action.Tag) != null)
            {
                if (a == "Properties")
                {
                    base.SelectionData.ShowPropertySheet("Key/Value Properties");
                    return;
                }
                if (!(a == "Delete"))
                {
                    return;
                }
                StringBuilder stringBuilder = new StringBuilder();
                foreach (ResultNode resultNode in ((IEnumerable)base.SelectedNodes))
                {
                    base.ResultNodes.Remove(resultNode);
                    stringBuilder.Append(resultNode.DisplayName);
                }
                bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
                string[] keys = sSO.GetKeys(base.ScopeNode.DisplayName);
                string[] values = sSO.GetValues(base.ScopeNode.DisplayName);
                string[] array = new string[keys.Length - 1];
                string[] array2 = new string[values.Length - 1];
                string b = stringBuilder.ToString();
                int num = 0;
                if (keys.Length == 1)
                {
                    array = new string[1];
                    array2 = new string[1];
                    array[0] = "";
                    array2[0] = "";
                }
                else
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (!(keys[i] == b))
                        {
                            array[num] = keys[i];
                            array2[num] = values[i];
                            num++;
                        }
                    }
                }
                sSO.CreateApplicationFieldsValues(base.ScopeNode.DisplayName, array, array2);
            }
        }

        protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        {
            if (base.SelectedNodes.Count == 0)
            {
                throw new Exception("There should be at least one node selection");
            }
            propertyPageCollection.Add(new ModifyPropertyPage(base.ScopeNode.LanguageIndependentName));
        }

        protected internal static void OnListViewChanged(string displayName)
        {
            UserListView.ListViewChanged?.Invoke(null, new EventArgs<string>(displayName));
        }

        public void Refresh()
        {
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            string displayName = base.ScopeNode.DisplayName;
            string[] keys = sSO.GetKeys(displayName);
            string[] values = sSO.GetValues(displayName);
            base.ResultNodes.Clear();
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] != null && !(keys[i] == ""))
                {
                    ResultNode resultNode = new ResultNode();
                    resultNode.DisplayName = keys[i];
                    resultNode.SubItemDisplayNames.Add(values[i]);
                    base.ResultNodes.Add(resultNode);
                }
            }
        }

        private void UserListView_ListViewChanged(object sender, EventArgs<string> e)
        {
            if (base.ScopeNode.DisplayName == e.Value)
            {
                this.Refresh();
            }
        }
    }
}
