
//
// DocLoader.cs
//
// Original Author:
//    Tomas Restrepo (tomasr@mvps.org)
// Modified by:
//    Koen Van Oost (koen.vanoost@bizilante.be)
//

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Bizilante.PipelineTesting.Helper
{
    public static class DocLoader
    {
        /// <summary>
        /// Loads a document instance from a resource
        /// </summary>
        /// <param name="@namespace">Namespace of the resource</param>
        /// <param name="@name">Name of the resource</param>
        /// <returns></returns>
        public static Stream LoadStream(string @namespace, string @name)
        {
            string resName = @namespace + "." + @name;
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream strm = assembly.GetManifestResourceStream(resName);
            if (null == strm)
            {
                string[] resNames = assembly.GetManifestResourceNames();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0} not found in the list of available resources:", resName));
                foreach (string rName in resNames)
                    sb.AppendLine(rName);
                throw new Exception(sb.ToString());
            }
            return strm;
        }

        /// <summary>
        /// Extract the resource to the given folder.
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="name"></param>
        /// <param name="dir"></param>
        public static void ExtractToDir(string @namespace, string @name, string dir)
        {
            string fullname = Path.Combine(dir, name);
            using (Stream source = LoadStream(@namespace, @name))
            using (Stream target = File.Create(fullname))
                CopyStream(source, target);
        }

        private static void CopyStream(Stream source, Stream target)
        {
            byte[] buffer = new byte[4096];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                target.Write(buffer, 0, read);
        }

    } // class DocLoader

} // namespace Winterdom.BizTalk.PipelineTesting.Tests
