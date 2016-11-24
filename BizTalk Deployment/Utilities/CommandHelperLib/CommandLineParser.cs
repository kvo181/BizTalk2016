using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace bizilante.Tools.CommandLine
{
    public static class CommandLineParser
    {
        public static List<CommandLineArgumentException> Expand(ref NameValueCollection nameValueArgs, CommandLineArgDescriptorList commandLineArgDescriptorList)
        {
            if (commandLineArgDescriptorList == null)
            {
                throw new ArgumentNullException("commandLineArgDescriptorList");
            }
            List<CommandLineArgumentException> list = new List<CommandLineArgumentException>();
            NameValueCollection values = new NameValueCollection();
            for (int i = 0; i < nameValueArgs.Count; i++)
            {
                string key = nameValueArgs.GetKey(i);
                string[] strArray = nameValueArgs.GetValues(i);
                string name = null;
                if (key != null)
                {
                    foreach (CommandLineArgDescriptor descriptor in commandLineArgDescriptorList)
                    {
                        if (descriptor.Named)
                        {
                            string strA = descriptor.Name;
                            if (strA.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (name == null)
                                {
                                    name = strA;
                                    if (string.Compare(strA, key, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    string message = CommandLineResources.GetString(CommandLineResources.ResourceID.AmbiguousNamedArgument);
                                    list.Add(new CommandLineArgumentException(message, key, TraceLevel.Error));
                                }
                                break;
                            }
                        }
                    }
                }
                if (name != null)
                {
                    if (strArray != null)
                    {
                        foreach (string str5 in strArray)
                        {
                            values.Add(name, str5);
                        }
                    }
                    else
                    {
                        values.Add(name, null);
                    }
                }
                else if (strArray != null)
                {
                    foreach (string str7 in strArray)
                    {
                        values.Add(key, str7);
                    }
                }
                else
                {
                    values.Add(key, null);
                }
            }
            nameValueArgs = values;
            return list;
        }

        public static NameValueCollection Parse(string[] args)
        {
            NameValueCollection values = new NameValueCollection();
            if (args != null)
            {
                foreach (string str in args)
                {
                    string str2;
                    string str3;
                    ParseArg(str, out str2, out str3);
                    values.Add(str2, str3);
                }
            }
            return values;
        }

        private static void ParseArg(string arg, out string name, out string value)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("arg");
            }
            name = null;
            value = null;
            if (!arg.StartsWith("-", StringComparison.Ordinal) && !arg.StartsWith("/", StringComparison.Ordinal))
            {
                value = arg;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 1; i < arg.Length; i++)
                {
                    char c = arg[i];
                    if (!char.IsLetterOrDigit(c) && (c != '?'))
                    {
                        break;
                    }
                    builder.Append(c);
                }
                name = builder.ToString();
                arg = arg.Remove(0, 1 + name.Length);
                if (arg == "+")
                {
                    value = "+";
                }
                else if (arg == "-")
                {
                    value = "-";
                }
                else if (arg.StartsWith(":", StringComparison.Ordinal) || arg.StartsWith("=", StringComparison.Ordinal))
                {
                    value = arg.Substring(1);
                }
            }
        }

        public static List<CommandLineArgumentException> Validate(NameValueCollection nameValueArgs, CommandLineArgDescriptorList commandLineArgDescriptorList)
        {
            if (commandLineArgDescriptorList == null)
            {
                throw new ArgumentNullException("commandLineArgDescriptorList");
            }
            List<CommandLineArgumentException> list = new List<CommandLineArgumentException>();
            foreach (CommandLineArgDescriptor descriptor in commandLineArgDescriptorList)
            {
                string[] values = nameValueArgs.GetValues(descriptor.Named ? descriptor.Name : null);
                int num = (values == null) ? 0 : values.Length;
                if (num < descriptor.MinOccurs)
                {
                    CommandLineArgumentException item = new CommandLineArgumentException(CommandLineResources.GetFormattedString(CommandLineResources.ResourceID.RequiredArgumentNotSpecified, new object[] { descriptor.MinOccurs.ToString(CultureInfo.InvariantCulture) }), descriptor.Name, TraceLevel.Error);
                    list.Add(item);
                }
                else if (num > descriptor.MaxOccurs)
                {
                    CommandLineArgumentException exception2 = new CommandLineArgumentException(CommandLineResources.GetFormattedString(CommandLineResources.ResourceID.ExtraArgumentsSpecified, new object[] { descriptor.MaxOccurs.ToString(CultureInfo.InvariantCulture) }), descriptor.Name, TraceLevel.Error);
                    list.Add(exception2);
                }
            }
            for (int i = 0; i < nameValueArgs.Count; i++)
            {
                string key = nameValueArgs.GetKey(i);
                string[] strArray2 = nameValueArgs.GetValues(i);
                if (key != null)
                {
                    CommandLineArgDescriptor descriptor2 = commandLineArgDescriptorList.GetDescriptor(key);
                    if (descriptor2 == null)
                    {
                        CommandLineArgumentException exception3 = new CommandLineArgumentException(CommandLineResources.GetString(CommandLineResources.ResourceID.UnrecognizedNamedArgument), key, TraceLevel.Error);
                        list.Add(exception3);
                    }
                    else if (descriptor2.Type == CommandLineArgDescriptor.ArgumentType.Simple)
                    {
                        if ((strArray2 != null) && (strArray2.Length > 0))
                        {
                            CommandLineArgumentException exception4 = new CommandLineArgumentException(CommandLineResources.GetString(CommandLineResources.ResourceID.ValueForSimpleArgument), key, TraceLevel.Error);
                            list.Add(exception4);
                        }
                    }
                    else if ((descriptor2.Type == CommandLineArgDescriptor.ArgumentType.Boolean) && ((strArray2 == null) || (((strArray2 != null) && (strArray2[0] != "+")) && (strArray2[0] != "-"))))
                    {
                        CommandLineArgumentException exception5 = new CommandLineArgumentException(CommandLineResources.GetString(CommandLineResources.ResourceID.NoFlagForBooleanArgument), key, TraceLevel.Error);
                        list.Add(exception5);
                    }
                }
            }
            return list;
        }
    }
}

