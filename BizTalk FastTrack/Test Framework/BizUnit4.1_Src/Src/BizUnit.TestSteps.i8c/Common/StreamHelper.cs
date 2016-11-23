//---------------------------------------------------------------------
// File: StreamHelper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using StreamHelper1 = BizUnit.TestSteps.Common.StreamHelper;

namespace BizUnit.TestSteps.i8c.Common
{
	/// <summary>
	/// Helper class for stream opperations
	/// </summary>
	public class StreamHelper
	{
		/// <summary>
		/// Helper method to load a disc FILE into a MemoryStream (as unicode string)
		/// </summary>
		/// <param name="filePath">The path to the FILE containing the data</param>
		/// <returns>MemoryStream containing the data in the FILE</returns>
		public static MemoryStream LoadFileToStream(string filePath)
		{
			var ms = StreamHelper1.LoadFileToStream(filePath);
			var strm = StreamHelper1.EncodeStream(ms, Encoding.Unicode);
			return StreamHelper1.LoadMemoryStream(strm);
		}

		/// <summary>
		/// Helper method to load a disc FILE into a string
		/// </summary>
		/// <param name="filePath">The path to the FILE containing the data</param>
		/// <returns>string containing the data in the FILE</returns>
		public static string LoadFileToString(string filePath)
		{
			var ms = StreamHelper1.LoadFileToStream(filePath);
			return StreamHelper.WriteStreamToString(ms);
		}

		/// <summary>
		/// Helper method to write the data in a stream out as a string
		/// </summary>
		/// <param name="ms">Stream containing the data to write</param>
		public static string WriteStreamToString(Stream ms)
		{
			/*
			var strm = StreamHelper1.EncodeStream(ms, Encoding.Unicode);

			ms.Seek(0, SeekOrigin.Begin);
			var sr = new StreamReader(strm);
			var result = sr.ReadToEnd();
			strm.Close();
			 */
			
			ms.Seek(0, SeekOrigin.Begin);
			var sr = new StreamReader(ms);
			var result = sr.ReadToEnd();
			ms.Close();

			return result;
		}

	}
}
