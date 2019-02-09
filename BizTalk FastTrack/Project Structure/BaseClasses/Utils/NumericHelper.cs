using System;

namespace bizilante.BaseClasses
{
    /// <summary>
    /// Some Helper functions for Numeric Values
    /// </summary>
    public sealed class NumericHelper
    {
        /// <summary>
        /// Format a value to a Fixed length string
        /// </summary>
        /// <param name="value">Value we need to format</param>
        /// <param name="length">Length formatted string</param>
        /// <param name="decimalLength">Decimal Length formatted string</param>
        /// <returns>Formatted string</returns>
        public static string FormatFixedFieldLengthWithDecimal(string value, string length, string decimalLength)
        {
            if (value == null) value = string.Empty;
            try
            {
                decimal d = Convert.ToDecimal(value);

                int dl = Convert.ToInt32(decimalLength);
                int l = Convert.ToInt32(length) - dl;
                if (l <= 0) throw new Exception();

                int factor = 1;
                for (int i = 0; i < dl; i++)
                {
                    factor *= 10;
                }

                int rest = (int)((d * factor) % factor);
                int geheelDeel = (int)(d - (rest / factor));

                return FormatWithFixedFieldLength(Convert.ToString(geheelDeel), l, '0') + FormatWithFixedFieldLength(Convert.ToString(rest), dl, '0');
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Format a value to a Fixed length string
        /// </summary>
        /// <param name="value">Value we need to format</param>
        /// <param name="length">Length formatted string</param>
        /// <param name="filler">Value to add on the left side of the string</param>
        /// <returns>Formatted string</returns>
        private static string FormatWithFixedFieldLength(string value, int length, char filler)
        {
            if (value.Length > length) return value.Substring(value.Length - length);
            return value.PadLeft(length, filler);
        }
    }
}
