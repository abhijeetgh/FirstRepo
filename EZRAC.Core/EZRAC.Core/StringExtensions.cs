using System;
using System.Globalization;

namespace EZRAC.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the format item in a specified System.String with the text equivalent of the value 
        /// of a corresponding System.Object instance in a specified array.
        /// </summary>
        /// <param name="instance">A string to format.</param>
        /// <param name="args">An System.Object array containing zero or more objects to format.</param>
        /// <returns>
        /// A copy of format in which the format items have been replaced by the System.String 
        /// equivalent of the corresponding instances of System.Object in args.
        /// </returns>
        public static string FormatWith(this string instance, params object[] args)
        {
            return String.Format(CultureInfo.CurrentCulture, instance, args);
        }
    }
}
