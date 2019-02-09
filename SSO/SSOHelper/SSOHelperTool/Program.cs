using System;
using System.Linq;

namespace SSOHelperTool
{
    class Program
    {
        static void Main(string[] args)
        {
            // first parameter is the command : decrypt or import
            // first parameter is the filename
            // second parameter is the encryption key
            //System.Diagnostics.Debugger.Launch();

            if (args.Count() != 3)
                throw new ArgumentException(string.Format("We need 3 arguments: command, filename and encryption key"));

            string appName = string.Empty;
            string filename = string.Empty;

            switch (args[0].ToLower())
            {
                case "decrypt":
                    Console.WriteLine("Decrypt the file '{0}' using the encryption key '{1}':", args[1], args[2]);
                    Console.WriteLine("------");
                    string data = bizilante.SSO.Tools.SSOHelper.Decrypt(args[1], args[2], out appName, out filename);
                    Console.WriteLine("Decrypted SSO application : '{0}'", appName);
                    Console.Write(data);
                    Console.WriteLine();
                    Console.WriteLine("SSO XML saved in : '{0}'", filename);
                    Console.WriteLine("------");
                    break;

                case "decryptnosave":
                    Console.WriteLine("Decrypt the file '{0}' using the encryption key '{1}':", args[1], args[2]);
                    Console.WriteLine("------");
                    string data1 = bizilante.SSO.Tools.SSOHelper.Decrypt(args[1], args[2], out appName);
                    Console.WriteLine("Decrypted SSO application : '{0}'", appName);
                    Console.Write(data1);
                    Console.WriteLine();
                    Console.WriteLine("------");
                    break;

                case "import":
                    Console.WriteLine("Importing the file '{0}':", args[1]);
                    Console.WriteLine("------");
                    bizilante.SSO.Tools.SSOHelper.Import(args[1], out appName);
                    Console.WriteLine("Imported SSO application : '{0}'", appName);
                    Console.WriteLine("------");
                    break;

                case "export":
                    Console.WriteLine("Export to file '{0}' using the encryption key '{1}':", args[1], args[2]);
                    Console.WriteLine("------");
                    bizilante.SSO.Tools.SSOHelper.Export(args[1], args[2], out appName, out filename);
                    Console.WriteLine("Exported SSO application : '{0}' to '{1}'", appName, filename);
                    Console.WriteLine("------");
                    break;

            }

        }
    }
}
