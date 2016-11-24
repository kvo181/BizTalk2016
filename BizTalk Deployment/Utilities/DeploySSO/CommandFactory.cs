using System;
using System.Collections.Specialized;
using bizilante.Tools.CommandLine;

namespace bizilante.Deployment.Apps.SSO
{
    static class CommandFactory
    {
        public static Command Create(NameValueCollection args)
        {
            if (args == null)
            {
                args = new NameValueCollection();
            }
            string str = null;
            string[] values = args.GetValues((string) null);
            if ((values != null) && (values.Length > 0))
            {
                str = values[0].ToUpperInvariant();
            }
            switch (str)
            {
                case "DEPLOY":
                    return new DeployCommand(args);
            }
            return new HelpCommand(args);
        }
    }
}

