using RateShopper.ScrapperAPIService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RateShopper.ScrapperAPIService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Filters.Add(new ElmahErrorAttribute());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
