using System;
using System.Text;

namespace bizilante.Tools.CommandLine
{
    public static class ConsoleHelper
    {
        private static string CoreWrap(string text, int leftMargin, int rightMargin, int firstIndent)
        {
            StringBuilder builder = new StringBuilder();
            string[] strArray = text.Replace(Environment.NewLine, "\n").Split(new char[] { '\n' });
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i];
                StringBuilder builder2 = new StringBuilder();
                builder2.Append(new string(' ', leftMargin + firstIndent));
                bool flag = true;
                string[] strArray2 = str.Split(new char[] { ' ' });
                for (int j = 0; j < strArray2.Length; j++)
                {
                    string str2 = strArray2[j];
                    string str3 = ((j == 0) || flag) ? string.Empty : " ";
                    int num3 = ((builder2.Length + str3.Length) + str2.Length) + 1;
                    if (num3 >= rightMargin)
                    {
                        if (builder2.Length > 0)
                        {
                            builder.AppendLine(builder2.ToString());
                        }
                        builder2 = new StringBuilder();
                        builder2.Append(new string(' ', leftMargin));
                        builder2.Append(str2);
                        flag = false;
                    }
                    else
                    {
                        builder2.Append(str3);
                        builder2.Append(str2);
                        flag = false;
                    }
                }
                if (builder2.Length > 0)
                {
                    if (i < (strArray.Length - 1))
                    {
                        builder.AppendLine(builder2.ToString());
                    }
                    else
                    {
                        builder.Append(builder2.ToString());
                    }
                }
            }
            return builder.ToString();
        }

        public static string Wrap(string text, int leftMargin, int rightMargin, int firstIndent)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (leftMargin < 0)
            {
                leftMargin = 0;
            }
            if (rightMargin < 0)
            {
                rightMargin = 0;
            }
            if (rightMargin <= leftMargin)
            {
                rightMargin = 0x7fffffff;
            }
            if (((leftMargin + firstIndent) < 0) || ((leftMargin + firstIndent) >= rightMargin))
            {
                firstIndent = 0;
            }
            return CoreWrap(text, leftMargin, rightMargin, firstIndent);
        }
    }
}

