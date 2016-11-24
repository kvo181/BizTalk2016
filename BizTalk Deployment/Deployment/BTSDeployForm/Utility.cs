using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace bizilante.Deployment.BTSDeployForm
{
    public class Utility
    {
        static Dictionary<Color, ConsoleColor> map = new Dictionary<Color, ConsoleColor>();

        private static void InitColorMap()
        {
            map[Color.Black] = ConsoleColor.Black;
            map[Color.Blue] = ConsoleColor.Blue;
            map[Color.Cyan] = ConsoleColor.Cyan;
            map[Color.DarkBlue] = ConsoleColor.DarkBlue;
            map[Color.DarkCyan] = ConsoleColor.DarkCyan;
            map[Color.DarkGray] =  ConsoleColor.DarkGray;
            map[Color.DarkGreen] = ConsoleColor.DarkGreen;
            map[Color.DarkMagenta] = ConsoleColor.DarkMagenta;
            map[Color.DarkRed] = ConsoleColor.DarkRed;
            map[Color.Gray] = ConsoleColor.Gray;
            map[Color.Green] = ConsoleColor.Green;
            map[Color.Magenta] = ConsoleColor.Magenta;
            map[Color.Red] = ConsoleColor.Red;
            map[Color.White] = ConsoleColor.White;
            map[Color.Yellow] = ConsoleColor.Yellow;
        }

        public static ConsoleColor ToConsoleColor(Color color)
        {
            if (map.Count == 0)
                InitColorMap();
            if (!map.ContainsKey(color))
                return ConsoleColor.Black;
            return map[color];
        }
        public static Color ToColor(ConsoleColor color)
        {
            if (map.Count == 0)
                InitColorMap();
            if (!map.ContainsValue(color))
                return Color.Black;
            return map.Where(cc => cc.Value == color).First().Key;
        }

    }
}
