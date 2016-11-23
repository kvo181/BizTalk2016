namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class NativeMethods
    {
        public const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        public const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
        public const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
        public const int FORMAT_MESSAGE_FROM_HMODULE = 0x800;
        public const int FORMAT_MESSAGE_FROM_STRING = 0x400;
        public const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        public const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        public const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0xff;
        public const int LOAD_LIBRARY_AS_DATAFILE = 2;

        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        public static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, ref IntPtr[] arguments);
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, int dwFlags);
    }
}

