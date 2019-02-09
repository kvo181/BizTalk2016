using System;
using System.Collections;

namespace bizilante.BaseClasses
{
    /// <summary>
    /// Some Helper functions for strings
    /// </summary>
    public sealed class StringHelper
    {
        /// <summary>
        /// Convert ArrayList to an Array
        /// </summary>
        /// <param name="list">ArrayList</param>
        /// <returns>An Array</returns>
        public static string[] ToStringArray(ArrayList list)
        {
            return (string[])list.ToArray(typeof(string));
        }

        /// <summary>
        /// Join a list of strings
        /// </summary>
        /// <param name="seperator">Seperator between the strings</param>
        /// <param name="list">List of Strings</param>
        /// <returns>One string seperated with our defined seperator</returns>
        public static string Join(string seperator, ArrayList list)
        {
            return string.Join(seperator, StringHelper.ToStringArray(list));
        }

        /// <summary>
        /// Replace the oldValue with the newValue in the given string
        /// </summary>
        /// <param name="value">Old string</param>
        /// <param name="oldValue">String to be replaced</param>
        /// <param name="newValue">String instead of the old string</param>
        /// <returns>New string</returns>
        public static string Replace(string value, string oldValue, string newValue)
        {
            return value.Replace(oldValue, newValue);
        }

        /// <summary>
        /// To Uppercase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUpper(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Trim().ToUpper();
        }

        /// <summary>
        /// Split a string into parts and return the part referred to by index (1-based)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delim"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SplittAndReturn(string value, string delim, int index)
        {
            string[] values = value.Split(new string[] { delim }, StringSplitOptions.None);
            return values[index - 1].Trim();
        }

        /// <summary>
        /// Format string to a fixed field with a filler
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatWithFixedFieldLength(string value, string length, string filler)
        {
            if (value == null) value = string.Empty;
            try
            {
                return FormatWithFixedFieldLength(value.Trim(), Convert.ToInt32(length), Convert.ToChar(filler));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string FormatWithFixedFieldLength(string value, int length, char filler)
        {
            if (value.Length > length) return value.Substring(value.Length - length);
            return value.PadLeft(length, filler);
        }
    }
}
