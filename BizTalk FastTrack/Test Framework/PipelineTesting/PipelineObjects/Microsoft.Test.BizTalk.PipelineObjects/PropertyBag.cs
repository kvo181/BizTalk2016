namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class PropertyBag : IBasePropertyBag, IEnumerable
    {
        private Hashtable properties = new Hashtable();

        public void CloneProperties(Hashtable properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            this.properties = (Hashtable) properties.Clone();
        }

        public IEnumerator GetEnumerator()
        {
            return this.properties.GetEnumerator();
        }

        protected string GetKey(string name, string namesp)
        {
            string str = name;
            if (namesp != null)
            {
                str = str + '@' + namesp;
            }
            return str;
        }

        public object Read(string name, string namesp)
        {
            return this.properties[this.GetKey(name, namesp)];
        }

        public object ReadAt(int index, out string name, out string namesp)
        {
            name = null;
            namesp = null;
            int num = 0;
            foreach (string str in this.properties.Keys)
            {
                if (num++ == index)
                {
                    string[] strArray = str.Split(new char[] { '@' });
                    if (strArray.Length == 0)
                    {
                        throw new InvalidOperationException("No name and namespace are found");
                    }
                    name = strArray[0];
                    if (strArray.Length > 1)
                    {
                        namesp = strArray[1];
                    }
                    return this.properties[str];
                }
            }
            return null;
        }

        public void Write(string name, string namesp, object obj)
        {
            this.properties[this.GetKey(name, namesp)] = obj;
        }

        public uint CountProperties
        {
            get
            {
                return (uint) this.properties.Count;
            }
        }

        public Hashtable Properties
        {
            get
            {
                return this.properties;
            }
        }
    }
}

