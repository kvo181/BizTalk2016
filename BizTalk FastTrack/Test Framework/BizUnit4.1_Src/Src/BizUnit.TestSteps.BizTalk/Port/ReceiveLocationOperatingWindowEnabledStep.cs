//---------------------------------------------------------------------
// File: ReceiveLocationOperatingWindowEnabledStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Management;
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Port
{
	/// <summary>
    /// The ReceiveLocationOperatingWindowEnabledStep test step checks to determine whether a specific receive location is enabled or disabled.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.ReceiveLocationEnabledStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<ReceiveLocationName>GovenorIn</ReceiveLocationName>
	///		<IsDisabled>true</IsDisabled>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>ReceiveLocationName</term>
	///			<description>The name of the receive location to check</description>
	///		</item>
	///		<item>
	///			<term>IsDisabled</term>
	///			<description>If true is specified then the test step will check to see that the receive location is disabled, if false, the step will check it is enabled</description>
	///		</item>
	///	</list>
	///	</remarks>

    public class ReceiveLocationOperatingWindowEnabledStep : TestStepBase
	{
        ///<summary>
        /// The name of the receive location to check
        ///</summary>
        public string ReceiveLocationName { get; set; }
        ///<summary>
        /// Enable or disable the Service Window
        ///</summary>
        public bool Enable { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
		{
			ManagementObject mo = GetreceiveLocationWmiObject(ReceiveLocationName);

			bool isActualEnabled;

			using(mo)
			{
                isActualEnabled = (bool)mo.GetPropertyValue("OperatingWindowEnabled");		
			}

            if (Enable != isActualEnabled)
			{
                PutOptions p = new PutOptions();
                p.Type = PutType.UpdateOnly;
                mo.SetPropertyValue("OperatingWindowEnabled", Enable);
                ManagementPath result = mo.Put(p);

                context.LogInfo(string.Format("The receive location: {0} service window is {1}", ReceiveLocationName, Enable ? "enabled" : "disabled"));
			}
			else
			{
                context.LogInfo(string.Format("The receive location: {0} service windows was already in the state {1}", ReceiveLocationName, Enable ? "enabled" : "disabled"));
			}
		}

	    public override void Validate(Context context)
	    {
	        ArgumentValidation.CheckForEmptyString(ReceiveLocationName, "ReceiveLocationName");
	    }

	    private static ManagementObject GetreceiveLocationWmiObject(string receiveLocation)
		{
			var searcher = new ManagementObjectSearcher();
			var scope = new ManagementScope("root\\MicrosoftBizTalkServer");
			searcher.Scope = scope;
		
			// Build a Query to enumerate the MSBTS_ReceiveLocation instances if an argument
			// is supplied use it to select only the matching RL.
			var query = new SelectQuery
			                {
			                    QueryString = String.Format("SELECT * FROM MSBTS_ReceiveLocation WHERE Name =\"{0}\"", receiveLocation)
			                };

		    // Set the query for the searcher.
			searcher.Query = query;

			// Execute the query and determine if any results were obtained.
			ManagementObjectCollection queryCol = searcher.Get();
			
			ManagementObjectCollection.ManagementObjectEnumerator me = queryCol.GetEnumerator();
			me.Reset();
			if ( me.MoveNext() )
			{
				return (ManagementObject)me.Current;
			}
			else
			{
				throw new ApplicationException(string.Format("The WMI object for the receive location:{0} could not be retrieved.", receiveLocation ));
			}
		}
	}
}
