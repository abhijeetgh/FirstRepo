using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class AjaxExtensions
    {
        public static IHtmlString MyActionLink(
            this AjaxHelper ajaxHelper,
            string linkText,
            string actionName,
            AjaxOptions ajaxOptions
        )
        {
            var targetUrl = UrlHelper.GenerateUrl(null, actionName, null, null, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(linkText, targetUrl,String.Empty, ajaxOptions ?? new AjaxOptions()));
        }

        public static IHtmlString MyActionLink(
            this AjaxHelper ajaxHelper,
            string linkText,
            string actionName,
            string htmlClasses,
            object routeValues,
            AjaxOptions ajaxOptions
        )
        {
            System.Web.Routing.RouteValueDictionary routeVals = new System.Web.Routing.RouteValueDictionary(routeValues);

            var targetUrl = UrlHelper.GenerateUrl(null, actionName, null, routeVals, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(linkText, targetUrl, htmlClasses, ajaxOptions ?? new AjaxOptions()));
        }

        public static IHtmlString MyActionLink(
           this AjaxHelper ajaxHelper,
           string linkText,
           string actionName,
           string controllerName,
           string htmlClasses,
           object routeValues,
           AjaxOptions ajaxOptions
       )
        {
            System.Web.Routing.RouteValueDictionary routeVals = new System.Web.Routing.RouteValueDictionary(routeValues);

            var targetUrl = UrlHelper.GenerateUrl(null, actionName, controllerName, routeVals, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(linkText, targetUrl, htmlClasses, ajaxOptions ?? new AjaxOptions()));
        }

        private static string GenerateLink(
            string linkText,
            string targetUrl,
            string classes,
            AjaxOptions ajaxOptions
        )
        {
            var a = new TagBuilder("a")
            {
                InnerHtml = linkText
            };
            a.MergeAttribute("href", targetUrl);
            a.MergeAttribute("class", classes);
            a.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            return a.ToString(TagRenderMode.Normal);
        }


        public static IHtmlString MyActionLink(
         this HtmlHelper htmlHelper,
         string linkText,
         string actionName,
         string controllerName,
         string htmlClasses,
         object routeValues,
           IDictionary htmlAttributes,
          string onClick
      )
        {
            System.Web.Routing.RouteValueDictionary routeVals = new System.Web.Routing.RouteValueDictionary(routeValues);

            var targetUrl = UrlHelper.GenerateUrl(null, actionName, controllerName, routeVals, htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(linkText, targetUrl, htmlClasses,htmlAttributes, onClick));
        }

        private static string GenerateLink(
            string linkText,
            string targetUrl,
            string classes,
             IDictionary htmlAttributes,
            string onClick
        )
        {
            var a = new TagBuilder("a")
            {
                InnerHtml = linkText
            };
            a.MergeAttribute("href", targetUrl);
            a.MergeAttribute("class", classes);
            a.MergeAttribute("onclick", onClick);
            //a.MergeAttribute("htmlAttributes", htmlAttributes);

            return a.ToString(TagRenderMode.Normal);
        }

    } 
}