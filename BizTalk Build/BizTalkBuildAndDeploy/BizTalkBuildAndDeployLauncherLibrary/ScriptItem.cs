using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BizTalkBuildAndDeployLauncherLibrary
{
    class ScriptItem
    {
        public ScriptItem(string fullName)
        {
            FileInfo fi = new FileInfo(fullName);
            Name = fi.Name;
            FullName = fi.FullName;
        }
        public string Name { get; set; }
        public string FullName { get; set; }

        public static List<ScriptItem> Create(List<string> scripts)
        {
            List<ScriptItem> scriptItems = new List<ScriptItem>();
            foreach (string fullName in scripts)
                scriptItems.Add(new ScriptItem(fullName));
            return scriptItems;
        }
    }
}
