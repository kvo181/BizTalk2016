using EnvDTE;
using EnvDTE80;
using Microsoft.BizTalk.Wizard;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VSLangProj;
using System.Linq;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	public delegate void AddWizardResultEvent(object sender,PropertyPairEvent e);
	public delegate void AddTransmitHandlerPropertyEvent(object sender,PropertyPairEvent e);
	public delegate void AddDesignerPropertyEvent(object sender,PropertyPairEvent e);
	
	/// <summary>
	/// List of constants to find values in the namevaluecollection
	/// </summary>
	internal class WizardValues
	{
		/// <summary>
		/// defines the version of the component, as entered by the user
		/// </summary>
		public const string ComponentVersion = "ComponentVersion";
		/// <summary>
		/// defines the classname, as entered by the user
		/// </summary>
		public const string ClassName = "ClassName";
		/// <summary>
		/// defines the description (single-line) of the component, as entered by the user
		/// </summary>
		public const string ComponentDescription = "ComponentDescription";
		/// <summary>
		/// defines the namespace in which the component should reside, as entered by the user
		/// </summary>
		public const string Namespace = "Namespace";
		/// <summary>
		/// defines the component name, as entered by the user
		/// </summary>
		public const string ComponentName = "ComponentName";
		/// <summary>
		/// defines the default namespace for the newly created project, as entered by the user
		/// </summary>
		public const string NewProjectNamespace = "NewProjectNamespace";
		/// <summary>
		/// defines the icon this component will display within the toolbox of Visual Studio
		/// </summary>
		public const string ComponentIcon = "ComponentIcon";
		/// <summary>
		/// defines the type of pipeline component the user wishes to have generated
		/// </summary>
		public const string PipelineType = "PipelineType";
		/// <summary>
		/// defines the stage in which the user would like it's generated
		/// pipeline component to reside
		/// </summary>
		public const string ComponentStage = "ComponentStage";
		/// <summary>
		/// defines whether the user wants to let the wizard implement the IProbeMessage
		/// interface, which allows the pipeline component to determine for itself whether
		/// it's interested in processing an inbound message
		/// </summary>
		public const string ImplementIProbeMessage = "ImplementIProbeMessage";
		/// <summary>
		/// defines the programming languages in which the pipeline component should
		/// be implemented, as choosen by the user
		/// </summary>
		public const string ImplementationLanguage = "ImplementationLanguage";
	}

	/// <summary>
	/// defines the types of pipeline components we support
	/// see SDK\Include\Pipeline_Int.idl
	/// </summary>
	internal enum componentTypes
	{
		/// <summary>
		/// links to CATID_Decoder
		/// </summary>
		Decoder = 0,
		/// <summary>
		/// links to CATID_DisassemblingParser
		/// </summary>
		DisassemblingParser,
		/// <summary>
		/// links to CATID_Validate
		/// </summary>
		Validate,
		/// <summary>
		/// links to CATID_PartyResolver
		/// </summary>
		PartyResolver,
		/// <summary>
		/// links to CATID_Any
		/// </summary>
		Any,

		/// <summary>
		/// links to CATID_Encoder
		/// </summary>
		Encoder,
		//PreAssembler,	// BUG: Pre-Assembler has no specific CATID associated
		/// <summary>
		/// links to CATID_AssemblingSerializer
		/// </summary>
		AssemblingSerializer,
	}

	/// <summary>
	/// defines the supported languages we generate sourcecode for
	/// </summary>
	internal enum implementationLanguages
	{
		CSharp = 0,
		VBNet = 1
	}

	/// <summary>
	/// Class (com-object) called by VS2003.NET to start a new pipeline component
	/// project.
	/// </summary>
	[ProgId("VSWizard.BizTalkPipeLineComponentWizard")]
    [Guid("4A8C4088-9461-4890-8741-FE017B111AA0")]
	public class BizTalkPipeLineWizard : IDTWizard
	{

		enum ContextOptions : int
		{
			WizardType,
			ProjectName, 
			LocalDirectory,
			InstallationDirectory,
			FExclusive,
			SolutionName,
			Silent
		}

		private string _ClassName = null;
		private string _NewProjectNamespace = null;
		private string _Namespace = null;
		private string _ComponentDescription = null;

		private string _BizTalkInstallPath = null;
        private string _TargetVSVersion = null;
        private string _VisualStudioInstallPath = null;
		private string _ProjectDirectory = null;
		private string _ProjectName = null;
		private string _SolutionName = null;

		private const string _ProjectNamespace = "MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard";
		private DTE2 _Application = null;

		bool _FExclusive;
		private Solution2 _PipelineComponentSolution = null;

		private Hashtable _WizardResults = null;
		private Hashtable _DesignerProperties = null;
		
		public BizTalkPipeLineWizard()
		{
			const string BizTalkKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
			RegistryKey BizTalkReg = Registry.LocalMachine.OpenSubKey(BizTalkKey);
			_BizTalkInstallPath = BizTalkReg.GetValue("InstallPath").ToString();
			BizTalkReg.Close();
		}

		/// <summary>
		/// Main function for wizard project. Calls the wizard-
		/// form and then delegates control to the createsolution function
		/// </summary>
		/// <param name="Application"></param>
		/// <param name="hwndOwner"></param>
		/// <param name="ContextParams"></param>
		/// <param name="CustomParams"></param>
		/// <param name="retval"></param>
		public void Execute(object Application, int hwndOwner, ref object[] ContextParams, ref object[] CustomParams, ref EnvDTE.wizardResult retval)
		{
			_DTE IDEObject = (_DTE)Application;
			_Application = (DTE2)Application;

			try
			{
				PipeLineComponentWizardForm WizardForm = new PipeLineComponentWizardForm();
				if (WizardForm.ShowDialog() == DialogResult.OK)
				{
					//Retrieve the wizard data
					_WizardResults = WizardForm.WizardResults;
					//_TransmitHandlerProperties = WizardForm.TransmitHandlerProperties;
					_DesignerProperties = WizardForm.DesignerProperties;
                    // Default Designer Property = Enabled (boolean)
                    if (!_DesignerProperties.ContainsKey("Enabled"))
                        _DesignerProperties.Add("Enabled", "bool");
					//Create the solution
					CreateSolution(IDEObject,ContextParams);
					retval = wizardResult.wizardResultSuccess;
				}
				else
				{
					retval = wizardResult.wizardResultCancel;					
					return;
				}

			}
			catch(Exception err)
			{
				Trace.WriteLine(err.ToString());	
				MessageBox.Show(err.ToString());
				retval = wizardResult.wizardResultFailure;
			}
		}
	
		/// <summary>
		/// Creates the solution and calls the functions to create the projects
		/// </summary>
		/// <param name="IDEObject"></param>
		/// <param name="ContextParams"></param>
		public void CreateSolution(_DTE IDEObject,object[] ContextParams)
		{
			TraceAllValues(ContextParams);

			//Get the "official" wizard results
			_ProjectDirectory = ContextParams[(int)ContextOptions.LocalDirectory].ToString();
			_ProjectName = ContextParams[(int)ContextOptions.ProjectName].ToString();
			_SolutionName = ContextParams[(int)ContextOptions.SolutionName].ToString();
			_FExclusive = bool.Parse(ContextParams[(int)ContextOptions.FExclusive].ToString());	

			//Get the custom wizard results
			_ClassName = (string) _WizardResults[WizardValues.ClassName];
			_NewProjectNamespace = (string) _WizardResults[WizardValues.NewProjectNamespace];
			_Namespace = (string) _WizardResults[WizardValues.Namespace];
			_ComponentDescription = (string) _WizardResults[WizardValues.ComponentDescription];
            
			if (!_FExclusive)//New solution or existing?
			{
				// Get a reference to the solution from the IDE Object
                _PipelineComponentSolution = (Solution2)IDEObject.Solution;
			}
			else
			{
				// Use the solution class to create a new solution 
                _PipelineComponentSolution = (Solution2)IDEObject.Solution;
				_PipelineComponentSolution.Create(_ProjectDirectory, _ProjectName);
			}

            SaveSolution();

			//Create the projects
			CreateProject(_PipelineComponentSolution);

            SaveSolution();
		}

        private void SaveSolution()
        {
            if (!Directory.Exists(_ProjectDirectory)) Directory.CreateDirectory(_ProjectDirectory);

            // Save the solution file
            _PipelineComponentSolution.SaveAs(_PipelineComponentSolution.FileName.Length == 0 ? (_ProjectDirectory + @"\" + _ProjectName + ".sln") : _PipelineComponentSolution.FileName);
        }

		/// <summary>
		/// Creates the designtime project and adds the appropriate files to the
		/// project.
		/// </summary>
		public void CreateProject(Solution2 mySolution)
		{
			// first, retrieve the visual studio installation folder
			RegistryKey regkey;

            // notice no try/catch, we want exceptions bubbling up as there's really nothing
            // we can do about them here.

			// retrieve the BizTalk Server installation folder
			string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
			regkey = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey);

			this._BizTalkInstallPath = regkey.GetValue("InstallPath").ToString();
            // This is no longer present in the registry under the BizTalk Server key
            this._TargetVSVersion = "14.0"; // regkey.GetValue("TargetVSVersion").ToString();

			regkey.Close();

			// Visual studio installation folder
            string vsInstallFolderRegistryKey = string.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}_Config", this._TargetVSVersion);
			regkey = Registry.CurrentUser.OpenSubKey(vsInstallFolderRegistryKey);

			// set the actual Visual Studio installation folder for later use
			this._VisualStudioInstallPath = regkey.GetValue("InstallDir").ToString();

			regkey.Close();

			string projectTemplate = null;
			string projectFileName = null;
			string classFileExtension = null;
			
			implementationLanguages language = (implementationLanguages) _WizardResults[WizardValues.ImplementationLanguage];

			switch(language)
			{
				case implementationLanguages.CSharp:
					projectTemplate = (mySolution as Solution2).GetProjectTemplate("ClassLibrary.zip", "CSharp");
                    projectFileName = _ProjectName + ".csproj";
					classFileExtension = ".cs";
					
                    break;
				case implementationLanguages.VBNet:
                    projectTemplate = (mySolution as Solution2).GetProjectTemplate("ClassLibrary.zip", "VisualBasic");
                    projectFileName = _ProjectName + ".vbproj";
					classFileExtension = ".vb";
					
                    break;
				default:
					MessageBox.Show(String.Format("Language \"{0}\" not supported", language));
					return;
			}


            var startingProjects = (from Project p in this._Application.Solution.Projects
                                          select p).ToArray();

            // add the specified project to the solution
			mySolution.AddFromTemplate(projectTemplate, _ProjectDirectory, _ProjectName, this._FExclusive);

            Project pipelineComponentProject = null;

            // Query for te added project
            pipelineComponentProject = (from Project p in this._Application.Solution.Projects
                                            where !startingProjects.Any(s => s.UniqueName == p.UniqueName)
                                            select p).First();

            // delete the Class1.cs|vb|jsharp|... the template adds to the project
            pipelineComponentProject.ProjectItems.Item("Class1" + classFileExtension).Delete();

            // adjust project properties
            pipelineComponentProject.Properties.Item("RootNameSpace").Value = (string)_WizardResults[WizardValues.Namespace];
            pipelineComponentProject.Properties.Item("AssemblyName").Value = (string)_WizardResults[WizardValues.ClassName];

			// Get a reference to the Visual Studio Project and 
			// use it to add a reference to the framework assemblies
			VSProject PipelineComponentVSProject = (VSProject)pipelineComponentProject.Object;
            if (null != PipelineComponentVSProject.Project)
            {
                foreach (Property prop in PipelineComponentVSProject.Project.Properties)
                {
                    switch (prop.Name)
                    {
                        case "TargetFrameworkMoniker":
                            prop.Value = ".NETFramework,Version=v4.5";
                            break;
                        case "TargetFramework":
                            prop.Value = (0x040005).ToString();
                            break;
                    }
                }
            }

            //PipelineComponentVSProject.Project.Properties.Item("TargetFrameworkMoniker").Value = ".NETFramework,Version=v4.5";
            //PipelineComponentVSProject.Project.Properties.Item("TargetFramework").Value = (0x040005).ToString();

			PipelineComponentVSProject.References.Add("System.dll");
			PipelineComponentVSProject.References.Add("System.Xml.dll");
			PipelineComponentVSProject.References.Add("System.Drawing.dll");
			PipelineComponentVSProject.References.Add(Path.Combine(_BizTalkInstallPath, @"Microsoft.BizTalk.Pipeline.dll"));
			PipelineComponentVSProject.References.Add(Path.Combine(_BizTalkInstallPath, @"Microsoft.BizTalk.Messaging.dll"));
            PipelineComponentVSProject.References.Add(Path.Combine(_BizTalkInstallPath, @"Microsoft.BizTalk.Streaming.dll"));

			// add our resource bundle
			string resourceBundle = Path.Combine(_ProjectDirectory, ((string) _WizardResults[WizardValues.ClassName]) + ".resx");
			ResXResourceWriter resx = new ResXResourceWriter(resourceBundle);
			resx.AddResource("COMPONENTNAME", _WizardResults[WizardValues.ComponentName] as string);
			resx.AddResource("COMPONENTDESCRIPTION", _WizardResults[WizardValues.ComponentDescription] as string);
			resx.AddResource("COMPONENTVERSION", _WizardResults[WizardValues.ComponentVersion] as string);
			resx.AddResource("COMPONENTICON", _WizardResults[WizardValues.ComponentIcon]);
			resx.Close();

			pipelineComponentProject.ProjectItems.AddFromFile(resourceBundle);

			// get the enum value of our choosen component type
			componentTypes componentType = (componentTypes) Enum.Parse(typeof(componentTypes), _WizardResults[WizardValues.ComponentStage] as string);

			// create our actual class
			string pipelineComponentSourceFile = Path.Combine(_ProjectDirectory, ((string) _WizardResults[WizardValues.ClassName]) + classFileExtension);
			PipelineComponentCodeGenerator.generatePipelineComponent(
				pipelineComponentSourceFile,
				_WizardResults[WizardValues.Namespace] as string,
				_WizardResults[WizardValues.ClassName] as string,
				(bool) _WizardResults[WizardValues.ImplementIProbeMessage],
				_DesignerProperties,
				componentType,
				(implementationLanguages) _WizardResults[WizardValues.ImplementationLanguage]);

			pipelineComponentProject.ProjectItems.AddFromFile(pipelineComponentSourceFile);

			#region add the component utilities class, if needed
			if(DesignerVariableType.SchemaListUsed)
			{
				string BizTalkUtilitiesFileName = "Microsoft.BizTalk.Component.Utilities.dll";
				Stream stream = this.GetType().Assembly.GetManifestResourceStream(_ProjectNamespace  + "." + BizTalkUtilitiesFileName);
				using(BinaryReader br = new BinaryReader(stream))
				{
					using(FileStream fs =  new FileStream(Path.Combine(_ProjectDirectory, BizTalkUtilitiesFileName), FileMode.Create))
					{
						// temporary storage
						byte[] b = null;

						// read the stream in blocks of ushort.MaxValue
						while((b = br.ReadBytes(ushort.MaxValue)).Length > 0)
						{
							// write what we've read to the output file
							fs.Write(b, 0, b.Length);
						}
					}
				}

				PipelineComponentVSProject.References.Add(Path.Combine(_ProjectDirectory, BizTalkUtilitiesFileName));
			}
			#endregion

			// save the project file
			PipelineComponentVSProject.Project.Save(Path.Combine(_ProjectDirectory, projectFileName));

            // only do this if we're creating a new solution, otherwise the user
            // might get distracted from his/her current work
#if RELEASE
            if (this._FExclusive)
            {
#endif
                // retrieve the main code file from the added project
                ProjectItem item = PipelineComponentVSProject.Project.ProjectItems.Item(Path.GetFileName(pipelineComponentSourceFile));

                // let's open up the main code file and show it so the user can start editing
                Window mainSourceFile = item.Open(Constants.vsViewKindPrimary);

                // set the editor to the newly created sourcecode
                mainSourceFile.Activate();
#if RELEASE
            }
#endif
		}

		/// <summary>
		/// This will dump all of the values in the namedvaluecollections
		/// and in the contextparams coll. to the debug window 
		/// (or better: debugview).
		/// </summary>
		/// <param name="ContextParams"></param>
		private void TraceAllValues(object[] ContextParams)
		{
			Trace.WriteLine("++ Start ContextParams");
			foreach(object o in ContextParams)
			{
				Trace.WriteLine(o.ToString());
			}
			Trace.WriteLine("-- End ContextParams");

			Trace.WriteLine("++ Start _WizardResults");
			foreach(object o in _WizardResults)
			{
				// only trace strings
				if(o is string)
				{
					Trace.WriteLine("Name:" + (string) o + " - Value = " + _WizardResults[(string) o]);
				}
			}
			Trace.WriteLine("-- End _WizardResults");

			Trace.WriteLine("++ Start _DesignerProperties");
			foreach(object o in _DesignerProperties)
			{
				if(o is string)
				{
					Trace.WriteLine("Name:" + (string) o + " - Value = " + _DesignerProperties[(string) o]);
				}
			}
			Trace.WriteLine("-- End _DesignerProperties");

			Trace.WriteLine("++ Start _DesignerProperties");
			foreach(object o in _DesignerProperties)
			{
				if(o is string)
				{
					Trace.WriteLine("Name:" + (string) o + " - Value = " + _DesignerProperties[(string) o]);
				}
			}
			Trace.WriteLine("-- End _DesignerProperties");
		}

		/// <summary>
		/// Helper to get resource from manifest.
		/// </summary>
		/// <param name="resource">Full resource name</param>
		/// <returns>Resource value</returns>
		private string GetResource(string resource) 
		{
			string value = null;
			if (null != resource) 
			{
				Assembly assem = this.GetType().Assembly;
				Stream stream = assem.GetManifestResourceStream(resource);
				Trace.WriteLine(resource);
				StreamReader reader = null;

				using (reader = new StreamReader(stream)) 
				{
					value = reader.ReadToEnd();
				}
			}
			return value;
		}

	}
}
