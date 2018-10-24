using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aFun.Models;
namespace aFun.Controllers
{
    public class AjaxController : Controller
    {
        //GetSearchAjax
        // GET: /Ajax/

        public ActionResult Index()
        {
           
            return View();
        }


        public ActionResult Search(string key)
        {
            JObject mo = MyControllers.GetSearchAjax(key,3);
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            string a = mo.ToString();
            JArray slaw = JArray.Parse(mo["LAW"].ToString());
            JArray snew = JArray.Parse(mo["NEW"].ToString());
            JArray sbook = JArray.Parse(mo["BOOK"].ToString());
            JArray svideo = JArray.Parse(mo["VIDEO"].ToString());
            JArray svideo_TH = JArray.Parse(mo["VIDEOTH"].ToString());
            ViewBag.aLaw = slaw;
            ViewBag.anew = snew;
            ViewBag.abook = sbook;
            ViewBag.avideoth = svideo_TH;
            ViewBag.avideo = svideo;

            return View();
        }

    }
}
