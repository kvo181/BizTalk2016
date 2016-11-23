namespace Microsoft.BizTalk.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("05ac2010-2ed5-41f0-a961-2838a1836a22"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INamedItem
    {
        string Name { get; }
        object Value { [return: MarshalAs(UnmanagedType.IUnknown)] get; }
    }
}

