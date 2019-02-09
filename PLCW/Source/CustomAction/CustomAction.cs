using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.Installation
{
	/// <summary>
	/// basically installs (registers) and removes (unregisters and cleans up) 
	/// the BizTalk Pipeline Component Wizard
	/// </summary>
	[RunInstaller(true)]
	public class CustomActions : Installer
	{
		/// <summary>
		/// contains the BizTalk Server [version] installation folder
		/// </summary>
		private string _BizTalkInstallPath = null;
        /// <summary>
        /// contains the retrieved BizTalk Server target Visual Studio version from registry.
        /// </summary>
        private string _TargetVSVersion = null;
        /// <summary>
		/// contains the Visual Studio Wizard definition file location
		/// </summary>
		private string _BizTalkVszFileLocation = null;
		/// <summary>
		/// contains the path to the running .NET framework version for use of RegAsm.exe
		/// </summary>
		private string _DotNetFrameworkPath = null;
		/// <summary>
		/// stores any exception that might occur for review
		/// </summary>
		private Exception _Exception = null;
		/// <summary>
		/// defines whether the occured exception is a 'general' exception
		/// </summary>
		private bool _GeneralError = false;
		/// <summary>
		/// contains the Visual Studio installation folder
		/// </summary>
		private string _VisualStudioInstallPath = null;
		/// <summary>
		///  contains the path to the base folder where the Wizard definition file resides
		/// </summary>
		private string _VsDirPath = null;
		/// <summary>
		/// defines the Wizard definition file
		/// </summary>
		private const string vszFile = "BizTalkPipeLineComponentWizard.vsz";

		/// <summary>
		/// plain constructor, determines the locations of various of the used components
		/// (BizTalk Server, .NET framework, Visual Studio)
		/// </summary>
		public CustomActions()
		{
			// regkey will contain the opened registry key for retrieving data
			RegistryKey regkey;

			try
			{
				// retrieve the BizTalk Server installation folder
				string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
				regkey = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey);

				try
				{
					this._BizTalkInstallPath = regkey.GetValue("InstallPath").ToString();
                    this._TargetVSVersion = regkey.GetValue("TargetVSVersion").ToString();
					this._BizTalkVszFileLocation = Path.Combine(this._BizTalkInstallPath, string.Format(@"Developer Tools\BizTalkProjects\{0}", vszFile));

					regkey.Close();
				}
				catch
				{
                    base.Context.LogMessage(string.Format(@"Unable to locate BizTalk installation folder from registry. Tried InstallPath (2006/2009) in HKLM\{0}", bizTalkInstallRegistryKey));
				}

				// Visual studio installation folder
				this._VsDirPath = Path.Combine(this._BizTalkInstallPath, @"Developer Tools\BizTalkProjects\BTSProjects.vsdir");
				string vsInstallFolderRegistryKey = string.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}", this._TargetVSVersion);

				try
				{
					regkey = Registry.LocalMachine.OpenSubKey(vsInstallFolderRegistryKey);

					// set the actual Visual Studio installation folder for later use
					this._VisualStudioInstallPath = regkey.GetValue("InstallDir").ToString();

					regkey.Close();
				}
				catch
				{
					base.Context.LogMessage(string.Format("Unable to find Visual Studio installation path for version {0}", this._TargetVSVersion));
				}

				// .NET framework installation folder
				regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
				this._DotNetFrameworkPath = regkey.GetValue("InstallRoot").ToString();
				string frameworkVersion = string.Format("v{0}.{1}.{2}", Environment.Version.Major, Environment.Version.Minor, Environment.Version.Build);

				// the path to the .NET framework folder is in the form vx.y.z, where x.y.z is Major, Minor and Build
				// version of the framework. within the folder defined in HKLM\SOFTWARE\Microsoft\.NETFramework
				this._DotNetFrameworkPath = Path.Combine(this._DotNetFrameworkPath, frameworkVersion);

				regkey.Close();

			}
			catch(Exception e)
			{
				base.Context.LogMessage(e.Message);

				this._Exception = e;
				this._GeneralError = true;
			}
		}

		/// <summary>
		/// actually registers our wizard within the Visual Studio environment by adding a line to BTSProjects.vsdir
		/// </summary>
		/// <returns>whether the action actually succeeded</returns>
		private bool AddVsDirLine()
		{
			string vszLine = null;
			string definitionBuffer = null;

			try
			{
				Guid guid = new Guid("ef7e327e-cd33-11d4-8326-00c04fa0ce8d");
				vszLine = vszFile + "| |BizTalk Server Pipeline Component Project|300|Creates a BizTalk Server PipeLine component|{" + guid.ToString() + "}|226| |#133";
				// reset file attributes
				if ((File.GetAttributes(this._VsDirPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					File.SetAttributes(this._VsDirPath, FileAttributes.Normal);
				}
				using (StreamReader reader = new StreamReader(this._VsDirPath))
				{
					definitionBuffer = reader.ReadToEnd();
				}

				// only append the wizard line of not present
				if (definitionBuffer.IndexOf(vszFile) == -1)
				{
					using(StreamWriter writer = File.AppendText(this._VsDirPath))
					{
						writer.WriteLine(vszLine);
					}
				}

				// set the RO flag to the file
				File.SetAttributes(this._VsDirPath, FileAttributes.ReadOnly);
			}
			catch (Exception e)
			{
				return this.HandleError("AddVsDirLine", e);
			}

			return true;
		}

		/// <summary>
		/// creates the .vsz file. existing file is removed if need be
		/// </summary>
		/// <returns>whether the operation succeeded</returns>
		private bool AddVszFile()
		{
			try
			{
				this.RemoveVszFile();

				using (TextWriter writer = new StreamWriter(this._BizTalkVszFileLocation, false))
				{
					writer.WriteLine("VSWIZARD 7.0");
					writer.WriteLine("Wizard=VSWizard.BizTalkPipeLineComponentWizard");
					writer.WriteLine("Param=\"WIZARD_NAME = BizTalkPipeLineComponentWizard\"");
					writer.WriteLine("Param=\"WIZARD_UI = FALSE\"");
					writer.WriteLine("Param=\"PROJECT_TYPE = CSPROJ\"");
				}
			}
			catch (Exception e)
			{
				return this.HandleError("AddVszFile", e);
			}
			return true;
		}

		public override void Commit(IDictionary savedState)
		{
			base.Commit(savedState);

			try
			{
				if (!this._GeneralError)
				{
					return;
				}
				throw this._Exception;
			}
			catch (Exception e)
			{
				base.Context.LogMessage(e.Message);

				throw e;
			}
		}

		private bool HandleError(string functionName, Exception e)
		{
			base.Context.LogMessage(e.Message);

			DialogResult result2 = MessageBox.Show(e.ToString(), "PipelineComponentWizard Installer", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

			switch (result2)
			{
				case DialogResult.Abort:
				{
					throw e;
				}
				case DialogResult.Retry:
				{
					return false;
				}
				case DialogResult.Ignore:
				{
					if (MessageBox.Show("If you choose to ignore you will have to perform some actions manually as described in the readme. " + Environment.NewLine + "Continue?", "PipelineComponentWizard Installer - " + functionName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						throw e;
					}
					break;
				}
			}

			return true;
		}

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			
			try
			{
				if (this._GeneralError)
				{
					throw this._Exception;
				}
				while (!this.AddVszFile())
				{
				}
				while (!this.AddVsDirLine())
				{
				}
				while (!this.RegisterPipelineComponentWizard(false))
				{
				}
			}
			catch (Exception e)
			{
				base.Context.LogMessage(e.Message);

				throw e;
			}
		}

		/// <summary>
		/// performs the necessery steps to either register or remove the wizard
		/// </summary>
		/// <param name="unregister">whether the installer is registering or removing the wizards</param>
		/// <returns>whether the operation succeeded</returns>
		private bool RegisterPipelineComponentWizard(bool unregister)
		{
			string regAsmLocation;
			string regAsmArguments;
			ProcessStartInfo piInfo;
			Process process;

			try
			{
				// we use RegAsm.exe by spawning it just like the command-line would
				regAsmLocation = Path.Combine(this._DotNetFrameworkPath, "RegAsm.exe");

				// append /u if we're removing
				if (unregister)
				{
					regAsmLocation = regAsmLocation + " /u";
				}

				// format the RegAsm arguments
				regAsmArguments = string.Format("\"{0}\"", Path.Combine(base.Context.Parameters["ApplicationPath"], "PipelineComponentWizard.dll"));
				regAsmArguments += " /codebase /s";

				// create and run the command-line in the background
				piInfo = new ProcessStartInfo(regAsmLocation, regAsmArguments);
				piInfo.CreateNoWindow = true;
				piInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process = Process.Start(piInfo);
				process.WaitForExit();
			}
			catch (Exception e)
			{
				if (!unregister)
				{
					return this.HandleError("RegisterPipelineComponentWizard", e);
				}

				return true;
			}

			return true;
		}

		/// <summary>
		/// removes the wizard definition file
		/// </summary>
		private void RemoveVszFile()
		{
			FileInfo fi;
			try
			{
				fi = new FileInfo(this._BizTalkVszFileLocation);
				if (!fi.Exists)
				{
					return;
				}
				fi.Delete();
			}
			catch (Exception e)
			{
				base.Context.LogMessage(e.Message);
			}
		}

		public override void Rollback(IDictionary savedState)
		{
			try
			{
				if (this._GeneralError)
				{
					throw this._Exception;
				}

				this.RemoveVszFile();
			}
			catch (Exception e)
			{
				base.Context.LogMessage(e.Message);
			}
			base.Rollback(savedState);
		}

		public override void Uninstall(IDictionary savedState)
		{
			try
			{
				if (this._GeneralError)
				{
					throw this._Exception;
				}

				this.RemoveVszFile();
				this.RegisterPipelineComponentWizard(true);
			}
			catch (Exception e)
			{
				base.Context.LogMessage(e.Message);
			}
			base.Uninstall(savedState);
		}
	}
}