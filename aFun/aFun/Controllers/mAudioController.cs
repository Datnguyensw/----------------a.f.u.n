using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aFun.Models;

namespace WapaFun.Controllers
{
    public class mAudioController : Controller
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
        public ActionResult Index()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo_cate = MyControllers.GetListChildCate(Core.C_AUDIO);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                ViewBag.ma_cate = ma_cate;
            }
            catch (Exception)
            {
            }
            return View();

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
                JObject mo = MyControllers.GetLawByCate(Core.TXAUDIO,i_category, i_pageIndex, i_pageSize);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["LAW"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                ViewBag.i_category = i_category;
                ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
            }
            catch (Exception)
            {
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
                string LoginName = "UNKNOW";
                string isservice = "";
                string SERVICE_CODE, EXPIRE_DATE;
                int PLUS = -1;
                SERVICE_CODE = EXPIRE_DATE = "";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetLawById(Core.TXAUDIO, id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = new JObject();
                JObject res = new JObject();
                JArray ma_other = new JArray();
                info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                ma_other = JArray.Parse(mo["CATEGORY"].ToString());
                res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                isservice = info["ISSERVICE"].ToString();
                if (isservice == "1")
                {
                    JArray ser = JArray.Parse(mo["SERVICE"].ToString());
                    if (ser != null)
                    {
                        foreach (var item in ser)
                        {
                            if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "EB")
                            {
                                SERVICE_CODE = item["SERVICE_CODE"] != null ? item["SERVICE_CODE"].ToString() : "";
                                EXPIRE_DATE = item["EXPIRE_DATE"].ToString();
                                string m = Convert.ToDateTime(EXPIRE_DATE).ToShortDateString().CompareTo(DateTime.Now.ToShortDateString()).ToString();
                                PLUS = Convert.ToInt32(m);
                            }
                        }
                    }
                    if (SERVICE_CODE == "LTH" && PLUS >= 0)
                    {
                        ViewBag.info = info;
                        ViewBag.res = res;
                        ViewBag.ma_other = ma_other;
                        i_category = info["CATE_CODE"].ToString();

                        JObject mo_comment = MyControllers.GetListComment(id);
                        mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                        JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                        ViewBag.ma_comment = ma_comment;

                        return View();
                    }
                    else if (SERVICE_CODE == "EB" && PLUS < 0)
                    {
                        Session["mess"] = @"<div class=""mess_error"">Gói sách điện tử hết hạn. Vui lòng hủy gói sách điện tử và đăng ký lại để được sử dụng dịch vụ.</div>";
                        Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                        return Redirect("/thong-bao.html");
                    }
                    else
                    {
                        Session["mess"] = @"<div class=""mess_error"">Quý khách chưa đăng ký gói sách điện tử. Vui lòng đăng ký gói để được sử dụng dịch vụ.</div>";
                        Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                        return Redirect("/thong-bao.html");
                    }//end ser
                }
                else
                {
                    ViewBag.info = info;
                    ViewBag.res = res;
                    ViewBag.ma_other = ma_other;
                    i_category = info["CATE_CODE"].ToString();

                    JObject mo_comment = MyControllers.GetListComment(id);
                    mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                    JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                    ViewBag.ma_comment = ma_comment;
                    return View();
                }
            }
            catch (Exception)
            {
                Session["mess"] = @"<div class=""mess_error"">Hệ thống đang quá tải. Quý khách vui lòng quay lại sau.</div>";
                Session["re_url"] = "/";
                return Redirect("/thong-bao.html");
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
