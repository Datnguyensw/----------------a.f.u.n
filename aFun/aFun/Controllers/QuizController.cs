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
    public class QuizController : Controller
    {
        //
        // GET: /Quiz/
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
        public ActionResult Info()
        {
            //if (Session["LoginName"] == null && Session["msisdn"] == null)
            //{
            //    Response.Redirect(MakeLink.URLLogin());
            //}
            //if (Session["LoginName"] == null && Session["msisdn"] != null)
            //{
            //    Response.Redirect(MakeLink.URLRegister());
            //}
            InfoAccModel model = new InfoAccModel();
            try
            {
                LoadDataHomeQuick();
                List<SelectListItem> lstSex = new List<SelectListItem>();
                lstSex.Add(new SelectListItem() { Text = "Nam", Value = "1" });
                lstSex.Add(new SelectListItem() { Text = "Nữ", Value = "2" });
                lstSex.Add(new SelectListItem() { Text = "Khác", Value = "3" });

                int createStatus = -1;
                JObject mo = MyControllers.InfoAcc(Session["LoginName"].ToString(), Session["LoginCode"].ToString());
                createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                if (createStatus == 0)
                {
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["USER"].ToString())[0].ToString());
                    model.Fullname = mo["FULL_NAME"].ToString();
                    model.Birthday = mo["BIRTHDAY"].ToString();
                    model.SelectedSex = Convert.ToInt32(mo["SEX"].ToString());
                    model.ListSex = lstSex;
                    model.Point = mo["CURRENT_POINT"].ToString();
                    //model.TopLevel = "2";
                }
            }
            catch (Exception)
            {
                model = null;
            }
            return View(model);
        }
        public ActionResult Help()
        {
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
        public ActionResult ListWin()
        {
            if (Session["LoginName"] == null && Session["msisdn"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                Response.Redirect(MakeLink.URLLogin());
            }
            ////if (Session["LoginName"] == null && Session["msisdn"] != null)
            ////{
            ////    Response.Redirect(MakeLink.URLRegister());
            ////}
            try
            {
                LoadDataHomeQuick();
                JObject mo = MyControllers.GetBonus();
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["BONUS"].ToString());
                ViewBag.ma = ma;
            }
            catch (Exception)
            {
            }

            return View();
        }
        public ActionResult ListTop()
        {
           
            //if (Session["LoginName"] == null && Session["msisdn"] != null)
            //{
            //    Response.Redirect(MakeLink.URLRegister());
            //}
            try
            {
                LoadDataHomeQuick();
                JObject mo = MyControllers.GetCharts(Session["LoginName"].ToString());
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["CHART"].ToString());
                ViewBag.ma = ma;
            }
            catch (Exception)
            {
            }

            return View();
        }
        public ActionResult Play()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                JArray ma_ser = new JArray();
                JObject mo = MyControllers.InfoAcc(Session["LoginName"].ToString(), Session["LoginCode"].ToString());
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string SERVICE_CODE, EXPIRE_DATE;
                int PLUS = -1;
                SERVICE_CODE= EXPIRE_DATE="";
                if (mo["SERVICE"] != null && mo["SERVICE"].ToString() != "[]")
                {
                    ma_ser=JArray.Parse(mo["SERVICE"].ToString());
                    foreach(var item in ma_ser)
                    {
                        if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "TP")
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
                                 m = DateTime.ParseExact(EXPIRE_DATE, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat).CompareTo(DateTime.Now).ToString();
                            }
                            PLUS = Convert.ToInt32(m);
                        }
                    }
                   
                }
                mo = JObject.Parse(JArray.Parse(mo["USER"].ToString())[0].ToString());
                ViewBag.mo = mo;
                ViewBag.INDEX_TOTAL =mo["INDEX_TOTAL"]==null? "" : mo["INDEX_TOTAL"].ToString();

                if (SERVICE_CODE == "TP" && PLUS >= 0)
                {
                    return View();
                }
                else if (SERVICE_CODE == "TP" && PLUS < 0)
                {
                    Session["mess"] = @"<div class=""mess_error"">Gói trắc nghiệm pháp luật hết hạn. Vui lòng hủy gói trắc nghiệm và đăng ký lại để được sử dụng dịch vụ.</div>";
                    Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                    return Redirect("/thong-bao.html");
                }
                else
                {
                    Session["mess"] = @"<div class=""mess_error"">Quý khách chưa đăng ký gói trắc nghiệm pháp luật. Vui lòng đăng ký gói để được sử dụng dịch vụ.</div>";
                    Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                    return Redirect("/thong-bao.html");
                } 
            }

        }
        public ActionResult LoadQuestion()
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect(MakeLink.URLLogin());
            }
            else
            {
                string createStatus = "-1";
                JObject mo = MyControllers.getQuestion(Session["LoginName"].ToString());
                createStatus = mo["Header"]["Code"].ToString();
                if (createStatus == "0")
                {
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    //string value = JsonConvert.SerializeObject(mo);
                    Response.Write(mo);
                }
                else
                {
                    Response.Write(createStatus);
                }
                return null;
            }
        }
        public ActionResult answerQuestion( string AnswerID)
        {
            string createStatus = "-1";
            JObject mo = MyControllers.answerQuestion(Session["LoginName"].ToString(), AnswerID);
            createStatus = mo["Header"]["Code"].ToString();
            if (createStatus=="0")
            {
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                Response.Write(mo);
            }
            else
            {
                Response.Write(createStatus);
            }
            return null;
        }
    }
}
