using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using EZRAC.Risk.UI.Web.Controllers;

namespace EZRAC.Risk.UI.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Sid is the unique identifier in User. So, set this for unique claim
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Sid;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                HttpContextWrapper context = new HttpContextWrapper(Context);
                if (context.Request.IsAjaxRequest())
                {

                    Exception ex = Server.GetLastError();
                    if (ex != null)
                    {
                        if (ex.GetBaseException() != null)
                            ex = ex.GetBaseException();
                    }

                    //Log Error in DB                    

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Response.Clear();
                    Response.TrySkipIisCustomErrors = true;
                    Response.ContentType = "application/json";
                    Response.StatusCode = 500;
                    Response.Write(serializer.Serialize(new
                    {
                        Message = ex.Message,
                        IsModelStateError = ex.GetType() 
                    }));

                    Response.End();
                }
                else
                {
                    var httpContext = ((MvcApplication)sender).Context;
                    var ex = Server.GetLastError();
                    var controller = new ErrorController();
                    var routeData = new RouteData();
                    var action = "Error";
                    var httpEx = ex as HttpException;

                    if (httpEx != null)
                    {
                        httpContext.Response.StatusCode = httpEx != null ? httpEx.GetHttpCode() : 500;
                        switch (httpEx.GetHttpCode())
                        {
                            case 403:
                                action = "PageForbidden";
                                break;
                            case 404:
                                action = "NotFound";
                                break;
                            case 500:
                                action = "Error";
                                break;
                            // others if any
                            default:
                                action = "Error";
                                break;
                        }
                    }

                    httpContext.ClearError();
                    httpContext.Response.Clear();
                    httpContext.Response.ContentType = "text/html";
                    httpContext.Response.TrySkipIisCustomErrors = true;

                    routeData.Values["controller"] = "Errors";
                    routeData.Values["action"] = action;                  

                    //Log Error in DB
                   

                    //Redirect to Error Controller for further processing
                    ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
