using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeploymentDbFormsControlLibrary
{
    /// <summary>
    /// Display a Form to warn the user that the application version (s)he is deploying is older than 
    /// the currently deployed version
    /// </summary>
    public partial class VersionWarning : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appliction">BizTalk application name</param>
        /// <param name="version">Current BizTalk application version</param>
        /// <param name="dbversion">Current BizTalk application version (dixit DeploymentDb)</param>
        /// <param name="newversion">The tobe deployed BizTalk application version</param>
        /// <returns>A new VersionWarning form to display or NULL there is no warning.</returns>
        /// <exception cref="ArgumentNullException">application, version and newversion cannot be empty or null</exception>
        public static VersionWarning Create(string appliction, string version, string dbversion, string newversion)
        {
            #region Argument validation
            if (string.IsNullOrEmpty(appliction))
                throw new ArgumentNullException("application", "Value cannot be empty or null");
            if (string.IsNullOrEmpty(appliction))
                throw new ArgumentNullException("version", "Value cannot be empty or null");
            if (string.IsNullOrEmpty(appliction))
                throw new ArgumentNullException("newversion", "Value cannot be empty or null");
            if (version.Split(new string[] { "." }, StringSplitOptions.None).Length != 4)
                throw new ArgumentException("version", "Version not in the form 'major.minor.build.revision'");
            if (newversion.Split(new string[] { "." }, StringSplitOptions.None).Length != 4)
                throw new ArgumentException("version", "Version not in the form 'major.minor.build.revision'");
            #endregion

            // Is there a warning?
            if (!ShouldWarn(version, newversion))
                return null;

            bizilante.Windows.Forms.DeploymentDbFormsControlLibrary.DeploymentVersion deploymentVersion = 
                new bizilante.Windows.Forms.DeploymentDbFormsControlLibrary.DeploymentVersion
                {
                Application = appliction,
                Version = version,
                NewVersion = newversion,
                DbVersion = dbversion
            };
            return new VersionWarning(deploymentVersion);

        }

        public VersionWarning(bizilante.Windows.Forms.DeploymentDbFormsControlLibrary.DeploymentVersion version)
        {
            InitializeComponent();
            this.bindingSourceVersion.DataSource = version;
        }

        /// <summary>
        /// Returns true if the new version is older
        /// </summary>
        /// <param name="version">Current version</param>
        /// <param name="newversion">New version</param>
        /// <returns>true or false</returns>
        private static bool ShouldWarn(string version, string newversion)
        {
            // 0.0.0.0 = not yet deployed and visible in the BizTalk Management Console
            if (version == "0.0.0.0") return true;

            string[] versionarray = version.Split(new string[] { "." }, StringSplitOptions.None);
            string[] newversionarray = newversion.Split(new string[] { "." }, StringSplitOptions.None);

            int[] versions = new int[] { 0, 0, 0, 0 };
            int[] newversions = new int[] { 0, 0, 0, 0 };

            for (int i = 0; i < versionarray.Length; i++)
                versions[i] = int.Parse(versionarray[i]);
            for (int i = 0; i < newversionarray.Length; i++)
                newversions[i] = int.Parse(newversionarray[i]);

            // If current > new return true
            if (versions[0] > newversions[0])
                return true;
            else if (versions[0] == newversions[0])
            {
                if (versions[1] > newversions[1])
                    return true;
                else if (versions[1] == newversions[1])
                {
                    if (versions[2] > newversions[2])
                        return true;
                    else if (versions[2] == newversions[2])
                        if (versions[3] > newversions[3]) return true;
                }
            }

            // the new version is no problem
            return false;
        }
    }
}
