using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using bizilante.Helpers.LogDeployment.Properties;

namespace bizilante.Helpers.LogDeployment
{
    public class Utils
    {
        public static string GetDeploymentDb()
        {
            string deploymentDb = string.Empty;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                deploymentDb = string.Format("{0}:{1}", conn.DataSource, conn.Database);
            }
            return deploymentDb;
        }

        public static long InsertIntoDeployment(string group, string environment, string user, DateTime dtDate, string application, string version, string action, string note)
        {
            long id = -1;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.InsertDeployment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_Group", group);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Environment", environment);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_User", user);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Date", dtDate);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Application", application);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Version", version);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Action", action);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Note", note);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_id_deployment", SqlDbType.BigInt);
                sqlParam.Direction = ParameterDirection.Output;
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                    id = (long)cmd.Parameters["@p_id_deployment"].Value;
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.InsertDeployment failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.InsertDeployment failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return id;
        }

        public static bool InsertIntoLog(long id_deployment, string step, DateTime dtDate, string description)
        {
            bool bSuccess = false;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.InsertLog", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_id_deployment", id_deployment);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_step", step);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_datetime", dtDate);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_description", description);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                    bSuccess = true;
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.InsertLog failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.InsertLog failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return bSuccess;
        }

        public static bool InsertIntoLogFile(long id_deployment, string logInfo)
        {
            bool bSuccess = false;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.InsertLogFile", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_id_deployment", id_deployment);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_info", logInfo);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                    bSuccess = true;
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.InsertLogFile failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.InsertLogFile failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return bSuccess;
        }

        public static void SetApplicationVersion(string application, string version)
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.SetApplicationVersion", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_Application", application);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Version", version);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.SetApplicationVersion failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.SetApplicationVersion failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        public static void SetEndDeployment(long id_deployment, DateTime dtDate, bool failed, string error)
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.SetEndDeployment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_id_deployment", id_deployment);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_date", dtDate);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_failed", failed);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_error", error);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.SetEndDeployment failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.SetEndDeployment failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        public static long InsertIntoPackage(long id_deployment, string title, string author, string subject, string comments, string keywords, string createtime, string packagecode, string productcode, string application)
        {
            long id = -1;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.InsertPackage", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_Title", title);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_ID_Deployment", id_deployment);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Author", author);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Subject", subject);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Comments", comments);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Keywords", keywords);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Createtime", createtime);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Packagecode", packagecode);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Productcode", productcode);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Application", application);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_id_package", SqlDbType.BigInt);
                sqlParam.Direction = ParameterDirection.Output;
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                    id = (long)cmd.Parameters["@p_id_package"].Value;
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.InsertPackage failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.InsertPackage failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return id;
        }

        public static bool InsertIntoFiles(long id_package, string resourcetype, string luid, string filename, string version)
        {
            bool bSuccess = false;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.InsertFile", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_id_package", id_package);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_resourcetype", resourcetype);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_luid", luid);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_filename", filename);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_version", version);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                    bSuccess = true;
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.InsertFile failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.InsertFile failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return bSuccess;
        }

        public static void SetUnInstallPackage(string group, string application, string productcode, string user, DateTime dtDate)
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("dbo.SetUninstallPackage", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_Group", group);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Application", application);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_ProductCode", productcode);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_UnInstallDateTime", dtDate);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_UnInstallUser", user);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    int num = cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("dbo.SetUninstallPackage failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("dbo.SetUninstallPackage failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        public static void LogUnInstallApp(string group, string applicationname, string user, DateTime dtDate)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\", true))
            {
                try
                {
                    using (RegistryKey key2 = key.OpenSubKey(applicationname))
                    {
                        if (key2 == null)
                            return;

                        if (!key2.GetValue("Uninstallstring").ToString().Contains("BtsTask.exe UninstallApp"))
                            return;

                        string[] subKeyNames = key2.GetSubKeyNames();
                        for (int i = subKeyNames.Length - 1; i >= 0; i--)
                        {
                            Guid guid = new Guid(subKeyNames[i]);
                            SetUnInstallPackage(group, applicationname, guid.ToString("B"), user, dtDate);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format("Exception when uninstalling application {0} on {1}: [{2}]", applicationname, group, exception.Message), exception);
                }
            }
        }

        public static string GetApplicationVersion(string group, string application)
        {
            string version = string.Empty;
            using (SqlConnection conn = new SqlConnection(Settings.Default.DeploymentDb))
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 grp.[Group], app.[Application], dep.[Version], dep.DeploymentDate FROM [dbo].[Deployment] dep with (nolock) JOIN dbo.[Application] app with (nolock) ON app.ID = dep.ID_Application JOIN dbo.[Group] grp with (nolock) ON grp.ID = dep.ID_Group where dep.Failed = 0 AND dep.[Action] IN ('Install', 'Initialisation') and app.[Application] = @p_Application AND grp.Environment = @p_Group ORDER BY dep.ID DESC", conn);
                cmd.CommandType = System.Data.CommandType.Text;

                SqlParameterCollection sqlColl = cmd.Parameters;
                SqlParameter sqlParam = new SqlParameter("@p_Application", application);
                sqlColl.Add(sqlParam);
                sqlParam = new SqlParameter("@p_Group", group);
                sqlColl.Add(sqlParam);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows && reader.Read())
                        version = reader.GetString(2);
                    reader.Close();
                }
                catch (SqlException sqlex)
                {
                    string message = string.Empty;
                    foreach (SqlError e in sqlex.Errors)
                        message += e.Message + Environment.NewLine;
                    throw new Exception(string.Format("GetApplicationVersion select failed : {0}", message), sqlex);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("GetApplicationVersion failed : {0}", ex.Message), ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            return version;
        }
    }
}
