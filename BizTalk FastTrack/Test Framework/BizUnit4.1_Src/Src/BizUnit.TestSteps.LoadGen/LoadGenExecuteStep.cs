//---------------------------------------------------------------------
// File: LoadGenExecuteStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using BizUnit.Xaml;
using LoadGen;
using System;
using System.Xml;
using System.Globalization;
using System.Threading;

namespace BizUnit.TestSteps.Load
{
	/// <summary>
	/// The LoadGenExecuteStep step executes a LoadGen test
	/// </summary>
	///  
	/// <remarks>
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.LoadGenSteps.LoadGenExecuteStep, BizUnit.LoadGenSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<LoadGenTestConfig>c:\LoadTests\MemphisFeedPerfTest001.xml</LoadGenTestConfig>
	///	</TestStep>
	/// </code>
	///
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>LoadGenTestConfig</term>
	///			<description>The path to the LoadGen test configuration</description>
	///		</item>
	///	</list>
	///	</remarks>
	public class LoadGenExecuteStep : TestStepBase
	{
		private Context _ctx;
		private bool _bExitApp;

		/// <summary>
		/// The path to the LoadGen test configuration
		/// </summary>
		public string LoadGenTestConfig { get; set; }

		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			_ctx = context;

			try
			{
				context.LogInfo("About to execute LoadGen script: {0}", LoadGenTestConfig);

				var doc = new XmlDocument();
				doc.Load(LoadGenTestConfig);
				if (string.Compare(doc.FirstChild.Name, "LoadGenFramework", true, new CultureInfo("en-US")) != 0)
					throw new ConfigException("LoadGen Configuration File Schema Invalid!");

				var loadGen = new LoadGen.LoadGen(doc.FirstChild);
				loadGen.LoadGenStopped += LoadGenStopped;
				loadGen.Start();
			}
			catch (ConfigException cex)
			{
				context.LogError(cex.Message);
				throw;
			}
			catch (Exception ex)
			{
				context.LogError(ex.Message);
				throw;
			}

			while (!_bExitApp)
			{
				Thread.Sleep(0x3e8);
			}
			Thread.Sleep(0x1388);
		}

		public override void Validate(Context context)
		{
			if (string.IsNullOrEmpty(LoadGenTestConfig))
				throw new StepValidationException("LoadGenTestConfig may not be null or empty", this);

			var doc = new XmlDocument();
			doc.Load(LoadGenTestConfig);
			if (string.Compare(doc.FirstChild.Name, "LoadGenFramework", true, new CultureInfo("en-US")) != 0)
				throw new StepValidationException("LoadGen Configuration File Schema Invalid!", this);
		}

		private void LoadGenStopped(object sender, LoadGenStopEventArgs e)
		{
			TimeSpan span1 = e.LoadGenStopTime.Subtract(e.LoadGenStartTime);
			_ctx.LogInfo("FilesSent: " + e.NumFilesSent);
			_ctx.LogInfo("StartTime: " + e.LoadGenStartTime);
			_ctx.LogInfo("StopTime:  " + e.LoadGenStopTime);
			_ctx.LogInfo("DeltaTime: " + span1.TotalSeconds + "Secs.");
			_ctx.LogInfo("Rate:      " + ((e.NumFilesSent) / span1.TotalSeconds));

			_bExitApp = true;
		}
	}
}
