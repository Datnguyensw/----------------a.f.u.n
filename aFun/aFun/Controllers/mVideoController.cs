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
    public class mVideoController : Controller
    {
        //
        // GET: /Book/
        string i_tab = Common.booknume[0];
        int i_pageIndex = 0;
        string i_category = "";
        int i_pageSize = Int32.Parse(Iconfig.P_video);
        string _catecode = "ALL",_type="";

        string _modCode = "";
        int btype=0, type = 0;
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
                    string m = mo.ToString();
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

        bool chekpack() {
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
        
        public ActionResult Index()
        {
            string pack_adu="NOT";
            if (chekpack()) {
                pack_adu = "OK";
            }
            ViewBag.PACKADU = pack_adu;
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
             //   LoadDataHomeQuick();
                RequestParam();
                JObject mo_cate = MyControllers.GetListChildCate(Core.A_VIDEO);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                string cate = ma_cate.ToString();

                JObject cate_gory = MyControllers.Get_cate_item("0002", "0002", "0");
                JObject array_al = JObject.Parse(cate_gory["Body"]["Data"].ToString());
                cate_gory = JObject.Parse(cate_gory["Body"]["Data"].ToString());//1

                string b = cate_gory.ToString();

                JArray ma_ = JArray.Parse(cate_gory["CATEGORY"].ToString());//2
                JArray parent = JArray.Parse(cate_gory["PARENT"].ToString());//2ORDER
                JArray order = JArray.Parse(cate_gory["ORDER"].ToString());//2ORDER

                ViewBag.ma_cate = ma_cate;
                string ms = parent.ToString();
                string msd = ma_.ToString();
                string m = mo_cate.ToString();
                ViewBag.MENULEFT = cate_gory["MENU"] == null ? null : JArray.Parse(cate_gory["MENU"].ToString());
                ViewBag.cate_gory = ma_;//
                ViewBag.item_cate = parent;///
                ViewBag.list_oder = order;///
                ViewBag.list_arr = array_al;///
                ViewBag._tpage = Iconfig.P_video;
           
            }
            catch (Exception e)
            {
                string a = e.ToString();
            }
            return View();

        }
        public ActionResult List_by_item()
        {
            string pack_adu = "NOT";
            if (chekpack())
            {
                pack_adu = "OK";
            }
            ViewBag.PACKADU = pack_adu;
            //   <action type="Rewrite" url="/Law/List_by_item?ccode={R:3}&amp;pg={R:4}" />
           
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                JObject lisdata = MyControllers.get_3014(i_pageIndex, i_pageSize, _catecode, "0002", "6", type);//0

            }
            catch (Exception)
            {
            }
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo_cate = MyControllers.GetListChildCate(_catecode);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                if (ma_cate == null || ma_cate.Count <= 1)
                {
                    try
                    {
                        if (i_pageIndex < 0)
                        {
                            i_pageIndex = 0;
                        }
                        JObject mo = MyControllers.get_3014(i_pageIndex, i_pageSize, _catecode, "0002", "6", type);//0
                        mo = JObject.Parse(mo["Body"]["Data"].ToString());
                        JArray ma = JArray.Parse(mo["LAW"].ToString());
                        
                        string abc = mo.ToString();
                        mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                        ViewBag.ma = ma;
                        ViewBag.CurrentPage = i_pageIndex + 1;
                        ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                        string t = mo["total"].ToString();
                        ViewBag._catecode = _catecode;
                        ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                        //
                        ViewBag._tpage = Iconfig.P_video;
                        ViewBag.URL_PAGEn2 = "/" + MakeLink.Page_MakeLink(ma[0]["CATE_NAME"].ToString()) + "a" + ma[0]["CATE_CODE"].ToString() + "fun" + type + ".html";
                        ViewBag.URL_PAGEn1 = "/video/trang";
                        string btyle__ = "";// mới=1; chọn lọc=2; miễn phí=4;
                        if (type == 1) {
                            btyle__ = "Mới";
                        }
                        if (type == 4)
                        {
                            btyle__ = "miễn phí";
                        }
                        if (type == 2)
                        {
                            btyle__ = "chọn lọc";
                        }
                        ViewBag.btyle = btyle__;
                    }
                    catch (Exception)
                    {
                    }

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
                JObject mo = MyControllers.GetLawByCate(Core.TXVIDEO, i_category, i_pageIndex, i_pageSize);
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
        public ActionResult Video_New()
        {
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo = MyControllers.GetLawByCate(Core.TXVIDEO, i_category, i_pageIndex, i_pageSize,"1");
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
        public ActionResult List_by_ID() {
            string pack_adu = "NOT";
            if (chekpack())
            {
                pack_adu = "OK";
            }
            ViewBag.PACKADU = pack_adu;
            ///mVideo/List_by_ID?ccode={R:2}&amp;id_h={R:2}&amp;pg={R:2}
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                LoadDataHomeQuick();
                RequestParam();//i_category, i_pageIndex, i_pageSize
                JObject mo = MyControllers.get_3014(i_pageIndex,i_pageSize,_catecode,_modCode,btype+"",type);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JArray ma = JArray.Parse(mo["LAW"].ToString());
                mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                ViewBag.ma = ma;
                ViewBag.CurrentPage = i_pageIndex + 1;
                ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                ViewBag.i_category = i_category;
                ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                ViewBag.URL_PAGEn2 = "/" + MakeLink.Page_MakeLink(ma[0]["CATE_NAME"].ToString()) + "a" + ma[0]["CATE_CODE"].ToString() + "fun+" + type + ".html";
                ViewBag.URL_PAGEn1 = "/video/trang";
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
            }
            return View();
        }
        public ActionResult Video_Hot()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo = MyControllers.GetLawByCate(Core.TXVIDEO, i_category, i_pageIndex, i_pageSize,"2");
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
                string buy = "";
                string SERVICE_CODE, EXPIRE_DATE;
                int PLUS = -1;
                SERVICE_CODE = EXPIRE_DATE = "";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();

                }
                else {
                    ViewBag.msr = "Thông Báo :Hãy đăng nhập để được xem nhiều hơn .";
                }
                
                JObject mo = MyControllers.GetLawById(Core.A_VIDEO, id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string data = mo.ToString();

                JObject info = new JObject();
                JArray Cart = new JArray();
                JObject res = new JObject();
                JArray ma_other = new JArray();
                info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                Cart = JArray.Parse(mo["CART"].ToString());
                ma_other = JArray.Parse(mo["CATEGORY"].ToString());
                res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                isservice = info["ISSERVICE"].ToString();
                try{
                    if (Cart != null)
                    {
                        buy = Cart[0]["CART_CODE"].ToString();
                    }
                    else {
                        buy = "";
                    }
                }catch(Exception){
                    buy = "";
                }

                if (isservice == "1")
                {
                    JArray ser = JArray.Parse(mo["SERVICE"].ToString());
                    if (ser != null)
                    {
                        foreach (var item in ser)
                        {
                            if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "VD")
                            {
                                SERVICE_CODE = item["SERVICE_CODE"] != null ? item["SERVICE_CODE"].ToString() : "";
                                EXPIRE_DATE = item["EXPIRE_DATE"].ToString();
                                string m = Convert.ToDateTime(EXPIRE_DATE).ToShortDateString().CompareTo(DateTime.Now.ToShortDateString()).ToString();
                                PLUS = Convert.ToInt32(m);
                            }
                        }
                    }
                    if (buy != "" || (SERVICE_CODE == "VD" && PLUS >= 0))
                    {
                        ViewBag.Full = "1";
                        ViewBag.Play = "ok";
                        ViewBag.info = info;
                        ViewBag.res = res;
                        ViewBag.ma_other = ma_other;
                        i_category = info["CATE_CODE"].ToString();
                        ViewBag.ISSERVICE = "AFUN";
                        ViewBag.AUTO_ID = id;
                        JObject mo_comment = MyControllers.GetListComment(id);
                        mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                        JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                        ViewBag.ma_comment = ma_comment;
                        return View();
                    }
                    else
                    {
                        if (buy == "")
                        {
                            if (SERVICE_CODE == "VD" && PLUS < 0)
                            {
                                ViewBag.msr = "Thông Báo.gói video tổng hợp hết hạn. Vui lòng hủy gói Video tổng hợp và đăng ký lại để được sử dụng dịch vụ";
                                Session["mess"] = @"<div class=""mess_error"">Gói Video tổng hợp hết hạn. Vui lòng hủy gói luật tổng hợp và đăng ký lại để được sử dụng dịch vụ.</div>";
                                Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                                return Redirect("/thong-bao.html");
                            }
                            else
                            {
                                ViewBag.msr = "Quý khách chưa đăng ký video tổng hợp. Vui lòng đăng ký gói để được sử dụng dịch vụ";
                                Session["mess"] = @"<div class=""mess_error"">Quý khách chưa đăng ký gói video hợp. Vui lòng đăng ký gói để được sử dụng dịch vụ.</div>";
                                Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                                return Redirect("/thong-bao.html");
                            }//end ser
                        }
                    }
                }
                else
                {
                    ViewBag.info = info;
                    ViewBag.res = res;
                    ViewBag.ma_other = ma_other;
                    i_category = info["CATE_CODE"].ToString();

                    ViewBag.AUTO_ID = id;
                    JObject mo_comment = MyControllers.GetListComment(id,"2");
                    mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                    JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                    ViewBag.ma_comment = ma_comment;
                    if (Session["LoginName"] != null)
                    {

                        //if (PLUS >= 0 ||info["PRICE"].ToString() == "0" || info["PRICE"].ToString() == "")
                        //{
                            ViewBag.Full = "1";
                            ViewBag.ISSERVICE = "AFUN";
                        //}
                        //else {
                        //    ViewBag.Full = null;
                        //}
                    }
                    else
                    {
                        ViewBag.ISSERVICE = null;
                        ViewBag.Full = null;
                    }
                       

                }
                return View();
            }
            catch (Exception)
            {
                Session["mess"] = @"<div class=""mess_error"">Hệ thống đang quá tải. Quý khách vui lòng quay lại sau.</div>";
                Session["re_url"] = "/";
                return Redirect("/thong-bao.html");
            }
        }
     
        public ActionResult Download_2(String id)
        {
            string path = "";
            string folder = "";
            string SERVICE_CODE, EXPIRE_DATE;
            int PLUS = -1;
            SERVICE_CODE = EXPIRE_DATE = "";
            try
            {
                string LoginName = "UNKNOW";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                    JObject mo = MyControllers.GetLawById(Core.TXVIDEO, id, LoginName);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                    JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                    JArray ser = JArray.Parse(mo["SERVICE"].ToString());
                    if (ser != null)
                    {
                        foreach (var item in ser)
                        {
                            if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "LTH")
                            {
                                SERVICE_CODE = item["SERVICE_CODE"] != null ? item["SERVICE_CODE"].ToString() : "";
                                EXPIRE_DATE = item["EXPIRE_DATE"].ToString();
                                string m = Convert.ToDateTime(EXPIRE_DATE).CompareTo(DateTime.Now).ToString();
                                PLUS = Convert.ToInt32(m);
                            }
                        }
                    }
                    if (SERVICE_CODE == "LTH" && PLUS >= 0)
                    {
                        //check trang thai cho doc
                        JObject packge_book = new JObject();
                        JObject packge_acc = new JObject();
                        JArray ma = new JArray();
                        if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                        {
                            folder = res["PPATH"] != null ? res["PPATH"].ToString() : "";
                            res = JObject.Parse(JArray.Parse(res["RPATH"].ToString())[0].ToString());
                            path = folder + res["CPATH"].ToString();
                        }

                        return Redirect(path);
                    }
                    else if (SERVICE_CODE == "TP" && PLUS < 0)
                    {
                        Session["mess"] = @"<div class=""mess_error"">Gói luật tổng hợp hết hạn. Vui lòng hủy gói luật tổng hợp và đăng ký lại để được sử dụng dịch vụ.</div>";
                        Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                        return Redirect("/thong-bao.html");
                    }
                    else
                    {
                        Session["mess"] = @"<div class=""mess_error"">Quý khách chưa đăng ký gói luật tổng hợp. Vui lòng đăng ký gói để được sử dụng dịch vụ.</div>";
                        Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                        return Redirect("/thong-bao.html");
                    }//end ser
                    
                }
                else
                {
                    return Redirect("/dang-nhap.html");
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public ActionResult Download(String id)
        {
            string path = "";
            string folder = "";
            string SERVICE_CODE, EXPIRE_DATE;
            int PLUS = -1;
            SERVICE_CODE = EXPIRE_DATE = "";
            try
            {
                string LoginName = "UNKNOW";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                    JObject mo = MyControllers.GetLawById(Core.TXVIDEO, id, LoginName);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                    JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                    JArray ser = JArray.Parse(mo["SERVICE"].ToString());
                    if (ser != null)
                    {
                        foreach (var item in ser)
                        {
                            if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "LTH")
                            {
                                SERVICE_CODE = item["SERVICE_CODE"] != null ? item["SERVICE_CODE"].ToString() : "";
                                EXPIRE_DATE = item["EXPIRE_DATE"].ToString();
                                string m = Convert.ToDateTime(EXPIRE_DATE).CompareTo(DateTime.Now).ToString();
                                PLUS = Convert.ToInt32(m);
                            }
                        }
                    }
                    if (SERVICE_CODE == "LTH" && PLUS >= 0)
                    {
                        //check trang thai cho doc
                        JObject packge_book = new JObject();
                        JObject packge_acc = new JObject();
                        JArray ma = new JArray();
                        if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                        {
                            folder = res["PPATH"] != null ? res["PPATH"].ToString() : "";
                            res = JObject.Parse(JArray.Parse(res["RPATH"].ToString())[0].ToString());
                            path = folder + res["CPATH"].ToString();
                        }

                        return Redirect(path);
                    }
                    else if (SERVICE_CODE == "TP" && PLUS < 0)
                    {
                        Session["mess"] = @"<div class=""mess_error"">Gói luật tổng hợp hết hạn. Vui lòng hủy gói luật tổng hợp và đăng ký lại để được sử dụng dịch vụ.</div>";
                        Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                        return Redirect("/thong-bao.html");
                    }
                    else
                    {
                        Session["mess"] = @"<div class=""mess_error"">Quý khách chưa đăng ký gói luật tổng hợp. Vui lòng đăng ký gói để được sử dụng dịch vụ.</div>";
                        Session["re_url"] = "/tai-khoan/thong-tin-goi-dich-vu.html";
                        return Redirect("/thong-bao.html");
                    }//end ser

                }
                else
                {
                    return Redirect("/dang-nhap.html");
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment()
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString(); 
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
                JObject mo = MyControllers.AddComment(AUTO_ID, "2", "aFun", HttpUtility.HtmlEncode(commentText), LoginName);
                result = Convert.ToInt32(mo["Header"]["Code"].ToString());
                if (result == 0)
                {
                    Session["re_url"] = urlReffrence;
                    Session["mess"] = @"<div class=""mess_sucess"">Gửi bình luận thành công.</div>";
                    return Redirect(MakeLink.URLMsg());
                }
                else
                {
                    Session["re_url"] = urlReffrence;
                    Session["mess"] = @"<div class=""mess_error"">Gửi bình luận thất bại.</div>";
                    return Redirect(MakeLink.URLMsg());
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
          //  /mVideo/List_by_ID?ccode={R:2}&amp;id_h={R:2}&amp;pg={R:2}
            //video phan trang
            if (Request.QueryString["ccode"]!=null) {
                _catecode = Request.QueryString["ccode"].ToString();
            }
            if (Request.QueryString["type"] != null)
            {
             int.TryParse(Request.QueryString["type"], out type);
            }
            if (Request.QueryString["mcode"] != null)
            {
                _modCode = Request.QueryString["mcode"].ToString();
            }
            //string slipc = Request.QueryString["page"].ToString();
            if (Request.QueryString["page"] != null)
            {
                try
                {
                    string slipc = Request.QueryString["page"].ToString();
                    string[] tokens = slipc.Split('g');

                    if (tokens.Length > 0)
                    {

                        string last = tokens[1].ToString();
                        if (last == "")
                        {
                            last = "1";
                        }
                        int.TryParse(last, out i_pageIndex);
                        i_pageIndex = i_pageIndex - 1;
                        if (i_pageIndex < 0) {
                            i_pageIndex = 0;
                        }
                    }
                    else
                    {

                    }
                }catch(Exception){

                }

            }

        }
    }
}
