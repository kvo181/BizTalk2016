using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Microsoft.BizTalk.ApplicationDeployment;
using System.Diagnostics;

namespace bizilante.Windows.Forms.Controls.ListPackageFormsControlLibrary
{
    public partial class UserControlInstalledPackages : UserControl
    {
        private List<string> _fileNames;
        /// <summary>
        /// List of MSI files
        /// </summary>
        public List<string> FileNames
        {
            get { return _fileNames; }
            set 
            { 
                _fileNames = value;
                SetDataSource();
            }
        }

        public UserControlInstalledPackages()
        {
            InitializeComponent();
        }

        private void installedPackagesBindingSource_PositionChanged(object sender, EventArgs e)
        {
            if (installedPackagesBindingSource.Current != null)
            {
                DataRowView drvw = installedPackagesBindingSource.Current as DataRowView;
                InstalledPackages.PackageRow rw = drvw.Row as InstalledPackages.PackageRow;
                if (rw != null)
                    installedResoucesBindingSource.Filter = string.Format("PackageCode = '{0}'", rw.PackageCode);
            }
        }

        private void toolStripMenuItemShowContent_Click(object sender, EventArgs e)
        {
            if (installedResoucesBindingSource.Current == null) return;
            // What do we want to show?
            DataRowView drvw = installedResoucesBindingSource.Current as DataRowView;
            InstalledPackages.ResourcesRow rw = drvw.Row as InstalledPackages.ResourcesRow;
            switch (rw.Type)
            {
                case "System.BizTalk:Assembly":
                case "System.BizTalk:BizTalkAssembly":
                    // Use reflection to visualize the resource
                    break;
                default:
                    // Use notepad to visualize the resource
                    break;
            }
        }

        private void SetDataSource()
        {
            installedPackages.Clear();
            if ((null == FileNames) || (FileNames.Count == 0)) return;

            foreach (string filename in FileNames)
            {
                Dictionary<string, string> properties;
                string path = Helpers.ListPackageHelper.Helper.ExtractFiles(filename, out properties);
                DirectoryInfo di = new DirectoryInfo(path);
                ListPackageContent(filename, di, properties);
                di.Delete(true);
            }

            // Establish a relationship between the two tables.
            DataRelation relation = new DataRelation("PackageResources",
                installedPackages.Tables["Package"].Columns["PackageCode"],
                installedPackages.Tables["Resources"].Columns["PackageCode"]);
            if (!installedPackages.Relations.Contains("PackageResources"))
                installedPackages.Relations.Add(relation);

            // Bind the master data connector to the Customers table.
            installedPackagesBindingSource.DataSource = installedPackages;
            installedPackagesBindingSource.DataMember = "Package";

            // Bind the details data connector to the master data connector,
            // using the DataRelation name to filter the information in the 
            // details table based on the current row in the master table. 
            installedResoucesBindingSource.DataSource = installedPackages;
            installedResoucesBindingSource.DataMember = "Resources";
        }

        private void ListPackageContent(string msiPath, DirectoryInfo di, Dictionary<string, string> properties)
        {
            IInstallPackage package = null;
            try
            {
                package = DeploymentUnit.ScanPackage(msiPath);
                if (package != null)
                {
                    string productCode = string.Empty;
                    if ((null != properties) && properties.ContainsKey("ProductCode"))
                        productCode = properties["ProductCode"];
                    this.installedPackages.Package.AddPackageRow(
                        package.Title,
                        package.Author,
                        package.Subject,
                        package.Comments,
                        package.CreateTime,
                        package.RevisionNumber,
                        productCode);

                    if ((package.Resources != null) && (package.Resources.Length != 0))
                    {
                        foreach (IDeploymentResource resource in package.Resources)
                        {
                            string filename = string.Empty;
                            string version = string.Empty;
                            // Get the corresponding file
                            if (resource.Properties.ContainsKey("DestinationLocation"))
                            {
                                filename = (string)resource.Properties["DestinationLocation"];
                                if (!string.IsNullOrEmpty(filename))
                                {
                                    FileInfo fi = new FileInfo(filename);
                                    FileInfo[] fis = di.GetFiles(fi.Name, SearchOption.AllDirectories);
                                    if (fis.Length > 0)
                                    {
                                        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fis[0].FullName);
                                        version = versionInfo.FileVersion;
                                    }
                                }
                            }

                            this.installedPackages.Resources.AddResourcesRow(
                                resource.ResourceType,
                                resource.Luid,
                                filename,
                                version,
                                package.RevisionNumber);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("ListPackageContent Error occured: {0}", exception.Message), exception);
            }
        }
    }
}
