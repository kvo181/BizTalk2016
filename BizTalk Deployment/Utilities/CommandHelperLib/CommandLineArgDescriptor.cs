using System;
using System.Globalization;
using System.Text;

namespace bizilante.Tools.CommandLine
{
    public class CommandLineArgDescriptor
    {
        private string description;
        private int maxOccurs;
        private int minOccurs;
        private string name;
        private bool named;
        private ArgumentType type;

        public CommandLineArgDescriptor(bool named, string name, string description, ArgumentType type)
        {
            this.named = named;
            this.name = name;
            this.description = description;
            this.type = type;
            this.maxOccurs = 1;
        }

        public CommandLineArgDescriptor(bool named, string name, string description, ArgumentType type, int minOccurs, int maxOccurs)
        {
            this.named = named;
            this.name = name;
            this.description = description;
            this.type = type;
            this.minOccurs = minOccurs;
            this.maxOccurs = maxOccurs;
        }

        public string GetOption()
        {
            string str;
            if (this.named)
            {
                str = string.Format(CultureInfo.InvariantCulture, "-{0} {1}", new object[] { this.name.PadRight(0x11, ' '), this.description });
            }
            else
            {
                str = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { this.name.PadRight(0x12, ' '), this.description });
            }
            return ConsoleHelper.Wrap(str, 0x15, Console.BufferWidth, -19);
        }

        public string GetUsage()
        {
            StringBuilder builder = new StringBuilder();
            bool flag = this.MinOccurs == 0;
            builder.Append(flag ? "[" : string.Empty);
            builder.Append(this.named ? "-" : string.Empty);
            builder.Append(this.name);
            if (this.named)
            {
                if (this.type == ArgumentType.String)
                {
                    builder.Append(":value");
                }
                else if (this.type == ArgumentType.Boolean)
                {
                    builder.Append("[+|-]");
                }
                else
                {
                    builder.Append(string.Empty);
                }
            }
            builder.Append(flag ? "]" : string.Empty);
            return builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            bool flag = this.MinOccurs == 0;
            builder.Append(flag ? "Optional " : "Required ");
            builder.Append(this.named ? "Named " : "Unnamed ");
            builder.Append(this.type.ToString() + " ");
            builder.Append(this.name);
            return builder.ToString();
        }

        public int MaxOccurs
        {
            get
            {
                return this.maxOccurs;
            }
        }

        public int MinOccurs
        {
            get
            {
                return this.minOccurs;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public bool Named
        {
            get
            {
                return this.named;
            }
        }

        public ArgumentType Type
        {
            get
            {
                return this.type;
            }
        }

        public enum ArgumentType
        {
            Simple,
            Boolean,
            String
        }
    }
}

