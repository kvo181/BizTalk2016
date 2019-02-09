using System;

namespace bizilante.ManagementConsole.SSO
{
    public class EventArgs<T> : EventArgs
    {
        private T m_value;
        public T Value
        {
            get
            {
                return m_value;
            }
        }
        public EventArgs(T value)
        {
            m_value = value;
        }
    }
    public class EventArgs<T, S> : EventArgs
    {
        private T _value;

        private S _information;

        public T Value
        {
            get
            {
                return this._value;
            }
        }

        public S Information
        {
            get
            {
                return this._information;
            }
        }

        public EventArgs(T value, S information)
        {
            this._value = value;
            this._information = information;
        }
    }
}
