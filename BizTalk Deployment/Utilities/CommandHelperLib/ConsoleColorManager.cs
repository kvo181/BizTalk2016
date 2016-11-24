using System;
using System.Configuration;
using System.Diagnostics;

namespace bizilante.Tools.CommandLine
{
    public class ConsoleColorManager
    {
        private ConsoleColor[] colors = new ConsoleColor[5];
        private static ConsoleColorManager instance;

        private ConsoleColorManager(ConsoleColor defaultColor)
        {
            for (int i = 0; i < this.colors.Length; i++)
            {
                this.colors[i] = defaultColor;
            }
            this.Initialize();
        }

        public ConsoleColor GetColor(TraceLevel traceLevel)
        {
            return this.colors[(int) traceLevel];
        }

        public static ConsoleColorManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ConsoleColorManager(Console.ForegroundColor);
            }
            return instance;
        }

        private void Initialize()
        {
            for (int i = 0; i < 5; i++)
            {
                int num2;
                string str = "Console.ForegroundColor." + ((TraceLevel) i).ToString();
                string s = ConfigurationManager.AppSettings[str];
                if (((s != null) && (s.Length > 0)) && int.TryParse(s, out num2))
                {
                    this.colors[i] = (ConsoleColor) num2;
                }
            }
        }
    }
}

