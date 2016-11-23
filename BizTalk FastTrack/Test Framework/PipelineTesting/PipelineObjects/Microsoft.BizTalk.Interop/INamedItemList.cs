namespace Microsoft.BizTalk.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("05ac2011-2ed5-41f0-a961-2838a1836a22"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INamedItemList
    {
        int Count { get; }
        INamedItem GetItem(int index);
    }
}

