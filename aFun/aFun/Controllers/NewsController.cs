using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aFun.Models;
using HtmlAgilityPack;

namespace WapaFun.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /Book/
        string i_tab = Common.booknume[0];
        int i_pageIndex = 0;
        string i_category = "ALL";
        int i_pageSize = 10;
        public void LoadDataHomeQuick()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
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

        public ActionResult Cate()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo_cate = MyControllers.GetListChildCate(i_category);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                JArray ma_cate = new JArray();
                try
                {
                    ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                }
                catch (Exception)
                {
                    ma_cate = null;
                }

                if (ma_cate == null || ma_cate.Count <= 1)
                {
                    JObject mo = MyControllers.GetListNews(i_category, i_pageIndex, i_pageSize);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JArray ma = JArray.Parse(mo["NEW"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                    ViewBag.i_category = i_category;
                    ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                }
                else
                {
                    mo_cate = JObject.Parse(ma_cate[0].ToString());
                    ViewBag.CATENAME = mo_cate["TXNAME"] != null ? mo_cate["TXNAME"].ToString() : "";
                    ViewBag.ma_cate = ma_cate;
                }
            }
            catch (Exception)
            {
                ViewBag.CATENAME = "Tin tức";
            }
            return View();
        }
        public ActionResult Details(string id)
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                LoadDataHomeQuick();
                JObject mo = MyControllers.GetNewById(id);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["NEW"].ToString())[0].ToString());
                ViewBag.mo = mo;
                i_category = mo["CATE_CODE"].ToString();
                JObject mo_other = MyControllers.GetListNews(i_category, i_pageIndex, i_pageSize);
                mo_other = JObject.Parse(mo_other["Body"]["Data"].ToString());
                JArray ma_other = JArray.Parse(mo_other["NEW"].ToString());

                JObject mo_comment_= MyControllers.GetListComment(id, "3");
               mo_comment_ = JObject.Parse(mo_comment_["Body"]["Data"].ToString());
               JArray ma_comment = JArray.Parse(mo_comment_["COMMENT"].ToString()); 
                 

                    
                ViewBag.ma_other = ma_other;
                ViewBag.AUTO_ID = id;
                 ViewBag.ma_comment = ma_comment;
            }
            catch (Exception)
            {
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment()
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                string LoginName = Session["LoginName"].ToString();
                string urlReffrence = Request.UrlReferrer.ToString();
                //string commentTitle = Request.Params["txtTitle"];
                string commentText = Request.Params["txtComment"];
                string AUTO_ID = Request.Params["AUTO_ID"].ToString();
                int result = -1;
                //0: video, audio,law - 1: tin tuc
                JObject mo = MyControllers.AddComment(AUTO_ID, "3", "aFun", HttpUtility.HtmlEncode(commentText), LoginName);
                result = Convert.ToInt32(mo["Header"]["Code"].ToString());
                if (result == 0)
                {
                    Session["re_url"] = urlReffrence;
                    Session["mess"] = @"<div class=""mess_sucess"">Gửi bình luận thành công.</div>";
                    return Redirect("/thong-bao.html");
                }
                else
                {
                    Session["re_url"] = urlReffrence;
                    Session["mess"] = @"<div class=""mess_error"">Gửi bình luận thất bại.</div>";
                    return Redirect("/thong-bao.html");
                }
            }
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
