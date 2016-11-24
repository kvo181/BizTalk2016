using System;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text;

namespace bizilante.Deployment.Apps
{
    internal static class StringResources
    {
        private static ResourceManager resourceManager = new ResourceManager(typeof(StringResources));

        public static string GetFormattedString(ResourceID name, params object[] args)
        {
            Exception exception = null;
            try
            {
                string format = resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
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
            builder.Append(name);
            foreach (object obj2 in ArrayList.Adapter(args))
            {
                builder.Append(" ");
                builder.Append(obj2);
            }
            return builder.ToString();
        }

        public static string GetString(ResourceID name)
        {
            Exception exception = null;
            try
            {
                return resourceManager.GetString(name.ToString(), CultureInfo.CurrentUICulture);
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

        internal enum ResourceID
        {
            None,
            HeaderDescription,
            HeaderCopyright,
            CommandResultFailed,
            CommandResultSucceeded,
            CommandCompleted,
            CancelKeyPress
        }
    }
}

