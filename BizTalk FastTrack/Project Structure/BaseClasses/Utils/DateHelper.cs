using System;

namespace bizilante.BaseClasses
{
    /// <summary>
    /// Some Helper function for DateTime formatting
    /// </summary>
    public sealed class DateHelper
    {
        /// <summary>
        /// Formats a datetime string to the given format
        /// </summary>
        /// <param name="datetime">DateTime in string Format</param>
        /// <param name="format">Format of the output</param>
        /// <returns>Formatted DateTime</returns>
        public static string FormatDateTimeString(string datetime, string format)
        {
            DateTime dt;
            if (!DateTime.TryParse(datetime, out dt)
                || string.IsNullOrEmpty(datetime)
                || string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }
            dt = DateTime.Parse(datetime);
            return dt.ToString(format);
        }

        /// <summary>
        /// Formats a datetime string to the given format
        /// </summary>
        /// <param name="datetime">DateTime in string Format</param>
        /// <param name="format">Format of the output</param>
        /// <param name="currentformat">Current format of the input</param>
        /// <returns>Formatted DateTime</returns>
        public static string FormatDateTimeStringFromCurrentFormat(string datetime, string format, string currentformat)
        {
            // currentformat e.g. DD/MM/YYYY for .NET this has to be dd/MM/yyyy
            try
            {
                DateTime dt = DateTime.ParseExact(datetime, currentformat.Replace("D", "d").Replace("Y", "y"), null);
                return dt.ToString(format);
            }
            catch
            {
                return datetime;
            }
        }


        /// <summary>
        /// Converts the given DateTime object to a formatted JDE Date String
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <param name="leadingZero">Whether or not the converted string includes a leading zero (if applicable)</param>
        /// <returns>JDE Date representation</returns>
        public static string DateTimeToJDEDateString(DateTime date, bool leadingZero = true)
        {
            // use the DateTime to format the JDE Date string
            return string.Format("{0}{1:yy}{2}", (date.Year < 2000 ? (leadingZero ? "0" : "") : "1"), date, date.DayOfYear);
        }

        /// <summary>
        /// Converts the given formatted JDE Date string to a DateTime object
        /// </summary>
        /// <param name="date">JDE Date string to convert</param>
        /// <returns>DateTime object based on the JDE Date string</returns>
        public static DateTime JDEDateStringToDateTime(string date)
        {
            if (date.Length < 5 || date.Length > 6)
            {
                throw new ArgumentException(string.Format("The given string '{0}' doesn't have a valid JDE Date format! (Required format: [C]YYDDD)", date));
            }

            // for ease of use, add the optional 0 if it was left out
            if (date.Length == 5)
            {
                date = "0" + date;
            }

            // check for valid century
            string century = date.Substring(0, 1);
            if (century != "0" && century != "1")
            {
                throw new ArgumentException(string.Format("The given string '{0}' doesn't have a valid Century identifier! (Required '0' or '1', given '{1}')", date, century));
            }

            // construct DateTime based on the values of the JDE Date string
            try
            {
                int year = Int32.Parse(string.Format("{0}{1}", (date.Substring(0, 1) == "0" ? "19" : "20"), date.Substring(1, 2)));
                int dayOfYear = Int32.Parse(date.Substring(3, 3));

                // check for valid day of year
                if (dayOfYear < 1 || dayOfYear > 366)
                {
                    throw new ArgumentException(string.Format("The given string '{0}' doesn't have a valid day identifier! (Required between '1' and '366', given '{1}')", date, century));
                }

                return new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(string.Format("The given string '{0}' doesn't have a valid JDE Date format! (Required format: [C]YYDDD)", date), ex);
            }
        }
    }
}
