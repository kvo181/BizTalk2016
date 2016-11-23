namespace Microsoft.BizTalk.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("10051109-920A-44BF-A850-377EDA8A1E09"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBTPropertyInfoList
    {
        int Count { get; }
        PropertyInfo Item(int index);
        PropertyInfo Find(string bstrName);
    }
}

