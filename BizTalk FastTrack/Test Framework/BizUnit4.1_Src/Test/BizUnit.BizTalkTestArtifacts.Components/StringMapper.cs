using System;
using System.Collections.Generic;
using System.Text;

namespace BizUnit.BizTalkTestArtifacts.Components
{
    [Serializable]
    public class StringMapper
    {
        public string convertString(string inString)
        {
            // Do some random string mapping stuff:
            switch (inString)
            {
                case "Blah1":
                    return "Blah";
                case "Blah2":
                    return "BlahBlah";
                default:
                    return "Blah";
            }
        }
    }
}
