using bizilante.SSO.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSOHelper
{
    public partial class SSOHelperForm : Form
    {
        private string companyName;
        private string username;

        public SSOHelperForm()
        {
            InitializeComponent();
            companyName = ConfigurationManager.AppSettings["CompanyName"];
            username = ConfigurationManager.AppSettings["Username"];
            this.Text = string.Format("{0}@{1}.com", username, companyName);

            GetListOfSsoApplications();
        }

        private async void GetListOfSsoApplications()
        {
            List<SSOAppInfo> applications = null;
            await Task.Run(() =>
            {
                SSO sso = new SSO(username, companyName);
                applications = sso.GetListOfApplications();
            });
            SortableBindingList<SSOAppInfo> bindingList = new SortableBindingList<SSOAppInfo>(applications);
            dataGridApplications.DataSource = bindingList;
        }

        private void btnSetCompany_Click(object sender, EventArgs e)
        {
            GetCompanyForm form = new GetCompanyForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                companyName = form.CompanyName;
                username = form.Username;
                this.Text = string.Format("{0}@{1}.com", username, companyName);
                GetListOfSsoApplications();
            }
        }

        private async void dataGridApplications_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridApplications.SelectedRows.Count == 0) return;
            int rowIndex = dataGridApplications.SelectedRows[0].Index;
            DataGridViewRow row = dataGridApplications.Rows[rowIndex];
            // Get selected application data
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            string artefactName = string.Empty;
            await Task.Run(() =>
            {
                string appName = (row.DataBoundItem as SSOAppInfo).Name;
                string appDescription = (row.DataBoundItem as SSOAppInfo).Description;
                SSO sso = new SSO(username, companyName);
                Guid appGuid;
                if (!Guid.TryParse(appName, out appGuid))
                    sso.GetKeyValues((row.DataBoundItem as SSOAppInfo).Name, keyValues);
                else
                    artefactName = sso.GetAdapterConfig(appName, appDescription, keyValues);
            });
            SortableBindingList<SSOKeyValue> bindingList = new SortableBindingList<SSOKeyValue>(
                keyValues.Select(x => new SSOKeyValue { Key = x.Key, Value = x.Value }).ToList()
                );
            dataGridKeyValues.DataSource = bindingList;
            lblName.Text = artefactName;
        }
    }
}
