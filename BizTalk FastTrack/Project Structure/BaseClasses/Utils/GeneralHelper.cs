using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace bizilante.BaseClasses
{
    /// <summary>
    /// Some General Helper functions
    /// </summary>
    public sealed class GeneralHelper
    {
        /// <summary>
        /// Evaluate string. If it's null or empty, return the value
        /// </summary>
        /// <param name="statement">Statement Value</param>
        /// <param name="value">Value</param>
        /// <returns>Statement OR Value</returns>
        public static string IfIsEmptyReturnValue(string statement, string value)
        {
            if (string.IsNullOrEmpty(statement)) return value;
            return statement;
        }

        private static int _counter = 0;
        /// <summary>
        /// Gets the next counter
        /// </summary>
        /// <returns>Converted counter result</returns>
        public static string GetNextCounter()
        {
            return Convert.ToString(++_counter);
        }
    }
}
