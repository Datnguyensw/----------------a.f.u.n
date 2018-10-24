using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace aFun
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.DOMConfigurator.Configure();

            //string ip = aFun.Models.Common.GetClientIpAddress();
            //HttpCookie cookread = new HttpCookie("read" + ip);
            //cookread.Name = "read" + ip;
            //cookread.Value = "1";
            //cookread.Expires = DateTime.Now.AddDays(1);
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            if (Session["LoginName"] != null)
            {
                System.Web.HttpContext.Current.Session.Abandon();
                System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                
            }
            //string sessionId = System.Web.HttpContext.Current.Session.SessionID;
        }
        protected void Session_End(object sender, EventArgs e)
        {
            if (Session["LoginName"] != null)
            {
                System.Web.HttpContext.Current.Session.Abandon();
                System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                //string sessionId = System.Web.HttpContext.Current.Session.SessionID;
            }

        }

    }
}