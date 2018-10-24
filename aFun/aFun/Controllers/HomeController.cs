using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aFun.Models;
using log4net;

namespace aFun.Controllers
{

    public class HomeController : Controller
    {
        //
        // GET: /Home/
        int i_tab = 0;
        int i_pageIndex = 0;
        int i_category = 0;
        int i_pageCurent = 0;
        int i_pageSize = 10;
        private ILog logger = log4net.LogManager.GetLogger(typeof(HomeController));
        bool chekpack()
        {
            if (Session["LoginName"] != null)
            {
                try
                {
                    int createStatus;
                    JObject mo = MyControllers.ListPackge(Session["LoginName"].ToString(), "1");
                    //createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    string mosd = mo.ToString();
                    mo = JObject.Parse(mo["Data"]["USER"].ToString());
                    string mmmos = mo.ToString();
                    JArray ma = new JArray();
                    foreach (var a in ma)
                    {
                        createStatus = Convert.ToInt32(a["REDAY"].ToString());
                        if (a["SERVICE_CODE"].ToString() == "VD" && createStatus >= 0)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return false;

        }
        
        public void check3g()
        {

            //string ipadd = System.Web.HttpContext.Current.Request.UserHostAddress;
            //ipadd = "10.146.14.84";
            //string Telco = RadiusVTL.Telco(ipadd);
            string msisdnnull = "";
            String msisdn = "";
            if (Request.QueryString["m"] != null)
            {
                if (Request.QueryString["m"] != "0null" && Request.QueryString["m"] != "null" && Request.QueryString["m"] != "0" && Session["msisdn"] == null)
                {
                    Session["msisdn"] = Request.QueryString["m"].ToString();
                    msisdnnull = "";
                }
                else
                {
                    Session["msisdn"] = null;
                    msisdnnull = "1";
                }
            }
            else
            {
                Session["msisdn"] = null;
                msisdnnull = "";
            }
            //Session["msisdn"] = "84989260781";
            //Session["msisdn"] = "84988954658";
            if (Session["msisdn"] == null && Session["user_login"] == null && msisdnnull != "1")
            {
                string urlvc = aFun.Models.ResponeUrlVTL.getUrlDetect(System.Configuration.ConfigurationManager.AppSettings["getUrlDetect"], "WAP");
                if (urlvc.Contains('&'))
                {
                    string cprequest = "";
                    string[] spliturl = urlvc.Split('&');
                    logger.Info("urlvc=" + urlvc);
                    if (spliturl.Length > 0)
                    {
                        cprequest = spliturl[5].ToString();
                        cprequest = cprequest.Substring(4);
                    }
                    JObject mo = new JObject();
                    try
                    {
                        mo = MyControllers.LogUrlmps(cprequest.Trim(), MakeLink.DefaultURLWap(), "M");
                    }
                    catch
                    {
                        mo = null;
                    }
                    if (mo != null)
                    {
                        Response.Redirect(urlvc);
                    }
                }
            }

            if (Session["msisdn"] != null && Session["user_login"] == null)
            {
                //string statuslog = "";
                string phonenumber = Session["msisdn"].ToString();
                if (phonenumber.StartsWith("84"))
                {
                    phonenumber = "0" + phonenumber.Substring(2);
                }
                JObject mo = new JObject();
                int createStatus = -1;
                int num_pack = 0;
                string SERVICE_CODE, EXPIRE_DATE;
                int PLUS = -1;
                SERVICE_CODE = EXPIRE_DATE = "";
                mo = MyControllers.Login(phonenumber, "1234567", 1);
                createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                if (createStatus == 0)
                {
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    if (mo["SERVICE"] != null && !mo["SERVICE"].ToString().StartsWith("[]"))
                    {
                        JArray ma = JArray.Parse(mo["SERVICE"].ToString());
                        if (ma != null && ma.Count > 0)
                        {
                            foreach (var item in ma)
                            {
                                SERVICE_CODE = item["SERVICE_CODE"] != null ? item["SERVICE_CODE"].ToString() : "";
                                EXPIRE_DATE = item["EXPIRE_DATE"].ToString();
                                string m = "-1";
                                if (!string.IsNullOrEmpty(EXPIRE_DATE) && EXPIRE_DATE.Length > 10)
                                {
                                    m = Convert.ToDateTime(EXPIRE_DATE).CompareTo(DateTime.Now).ToString();
                                }
                                else if (!string.IsNullOrEmpty(EXPIRE_DATE) && EXPIRE_DATE.Length == 10)
                                {
                                    m = DateTime.ParseExact(EXPIRE_DATE, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat).CompareTo(DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat)).ToString();
                                }
                                PLUS = Convert.ToInt32(m);
                                if (PLUS >= 0)
                                {
                                    num_pack++;
                                }
                            }

                            Session["PACKGE"] = num_pack;
                        }

                    }
                    mo = JObject.Parse(JArray.Parse(mo["USER"].ToString())[0].ToString());
                    Session["LoginCode"] = mo["USER_CODE"].ToString();
                    Session["LoginName"] = mo["USER_NAME"].ToString();
                    Session["LoginDisplay"] = mo["FULL_NAME"].ToString();

                }
            }
            if (System.Configuration.ConfigurationManager.AppSettings["redirectcharge_mps"].ToString() == "1")
            {
                if (Session["msisdn"] != null && Session["PACKGE"] == null && Session["RegPackge"] == null)
                {
                    Response.Redirect("/acc/RegPackge");
                }
            }


        }
        
        
        public void LoadDataHome()
        {
            try
            {
                Session["showboxlogin"] = null;
                if (System.Configuration.ConfigurationManager.AppSettings["redirectweb"].ToString() == "1")
                {
                    string useragent = Request.UserAgent;
                    logger.Info("Log useragent: " + useragent);
                    if (!CheckDevice.IsMobileDevice(useragent))
                    {
                        Response.Redirect("http://www.aFun.vn" + Request.RawUrl.ToString());
                    }
                    
                }
                
                JObject mo = MyControllers.GetDataHome();
                if (mo != null)
                {

                    JObject jfooter = JObject.Parse(mo["Footer"].ToString());
                    Core.token = jfooter["Token"].ToString();
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    string m = mo.ToString(); 
                    ViewBag.TAB = mo["HOME"] == null ? null : JArray.Parse(mo["HOME"].ToString());
                    ViewBag.MENULEFT = mo["MENU"] == null ? null : JArray.Parse(mo["MENU"].ToString());
                    ViewBag.SLIDE = mo["SLIDE"] == null ? null : JArray.Parse(mo["SLIDE"].ToString());
                  // ViewBag.BOOK = mo["0004"] == null ? null : JArray.Parse(mo["0004"].ToString());
                    ViewBag.NEW = mo["0003"] == null ? null : JArray.Parse(mo["0003"].ToString());
                    ViewBag.VIDEO = mo["0004"] == null ? null : JArray.Parse(mo["0004"].ToString());
                    ViewBag.mo = mo;
                }
                else
                {
                    ViewBag.TAB = null;
                    ViewBag.MENULEFT = null;
                    ViewBag.SLIDE = null;
                    ViewBag.mo = mo;
                }
            }
            catch (Exception)
            {
            }
        }
        public void LoadDataHomeQuick()
        {
            try
            {
                Session["showboxlogin"] = null;
                //string useragent = Request.UserAgent;
                //logger.Info(" log useragent:" + useragent);
                JObject mo = MyControllers.GetDataHome();
                if (mo != null)
                {
                    JObject jfooter = JObject.Parse(mo["Footer"].ToString());
                    Core.token = jfooter["Token"].ToString();
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    string home = mo.ToString();
                    ViewBag.TAB = mo["HOME"] == null ? null : JArray.Parse(mo["HOME"].ToString());
                    ViewBag.MENULEFT = mo["MENU"] == null ? null : JArray.Parse(mo["MENU"].ToString());
                    ViewBag.SLIDE = mo["SLIDE"] == null ? null : JArray.Parse(mo["SLIDE"].ToString());
                }
                else
                {
                    ViewBag.MENULEFT = null;
                    ViewBag.SLIDE = null;
                }
            }
            catch (Exception)
            {
            }
        }

        public void LoadDataHomeBook() { 
        
        }
        
        public ActionResult Index()
        {
            
            LoadDataHome();
            string pack_adu = "NOT";
            if (chekpack())
            {
                pack_adu = "OK";
            }
            ViewBag.PACKADU = pack_adu;
            if (System.Configuration.ConfigurationManager.AppSettings["check3g"].ToString() == "1")
            {

                if (Session["msisdn"] == null && Session["LoginName"] == null)
                {
                    try
                    {
                        check3g();
                    }
                    catch (Exception)
                    {
                    }
                    //CheckDevice.login3g();
                }
            }
            return View();
        }

        public ActionResult Search(string key)
        {
            SearchAllObjectModel model = new SearchAllObjectModel();
            
                LoadDataHomeQuick();
                RequestParam();
                if (string.IsNullOrEmpty(key)) key = "all";
                model.keyword = key;
                //ViewBag.TAB_DEFAULT = i_tab;
                key = MakeLink.ConvertVN(key.ToLower());
               JObject mo = MyControllers.GetSearchAjax(key,5);
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
                return View(model);
            //return View(model);
        }
        public ActionResult test() {
          
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Search(string key, SearchAllObjectModel model)
        {
            
                    LoadDataHomeQuick();
                    RequestParam();
                    key = model.keyword;
                    key = MakeLink.ConvertVN(key.ToLower());
                  JObject mo = MyControllers.GetSearchAjax(key,5);
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
                return View(model);

        }
        public ActionResult Help()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            LoadDataHomeQuick();
            JObject mo = MyControllers.GetSysvarByCode("GUIDE");
            try
            {
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["GUIDE"].ToString());
                mo = JObject.Parse(ma[0].ToString());
                ViewBag.info = mo;
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public ActionResult About()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            LoadDataHomeQuick();
            JObject mo = MyControllers.GetSysvarByCode("ABOUT");
            try
            {
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["GUIDE"].ToString());
                mo = JObject.Parse(ma[0].ToString());
                ViewBag.info = mo;
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public ActionResult CaseUser()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            LoadDataHomeQuick();
            JObject mo = MyControllers.GetSysvarByCode("RULE");
            try
            {
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["GUIDE"].ToString());
                mo = JObject.Parse(ma[0].ToString());
                ViewBag.info = mo;
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public ActionResult CallBack()
        {
            LoadDataHomeQuick();
            return View();
        }
        public ActionResult Download()
        {
            LoadDataHomeQuick();
            return View();
        }
        public ActionResult DownloadApp(String id)
        {
            string path = "";
            string fullPath = "";
            if (id == "2")
            {
                path = Server.MapPath("/Download/aFun.apk");
                fullPath = Path.Combine(path);
                return File(fullPath, "application/vnd.android.package-archive", "aFun.apk");
            }
            else
            {
                path = Server.MapPath("/Download/aFun.ipa");
                fullPath = Path.Combine(path);
                return File(fullPath, "application/octet-stream", "aFun.ipa");
            }


        }
        public ActionResult Msg()
        {
            LoadDataHomeQuick();
            return View();
        }
        public ActionResult OpenMsgBox()
        {
            ViewBag.bookid = TempData["bookid"].ToString();
            //Session["BOOK_DUM"] = null;
            return PartialView("_MsgBox");
        }
        public ActionResult OpenMsgBoxApp()
        {
            return PartialView("_MsgBoxApp");
        }

        private void RequestParam()
        {
            if (Request.QueryString["t"] != null)
            {
                int.TryParse(Request.QueryString["t"], out i_tab);
            }
            if (Request.QueryString["c"] != null)
            {
                int.TryParse(Request.QueryString["c"], out i_category);
            }
            if (Request.QueryString["p"] != null)
            {
                int.TryParse(Request.QueryString["p"], out i_pageIndex);
                i_pageIndex = i_pageIndex - 1;
            }

        }


    }
}
