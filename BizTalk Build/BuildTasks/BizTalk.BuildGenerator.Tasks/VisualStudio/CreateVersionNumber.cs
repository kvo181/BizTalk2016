
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using System;
using System.IO;
namespace BizTalk.BuildGenerator.Tasks.VisualStudio
{
    public class CreateVersionNumber : Task
    {
        private const string DefaultContent = "using System.Reflection;\r\n//<Year><Month><Day><Hour+Min>\r\n[assembly: AssemblyVersion(\"{0}\")]\r\n[assembly: AssemblyFileVersion(\"{1}\")]\r\n//BizTalk Version\r\n[assembly: AssemblyInformationalVersionAttribute(\"{2}\")]";        

        public string BizTalkMajor { get; set; }
        
        public string BizTalkMinor { get; set; }
                
        public string VersionFilePath { get; set; }

        public override bool Execute()
        {            
            var args = new List<string> { DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Hour + "" + DateTime.Now.Minute };
            var versionNumber = string.Format(@"{0}.{1}.{2}.{3}", args.ToArray());
            var productInformationalVersion = string.Format("{0}.{1}.0.0", BizTalkMajor, BizTalkMinor);
            var content = string.Format(DefaultContent, versionNumber, versionNumber, productInformationalVersion);
            
            File.WriteAllText(VersionFilePath, content);            
            return true;
        }
    }
}
