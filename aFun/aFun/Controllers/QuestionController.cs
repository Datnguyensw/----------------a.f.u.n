using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aFun.Models;
using aFun.Controllers;

namespace WapaFun.Controllers
{
    public class QuestionController : Controller
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

            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo_all = MyControllers.GetQA("ALL", 0, 5);
                mo_all = JObject.Parse(mo_all["Body"]["Data"].ToString());
                JArray ma_all = JArray.Parse(mo_all["LAWQA"].ToString());
                ViewBag.ma_all = ma_all;

                if (Session["LoginName"] != null)
                {
                    JObject mo_my = MyControllers.GetQA(Session["LoginName"].ToString(), 0, 5);
                    mo_my = JObject.Parse(mo_my["Body"]["Data"].ToString());
                    JArray ma_my = JArray.Parse(mo_my["LAWQA"].ToString());
                    ViewBag.ma_my = ma_my;
                }

            }
            catch (Exception) { }
            return View();

        }
        public ActionResult Cate()
        {
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo = MyControllers.GetQA("ALL", i_pageIndex, i_pageSize);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["LAWQA"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());

            }
            catch (Exception) { }
            return View();
        }
        public ActionResult CateMy()
        {
            try
            {
                if (Session["LoginName"] == null)
                {
                    return Redirect("/dang-nhap.html");
                }
                else
                {
                    LoadDataHomeQuick();
                    RequestParam();
                    JObject mo = MyControllers.GetQA(Session["LoginName"].ToString(), i_pageIndex, i_pageSize);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JArray ma = JArray.Parse(mo["LAWQA"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                }

            }
            catch (Exception) { }
            return View();
        }
        public ActionResult Form()
        {

            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                return View();
               
            }
        }
        [HttpPost]
        public ActionResult Form(LAW_QAModel model)
        {

            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.CaptchaValue))
                    {
                        ModelState.AddModelError("", ErrorCodeToString(0));
                    }
                    else if (CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                    {
                        int createStatus = -1;
                        JObject mu = MyControllers.AddQA(model.TITLE, model.TXDESC, Session["LoginName"].ToString());
                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            ViewBag.Status = @"<div class=""mess_sucess"">" + ErrorCodeToString(1) + "</div>";
                        }
                        else
                        {
                            ViewBag.Status = @"<div class=""mess_error"">" + ErrorCodeToString(-1) + "</div>";
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(2));
                    }

                }

            }
            return View(model);
        }
        public ActionResult Details(string id)
        {
            try
            {
                    LoadDataHomeQuick();
                    string LoginName = "ALL";
                    JObject mo = MyControllers.getQAById(id);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["LAWQA"].ToString())[0].ToString());
                    ViewBag.mo = mo;
                    if (Session["LoginName"] != null)
                    {
                        LoginName = Session["LoginName"].ToString();
                    }
                    JObject mo_other = MyControllers.GetQA(LoginName, i_pageIndex, i_pageSize);
                    mo_other = JObject.Parse(mo_other["Body"]["Data"].ToString());
                    JArray ma_other = JArray.Parse(mo_other["LAWQA"].ToString());
                    ViewBag.ma_other = ma_other;
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
        private string ErrorCodeToString(int status)
        {
            //Code: 1.Ok, 0.Da ton tai, -1.Erorr, 2. Da tao nhung chua Active Code
            switch (status)
            {
                case 0:
                    return "Mã chống spam chưa nhập.";
                case 1:
                    return "Gửi câu hỏi thành công.";
                case 2:
                    return "Mã chống spam không chính xác.";
                default:
                    return "Lỗi trong quá trình thực thi.";
            }
        }
    }
}
