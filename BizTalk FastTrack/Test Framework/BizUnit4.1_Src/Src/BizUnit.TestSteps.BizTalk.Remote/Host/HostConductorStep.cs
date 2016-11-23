using System;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Remote.Host
{
	/// <summary>
	/// The HostConductorStep test step maybe used to start or stop a BizTalk host
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.HostConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<Action>start|stop</Action>
	///		<HostInstanceName>BizTalkServerApplication</HostInstanceName>
	///		<Server>RecvHost</Server>
	///     <Logon>zeus\\administrator</Logon>
	///     <PassWord>appollo*1</PassWord>
	///     <GrantLogOnAsService>true</GrantLogOnAsService>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>HostInstanceName</term>
	///			<description>The name of the host instance to start|stop</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
	///			<description>A value of start or stop<para>(start|stop)</para></description>
	///		</item>
	///		<item>
	///			<term>Server</term>
	///			<description>The server(s) where the Biztalk host instance is running, a commer delimeted list of servers may be supplied (optional)</description>
	///		</item>
	///		<item>
	///			<term>Logon</term>
	///			<description>String containing the logon information used by the host instance (optional - unless Server is supplied)</description>
	///		</item>
	///		<item>
	///			<term>PassWord</term>
	///			<description>String containing the password for the host (optional - unless Logon is supplied)</description>
	///		</item>
	///		<item>
	///			<term>GrantLogOnAsService (optional - unless Logon is supplied)</term>
	///			<description>Boolean determining whether the 'Log On As Service' privilege should be automatically granted to the specified logon user or not. This flag only has effect when the HostType property is set to In-process</description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class HostConductorStep : TestStepBase
	{
		///<summary>
		/// "start" or "stop" the host instance
		///</summary>
		public string Action { get; set; }

		///<summary>
		/// Name of the host instance
		///</summary>
		public string HostInstanceName { get; set; }

		///<summary>
		/// Server on which to start/stop the host instance
		///</summary>
		public string Servers { get; set; }

		///<summary>
		/// Username to use when starting/stopping via WMI
		///</summary>
		public string Logon { get; set; }

		///<summary>
		/// Password
		///</summary>
		public string PassWord { get; set; }

		///<summary>
		/// Grant the user LogOnAsService
		///</summary>
		public bool GrantLogOnAsService { get; set; }

		/// <summary>
		/// Enable/Disable a remote host
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(Context context)
		{
			// Need to invoke WMI also on other servers than the webserver hosting the BizUnit service.
			var client = ServiceHelper.GetBizUnitService(context);
			var step = new BizUnitWcfServiceLibrary.HostConductorStep
			{
				Action = Action,
				HostInstanceName = HostInstanceName,
				GrantLogOnAsService = GrantLogOnAsService,
				Logon = Logon,
				PassWord = PassWord,
				Servers = Servers
			};
			context.LogInfo(step.ToString(), new object[] {});
			client.HostConductorStep(step);
		}

		/// <summary>
		/// Validate the class instance
		/// </summary>
		/// <param name="context"></param>
		public override void Validate(Context context)
		{
			if (string.IsNullOrEmpty(Action))
			{
				throw new ArgumentNullException("Action is either null or an empty string");
			}

			if (string.IsNullOrEmpty(HostInstanceName))
			{
				throw new ArgumentNullException("HostName is either null or an empty string");
			}

			if (string.IsNullOrEmpty(Servers))
			{
				throw new ArgumentNullException("Servers is either null or an empty string");
			}

			if (null != Logon && 0 < Servers.Length)
			{
				if (string.IsNullOrEmpty(PassWord))
				{
					throw new ArgumentNullException("PassWord is either null or an empty string");
				}
			}
		}
	}
}

