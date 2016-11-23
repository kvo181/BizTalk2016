namespace BizUnit.TestSteps.i8c.IIS
{
	/// <summary>
	/// Helper class for IIS.
	/// </summary>
	public class IISHelper
	{
		/// <summary>
		/// Method which performs a friendly lookup of possible ApplicationPool States
		/// </summary>
		/// <param name="stateCode">Original state code</param>
		/// <returns>Friendly state code description</returns>
		public static string GetFriendlyApplicationPoolState(int stateCode)
		{
			switch (stateCode)
			{
				case 0:
					return "Starting";
				case 1:
					return "Started";
				case 2:
					return "Stopping";
				case 3:
					return "Stopped";
				case 4:
					return "Unknown";
				default:
					return "Undefined value";
			}
		}
	}
}
