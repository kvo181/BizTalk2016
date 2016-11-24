using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Diagnostics;

namespace bizilante.Tools.CommandLine
{
    public static class CommandLineHelper
    {
        public static bool ArgSpecified(NameValueCollection nameValueArgs, string argName)
        {
            if (nameValueArgs != null)
            {
                foreach (string str in nameValueArgs.Keys)
                {
                    if (string.Compare(str, argName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ConsoleWriteArguments(NameValueCollection nameValueArgs)
        {
            if (nameValueArgs == null)
            {
                throw new ArgumentNullException("nameValueArgs");
            }
            for (int i = 0; i < nameValueArgs.Count; i++)
            {
                string str3;
                string key = nameValueArgs.GetKey(i);
                string str2 = nameValueArgs.Get(i);
                if (key == null)
                {
                    str3 = nameValueArgs[key];
                }
                else if (str2 == null)
                {
                    str3 = string.Format(CultureInfo.InvariantCulture, "-{0}", new object[] { key });
                }
                else
                {
                    str3 = string.Format(CultureInfo.InvariantCulture, "-{0}:{1}", new object[] { key, str2 });
                }
                Console.WriteLine(str3);
            }
            Console.WriteLine(string.Empty);
        }

        public static void TraceWriteArguments(string[] args, NameValueCollection nameValueArgs)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (nameValueArgs == null)
            {
                throw new ArgumentNullException("nameValueArgs");
            }
            Trace.WriteLine("Arguments:");
            for (int i = 0; i < args.Length; i++)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "  args[{0}]=\"{1}\"", new object[] { i.ToString(CultureInfo.InvariantCulture), args[i] }));
            }
            Trace.WriteLine(string.Empty);
            Trace.WriteLine("Name-value arguments:");
            for (int j = 0; j < nameValueArgs.Count; j++)
            {
                string key = nameValueArgs.GetKey(j);
                string str3 = nameValueArgs.Get(j);
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "  args[\"{0}\"]=\"{1}\"", new object[] { key, str3 }));
            }
            Trace.WriteLine(string.Empty);
        }
    }
}

