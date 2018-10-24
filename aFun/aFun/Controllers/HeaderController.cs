using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aFun.Controllers
{
    public class HeaderController : Controller
    {
        //
        // GET: /Header/

        public ActionResult Index()
        {
            string result = "";
            NameValueCollection headers = base.Request.Headers;
            for (int i = 0; i < headers.Count; i++)
            {
                string key = headers.GetKey(i);
                string value = headers.Get(i);
                result += key + " = " + value + "<br/>";
            }
            result +=  "IP = " + aFun.Models.Common.GetClientIpAddress() + "<br/>";
            ViewBag.result = result;
            return View();
        }


    }
}
