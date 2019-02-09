using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO
{
    internal class SSOHelper
    {
        public static bool ExportSSOApplication(string appName, string encryptionKey, ApplicationScopeNode currentNode)
        {
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            string[] keys = sSO.GetKeys(appName);
            string[] values = sSO.GetValues(appName);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?><SSOApplicationExport><applicationData>");
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] != null && !(keys[i] == ""))
                {
                    stringBuilder.Append(string.Concat(new string[]
                    {
                        "<add key=\"",
                        keys[i],
                        "\" value=\"",
                        HttpUtility.HtmlEncode(values[i]),
                        "\" />"
                    }));
                }
            }
            stringBuilder.Append("</applicationData></SSOApplicationExport>");
            bool result;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.Filter = "SSO Extract files (*.sso)|*.sso|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "*.sso";
                saveFileDialog.FileName = appName + ".sso";
                saveFileDialog.Title = "Export SSO Application";
                DialogResult dialogResult = currentNode.SnapIn.Console.ShowDialog(saveFileDialog);
                if (dialogResult != DialogResult.Cancel)
                {
                    StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false);
                    try
                    {
                        streamWriter.Write(bizilante.SSO.Helper.SSO.Encrypt(stringBuilder.ToString(), encryptionKey));
                        streamWriter.Flush();
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry("SSO MMC SnapIn - ExportSSOApplication \r\n", ex.ToString());
                        result = false;
                        return result;
                    }
                    finally
                    {
                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                }
                result = true;
            }
            return result;
        }

        public static bool ImportSSOApplication(string encryptionKey, string applicationFileName, string encryptedText)
        {
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            return sSO.ImportSSOApplication(encryptionKey, applicationFileName, encryptedText);
        }

        public static DialogResult OpenSSOImportFile(out string applicationFileName, out string encryptedText, ApplicationScopeNode callingNode)
        {
            DialogResult dialogResult = DialogResult.None;
            applicationFileName = string.Empty;
            encryptedText = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.DefaultExt = "*.sso";
                openFileDialog.Filter = "SSO Extract files (*.sso)|*.sso|All files (*.*)|*.*";
                openFileDialog.Title = "Import SSO Application";
                dialogResult = callingNode.SnapIn.Console.ShowDialog(openFileDialog);
                if (dialogResult == DialogResult.Cancel)
                {
                    return dialogResult;
                }
                applicationFileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                StreamReader streamReader = new StreamReader(openFileDialog.OpenFile());
                try
                {
                    encryptedText = streamReader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("SSO MMC SnapIn - ImportSSOApplication \r\n", ex.ToString());
                    throw;
                }
                finally
                {
                    streamReader.Dispose();
                }
            }
            return dialogResult;
        }

    }
}
