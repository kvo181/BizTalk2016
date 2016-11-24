using System;
using Microsoft.Win32;

namespace bizilante.Deployment.Apps
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("Please, specify the server and application name as argument!");

            string server = args[0];
            string applicationName = args[1];

            string result = string.Empty;

            // Get the package code ass
            using (RegistryKey key = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server, RegistryView.Registry32).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\", true))
            {
                try
                {
                    using (RegistryKey key2 = key.OpenSubKey(applicationName))
                    {
                        if (key2 == null)
                            return;

                        if (!key2.GetValue("Uninstallstring").ToString().Contains("BtsTask.exe UninstallApp"))
                            return;

                        string[] subKeyNames = key2.GetSubKeyNames();
                        for (int i = subKeyNames.Length - 1; i >= 0; i--)
                        {
                            Guid guid = new Guid(subKeyNames[i]);
                            result += string.IsNullOrEmpty(result) ? guid.ToString("B") : "," + guid.ToString("B");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format("Exception when accessing {0} for application {1}: [{2}]", key.Name, applicationName, exception.Message), exception);
                }
            }

            Console.WriteLine(result);
        }

        /// <example>GuidCompressor.exe {abcdefgh-ijkl-mnop-qrst-uvwxyz123456} N
        /// returns: hgfedcbalkjiponmrqtsvuxwzy214365</example>
        /// <example>GuidCompressor.exe hgfedcbalkjiponmrqtsvuxwzy214365 B
        /// returns: {abcdefgh-ijkl-mnop-qrst-uvwxyz123456}</example>
        static void GuidCompressor(string[] args)
        {
            Guid origGuid = new Guid(args[0]);
            //outputFormat should be N, D, B, P
            string outputFormat = args[1];

            string raw = origGuid.ToString("N");
            char[] aRaw = raw.ToCharArray();
            //compressed format reverses 11 byte sequences of the original guid
            int[] revs
                = new int[] { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 };
            int pos = 0;
            for (int i = 0; i < revs.Length; i++)
            {
                Array.Reverse(aRaw, pos, revs[i]);
                pos += revs[i];
            }
            string n = new string(aRaw);
            Guid newGuid = new Guid(n);
            //GUID in registry are all caps.
            Console.WriteLine(newGuid.ToString(outputFormat).ToUpper());
        }

    }
}
