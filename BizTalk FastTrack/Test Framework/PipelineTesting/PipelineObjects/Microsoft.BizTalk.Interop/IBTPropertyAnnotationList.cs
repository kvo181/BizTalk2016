namespace Microsoft.BizTalk.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("10051101-920A-44BF-A850-377EDA8A1E09"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBTPropertyAnnotationList
    {
        int Count { get; }
        PropertyAnnotation GetItem(int index);
    }
}

