using System;

namespace bizilante.Tools.CommandLine
{
    public class ConsoleColorChanger : IDisposable
    {
        private ConsoleColor foregroundColor = Console.ForegroundColor;

        public ConsoleColorChanger(ConsoleColor newForegroundColor)
        {
            Console.ForegroundColor = newForegroundColor;
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
                Console.ForegroundColor = this.foregroundColor;
            }
        }

        ~ConsoleColorChanger()
        {
            this.Dispose(false);
        }
    }
}

