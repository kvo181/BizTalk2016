using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace bizilante.BuildGenerator.Tasks
{
    public class ConfigWrapper
    {
        XmlDocument _xDoc = new XmlDocument();
        XmlNode _versionNode = null;

        public ConfigWrapper(string filename)
        {
            _xDoc.Load(filename);
            _versionNode = _xDoc.SelectSingleNode("//Version");
            if (null == _versionNode)
                throw new Exception(string.Format("Version node not found in the file: '{0}'", filename));
        }
    }
}
