using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aFun.Models;
using log4net;
namespace WapMbook.Controllers
{
    public class PictureController : Controller
    {
        //
        // GET: /Book/
        private ILog logger = log4net.LogManager.GetLogger(typeof(PictureController));
        string i_tab = Common.booknume[0];
        int i_pageIndex = 0;
        string i_category = "ALL";
        public void LoadDataHome() { }
        //public void LoadDataHome()
        //{
        //    try 
        //    {
        //        //string useragent = Request.UserAgent;
        //        //logger.Info(" log useragent:" + useragent);
        //        //logger.Info(" log AbsolutePath:" + Request.Url.AbsolutePath);
        //        //if (!WapMbook.Models.CheckDevice.IsMobileDevice(useragent))
        //        //{
        //        //    Response.Redirect("http://www.mbookstore.vn" + Request.Url.AbsolutePath);
        //        //}
        //        JObject mo = MyControllers.GetDataHome(Common.catetype[2]);
        //        if (mo != null)
        //        {

        //            JObject jfooter = JObject.Parse(mo["Footer"].ToString());
        //            Core.token = jfooter["Token"].ToString();
        //            mo = JObject.Parse(mo["Body"]["Data"].ToString());
        //            ViewBag.MENUTOP = mo["MENUTOP"] == null ? null : JArray.Parse(mo["MENUTOP"].ToString());
        //            ViewBag.PARENT = mo["PARENT"] == null ? null : JArray.Parse(mo["PARENT"].ToString());
        //            ViewBag.ORDER = mo["ORDER"] == null ? null : JArray.Parse(mo["ORDER"].ToString());
        //            ViewBag.CATEGORY = mo["CATEGORY"] == null ? null : JArray.Parse(mo["CATEGORY"].ToString());
        //            ViewBag.SLIDESHOW = mo["SLIDESHOW"] == null ? null : JArray.Parse(mo["SLIDESHOW"].ToString());
        //        }
        //        else
        //        {
        //            ViewBag.MENUTOP = null;
        //            ViewBag.CATEGORY = null;
        //            ViewBag.SLIDESHOW = null;
        //            ViewBag.PARENT = null;
        //            ViewBag.ORDER = null;

        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        
        public void LoadDataHomeQuick()
        {
            try
            {
                JObject mo = MyControllers.GetDataHomeQuick();
                if (mo != null)
                {

                    JObject jfooter = JObject.Parse(mo["Footer"].ToString());
                    Core.token = jfooter["Token"].ToString();
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    ViewBag.PARENT = mo["PARENT"] == null ? null : JArray.Parse(mo["PARENT"].ToString());
                    ViewBag.SLIDESHOW = mo["SLIDESHOW"] == null ? null : JArray.Parse(mo["SLIDESHOW"].ToString());
                }
                else
                {
                    ViewBag.SLIDESHOW = null;
                    ViewBag.PARENT = null;
                }
            }
            catch (Exception)
            {
            }
        }
        public ActionResult Index()
        {
            try
            {
              //  LoadDataHome();
                RequestParam();
                //ViewBag.TAB_DEFAULT = i_tab;
                JObject mo = MyControllers.GetBookByTabCate(Common.catetype[2]);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["BOOK"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult Tab()
        {
            try
            {
                LoadDataHome();
                RequestParam();
                ViewBag.TAB_DEFAULT = i_tab;
                JObject mo = MyControllers.GetBookByTabCate(Common.catetype[2], "ALL", Convert.ToInt32(i_tab).ToString());
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["BOOK"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
            }
            catch (Exception)
            {
            }
            return View();
        }
        public ActionResult Cate()
        {
            try
            {
                LoadDataHome();
                RequestParam();
                ViewBag.i_category = i_category;
                //i_category = "ALL";
                JObject mo = MyControllers.GetBookByTabCate(Common.catetype[2], i_category.ToString());
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["BOOK"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                if (ma.Count > 0)
                {
                    ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                }

            }
            catch (Exception)
            {
            }
            return View();
        }
        public ActionResult TabCate()
        {
            try
            {
                LoadDataHome();
                RequestParam();
                ViewBag.TAB_DEFAULT = i_tab;
                ViewBag.i_category = i_category;
                JObject mo = MyControllers.GetBookByTabCate(Common.catetype[2], i_category, Convert.ToInt32(i_tab).ToString());
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["BOOK"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                if (ma.Count > 0)
                {
                    ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return View();
        }
        public ActionResult Details(string id, string m)
        {
            try
            {
                LoadDataHome();
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["BDETAIL"].ToString())[0].ToString());
                //JObject res = JObject.Parse(JArray.Parse(mo["BAUTHOR"].ToString())[0].ToString());
                JArray ma_author = new JArray();
                if (string.IsNullOrEmpty(m)) m = "1";
                if (m == "2")
                {
                    ma_author = JArray.Parse(mo["BAUTHOR"].ToString());
                }
                else
                {
                    ma_author = JArray.Parse(mo["BCATEGORY"].ToString());
                }
                JObject mo_comment = MyControllers.GetListComment(id);
                mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());

                ViewBag.info = info;
                ViewBag.ma_author = ma_author;
                ViewBag.mtab = m;
                ViewBag.ma_comment = ma_comment;
                ViewBag.bookId = id;
            }
            catch (Exception)
            {
            }
            return View();
        }
        public ActionResult Comment()
        {
            if (Session["LoginName"] == null)
            {
                Response.Redirect("/dang-nhap.html");
                return null;
            }
            else
            {
                string LoginName = Session["LoginName"].ToString();
                string urlReffrence = Request.UrlReferrer.ToString();
                string commentTitle = Request.Params["txtTitle"];
                string commentText = Request.Params["txtComment"];
                string bookId = Request.Params["bookId"].ToString();

                JObject mo = MyControllers.AddComment(bookId, "mBook", HttpUtility.HtmlEncode(commentText), LoginName);
                Response.Redirect(urlReffrence);
                return null;
            }
        }

        public ActionResult Read(string id)
        {
            try
            {
                //LoadDataHomeQuick();
                RequestParam();
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string mmmdk = mo.ToString();
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                JObject blink = JObject.Parse(JArray.Parse(mo["LINK"].ToString())[0].ToString());
                //check trang thai cho doc
                JObject mulog = new JObject();
                JObject packge_book = new JObject();
                JObject packge_acc = new JObject();
                JArray ma = new JArray();
                int PLUS = -1;
                string PRICE = "0";
                string ISHOT = "";
                string CART_CODE, SERVICE_CODE, REMAIN;
                CART_CODE = SERVICE_CODE = REMAIN = "";
                if (info != null)
                {
                    PRICE = info["PRICE"].ToString();
                    ISHOT = info["ISHOT"].ToString();
                }
               
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    ma = JArray.Parse(res["RPATH"].ToString());
                }
                if (!string.IsNullOrEmpty(mo["CART"].ToString()) && mo["CART"].ToString() != "[]")
                {
                    packge_book = JObject.Parse(JArray.Parse(mo["BCART"].ToString())[0].ToString());
                    CART_CODE = packge_book["CART_CODE"] == null ? "" : packge_book["CART_CODE"].ToString();
                    REMAIN = packge_book["REMAIN"] == null ? "" : packge_book["REMAIN"].ToString();
                }

                if (!string.IsNullOrEmpty(mo["SERVICE"].ToString()) && mo["SERVICE"].ToString() != "[]")
                {
                    packge_acc = JObject.Parse(JArray.Parse(mo["SERVICE"].ToString())[0].ToString());
                    SERVICE_CODE = packge_acc["SERVICE_CODE"] == null ? "" : packge_acc["SERVICE_CODE"].ToString();
                    PLUS = packge_acc["PLUS"] == null ? -1 : Convert.ToInt32(packge_acc["PLUS"].ToString());
                }
                //check read and read demo
                //if(book= mua + tang and) else {doc demo}
                if (!string.IsNullOrEmpty(packge_book.ToString()) && (CART_CODE == "OWNER" || CART_CODE == "READING"))
                {
                    ViewBag.totalPage = ma.Count;
                    ViewBag.i_pageIndex = i_pageIndex + 1;
                }
                else if (CART_CODE != "OWNER" && CART_CODE != "READING" && !string.IsNullOrEmpty(packge_acc.ToString()) &&  SERVICE_CODE == "EB" && PLUS >= 0 && ISHOT!="2")
                {
                    //đánh dấu trang
                    int createStatus = -1;
                    string remain = "";
                    remain = "0#3";
                    JObject mu = null;//MyControllers.Markbook(id, LoginName, remain, "0", "0");
                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());

                    ViewBag.totalPage = ma.Count;
                    ViewBag.i_pageIndex = i_pageIndex + 1;
                }
                else
                {
                    ViewBag.totalPage = 1;
                    ViewBag.i_pageIndex = 1;
                    ViewBag.Demo = "1";
                    if (!string.IsNullOrEmpty(packge_acc.ToString()) && (SERVICE_CODE == "EB") && PLUS >= 0 && ISHOT != "2")
                    {
                        ViewBag.Packge = SERVICE_CODE;
                    }
                }

                if (ViewBag.Demo == "1")
                {
                    mulog = MyControllers.Log_ReadDemo(id, LoginName);
                }
                else
                {
                    mulog = MyControllers.Log_ReadFull(id, LoginName);
                    
                }
                if ((PRICE == "0"||PRICE == "") && Session["LoginName"] != null)
                {
                    ViewBag.Demo = "1";
                }
                ViewBag.Linkres = (blink["ENCYP_PATH"].ToString()).Replace("ENCRYP/", "").Trim();
                ViewBag.info = info;
                ViewBag.ma = ma;
            }

            catch (Exception)
            {
            }
            return View();
        }
        private void RequestParam()
        {
            if (Request.QueryString["t"] != null)
            {
                //int.TryParse(Request.QueryString["t"], out i_tab);
                //string.TryParse(Request.QueryString["t"], out i_tab);
                i_tab = Request.QueryString["t"].ToString();
            }
            if (Request.QueryString["c"] != null)
            {
                //int.TryParse(Request.QueryString["c"], out i_category);
                i_category = Request.QueryString["c"].ToString();
            }
            if (Request.QueryString["p"] != null)
            {
                int.TryParse(Request.QueryString["p"], out i_pageIndex);
                i_pageIndex = i_pageIndex - 1;
            }

        }
    }
}
