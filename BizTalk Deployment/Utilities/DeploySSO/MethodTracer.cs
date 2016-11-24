using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace bizilante.Deployment.Apps.SSO
{
    internal class MethodTracer : IDisposable
    {
        private MethodBase currentMethod;
        private DateTime endTime;
        private string separator = new string('-', 60);
        private DateTime startTime;

        public MethodTracer(MethodBase currentMethod)
        {
            this.currentMethod = currentMethod;
            this.startTime = DateTime.Now;
            this.Enter();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.endTime = DateTime.Now;
                this.Exit();
            }
        }

        private void Enter()
        {
            Trace.WriteLine(this.separator);
            string str = this.startTime.ToString("T", CultureInfo.InvariantCulture);
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, ">>> Entering {0}.{1} @ {2}", new object[] { this.currentMethod.DeclaringType.Name, this.currentMethod.Name, str }));
            Trace.WriteLine(string.Empty);
        }

        private void Exit()
        {
            Trace.WriteLine(string.Empty);
            string str = this.endTime.ToString("T", CultureInfo.InvariantCulture);
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "<<< Exiting {0}.{1} @ {2}", new object[] { this.currentMethod.DeclaringType.Name, this.currentMethod.Name, str }));
            TimeSpan span = this.endTime.Subtract(this.startTime);
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "(Elapsed time: {0} seconds)", new object[] { span.TotalSeconds }));
            Trace.WriteLine(this.separator);
        }

        ~MethodTracer()
        {
            this.Dispose(false);
        }

        public static void TraceAssembly()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Assembly: {0}", new object[] { executingAssembly.FullName }));
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Location: {0}", new object[] { executingAssembly.Location }));
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Product: {0} {1}", new object[] { versionInfo.ProductName, versionInfo.ProductVersion }));
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "File: {0} {1}", new object[] { versionInfo.OriginalFilename, versionInfo.FileVersion }));
            Trace.WriteLine(string.Empty);
        }
    }
}

