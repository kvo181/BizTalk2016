namespace Microsoft.BizTalk.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("10051104-920A-44BF-A850-377EDA8A1E09"), CoClass(typeof(BTSchemaCache)), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBTSchemaCache
    {
        void GetDocSpecInfoByMsgType(string msgType, int pipelineAssemblyID, out string docSpecName, out string assemblyName);
        void GetDocSpecInfoByDocSpecName(string docSpecName, int pipelineAssemblyID, out string assemblyName);
        IBTPropertyInfoList GetPropertyInfoList(string bstrNamespace);
        PropertyInfo GetPropertyInfo(string bstrName, string bstrNamespace);
        IBTDocumentSpec GetDocumentSpecByType(string bstrDocType);
        IBTDocumentSpec GetDocumentSpecByName(string bstrStrongName);
    }
}

