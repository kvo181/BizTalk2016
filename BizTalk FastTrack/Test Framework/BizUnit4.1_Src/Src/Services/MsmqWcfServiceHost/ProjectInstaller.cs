using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace MsmqWcfServiceHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                var svc = new ServiceController("MsmqWcfWindowsService");
                if (svc.Status != ServiceControllerStatus.Stopped)
                    throw new Exception(string.Format("{0}: Invalid status = {1}", "MsmqWcfWindowsService", svc.Status));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }

        private void ProjectInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                var svc = new ServiceController("MsmqWcfWindowsService");
                if (svc.Status != ServiceControllerStatus.Stopped)
                    svc.Stop();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }
    }
}
