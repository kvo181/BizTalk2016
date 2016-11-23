namespace Microsoft.BizTalk.Interop
{
    using System.Runtime.InteropServices;

    [ComImport, Guid("10051102-920A-44BF-A850-377EDA8A1E09"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBTDocumentSpec
    {
        IBTPropertyAnnotationList GetPropertyAnnotationList();
    }
}

