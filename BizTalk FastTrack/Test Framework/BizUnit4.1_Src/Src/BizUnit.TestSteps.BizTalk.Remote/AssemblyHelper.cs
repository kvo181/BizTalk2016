//---------------------------------------------------------------------
// File: AssemblyHelper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System.Reflection;
using System.IO;

namespace BizUnit.TestSteps.BizTalk.Remote
{
    ///<summary>
    ///</summary>
    public static class AssemblyHelper
    {
        ///<summary>
        ///</summary>
        ///<param name="path"></param>
        ///<returns></returns>
        public static Assembly LoadAssembly(string path)
        {
            string filename = Path.GetFileName(path);
            string newPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
            if (!System.IO.File.Exists(newPath))
            {
                System.IO.File.Copy(path, newPath, false);
            }

            return Assembly.LoadFrom(newPath);
        }
    }
}
