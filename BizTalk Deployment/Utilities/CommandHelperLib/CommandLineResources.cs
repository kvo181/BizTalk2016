using System;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text;

namespace bizilante.Tools.CommandLine
{
    internal static class CommandLineResources
    {
        private static ResourceManager staticResourceManager = new ResourceManager(typeof(CommandLineResources));

        public static string GetFormattedString(ResourceID name, params object[] args)
        {
            Exception exception = null;
            try
            {
                string format = staticResourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
                return ((format != null) ? string.Format(CultureInfo.CurrentUICulture, format, args) : name.ToString());
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            catch (FormatException exception4)
            {
                exception = exception4;
            }
            if (exception == null)
            {
                return name.ToString();
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(name.ToString());
            foreach (object obj2 in ArrayList.Adapter(args))
            {
                builder.Append(" ");
                builder.Append(obj2.ToString());
            }
            return builder.ToString();
        }

        public static string GetString(ResourceID name)
        {
            Exception exception = null;
            try
            {
                return staticResourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
            }
            catch (InvalidOperationException exception2)
            {
                exception = exception2;
            }
            catch (MissingManifestResourceException exception3)
            {
                exception = exception3;
            }
            return name.ToString();
        }

        public enum ResourceID
        {
            None,
            AmbiguousNamedArgument,
            ExtraArgumentsSpecified,
            NoFlagForBooleanArgument,
            RequiredArgumentNotSpecified,
            UnrecognizedNamedArgument,
            ValueForSimpleArgument
        }
    }
}

