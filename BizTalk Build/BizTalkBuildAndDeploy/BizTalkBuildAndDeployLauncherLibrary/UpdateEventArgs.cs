using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizTalkBuildAndDeployLauncherLibrary
{
    public class UpdateEventArgs : EventArgs
    {
        public UpdateEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
}
