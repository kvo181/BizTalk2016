namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("fff93000-75a2-450a-8a39-53120ca8d8fa"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICloneable
    {
        IntPtr Clone([In] ref Guid riid);
    }
}

