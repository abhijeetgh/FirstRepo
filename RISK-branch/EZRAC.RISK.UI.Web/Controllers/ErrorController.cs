using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ErrorController : Controller
    {
        //
        //// GET: /Error/
        //public ActionResult Index(string type)
        //{
        //    return new ElmahResult(type);
        //}

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Forbidden()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

    }


    public class ElmahHandledErrorLoggerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Log only handled exceptions, because all other will be caught by ELMAH anyway.
            if (context.ExceptionHandled)
                ErrorSignal.FromCurrentContext().Raise(context.Exception);
        }
    }


}

//class ElmahResult : ActionResult
//{
//    private string _resouceType;

//    public ElmahResult(string resouceType)
//    {
//        _resouceType = resouceType;
//    }

//    public override void ExecuteResult(ControllerContext context)
//    {
//        var factory = new Elmah.ErrorLogPageFactory();

//        if (!string.IsNullOrEmpty(_resouceType))
//        {
//            var pathInfo = "." + _resouceType;
//            HttpContext.Current.RewritePath(PathForStylesheet(), pathInfo, HttpContext.Current.Request.QueryString.ToString());
//        }

//        var httpHandler = factory.GetHandler(HttpContext.Current, null, null, null);
//        httpHandler.ProcessRequest(HttpContext.Current);
//    }

//    private string PathForStylesheet()
//    {
//        return _resouceType != "stylesheet" ? HttpContext.Current.Request.Path.Replace(String.Format("/{0}", _resouceType), string.Empty) : HttpContext.Current.Request.Path;
//    }
//}