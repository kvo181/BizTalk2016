using System;
using System.Collections.Generic;
using System.Text;

namespace bizilante.Tools.CommandLine
{
    public class CommandLineArgDescriptorList : List<CommandLineArgDescriptor>
    {
        public CommandLineArgDescriptor GetDescriptor(string name)
        {
            foreach (CommandLineArgDescriptor descriptor in this)
            {
                if (string.Compare(descriptor.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return descriptor;
                }
            }
            return null;
        }

        public string GetUsage()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CommandLineArgDescriptor descriptor in this)
            {
                builder.Append(" " + descriptor.GetUsage());
            }
            return builder.ToString();
        }
    }
}

