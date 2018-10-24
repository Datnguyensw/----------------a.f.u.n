using lemon.wapgw.cryptengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aFun.Models;
using System.Text;
using log4net;

namespace aFun.Controllers
{

    public class AccController : Controller
    {
        private static ILog logger = log4net.LogManager.GetLogger(typeof(AccController));
        //
        // GET: /Acc/
        int i_pageIndex = 0;
        int i_pagesize = 10;
        string url = "/";
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

            if (Session["LoginName"] == null)
            {
                Response.Redirect("/dang-nhap.html");
                return null;
            }
            else
            {
                LoadDataHomeQuick();
                return View();
            }
        }
        public ActionResult Info()
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                InfoAccModel model = new InfoAccModel();
                try
                {
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
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "accinfo"));
                    }
                }
                catch (Exception)
                {
                    model = null;
                }
                return View(model);
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Info(InfoAccModel model)
        {
            LoadDataHomeQuick();
            if (ModelState.IsValid)
            {
                try
                {
                    int createStatus = -1;
                    if (!Common.CheckDate(model.Birthday))
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidBirthday);
                    }
                    else
                    {
                        model.Birthday = model.Birthday;
                        JObject mo = MyControllers.UpdateAcc(Session["LoginName"].ToString(), Session["LoginCode"].ToString(), model.Fullname, model.SelectedSex.ToString(), model.Birthday);
                        createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            ViewBag.Message = @"<div class=""mess_sucess"">" + InfoAccMessage.Success + "</div>";
                            Session["LoginDisplay"] = model.Fullname;
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "accinfo"));
                        }
                    }
                }
                catch (Exception)
                {
                }

            }

            List<SelectListItem> lstSex = new List<SelectListItem>();
            lstSex.Add(new SelectListItem() { Text = "Nam", Value = "1" });
            lstSex.Add(new SelectListItem() { Text = "Nữ", Value = "2" });
            lstSex.Add(new SelectListItem() { Text = "Khác", Value = "3" });

            model.Fullname = model.Fullname;
            model.Birthday = model.Birthday;
            model.SelectedSex = model.SelectedSex;
            model.ListSex = lstSex;
            model.Point = model.Point;
            //model.TopLevel = "2";
            return View(model);
        }

        public ActionResult Login()
        {
            RequestParam();
            if (Session["LoginName"] != null)
            {
                return Redirect(url);
            }
            else
            {
                LoadDataHomeQuick();
                Session["showboxlogin"] = "1";
                return View();
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginAccModel model)
        {
            LoadDataHomeQuick();
            Session["showboxlogin"] = "1";
            RequestParam();
            if (ModelState.IsValid)
            {
                if (Common.ValidateViettel(model.MobileNumber))
                {
                    int createStatus = -1;
                    int num_pack = 0;
                    string SERVICE_CODE, EXPIRE_DATE;
                    int PLUS = -1;
                    SERVICE_CODE = EXPIRE_DATE = "";
                    if (Session["msisdn"] != null)
                    {
                        Core.is3g = 1;
                    }
                    else
                    {
                        Core.is3g = 0;
                    }
                    JObject mo = MyControllers.Login(model.MobileNumber, model.Password, Core.is3g);
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
                                        m = DateTime.ParseExact(EXPIRE_DATE, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat).CompareTo(DateTime.Now).ToString();
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
                        Session["mess"] = @"<div class=""mess_sucess"">" + LoginMessage.Success + "</div>";
                        if(Session["re_url"] ==null){
                        Session["re_url"] ="/";
                        }
                        Response.Redirect("/thong-bao.html");
                    }
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "login"));
                    }
                }
                else
                {
                    ModelState.AddModelError("", ValidateClient.InvalidMobile);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect("/");
        }
        
        public ActionResult Register()
        {
            if (Session["msisdn"] != null && Session["LoginName"] == null)
            {
                CheckDevice.register3g(Session["msisdn"].ToString());
                return Redirect("/");
            }
            else if (Session["msisdn"] != null && Session["LoginName"] != null)
            {
                return Redirect("/");
            }
            else
            {
                LoadDataHomeQuick();
                return View();
            }

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterAccModel model)
        {
            LoadDataHomeQuick();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.CaptchaValue))
                {
                    ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                }
                else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                {
                    ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                }
                else
                {
                    model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                    if (Common.ValidateViettel(model.MobileNumber))
                    {
                        int createStatus = -1;
                        if (Session["msisdn"] != null)
                        {
                            Core.is3g = 1;
                        }
                        else
                        {
                            Core.is3g = 0;
                        }

                        JObject mu = MyControllers.Register(model.MobileNumber, model.Password, Core.is3g);
                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            Session["MobileNumber"] = model.MobileNumber;
                            Session["mess"] = @"<div class=""mess_sucess"">" + RegisterMessage.Success + "</div>";
                            Session["re_url"] = "/kich-hoat-tai-khoan.html";
                            Response.Redirect("/thong-bao.html");
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "register"));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidMobile);
                    }
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Register3g()
        {
            return View();
        }

        public ActionResult ActiveReg()
        {
            if (Session["MobileNumber"] == null)
            {
                return Redirect("/dang-ky.html");
            }
            else
            {
                LoadDataHomeQuick();
                ActiveRegAccModel model = new ActiveRegAccModel();
                model.MobileNumber = Session["MobileNumber"].ToString();
                return View(model);
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActiveReg(ActiveRegAccModel model)
        {
            LoadDataHomeQuick();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.CaptchaValue))
                {
                    ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                }
                else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                {
                    ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                }
                else
                {
                    model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                    if (Common.ValidateViettel(model.MobileNumber))
                    {
                        int createStatus = -1;
                        if (Session["msisdn"] != null)
                        {
                            Core.is3g = 1;
                        }
                        else
                        {
                            Core.is3g = 0;
                        }
                        JObject mu = MyControllers.ActiveReg(model.MobileNumber, model.Otp);
                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            mu = JObject.Parse(mu["Body"]["Data"].ToString());
                            mu = JObject.Parse(JArray.Parse(mu["USER"].ToString())[0].ToString());
                            Session["LoginCode"] = mu["USER_CODE"].ToString();
                            Session["LoginName"] = mu["USER_NAME"].ToString();
                            Session["LoginDisplay"] = mu["FULL_NAME"].ToString();
                            Session["mess"] = @"<div class=""mess_sucess"">" + RegisterMessage.SuccessActive + "</div>";
                            Session["re_url"] = "/";
                            Response.Redirect("/thong-bao.html");
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "activereg"));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidMobile);
                    }
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Forget()
        {
            LoadDataHomeQuick();
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Forget(ForgetAccModel model)
        {
            LoadDataHomeQuick();
            if (ModelState.IsValid)
            {
                if (Request.Params["btnOTP"] != null)
                {
                    if (string.IsNullOrEmpty(model.CaptchaValue))
                    {
                        ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                    }
                    else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                    }
                    else
                    {
                        model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                        if (Common.ValidateViettel(model.MobileNumber))
                        {
                            int createStatus = -1;
                            JObject mu = MyControllers.GenOTP(model.MobileNumber, "1");
                            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                            if (createStatus == 0)
                            {
                                mu = JObject.Parse(mu["Body"]["Data"].ToString());
                                mu = JObject.Parse(JArray.Parse(mu["USER"].ToString())[0].ToString());
                                ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận: " + mu["CODE"].ToString() + "</div>";
                               // ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận đã được gửi tới số điện thoại của quý khách.</div>";

                            }
                            else
                            {
                                ModelState.AddModelError("", ACC_MESSAGE(createStatus, "otp"));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", ValidateClient.InvalidMobile);
                        }
                    }
                }
                if (Request.Params["btnUpdate"] != null)
                {
                    model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                    if (Common.ValidateViettel(model.MobileNumber))
                    {
                        int createStatus = -1;
                        JObject mu = MyControllers.Forgets(model.MobileNumber, model.CodeOTP);
                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            //mu = JObject.Parse(mu["Body"]["Data"].ToString());
                            //mu = JObject.Parse(JArray.Parse(mu["USER"].ToString())[0].ToString());
                            Session["mess"] = @"<div class=""mess_sucess"">" + ForgetMessage.Success + "</div>";
                            Session["re_url"] = "/dang-nhap.html";
                            Response.Redirect("/thong-bao.html");
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "forget"));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidMobile);
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePass()
        {
            if (Session["LoginName"] == null)
            {
                Response.Redirect("/dang-nhap.html");
                return null;
            }
            else
            {
                LoadDataHomeQuick();
                return View();
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePass(ChangePassAccModel model)
        {
            if (Session["LoginName"] == null)
            {
                Response.Redirect("/dang-nhap.html");
                return null;
            }
            else
            {
                LoadDataHomeQuick();
                if (ModelState.IsValid)
                {
                    try
                    {
                        int createStatus = -1;
                        JObject mo = MyControllers.ChangPass(Session["LoginName"].ToString(), model.Passwordnew, model.Password);
                        createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            ViewBag.Message = @"<div class=""mess_sucess"">" + ChangPassMessage.Success + "</div>";
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "changepass"));
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return View(model);
            }
        }
        public ActionResult Packge()
        {

            string reUrl_LTH = ""; string reUrlCan_LTH = ""; string reUrl_TP = ""; string reUrlCan_TP = ""; string reUrl_EB = "";
            string reUrlCan_EB = "";
            string reUrl_VD = "";
            string reUrlCan_VD = "";
            string resLTP = "";
            string cmdLTP = "";
            string pcode = "";
            int createStatus = -1;
              string uname = "";
                 
            if (Request.QueryString["res"] != null && Request.QueryString["cmd"] != null)
            {
                if (Request.QueryString["res"] != null)
                {
                    resLTP = Request.QueryString["res"].ToString();
                }
                if (Request.QueryString["cmd"] != null)
                {
                    cmdLTP = Request.QueryString["cmd"].ToString();
                }
                if (Session["PCode_id"] != null)
                {
                    pcode = Session["PCode_id"].ToString();
                }

            }

            //Session["msisdn"] = "84989260781";
///Session["PCode_id"]=pcode = "DKLTH";
           // resLTP = "0";
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                if (Session["msisdn"] != null)
                {
                    uname = Session["msisdn"].ToString();
                    uname = Common.formatPhone(uname, 0);
                }
                reUrl_LTH = aFun.Models.ResponeUrlVTL.getUrlChargeSub("REGISTER", Core.URL_LTH, "LEMONMEDIA", "", "", "1000", "Dang_Ky_Goi_" + Core.URL_LTH, uname, "WAP");
                reUrlCan_LTH = aFun.Models.ResponeUrlVTL.getUrlChargeSub("CANCEL", Core.URL_LTH, "LEMONMEDIA", "", "", "0", "Huy_Goi_" + Core.URL_LTH, uname, "WAP");
                //trieu phu
                reUrl_TP = aFun.Models.ResponeUrlVTL.getUrlChargeSub("REGISTER", Core.URL_TP, "LEMONMEDIA", "", "", "1000", "Dang_Ky_Goi_" + Core.URL_TP, uname, "WAP");
                reUrlCan_TP = aFun.Models.ResponeUrlVTL.getUrlChargeSub("CANCEL", Core.URL_TP, "LEMONMEDIA", "", "", "0", "Huy_Goi_" + Core.URL_TP, uname, "WAP");

                //BOOK
                reUrl_EB = aFun.Models.ResponeUrlVTL.getUrlChargeSub("REGISTER", Core.URL_EB, "LEMONMEDIA", "", "", "1000", "Dang_Ky_Goi_" + Core.URL_EB, uname, "WAP");
                reUrlCan_EB = aFun.Models.ResponeUrlVTL.getUrlChargeSub("CANCEL", Core.URL_EB, "LEMONMEDIA", "", "", "0", "Huy_Goi_" + Core.URL_EB, uname, "WAP");

                //
                //video
                reUrl_VD = aFun.Models.ResponeUrlVTL.getUrlChargeSub("REGISTER", Core.URL_VD, "LEMONMEDIA", "", "", "1000", "Dang_Ky_Goi" + Core.URL_VD, uname, "WAP");
                reUrlCan_VD = aFun.Models.ResponeUrlVTL.getUrlChargeSub("CANCEL", Core.URL_VD, "LEMONMEDIA", "", "", "0", "Huy_Goi_" + Core.URL_VD, uname, "WAP");

                try
                {
                    JObject mo = MyControllers.GetSysvarByCode("ISCOMFIRM_MOBI");
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    string mmmmd = mo.ToString();

                    JArray ma = JArray.Parse(mo["GUIDE"].ToString());
                    mo = JObject.Parse(ma[0].ToString());
                    ViewBag.ISCOMFIRM_MOBI = mo["TXVALUE"] != null ? mo["TXVALUE"].ToString() : "0";

                }
                catch (Exception ex)
                {
                    ViewBag.ISCOMFIRM_MOBI = "0";
                }
               
                //Session["msisdn"] = "0934413268";
                //Session["LoginName"] = "0934413268";
                // int createStatus = -1;
                string urlcomfirm = "";
                string trans_id, msisdn;
                trans_id = msisdn  = "0";
                if  (Session["PCode_id"] != null && ViewBag.ISCOMFIRM_MOBI == "1")
                {
                    string cp = "";
                    string id = "";
                    string SelectedPackge = "";

                  
                    if (!string.IsNullOrEmpty(pcode))
                    {
                        logger.Info("Thành công resLTP=" + resLTP + "; cmdLTP=" + cmdLTP);
                        //string cp = "";
                        //  string SelectedPackge = "";
                        if (!string.IsNullOrEmpty(pcode))
                        {
                            cp = pcode.Substring(0, 2);
                            SelectedPackge = pcode.Substring(2, pcode.Length - 2);
                        }
                        else
                        {

                        }
                        if (resLTP == "0" || resLTP == "414")
                        {

                            JObject mu = new JObject();
                            createStatus = -1;
                            if (cp == "DK")
                            {
                                // cac goi de dk dung chung 1 cm 2030 vva 2031 nen khng muon doi teh AddPackgeLTH thanh AddPackge
                                if (SelectedPackge == Core.LTH)
                                {
                                    mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else if (SelectedPackge == Core.TN)
                                {
                                    mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else if (SelectedPackge == Core.EB)
                                {
                                    mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else if (SelectedPackge == Core.VD)
                                {
                                    mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else
                                {

                                }
                                if (mu != null)
                                {
                                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                                }
                                if (createStatus == 0)
                                {
                                    ViewBag.Status = @"<span class=""blue"">Đăng ký gói thành công. Để nhận bản tin SMS vui lòng đăng ký nhận SMS <a href='/Acc/Regsms' title='Đăng ký nhận SMS'>tại đây</a></span>";
                                }
                                else if (createStatus == 401)
                                {
                                    ViewBag.Status = @"<span class=""red"">Tài khoản không đủ tiền.</span>";
                                }
                                else if (createStatus == 401)
                                {
                                    ViewBag.Status = @"<span class=""red"">Quý khách đã đăng ký gói rồi.Vui lòng sử dụng dịch vụ.</span>";
                                }
                                else
                                {
                                    ViewBag.Status = @"<span class=""red"">Đăng ký gói thất bại. " + ErrorCodeCharge(Convert.ToInt32(createStatus)) + "</span>";
                                }
                                logger.Info("Phone: " + Session["LoginName"].ToString() + "Code charge resLTP: " + resLTP + "; Error Code= " + createStatus + "; Message DK= " + ViewBag.Status);

                            }
                            else
                            {
                                if (SelectedPackge == Core.LTH)
                                {
                                    mu = MyControllers.CancelPackgeLTH(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else if (SelectedPackge == Core.TN)
                                {
                                    mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else if (SelectedPackge == Core.EB)
                                {
                                    mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else if (SelectedPackge == Core.VD)
                                {
                                    mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                                }
                                else
                                {

                                }
                                if (mu != null)
                                {
                                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                                }
                                if (createStatus == 0)
                                {
                                    ViewBag.Status = @"<span class=""blue"">Hủy gói thành công.";
                                }
                                else
                                {
                                    ViewBag.Status = @"<span class=""red"">Hủy gói thất bại. " + ErrorCodeCharge(Convert.ToInt32(createStatus)) + "</span>";
                                }
                                logger.Info("Phone: " + Session["LoginName"].ToString() + "Code charge resLTP: " + resLTP + "; Error Code= " + createStatus + "; Message HUY= " + ViewBag.Status);


                            }

                        }
                        else if (resLTP == "417")
                        {
                            ViewBag.Status = @"<span class=""red""Giao dịch không thành công. Khách hàng không đồng ý hoặc hết trả lời thanh toán.";
                            logger.Info("Phone: " + Session["LoginName"].ToString() + "Code charge resLTP: " + resLTP + "; Error Code= (-1) ; Message DK= " + ViewBag.Status);

                        }
                        else
                        {
                            if (cp == "DK")
                            {
                                ViewBag.Status = @"<span class=""red"">Đăng ký gói thất bại. " + ErrorCodeCharge(Convert.ToInt32(createStatus)) + "</span>";
                            }
                            else
                            {
                                ViewBag.Status = @"<span class=""red"">Hủy gói thất bại. " + ErrorCodeCharge(Convert.ToInt32(createStatus)) + "</span>";
                            }
                            logger.Info("resLTP=" + resLTP + "; cmdLTP=" + cmdLTP);
                        }
                    }
                }
                    //giai phong ma goi
                    Session["PCode_id"] = null;
                    //Load list package
                    LoadDataHomeQuick();
                    Session["showboxlogin"] = "1";
                    PackgeAccModel model = new PackgeAccModel();
                    try
                    {
                        createStatus = -1;
                        int REMAIN = -1;
                        string curpack = "";
                        JObject mo = MyControllers.ListPackge(Session["LoginName"].ToString(), "1");

                        createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            mo = JObject.Parse(mo["Body"]["Data"].ToString());
                            string mmmos = mo.ToString();
                            JArray ma = new JArray();

                            if (mo["SERVICE"] != null && !mo["SERVICE"].ToString().StartsWith("[]"))
                            {
                                ma = JArray.Parse(mo["SERVICE"].ToString());
                            }
                            //JArray ma = JArray.Parse(mo["SERVICE"].ToString());
                            JArray ma_se = new JArray();
                            bool select = false;
                            if (mo["USER"] != null && !mo["USER"].ToString().StartsWith("[]"))
                            {
                                ma_se = JArray.Parse(mo["USER"].ToString());

                            }
                            List<CheckBoxCate> lstPack = new List<CheckBoxCate>();
                            foreach (var item in ma)
                            {
                                CheckBoxCate chkcate = new CheckBoxCate();
                                chkcate.ID = item["SERVICE_CODE"].ToString();
                                chkcate.Text = item["TXNAME"].ToString();
                                if (ma_se != null && ma_se.Count > 0)
                                {
                                    Session["PACKGE"] = ma_se.Count;
                                }
                                else
                                {
                                    Session["PACKGE"] = 0;
                                }
                                foreach (var itemse in ma_se)
                                {
                                    if (itemse["SERVICE_CODE"].ToString() == chkcate.ID)
                                    {
                                        select = true;
                                        REMAIN = Convert.ToInt32(itemse["REDAY"].ToString());
                                    }
                                }

                                if (chkcate.ID == Core.LTH && select == true)
                                {
                                    urlcomfirm = reUrlCan_LTH; //CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.LTH, "1000", "goi_luat_tong_hop");

                                }
                                if (chkcate.ID == Core.LTH && select == false)
                                {
                                    urlcomfirm = reUrl_LTH;// CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.LTH, "1000", "goi_luat_tong_hop");
                                }
                                if (chkcate.ID == Core.TN && select == true)
                                {
                                    urlcomfirm = reUrlCan_TP;// CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.TN, "2000", "goi_trac_nghiem_phap_luat");//
                                }
                                if (chkcate.ID == Core.TN && select == false)
                                {
                                    urlcomfirm = reUrl_TP;//CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.TN, "2000", "goi_trac_nghiem_phap_luat");
                                }
                                //goi ebook
                                if (chkcate.ID == Core.EB && select == true)
                                {
                                    urlcomfirm = reUrlCan_EB;

                                }
                                if (chkcate.ID == Core.EB && select == false)
                                {
                                    urlcomfirm = reUrl_EB;//CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.EB, "1000", "goi_luat_tong_hop");
                                }
                                //goi video
                                if (chkcate.ID == Core.VD && select == true)
                                {
                                    urlcomfirm = reUrlCan_VD;//CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.VD, "1000", "goi_luat_tong_hop");

                                }
                                if (chkcate.ID == Core.VD && select == false)
                                {
                                    urlcomfirm = reUrl_VD;// CoreTelco.getUrlRequesComfirm(Session["LoginName"].ToString(), Core.VD, "1000", "goi_luat_tong_hop");
                                }
                                chkcate.Checked = select;
                                chkcate.REMAIN = REMAIN;
                                chkcate.reUrl = urlcomfirm;
                                lstPack.Add(chkcate);
                                logger.Info("(mps) " + chkcate.ID+ ": urlcomfirm" + urlcomfirm);

                                select = false;
                            }
                            model.SelectedPackge = curpack;
                            model.ListPackge = lstPack;
                            ViewBag.ma = ma;

                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "packge"));
                        }
                    }
                    catch (Exception)
                    {
                        model = null;
                    }
                    return View(model);
                
            }
        }


        public ActionResult PackgeAdd(string id, string CaptchaValue)
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                Session["showboxlogin"] = "1";
                try
                {
                    if (string.IsNullOrEmpty(CaptchaValue))
                    {
                        Response.Write("-2");
                    }
                    //else if (!CaptchaController.IsValidCaptchaValue(CaptchaValue.ToUpper()))
                    //{
                    //    Response.Write("-5");
                    //}
                    else
                    {
                        string cp = "";
                        string SelectedPackge = "";
                        if (!string.IsNullOrEmpty(id))
                        {
                            cp = id.Substring(0, 2);
                            SelectedPackge = id.Substring(2, id.Length - 2);
                        }
                        else
                        {

                        }
                        JObject mu = new JObject();
                        int createStatus = -1;
                        if (cp == "DK")
                        {

                            if (SelectedPackge == Core.LTH)
                            {
                                mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else if (SelectedPackge == Core.TN)
                            {
                                mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else if (SelectedPackge == Core.VD)
                            {
                                mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else if (SelectedPackge == Core.EB)
                            {
                                mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            if (SelectedPackge == Core.LTH)
                            {
                                mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else if (SelectedPackge == Core.TN)
                            {
                                mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else if (SelectedPackge == Core.VD)
                            {
                                mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else if (SelectedPackge == Core.EB)
                            {
                                mu = MyControllers.CancelPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                            }
                            else
                            {
                            }
                        }
                        if (mu != null)
                        {
                            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());

                        }
                        //Thông báo
                        Response.Write(createStatus);

                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public ActionResult PackgeVms2(string id, string CaptchaValue)
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                Session["showboxlogin"] = "1";
                try
                {
                    if (string.IsNullOrEmpty(CaptchaValue))
                    {
                        Response.Write("-2");
                    }
                    //else if (!CaptchaController.IsValidCaptchaValue(CaptchaValue.ToUpper()))
                    //{
                    //    Response.Write("-5");
                    //}
                    else
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            Session["PCode_id"] = id;
                            int mulogcfStatus = -1;
                            string cp, SelectedPackge;
                            cp = SelectedPackge = "";
                            cp = id.Substring(0, 2);
                            SelectedPackge = id.Substring(2, id.Length - 2);
                            // ghi log khach hang gọi sang comfirm Viettel portal
                            try
                            {
                                // ghi log khac hang dong ky dang ky qua comfirm Viettel portal
                                JObject mulogcf = new JObject();
                                mulogcf = MyControllers.Log_ComfirmViettelPortal(Session["LoginName"].ToString(), SelectedPackge, "P");
                                if (mulogcf != null)
                                {
                                    mulogcfStatus = Convert.ToInt32(mulogcf["Header"]["Code"].ToString());

                                }
                                logger.Info("mulogcfStatus P =" + mulogcfStatus + "- SelectedPackge=" + SelectedPackge);

                            }
                            catch (Exception ex)
                            {
                                logger.Info("Exception mulogcfStatus P =" + mulogcfStatus);
                            }
                        }
                        //Thông báo
                        Response.Write("0");
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
        public ActionResult PackgeVms(string id, string CaptchaValue, string req)
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                Session["showboxlogin"] = "1";
                //Session["msisdn"] = "84988954651";
                try
                {
                    if (string.IsNullOrEmpty(CaptchaValue))
                    {
                        Response.Write("-2");
                    }
                    //else if (!CaptchaController.IsValidCaptchaValue(CaptchaValue.ToUpper()))
                    //{
                    //    Response.Write("-5");
                    //}  

                    else if (Session["msisdn"] == null)
                    {
                        Response.Write("2");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            Session["PCode_id"] = id;
                        }
                        if (Session["LoginName"] != null)
                        {
                            logger.Info("(mps): PCode_id" + id + "; LoginName=" + Session["LoginName"].ToString());
                        }
                        //Thông báo

                        if (Session["cprequest"] != null || req != null)
                        {
                            req = req.Substring(4);
                            //string cprequest = "";
                            //cprequest = req; Session["cprequest"].ToString();
                            //Session["cprequest"] = null;
                            JObject mo = new JObject();
                            try
                            {//  mo = MyControllers.LogUrlmps(cprequest.Trim(), MakeLink.DefaultURLWap(), "M");
                                mo = MyControllers.LogUrlmps(req.Trim(), Core.LinkReg, "C");
                                logger.Info("Log_cprequest: " + Core.LinkReg + "cprequest: " + req);
                                Response.Write("0");
                            }
                            catch
                            {
                                mo = null;
                                Response.Write("-5");//ghi log cprequest loi
                            }
                        }
                        else
                        {
                            Session["cprequest"] = null;
                            Response.Write("-6");//cprequest null
                        }


                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
        public ActionResult Resms()
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                Session["showboxlogin"] = "1";
                LoadDataHomeQuick();
                PackgeAccModel model = new PackgeAccModel();
                try
                {
                    int createStatus = -1;
                    int REMAIN = -1;
                    string curpack = "";
                    JObject mo = MyControllers.ListPackge(Session["LoginName"].ToString(), "1");
                    createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                    if (createStatus == 0)
                    {
                        mo = JObject.Parse(mo["Body"]["Data"].ToString());
                        JArray ma = JArray.Parse(mo["SERVICE"].ToString());
                        JArray ma_se = new JArray();
                        bool select = false;
                        if (mo["USER"] != null && !mo["USER"].ToString().StartsWith("[]"))
                        {
                            ma_se = JArray.Parse(mo["USER"].ToString());
                        }
                        List<CheckBoxCate> lstPack = new List<CheckBoxCate>();
                        foreach (var item in ma)
                        {
                            CheckBoxCate chkcate = new CheckBoxCate();
                            chkcate.ID = item["SERVICE_CODE"].ToString();
                            chkcate.Text = item["TXNAME"].ToString();
                            foreach (var itemse in ma_se)
                            {
                                if (itemse["SERVICE_CODE"].ToString() == chkcate.ID && itemse["ISSMS"].ToString() == "1")
                                {
                                    select = true;
                                    REMAIN = Convert.ToInt32(itemse["REDAY"].ToString());
                                }
                            }
                            chkcate.Checked = select;
                            chkcate.REMAIN = REMAIN;
                            lstPack.Add(chkcate);
                            select = false;
                        }
                        model.SelectedPackge = curpack;
                        model.ListPackge = lstPack;
                        ViewBag.ma = ma;

                    }
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "sms"));
                    }
                }
                catch (Exception)
                {
                    model = null;
                }
                return View(model);
            }
        }

        public ActionResult ResmsAdd(string id, string CaptchaValue)
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(CaptchaValue))
                    {
                        Response.Write("-2");
                    }
                    else if (!CaptchaController.IsValidCaptchaValue(CaptchaValue.ToUpper()))
                    {
                        Response.Write("-5");
                    }
                    else
                    {
                        string cp = "";
                        string SelectedPackge = "";
                        if (!string.IsNullOrEmpty(id))
                        {
                            cp = id.Substring(0, 2);
                            SelectedPackge = id.Substring(2, id.Length - 2);
                        }
                        else
                        {

                        }
                        JObject mu = new JObject();
                        int createStatus = -1;
                        if (cp == "DK")
                        {
                            mu = MyControllers.PackgeSMS(Session["LoginName"].ToString(), SelectedPackge, "1");
                        }
                        else
                        {
                            mu = MyControllers.PackgeSMS(Session["LoginName"].ToString(), SelectedPackge, "0");

                        }
                        if (mu != null)
                        {
                            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        }
                        //Thông báo
                        Response.Write(createStatus);

                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public ActionResult History()
        {
            if (Session["LoginName"] == null)
            {
               // Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                HistoryAccModel model = new HistoryAccModel();
                try
                {

                    string datestart, dateEnd;
                    datestart = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                    dateEnd = DateTime.Now.ToString("dd/MM/yyyy");
                    if (Request.QueryString["p"] != null)
                    {
                        int.TryParse(Request.QueryString["p"], out i_pageIndex);
                        i_pageIndex = i_pageIndex - 1;
                    }

                    int createStatus = -1;
                    JObject mo = MyControllers.History(Session["LoginName"].ToString(), datestart, dateEnd, "ALL", i_pageIndex, 50);
                    createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                    JArray ma = new JArray();
                    if (createStatus == 0)
                    {
                        mo = JObject.Parse(mo["Body"]["Data"].ToString());
                        ma = JArray.Parse(mo["TRANSACTION"].ToString());
                        mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                        ViewBag.ma = ma;
                        ViewBag.CurrentPage = i_pageIndex + 1;
                        ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                    }
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "packge"));
                    }
                    JObject mo_ser = MyControllers.ListPackge("2");
                    mo_ser = JObject.Parse(mo_ser["Body"]["Data"].ToString());
                    JArray ma_ser = JArray.Parse(mo_ser["SERVICE"].ToString());
                    List<SelectListItem> lstPack = new List<SelectListItem>();
                    lstPack.Add(new SelectListItem() { Text = "Tất cả", Value = "ALL" });
                    foreach (var item in ma_ser)
                    {
                        lstPack.Add(new SelectListItem() { Text = item["TXNAME"].ToString(), Value = item["SERVICE_CODE"].ToString() });
                    }
                    model.SelectedServiceCode = "ALL";
                    model.ServiceList = lstPack;

                    List<SelectListItem> days = new List<SelectListItem>();
                    days.Add(new SelectListItem() { Text = "Ngày", Value = "0" });
                    for (int i = 1; i <= 31; i++)
                    {
                        days.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.DaysFrom = days;
                    model.DaysTo = days;
                    model.SelectedFromDayId = DateTime.Now.AddDays(-7).Day;
                    model.SelectedToDayId = DateTime.Now.Day;

                    List<SelectListItem> months = new List<SelectListItem>();
                    months.Add(new SelectListItem() { Text = "Tháng", Value = "0" });
                    for (int i = 1; i <= 12; i++)
                    {
                        months.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    List<SelectListItem> monthsF = new List<SelectListItem>();
                    monthsF.Add(new SelectListItem() { Text = "Tháng", Value = "0" });
                    for (int i = 2; i <= 12; i++)
                    {
                        monthsF.Add(new SelectListItem() { Text = i - 1 + "", Value = i - 1 + "" });
                    }
                    model.MonthsFrom = monthsF;
                    model.MonthsTo = months;
                    model.SelectedFromMonthId = DateTime.Now.Month-1;
                    model.SelectedToMonthId = DateTime.Now.Month;

                    List<SelectListItem> years = new List<SelectListItem>();
                    years.Add(new SelectListItem() { Text = "Năm", Value = "" });
                    int maxYear = DateTime.Now.Year;
                    int minYear = maxYear - 3;
                    for (int i = maxYear; i >= minYear; i--)
                    {
                        years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.YearsFrom = years;
                    model.YearsTo = years;
                    model.SelectedFromYearId = DateTime.Now.Year;
                    model.SelectedToYearId = DateTime.Now.Year;
                }
                catch (Exception)
                {
                    List<SelectListItem> lstPack = new List<SelectListItem>();
                    lstPack.Add(new SelectListItem() { Text = "Tất cả", Value = "ALL" });
                    model.SelectedServiceCode = "ALL";
                    model.ServiceList = lstPack;

                    List<SelectListItem> days = new List<SelectListItem>();
                    days.Add(new SelectListItem() { Text = "Ngày", Value = "0" });
                    for (int i = 1; i <= 31; i++)
                    {
                        days.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.DaysFrom = days;
                    model.DaysTo = days;
                    model.SelectedFromDayId = DateTime.Now.AddDays(-7).Day;
                    model.SelectedToDayId = DateTime.Now.Day;

                    List<SelectListItem> months = new List<SelectListItem>();
                    months.Add(new SelectListItem() { Text = "Tháng", Value = "0" });
                    for (int i = 1; i <= 12; i++)
                    {
                        months.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.MonthsFrom = months;
                    model.MonthsTo = months;
                    model.SelectedFromMonthId = 1;
                    model.SelectedToMonthId = 1;

                    List<SelectListItem> years = new List<SelectListItem>();
                    years.Add(new SelectListItem() { Text = "Năm", Value = "" });
                    int maxYear = DateTime.Now.Year;
                    int minYear = maxYear - 3;
                    for (int i = maxYear; i >= minYear; i--)
                    {
                        years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.YearsFrom = years;
                    model.YearsTo = years;
                    model.SelectedFromYearId = DateTime.Now.Year;
                    model.SelectedToYearId = DateTime.Now.Year;
                }
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult History(HistoryAccModel model)
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                try
                {
                    LoadDataHomeQuick();
                    string datestart, dateEnd;
                    if (model.SelectedFromMonthId == 0 || model.SelectedFromDayId == 0 || model.SelectedFromYearId == 0)
                    {
                        datestart = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        datestart = model.SelectedFromDayId.ToString("00") + "/" + model.SelectedFromMonthId.ToString("00") + "/" + model.SelectedFromYearId;
                    }
                    if (model.SelectedToMonthId == 0 || model.SelectedToDayId == 0 || model.SelectedToYearId == 0)
                    {
                        dateEnd = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dateEnd = model.SelectedToDayId.ToString("00") + "/" + model.SelectedToMonthId.ToString("00") + "/" + model.SelectedToYearId;
                    }
                    if (Request.QueryString["p"] != null)
                    {
                        int.TryParse(Request.QueryString["p"], out i_pageIndex);
                        i_pageIndex = i_pageIndex - 1;
                    }
                    int createStatus = -1;
                    JObject mo = MyControllers.History(Session["LoginName"].ToString(), datestart, dateEnd, model.SelectedServiceCode, i_pageIndex, 50);
                    createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                    JArray ma = new JArray();
                    if (createStatus == 0)
                    {
                        mo = JObject.Parse(mo["Body"]["Data"].ToString());
                        string mooo = mo.ToString();
                        ma = JArray.Parse(mo["TRANSACTION"].ToString());
                        mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                        ViewBag.ma = ma;
                        ViewBag.CurrentPage = i_pageIndex + 1;
                        ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                    }
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "packge"));
                    }
                    //Load data
                    JObject mo_ser = MyControllers.ListPackge("2");
                    mo_ser = JObject.Parse(mo_ser["Body"]["Data"].ToString());
                    string moo = mo_ser.ToString();
                    JArray ma_ser = JArray.Parse(mo_ser["SERVICE"].ToString());
                    List<SelectListItem> lstPack = new List<SelectListItem>();
                    lstPack.Add(new SelectListItem() { Text = "Tất cả", Value = "ALL" });
                    foreach (var item in ma_ser)
                    {
                        lstPack.Add(new SelectListItem() { Text = item["TXNAME"].ToString(), Value = item["SERVICE_CODE"].ToString() });
                    }
                    model.ServiceList = lstPack;

                    List<SelectListItem> days = new List<SelectListItem>();
                    days.Add(new SelectListItem() { Text = "Ngày", Value = "0" });
                    for (int i = 1; i <= 31; i++)
                    {
                        days.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.DaysFrom = days;
                    model.DaysTo = days;

                    List<SelectListItem> months = new List<SelectListItem>();
                    months.Add(new SelectListItem() { Text = "Tháng", Value = "0" });
                    for (int i = 1; i <= 12; i++)
                    {
                        months.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.MonthsFrom = months;
                    model.MonthsTo = months;

                    List<SelectListItem> years = new List<SelectListItem>();
                    years.Add(new SelectListItem() { Text = "Năm", Value = "" });
                    int maxYear = DateTime.Now.Year;
                    int minYear = maxYear - 3;
                    for (int i = maxYear; i >= minYear; i--)
                    {
                        years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.YearsFrom = years;
                    model.YearsTo = years;
                }
                catch (Exception)
                {
                    List<SelectListItem> lstPack = new List<SelectListItem>();
                    lstPack.Add(new SelectListItem() { Text = "Tất cả", Value = "ALL" });
                    model.SelectedServiceCode = "ALL";
                    model.ServiceList = lstPack;

                    List<SelectListItem> days = new List<SelectListItem>();
                    days.Add(new SelectListItem() { Text = "Ngày", Value = "0" });
                    for (int i = 1; i <= 31; i++)
                    {
                        days.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.DaysFrom = days;
                    model.DaysTo = days;
                    model.SelectedFromDayId = DateTime.Now.AddDays(-7).Day;
                    model.SelectedToDayId = DateTime.Now.Day;

                    List<SelectListItem> months = new List<SelectListItem>();
                    months.Add(new SelectListItem() { Text = "Tháng", Value = "0" });
                    for (int i = 1; i <= 12; i++)
                    {
                        months.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.MonthsFrom = months;
                    model.MonthsTo = months;
                    model.SelectedFromMonthId = DateTime.Now.Month-1;
                    model.SelectedToMonthId = DateTime.Now.Month;

                    List<SelectListItem> years = new List<SelectListItem>();
                    years.Add(new SelectListItem() { Text = "Năm", Value = "" });
                    int maxYear = DateTime.Now.Year;
                    int minYear = maxYear - 3;
                    for (int i = maxYear; i >= minYear; i--)
                    {
                        years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    model.YearsFrom = years;
                    model.YearsTo = years;
                    model.SelectedFromYearId = DateTime.Now.Year;
                    model.SelectedToYearId = DateTime.Now.Year;
                }

                return View(model);
            }
        }

        public ActionResult MsgBox()
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                Session["showboxlogin"] = "1";
                return View();
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MsgBox(string packcode)
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                LoadDataHomeQuick();
                Session["showboxlogin"] = "1";
                if (Request.Params["btnUpdate"] != null)
                {
                    try
                    {
                        string cp = Core.LTH;
                        string SelectedPackge = "";

                        JObject mu = new JObject();
                        int createStatus = -1;
                        if (SelectedPackge == Core.LTH)
                        {
                            mu = MyControllers.AddPackgeTN(Session["LoginName"].ToString(), SelectedPackge);
                        }
                        else if (SelectedPackge == Core.TN)
                        {
                            mu = MyControllers.AddPackgeLTH(Session["LoginName"].ToString(), SelectedPackge);
                        }
                        else
                        {

                        }
                        if (mu != null)
                        {
                            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                            if (createStatus == 0)
                            {
                                Session["mess"] = @"<div class=""mess_sucess"">" + LoginMessage.Success + "</div>";

                            }
                            else
                            {
                                Session["mess"] = @"<div class=""mess_mess_error"">" + ACC_MESSAGE(createStatus, "packge") + "</div>";
                            }

                        }
                        else
                        {
                            Session["mess"] = @"<div class=""mess_mess_error"">Hệ thống đang quá tải. Vui lòng quay lại sau ít phút.</div>";
                        }

                    }
                    catch (Exception)
                    {
                        Session["mess"] = @"<div class=""mess_mess_error"">Hệ thống đang quá tải. Vui lòng quay lại sau ít phút.</div>";
                    }
                    //Thông báo
                    Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                    return Redirect("/thong-bao.html");
                }
                if (Request.Params["btnHome"] != null)
                {
                    return Redirect("/");
                }
                return null;
            }

        }


        public ActionResult Bookcase()
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                return View();
            }
        }

        public ActionResult BookofBuy()
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");

            }
            else
            {
                try
                {
                    //ViewBag.TAB_DEFAULT = i_tab;
                    RequestParam();
                    JObject mo = MyControllers.ListBookTran(Session["LoginName"].ToString(), "1", i_pageIndex, i_pagesize);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JArray ma = JArray.Parse(mo["BOOK"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.Psize = i_pagesize;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                }
                catch (Exception)
                {
                }
                return View();
            }
        }

        public ActionResult Videobuy()
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                try
                {
                    //ViewBag.TAB_DEFAULT = i_tab;
                    RequestParam();
                    JObject mo = MyControllers.ListVideoTran(Session["LoginName"].ToString(), "1", i_pageIndex, i_pagesize);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JArray ma = JArray.Parse(mo["BOOK"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.Psize = i_pagesize;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                }
                catch (Exception)
                {
                }
                return View();
            }
        }


        public ActionResult BookofRead()
        {
            if (Session["LoginName"] == null)
            {
                return Redirect("/dang-nhap.html");
            }
            else
            {
                try
                {
                    //ViewBag.TAB_DEFAULT = i_tab;
                    RequestParam();
                    JObject mo = MyControllers.ListBookofRead(Session["LoginName"].ToString(), "0", i_pageIndex, i_pagesize);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JArray ma = JArray.Parse(mo["BOOK"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.Psize = i_pagesize;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                }
                catch (Exception)
                {
                }
                return View();
            }
        }

        public ActionResult BookofLike()
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                return Redirect("/dang-nhap.html");
            }
            else
            {
                try
                {
                    //ViewBag.TAB_DEFAULT = i_tab;
                    RequestParam();
                    JObject mo = MyControllers.ListBookLike(Session["LoginName"].ToString(), i_pageIndex, i_pagesize);

                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    string msd = mo.ToString();
                    JArray ma = JArray.Parse(mo["BOOK"].ToString());
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.Psize = i_pagesize;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                }
                catch (Exception)
                {
                }
                return View();
            }
        }

        public ActionResult BuyBox(string id)
        {
            LoadDataHomeQuick();
            string LoginName = "0", uname="0";
            string reUrl_EB="";
            Session["msisdn"] = "0988954651";
            Session["LoginName"] = "0988954651";
            Session["LoginDisplay"] = "0988954651";                      
            
            
            
            if (Session["LoginName"] != null)
            {
                LoginName = Session["LoginName"].ToString();
                uname = Common.formatPhone(LoginName, 0);
            }
            JObject mo = MyControllers.GetBookById(id, LoginName);
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
            ViewBag.info = info;
            BuyNotLogInViewModel model = new BuyNotLogInViewModel();
            string cate = "EBook";
            
            if (Session["msisdn"] != null)
            {
                LoginName = Session["msisdn"].ToString();
                uname = Common.formatPhone(LoginName, 0);
                Core.is3g = 1;
                reUrl_EB = aFun.Models.ResponeUrlVTL.getUrlChargeSub("DOWLOAD", "", "LEMONMEDIA", info["TXNAME"].ToString(), cate, "1000", "Mua_sach" + Core.URL_EB, uname, "WAP");

            }
            else
            {
                Core.is3g = 0;
                reUrl_EB = "null";
            }
            model.MobileNumber = LoginName;
            model.BookId = info["BOOK_ID"].ToString();
            model.BookName = info["TXNAME"].ToString();
            model.Price = info["PRICE"].ToString();
            model.Btype = info["BTYPE"].ToString();
            ViewBag.BookCode = info["BOOK_CODE"].ToString();
            ViewBag.reUrl_EB = reUrl_EB;
            
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BuyBox(BuyNotLogInViewModel model)
        {
            try
            {

                JObject mu = new JObject();
                if (Request.Params["btnOTP"] != null)
                {
                    if (string.IsNullOrEmpty(model.CaptchaValue))
                    {
                        ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                    }
                    else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                    }
                    else
                    {
                        model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                        if (Common.ValidateViettel(model.MobileNumber))
                        {
                            int createStatus = -1;
                            mu = MyControllers.GenOTP(model.MobileNumber, "2");
                            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                            if (createStatus == 0)
                            {
                                mu = JObject.Parse(mu["Body"]["Data"].ToString());
                                mu = JObject.Parse(JArray.Parse(mu["OPT"].ToString())[0].ToString());
                                ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận đã được gửi tới số điện thoại của quý khách.</div>";
                                //ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận: " + mu["TRAN_CODE"].ToString() + "</div>";
                            }
                            else
                            {
                                ModelState.AddModelError("", ACC_MESSAGE(createStatus, "otp"));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", ValidateClient.InvalidMobile);
                        }
                    }
                }
                string reurl = "/";
                if (Request.Params["btnUpdate"] != null)
                {
                    int createStatus = -1;

                    if (Session["msisdn"] != null)
                    {

                        mu = MyControllers.BuyBook(model.MobileNumber, model.BookId, "123", 1);

                    }
                    else
                    {
                        mu = MyControllers.BuyBook(model.MobileNumber, model.BookId, model.CodeOTP, 0);
                    }
                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                    if (createStatus == 0)
                    {

                        //ViewBag.Message = @"<div class=""mess_sucess"">" + BuyStatusMessage.Success + "</div>";
                        if (model.Btype == "TEXT")
                        {
                            reurl = MakeLink.MakeLinkERead(model.BookName, model.BookId);
                        }
                        else if (model.Btype == "AUDIO")
                        {
                            reurl = MakeLink.MakeLinkAudioRead(model.BookName, model.BookId);
                        }
                        else if (model.Btype == "IMAGE")
                        {
                            reurl = MakeLink.MakeLinkPictureRead(model.BookName, model.BookId);
                        }
                        else
                        {

                        }

                        //Session["mess"] = @"<div class=""mess_sucess"">" + BuyStatusMessage.Success + "</div>";
                        //Session["re_url"] = reurl;
                        //return Redirect("/thong-bao.html");
                        Session["reurl"] = reurl;
                        ViewBag.ReadBook = reurl;
                        ViewBag.NextBox = "1";
                        ViewBag.Message = @"<div class=""mess_sucess"">" + BuyStatusMessage.Success + "</div>";
                    }
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "buy"));
                    }
                }
                if (Request.Params["btnHome"] != null)
                {
                    return Redirect("/");
                }
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(model.BookId, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                ViewBag.info = info;
                model.BookId = info["BOOK_ID"].ToString();
                model.BookName = info["TXNAME"].ToString();
                model.Price = info["PRICE"].ToString();
                model.Btype = info["BTYPE"].ToString();
                ViewBag.BookCode = info["BOOK_CODE"].ToString();
            }
            catch (Exception)
            {
                model = null;
            }
            return View(model);
        }

        public ActionResult GiffBox(string id)
        {
            string LoginName = "0";
            if (Session["msisdn"] != null)
            {
                Core.is3g = 1;
            }
            else
            {
                Core.is3g = 0;
            }
            if (Session["LoginName"] != null)
            {
                LoginName = Session["LoginName"].ToString();
            }
            JObject mo = MyControllers.GetBookById(id, LoginName);
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            string mmkh = mo.ToString();
            JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
            ViewBag.info = info;
            
            GiffNotLogInViewModel model = new GiffNotLogInViewModel();
            model.MobileNumber = LoginName;
            model.BookId = info["BOOK_ID"].ToString();
            model.BookName = info["TXNAME"].ToString();
            model.Price = info["PRICE"].ToString();
            model.Btype = info["BTYPE"].ToString();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GiffBox(GiffNotLogInViewModel model)
        {
            try
            {
                JObject mu = new JObject();
                if (Request.Params["btnOTP"] != null)
                {
                    if (string.IsNullOrEmpty(model.CaptchaValue))
                    {
                        ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                    }
                    else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                    }
                    else
                    {
                        model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                        model.GiffMobileNumber = Common.formatPhone(model.GiffMobileNumber, 0);
                        if (Common.ValidateViettel(model.MobileNumber) && Common.ValidateViettel(model.GiffMobileNumber))
                        {
                            if (model.MobileNumber != model.GiffMobileNumber)
                            {
                                int createStatus = -1;
                                mu = MyControllers.GenOTP(model.MobileNumber, "4");
                                createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                                if (createStatus == 0)
                                {
                                    mu = JObject.Parse(mu["Body"]["Data"].ToString());
                                    mu = JObject.Parse(JArray.Parse(mu["OPT"].ToString())[0].ToString());
                                   //ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận đã được gửi tới số điện thoại của quý khách.</div>";
                                   ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận: " + mu["TRAN_CODE"].ToString() + "</div>";
                                }
                                else
                                {
                                    ModelState.AddModelError("", ACC_MESSAGE(createStatus, "otp"));
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Quý khách không thể tặng sách cho chính mình.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", ValidateClient.InvalidMobile);
                        }
                    }
                }
                string reurl = "/";
                if (Request.Params["btnUpdate"] != null)
                {
                    if (model.MobileNumber != model.GiffMobileNumber)
                    {
                        int createStatus = -1;
                        if (Session["msisdn"] != null)
                        {
                            mu = MyControllers.GiffBook(model.MobileNumber, model.GiffMobileNumber, model.BookId, "123", 1);
                        }
                        else
                        {
                            mu = MyControllers.GiffBook(model.MobileNumber, model.GiffMobileNumber, model.BookId, model.CodeOTP, 0);
                        }
                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            ViewBag.Message = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                            //ViewBag.Message = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                            if (model.Btype == "TEXT")
                            {
                                //reurl = MakeLink.MakeLinkERead(model.BookName, model.BookId);
                                reurl = MakeLink.MakeLinkEBook(model.BookName, model.BookId,"1");
                            }
                            else if (model.Btype == "AUDIO")
                            {
                                //reurl = MakeLink.MakeLinkAudioRead(model.BookName, model.BookId);
                                reurl = MakeLink.MakeLinkAudioBook(model.BookName, model.BookId);
                            }
                            else if (model.Btype == "IMAGE")
                            {
                                //reurl = MakeLink.MakeLinkPictureRead(model.BookName, model.BookId);
                                reurl = MakeLink.MakeLinkPicturebook(model.BookName, model.BookId);
                            }
                            else
                            {

                            }
                            //Session["mess"] = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                            //Session["re_url"] = reurl;
                            //return Redirect("/thong-bao.html");
                            ViewBag.ReadBook = reurl;
                            ViewBag.NextBox = "1";
                            ViewBag.Message = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "giff"));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Quý khách không thể tặng sách cho chính mình.");
                    }
                }
                if (Request.Params["btnHome"] != null)
                {
                    Response.Redirect("/");
                    return null;
                }
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(model.BookId, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                ViewBag.info = info;
                model.BookId = info["BOOK_ID"].ToString();
                model.BookName = info["TXNAME"].ToString();
                model.Price = info["PRICE"].ToString();
                model.Btype = info["BTYPE"].ToString();
            }
            catch (Exception)
            {
                model = null;
            }
            return View(model);
        }


        //mua video Tong hop 
        // do ttg gao
        public ActionResult BuyVDTH(string id)
        {
            LoadDataHomeQuick();
            string LoginName = "0";
            if (Session["msisdn"] != null)
            {
                Core.is3g = 1;
            }
            else
            {
                Core.is3g = 0;
            }
            if (Session["LoginName"] != null)
            {
                LoginName = Session["LoginName"].ToString();
            }
            //JObject mo = MyControllers.Getv(id, LoginName);
            JObject mo = MyControllers.GetLawById(Core.A_VIDEO, id, LoginName);
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            string bbbh= mo.ToString();
            JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
            ViewBag.info = info;
            BuyNotLogInViewModel model = new BuyNotLogInViewModel();
            model.MobileNumber = LoginName;
            model.BookId = info["LAW_ID"].ToString();
            model.BookName = info["TXNAME"].ToString();
            model.Price = info["PRICE"].ToString();
            model.Btype = info["BTYPE"].ToString();
            ViewBag.BookCode = info["LAW_CODE"].ToString();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BuyVDTH(BuyNotLogInViewModel model)
        {
            try
            {
                JObject mu = new JObject();
                if (Request.Params["btnOTP"] != null)
                {
                    if (string.IsNullOrEmpty(model.CaptchaValue))
                    {
                        ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                    }
                    else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                    }
                    else
                    {
                        model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                        if (Common.ValidateViettel(model.MobileNumber))
                        {
                            int createStatus = -1;
                            //type: 0.Dang ky, 1.quen mat khau, 2. opt mua book, 3. opt tang book, 4. mua goi, 
                            //5. tang goi; 6: mua video tong hop ; 
                            //7: tang video tong hop
                            mu = MyControllers.GenOTP(model.MobileNumber, "6");
                            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                            if (createStatus == 0)
                            {
                                mu = JObject.Parse(mu["Body"]["Data"].ToString());
                                mu = JObject.Parse(JArray.Parse(mu["OPT"].ToString())[0].ToString());
                                //ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận đã được gửi tới số điện thoại của quý khách.</div>";
                                ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận: " + mu["TRAN_CODE"].ToString() + "</div>";
                            }
                            else
                            {
                                ModelState.AddModelError("", ACC_MESSAGE(createStatus, "otp"));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", ValidateClient.InvalidMobile);
                        }
                    }
                }
                string reurl = "/";
                if (Request.Params["btnUpdate"] != null)
                {
                    int createStatus = -1;

                    if (Session["msisdn"] != null)
                    {

                        mu = MyControllers.Buyvideo(model.MobileNumber, model.BookId, "123", 1);

                    }
                    else
                    {
                        mu = MyControllers.Buyvideo(model.MobileNumber, model.BookId, model.CodeOTP, 0);
                    }
                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                    if (createStatus == 0)
                    {

                        //ViewBag.Message = @"<div class=""mess_sucess"">" + BuyStatusMessage.Success + "</div>";
                        if (model.Btype == "TEXT")
                        {
                            reurl = MakeLink.MakeLinkERead(model.BookName, model.BookId);
                        }
                        else if (model.Btype == "AUDIO")
                        {
                            reurl = MakeLink.MakeLinkAudioRead(model.BookName, model.BookId);
                        }
                        else if (model.Btype == "IMAGE")
                        {
                            reurl = MakeLink.MakeLinkPictureRead(model.BookName, model.BookId);
                        }
                        else if (model.Btype == "VIDEOTH")
                        {
                            //reurl = MakeLink.MakeLinkPictureRead(model.BookName, model.BookId);
                           // reurl = MakeLink.MakeLinkPicturebook(model.BookName, model.BookId);
                            string TXNAME = model.BookName;
                            if (TXNAME.Length > 65)
                            {
                                TXNAME = TXNAME.Substring(0, 65).ToString() + "...";
                            }
                            reurl = MakeLink.MakeLinkVideo(TXNAME, TXNAME, model.BookId);
                        }
                        else { 
                        }

                        //Session["mess"] = @"<div class=""mess_sucess"">" + BuyStatusMessage.Success + "</div>";
                        //Session["re_url"] = reurl;
                        //return Redirect("/thong-bao.html");
                        Session["reurl"] = reurl;
                        ViewBag.ReadBook = reurl;
                        ViewBag.NextBox = "1";
                        ViewBag.Message = @"<div class=""mess_sucess"">" + BuyStatusMessage.Success + "</div>";
                    }
                    else
                    {
                        ModelState.AddModelError("", ACC_MESSAGE(createStatus, "buy"));
                    }
                }
                if (Request.Params["btnHome"] != null)
                {
                    return Redirect("/");
                }
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                //JObject mo = MyControllers.GetBookById(model.BookId, LoginName);
                JObject mo = MyControllers.GetLawById(Core.A_VIDEO, model.BookId, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                ViewBag.info = info;
                model.BookId = info["LAW_ID"].ToString();
                model.BookName = info["TXNAME"].ToString();
                model.Price = info["PRICE"].ToString();
                model.Btype = info["BTYPE"].ToString();
                ViewBag.BookCode = info["LAW_CODE"].ToString();
            }
            catch (Exception)
            {
                model = null;
            }
            return View(model);
        }

        public ActionResult GiffVDTH(string id)
        {
            string LoginName = "0";
            if (Session["msisdn"] != null)
            {
                Core.is3g = 1;
            }
            else
            {
                Core.is3g = 0;
            }
            if (Session["LoginName"] != null)
            {
                LoginName = Session["LoginName"].ToString();
            }
         //   JObject mo = MyControllers.GetBookById(id, LoginName);
            JObject mo = MyControllers.GetLawById(Core.A_VIDEO, id, LoginName);
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            string mmkh = mo.ToString();
            JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
            ViewBag.info = info;

            GiffNotLogInViewModel model = new GiffNotLogInViewModel();
            model.MobileNumber = LoginName;
            model.BookId = info["LAW_ID"].ToString();
            model.BookName = info["TXNAME"].ToString();
            model.Price = info["PRICE"].ToString();
            model.Btype = info["BTYPE"].ToString();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GiffVDTH(GiffNotLogInViewModel model)
        {
            try
            {
                JObject mu = new JObject();
                if (Request.Params["btnOTP"] != null)
                {
                    if (string.IsNullOrEmpty(model.CaptchaValue))
                    {
                        ModelState.AddModelError("", ValidateClient.EmptyCaptcha);
                    }
                    else if (!CaptchaController.IsValidCaptchaValue(model.CaptchaValue.ToUpper()))
                    {
                        ModelState.AddModelError("", ValidateClient.InvalidCaptcha);
                    }
                    else
                    {
                        model.MobileNumber = Common.formatPhone(model.MobileNumber, 0);
                        model.GiffMobileNumber = Common.formatPhone(model.GiffMobileNumber, 0);
                        if (Common.ValidateViettel(model.MobileNumber) && Common.ValidateViettel(model.GiffMobileNumber))
                        {
                            if (model.MobileNumber != model.GiffMobileNumber)
                            {
                                int createStatus = -1;
                                mu = MyControllers.GenOTP(model.MobileNumber, "7");
                                createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                                if (createStatus == 0)
                                {
                                    mu = JObject.Parse(mu["Body"]["Data"].ToString());
                                    mu = JObject.Parse(JArray.Parse(mu["OPT"].ToString())[0].ToString());
                                    //ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận đã được gửi tới số điện thoại của quý khách.</div>";
                                    ViewBag.Message = @"<div class=""mess_sucess"">Mã xác nhận: " + mu["TRAN_CODE"].ToString() + "</div>";
                                }
                                else
                                {
                                    ModelState.AddModelError("", ACC_MESSAGE(createStatus, "otp"));
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Quý khách không thể tặng sách cho chính mình.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", ValidateClient.InvalidMobile);
                        }
                    }
                }
                string reurl = "/";
                if (Request.Params["btnUpdate"] != null)
                {
                    if (model.MobileNumber != model.GiffMobileNumber)
                    {
                        int createStatus = -1;
                        if (Session["msisdn"] != null)
                        {
                            mu = MyControllers.Giffvideo(model.MobileNumber, model.GiffMobileNumber, model.BookId, "123", 1);
                        }
                        else
                        {
                            mu = MyControllers.Giffvideo(model.MobileNumber, model.GiffMobileNumber, model.BookId, model.CodeOTP, 0);
                        }
                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                        if (createStatus == 0)
                        {
                            ViewBag.Message = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                            //ViewBag.Message = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                            if (model.Btype == "TEXT")
                            {
                                //reurl = MakeLink.MakeLinkERead(model.BookName, model.BookId);
                                reurl = MakeLink.MakeLinkEBook(model.BookName, model.BookId, "1");
                            }
                            else if (model.Btype == "AUDIO")
                            {
                                //reurl = MakeLink.MakeLinkAudioRead(model.BookName, model.BookId);
                                reurl = MakeLink.MakeLinkAudioBook(model.BookName, model.BookId);
                            }
                            else if (model.Btype == "VIDEOTH")
                            {
                                //reurl = MakeLink.MakeLinkPictureRead(model.BookName, model.BookId);
                                reurl = MakeLink.MakeLinkPicturebook(model.BookName, model.BookId);
                                string TXNAME = model.BookName;
                                if (TXNAME.Length > 65)
                                {
                                    TXNAME = TXNAME.Substring(0, 65).ToString() + "...";
                                }
                                reurl = MakeLink.MakeLinkVideo(TXNAME, TXNAME, model.BookId);
                            }
                            else
                            {

                            }
                            //Session["mess"] = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                            //Session["re_url"] = reurl;
                            //return Redirect("/thong-bao.html");
                            ViewBag.ReadBook = reurl;
                            ViewBag.NextBox = "1";
                            ViewBag.Message = @"<div class=""mess_sucess"">" + GiffStatusMessage.Success + "</div>";
                        }
                        else
                        {
                            ModelState.AddModelError("", ACC_MESSAGE(createStatus, "giff"));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Quý khách không thể tặng sách cho chính mình.");
                    }
                }
                if (Request.Params["btnHome"] != null)
                {
                    Response.Redirect("/");
                    return null;
                }
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetLawById(Core.A_VIDEO, model.BookId, LoginName);
                //JObject mo = MyControllers.GetBookById(model.BookId, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                ViewBag.info = info;
                model.BookId = info["LAW_ID"].ToString();
                model.BookName = info["TXNAME"].ToString();
                model.Price = info["PRICE"].ToString();
                model.Btype = info["BTYPE"].ToString();
            }
            catch (Exception)
            {
                model = null;
            }
            return View(model);
        }

        private string ACC_MESSAGE(int code, string cmd)
        {
            if (cmd == "register")
            {
                switch (code)
                {
                    case 0:
                        return "Đăng ký thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 1101:
                        return "Số điện thoại đã đăng ký.";
                    case 1102:
                        return "Đăng ký thất bại.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "activereg")
            {
                switch (code)
                {
                    case 0:
                        return "Kích hoạt tài khoản thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 1101:
                        return "Mã xác nhận không chính xác.";
                    case 1102:
                        return "Mã xác nhận không chính xác.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "login")
            {
                switch (code)
                {
                    case 0:
                        return "Đăng nhập thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 1101:
                        return "Đăng nhập thất bại.";
                    case 1102:
                        return "Số điện thoại hoặc mật khẩu không chính xác.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "forget")
            {
                switch (code)
                {
                    case 0:
                        return "Lấy lại mật khẩu thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 1101:
                        return "Lấy mật khẩu thất bại.";
                    case 1102:
                        return "Mã xác nhận không chính xác.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "changepass")
            {
                switch (code)
                {
                    case 0:
                        return "Đổi mật khẩu thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 1101:
                        return "Mật khẩu hiện tại không chính xác.";
                    case 1102:
                        return "Mật khẩu hiện tại không chính xác.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "accinfo")
            {
                switch (code)
                {
                    case 0:
                        return "Cập nhật thông tin cá nhân thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 106:
                        return "Lỗi xác nhận.";
                    case 107:
                        return "Thuê bao quý khách không phải là thuê bao của Viettel.";
                    case 108:
                        return "Tài khoản không đủ tiền để thực hiện giao dịch.";
                    case 1101:
                        return "Cập nhật thông tin cá nhân thất bại.";
                    case 1102:
                        return "Cập nhật thông tin cá nhân thất bại.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "packge")
            {
                switch (code)
                {
                    case 0:
                        return "Thay đổi gói cước thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 106:
                        return "Lỗi xác nhận.";
                    case 107:
                        return "Thuê bao quý khách không phải là thuê bao của Viettel.";
                    case 108:
                        return "Tài khoản không đủ tiền để thực hiện giao dịch.";
                    case 1101:
                        return "Gói cước này quý khách đã đăng ký.";
                    case 1102:
                        return "Giao dịch thất bại.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "sms")
            {
                switch (code)
                {
                    case 0:
                        return "Thay đổi nhận sms thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 106:
                        return "Lỗi xác nhận.";
                    case 107:
                        return "Thuê bao quý khách không phải là thuê bao của Viettel.";
                    case 108:
                        return "Tài khoản không đủ tiền để thực hiện giao dịch.";
                    case 1101:
                        return "Quý khách đã thực hiện thao tác này rồi.";
                    case 1102:
                        return "Giao dịch thất bại.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "history")
            {
                switch (code)
                {
                    case 0:
                        return "Thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 106:
                        return "Lỗi xác nhận.";
                    case 107:
                        return "Thuê bao quý khách không phải là thuê bao của Viettel.";
                    case 108:
                        return "Tài khoản không đủ tiền để thực hiện giao dịch.";
                    case 1101:
                        return "Số điện thoại đã đăng ký.";
                    case 1102:
                        return "Số điện thoại chưa đăng ký.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else if (cmd == "otp")
            {
                switch (code)
                {
                    case 0:
                        return "Thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 106:
                        return "Lỗi xác nhận.";
                    case 107:
                        return "Thuê bao quý khách không phải là thuê bao của Viettel.";
                    case 108:
                        return "Tài khoản không đủ tiền để thực hiện giao dịch.";
                    case 1101:
                        return "Gửi mã OTP thất bại.";
                    case 1102:
                        return "Số điện thoại chưa đăng ký.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }
            else
            {
                switch (code)
                {
                    case 0:
                        return "Thành công!";
                    case 10:
                        return "Request hết hạn.";
                    case 11:
                        return "Key AES fail";
                    case 100:
                        return "Cmd not exists.";
                    case 101:
                        return "Dữ liệu đầu vào không hợp lệ.";
                    case 102:
                        return "Format dữ liệu đầu vào không hợp lệ.";
                    case 103:
                        return "Warring.";
                    case 104:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                    case 105:
                        return "Not data Body.";
                    case 106:
                        return "Lỗi xác nhận.";
                    case 107:
                        return "Thuê bao quý khách không phải là thuê bao của Viettel.";
                    case 108:
                        return "Tài khoản không đủ tiền để thực hiện giao dịch.";
                    case 1101:
                        return "Số điện thoại đã đăng ký.";
                    case 1102:
                        return "Số điện thoại chưa đăng ký.";
                    case 1103:
                        return "Mật khẩu phải có ít nhất 6 ký tự.";
                    case 1104:
                        return "Email không hợp lệ.";
                    default:
                        return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
                }
            }

        }
     
        private void RequestParam()
        {
            if (Request.QueryString["p"] != null)
            {
                int.TryParse(Request.QueryString["p"], out i_pageIndex);
                i_pageIndex = i_pageIndex - 1;
            }
            if (Request.QueryString["red"] != null)
            {
                
                url = Server.UrlDecode(Request.QueryString["red"].ToString());
            }
            // string slipc = Request.QueryString["page"].ToString();
            
        }
        private string ErrorCodeCharge(int status)
        {
            //Code: 1.Ok, 0.Da ton tai, -1.Erorr, 2. Da tao nhung chua Active Code
            switch (status)
            {
                case 0:
                    return "Giao dịch thành công.";
                case 1:
                    return "Không tìm thấy số điện thoại .";
                case 4:
                    return "IP Client không nằm trong dải IP Pool.";
                case 11:
                    return "Thiếu tham số.";
                case 13:
                    return "Thiếu trường amount.";
                case 14:
                    return "Thiếu trường cp request id.";
                case 15:
                    return "Thiếu trường value.";
                case 16:
                    return "Thiếu trường aes key.";
                case 17:
                    return "Thiếu trường name item.";
                case 18:
                    return "Thiếu trường category item.";
                case 22:
                    return "CP code không hợp lệ/không active.";
                case 23:
                    return "Thanh toán không hợp lệ.";
                case 24:
                    return "Không có giao dịch confirm trước đó.";
                case 25:
                    return "CP Request Id không hợp lệ.";
                case 503:
                    return "Lỗi hệ thống MPS.";
                case 101:
                    return "Lỗi thực hiện nghiệp vụ.";
                case 102:
                    return "Lỗi khi thực hiện đăng ký dịch vụ.";
                case 103:
                    return "Lỗi khi thực hiện thanh toán dịch vụ bằng tài khoản mobile.";
                case 104:
                    return "Lỗi khi hủy dịch vụ.";
                case 201:
                    return "Chữ ký không hợp lệ.";
                case 202:
                    return "Thanh toán bị lỗi.";
                case 204:
                    return "Tồn tại tài khoản MPS nhưng chưa dùng dịch vụ của CP.";
                case 205:
                    return "Tồn tại tài khoản MPS và đã dùng dịch vụ của CP.";
                case 401:
                    return "Tài khoản không đủ thanh toán.";
                case 402:
                    return "Thuê bao không được đăng ký trong hệ thống thanh toán.";
                case 403:
                    return "Thuê bao không tồn tại.";
                case 404:
                    return "Thuê bao không có sẵn.";
                case 405:
                    return "Thuê bao đã thay đổi chủ sở hữu.";
                case 406:
                    return "Lỗi không tìm thấy dữ liệu mobile để thanh toán.";
                case 407:
                    return "CP không được phép thực hiện nghiệp vụ: tham số SUB, SER, CMD không hợp lệ/chưa được khai báo đầy đủ.";
                case 408:
                    return "Thuê bao đang sử dụng dịch vụ.";
                case 409:
                    return "Thuê bao bị chặn hai chiều.";
                case 410:
                    return "Số điện thoại không phải viettel.";
                case 411:
                    return "Thuê bao đã hủy dịch vụ.";
                case 412:
                    return "Thuê bao không sử dụng dịch vụ.";
                case 413:
                    return " Lấy giá tiền phù hợp để thanh toán bị lỗi: tham số SUB, SER, CMD, CATEGORY (với các hàm thanh toán CHARGE/DOWNLOAD) không hợp lệ/chưa được khai báo đầy đủ.";
                case 414:
                    return "Thuê bao hủy dịch vụ vẫn đang nằm trong chu kỳ charge cước (thời điểm charge < thời điểm charge tiếp theo của chu kỳ).";
                case 415:
                    return "Mã OTP không hợp lệ/chưa nhập mã OTP.";
                case 416:
                    return " Mã OTP không tồn tại/hết timeout cho phép.";
                case 417:
                    return " Mã OTP không tồn tại/hết timeout cho phép.";
                case 440:
                    return "Lỗi kết nối.";
                case 501:
                    return "Thuê bao chưa đăng ký dịch vụ.";
                default:
                    return "Lỗi trong quá trình thực thi.";
            }
        }
    
    }
}
