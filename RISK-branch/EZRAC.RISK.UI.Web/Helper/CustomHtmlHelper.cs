using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class CustomHtmlHelper
    {
        public static IHtmlString SortOrderMark(string sortBy,string markFor,bool sortOrder)
        {
            string htmlString = null;

             if (sortBy.Equals(markFor))
                        {
                            if (sortOrder)
                                htmlString="<a href='javascript:void(0);' class='display-inline  arrow-down sorter'></a>";
                            else
                                htmlString="<a href='javascript:void(0);' class='display-inline  arrow-up sorter'></a>";
                        }

            return new HtmlString(htmlString);
        }

        public static MvcHtmlString DisableIf(this MvcHtmlString htmlString, Func<bool> expression)
        {
            if (expression.Invoke())
            {
                var html = htmlString.ToString();
                const string disabled = "\"disabled\"";
                html = html.Insert(html.IndexOf(">",
                  StringComparison.Ordinal), " disabled= " + disabled);
                return new MvcHtmlString(html);
            }
            return htmlString;
        }

        public static string CustomReplace(this string result, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(newValue))
                result = result.Replace(oldValue, "NA");
            else
                result = result.Replace(oldValue, newValue);

            return result;
        }

    }
}