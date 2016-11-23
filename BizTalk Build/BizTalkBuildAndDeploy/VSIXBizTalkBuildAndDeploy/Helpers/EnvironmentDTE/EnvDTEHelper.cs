using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace VSIXBizTalkBuildAndDeploy.Helpers.EnvironmentDTE
{
    class EnvDTEHelper
    {
        public static string GetProjectTypeGuids(Project proj)
        {
            string sProjectTypeGuids = string.Empty;
            object objService;
            Microsoft.VisualStudio.Shell.Interop.IVsSolution objIVsSolution;
            Microsoft.VisualStudio.Shell.Interop.IVsHierarchy objIVsHierarchy = null;
            Microsoft.VisualStudio.Shell.Interop.IVsAggregatableProject objIVsAggregatableProject;
            int iResult;

            //objService = GetService(proj.DTE, typeof(Microsoft.VisualStudio.Shell.Interop.IVsSolution));
            objService = Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.IVsSolution));
            objIVsSolution = objService as Microsoft.VisualStudio.Shell.Interop.IVsSolution;
            iResult = objIVsSolution.GetProjectOfUniqueName(proj.UniqueName, out objIVsHierarchy);
            if (iResult == 0)
            {
                objIVsAggregatableProject = objIVsHierarchy as Microsoft.VisualStudio.Shell.Interop.IVsAggregatableProject;
                if (null != objIVsAggregatableProject)
                    iResult = objIVsAggregatableProject.GetAggregateProjectTypeGuids(out sProjectTypeGuids);
            }
            return sProjectTypeGuids;
        }
        [Obsolete("Please use the GetService method of the Package object", true)]
        internal static object GetService(object serviceProvider, System.Type type)
        {
            return GetService(serviceProvider, type.GUID);
        }
        [Obsolete("Please use the GetService method of the Package object", true)]
        internal static object GetService(object serviceProvider, System.Guid guid)
        {
            object objService = null;
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider objIServiceProvider;
            IntPtr objIntPtr;
            int hr;
            Guid objSIDGuid;
            Guid objIIDGuid;
            objSIDGuid = guid;
            objIIDGuid = objSIDGuid;
            objIServiceProvider = serviceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            hr = objIServiceProvider.QueryService(ref objSIDGuid, ref objIIDGuid, out objIntPtr);
            if (hr != 0)
            {
                System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);
            }
            else if (!objIntPtr.Equals(IntPtr.Zero))
            {
                objService = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(objIntPtr);
                System.Runtime.InteropServices.Marshal.Release(objIntPtr);
            }
            return objService;
        }
    }
}
