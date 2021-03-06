using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace bizilante.Deployment.BTSDeployHost.Utils
{
    public static class ColorAdapter
    {
        public static Color ToColor(this ConsoleColor clr)
        {
            return Adapt(clr, Color.White);
        }

        public static ConsoleColor ToConsoleColor(this Color clr)
        {
            return Adapt(clr, ConsoleColor.White);
        }

        private static readonly Dictionary<ConsoleColor, Color> ColorMap =
            new Dictionary<ConsoleColor, Color>
                {
                    {ConsoleColor.Black, Color.Black},
                    {ConsoleColor.Blue, Color.Blue},
                    {ConsoleColor.Cyan, Color.Cyan},
                    {ConsoleColor.DarkBlue, Color.DarkBlue},
                    {ConsoleColor.DarkCyan, Color.DarkCyan},
                    {ConsoleColor.DarkGray, Color.DarkGray},
                    {ConsoleColor.DarkGreen, Color.DarkGreen},
                    {ConsoleColor.DarkMagenta, Color.DarkMagenta},
                    {ConsoleColor.DarkRed, Color.DarkRed},
                    {ConsoleColor.DarkYellow, Color.DarkKhaki},
                    {ConsoleColor.Gray, Color.Gray},
                    {ConsoleColor.Green, Color.Green},
                    {ConsoleColor.Magenta, Color.Magenta},
                    {ConsoleColor.Red, Color.Red},
                    {ConsoleColor.White, Color.White},
                    {ConsoleColor.Yellow, Color.Yellow}
                };

        static internal Color Adapt(ConsoleColor consoleColor, Color @default)
        {
            if (!ColorMap.ContainsKey(consoleColor))
            {
                return @default;
            }

            return ColorMap[consoleColor];
        }

        static internal ConsoleColor Adapt(Color color,ConsoleColor @default)
        {
            if (!ColorMap.ContainsValue(color))
            {
                return (from clr in ColorMap
                         let red = Math.Abs(color.R - clr.Value.R)
                         let blue = Math.Abs(color.B - clr.Value.B)
                         let green = Math.Abs(color.G - clr.Value.G)
                         let delta = red + blue + green
                         select new { Color = clr, Delta = delta }
                    ).OrderBy(t => t.Delta).First().Color.Key;
            }

            return (from pair in ColorMap where pair.Value == color select pair.Key).FirstOrDefault();
        }
    }
}
