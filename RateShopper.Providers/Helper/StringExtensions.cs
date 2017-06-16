using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace RateShopper.Providers.Helper
{    
    public static class StringExtensions
    {
        /// <summary>
        /// Formats a string uses named, rather than number, tokens.
        /// </summary>
        /// <remarks>This method was borrowed from 'HenriFormatter' located at https://gist.github.com/47888</remarks>
        /// <param name="format">The format string, containing zero or more named tokens.</param>
        /// <param name="sources">The objects from which properties corresponding to token names will be extracted.</param>
        /// <returns>A formatted string.</returns>
        public static string FormatWith(this string format, params object[] sources)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (sources == null)
                throw new ArgumentNullException("sources");

            var source = sources.Length > 1 ? new ReplacementParameters(sources) : sources.FirstOrDefault();
            var result = new StringBuilder(format.Length * 2);
            var expression = new StringBuilder();
            var e = format.GetEnumerator();

            while (e.MoveNext())
            {
                var ch = e.Current;
                if (ch == '{')
                {
                    while (true)
                    {
                        if (!e.MoveNext())
                            throw new FormatException();

                        ch = e.Current;
                        if (ch == '}')
                        {
                            result.Append(OutExpression(source, expression.ToString()));
                            expression.Length = 0;
                            break;
                        }
                        if (ch == '{')
                        {
                            result.Append(ch);
                            break;
                        }
                        expression.Append(ch);
                    }
                }
                else if (ch == '}')
                {
                    if (!e.MoveNext() || e.Current != '}')
                        throw new FormatException();
                    result.Append('}');
                }
                else
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }        

        private static string OutExpression(object source, string expression)
        {
            string format = "{0}";

            int colonIndex = expression.IndexOf(':');
            if (colonIndex > 0)
            {
                format = "{0:" + expression.Substring(colonIndex + 1) + "}";
                expression = expression.Substring(0, colonIndex);
            }

            try
            {
                return DataBinder.Eval(source, expression, format);
            }
            catch (HttpException e)
            {
                throw new FormatException(e.Message);
            }
        }
    }
}
