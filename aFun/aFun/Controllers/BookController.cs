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
using System.Text;
namespace aFun.Controllers
{
    public class BookController : Controller
    {
        //
        // GET: /Book/
        string i_tab = Common.booknume[0];
        int i_pageIndex = 0, i_pageitem = 0;
        string i_category = "";
        int i_pageSize = Int32.Parse(Iconfig.P_book);
        string _catecode = "ALL", _type = "";
        int totalItem = 0;

        string _modCode = "";
        int btype = 0, type = 0;
        JArray parent_ = null;
        JArray order_ = null;

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
        public ActionResult Index()
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                JObject mo_cate = MyControllers.GetListChildCate(Core.A_BOOK);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());

                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                JObject cate_gory = MyControllers.Get_cate_item("0002","0004","1");//0
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

            }
            catch (Exception e)
            {
                string a = e.ToString();
            }
            return View();
        }
        private void load0002()
        {
            JObject cate_gory = MyControllers.Get_cate_item("0002", "0004", btype + "");//0
            JObject array_al = JObject.Parse(cate_gory["Body"]["Data"].ToString());
            cate_gory = JObject.Parse(cate_gory["Body"]["Data"].ToString());//1
            string b = cate_gory.ToString();
            parent_ = JArray.Parse(cate_gory["PARENT"].ToString());//2ORDER
            order_ = JArray.Parse(cate_gory["ORDER"].ToString());//2ORDER

            ViewBag.MENULEFT = cate_gory["MENU"] == null ? null : JArray.Parse(cate_gory["MENU"].ToString());

        }
        public ActionResult List_by_cate_and_type()
        {

            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {
                JObject lisdata = MyControllers.get_3016(i_pageIndex, i_pageSize, _catecode, btype, type);//0
            }
            catch (Exception)
            {

            }

            try
            {

                RequestParam();
                load0002();


                //if (ma_cate == null || ma_cate.Count <= 1)
                //{
                try
                {
                    JObject mo = MyControllers.get_3016(i_pageIndex, i_pageSize, _catecode, btype, type);//0
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    string abc = mo.ToString();
                    JArray ma = JArray.Parse(mo["BOOK"].ToString());
                    //string abc = mo.ToString();
                    mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                    ViewBag.ma = ma;
                    ViewBag.CurrentPage = i_pageIndex + 1;
                    ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                    string t = mo["total"].ToString();
                    ViewBag._catecode = _catecode;
                    //ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                    //

                    ViewBag._tpage = Iconfig.P_book;
                    string name_ = "";//mới=1; chọn lọc=2; miễn phí=4
                    string nametyle__ = "";// mới=1; chọn lọc=2; miễn phí=4;


                   
                    if (type == 1) { name_ = "moi"; nametyle__ = "Mới"; }
                    if (type == 2) { name_ = "chon-loc"; nametyle__ = "chọn lọc"; }
                    if (type == 4) { name_ = "mien-phi"; nametyle__ = "miễn phí"; }

                    string name_ci = "";
                    if (_catecode != "ALL")
                    {
                        JObject mo_cate = MyControllers.GetListChildCate(_catecode);
                        mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                        JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                        mo_cate = JObject.Parse(ma_cate[0].ToString());
                        ViewBag.CATENAME = mo_cate["TXNAME"] != null ? mo_cate["TXNAME"].ToString() : "";
                        //ViewBag.ma_cate = ma_cate;
                        ViewBag.URL_PAGEn2 = "/" + MakeLink.Page_MakeLink(mo_cate["TXNAME"].ToString() + "-") + name_ + "-a-" + btype + "-v-" + type + "-cate-" + _catecode + ".html";

                    }
                    else
                    {
                        foreach (var p in parent_)
                        {
                            int bd = Int32.Parse(p["KEY"].ToString());
                            if (btype == bd)
                            {
                                name_ci = p["NAME"].ToString();
                                ViewBag.CATENAME = p["NAME"].ToString();
                            }
                        }
                        ViewBag.URL_PAGEn2 = "/" + MakeLink.Page_MakeLink(name_ci + "-") + name_ + "-a-" + btype + "-v-" + type + ".html";

                    }
                    ViewBag.name_tyle = nametyle__;
                    ViewBag.URL_PAGEn1 = "/book/trang";
                    ViewBag.bTYLE = btype;
                    ViewBag.TYLE = type;
                    ViewBag._tpage = i_pageSize;

                }
                catch (Exception)
                {
                }

                //}
                //else
                //{
                //    mo_cate = JObject.Parse(ma_cate[0].ToString());
                //    ViewBag.CATENAME = mo_cate["TXNAME"] != null ? mo_cate["TXNAME"].ToString() : "";
                //    ViewBag.ma_cate = ma_cate;
                //}
            }
            catch (Exception)
            {
            }

            return View();
        }

        public ActionResult Details_text(string id, string m, string bty)
        {
            try
            {
                int like = 1, buy = 0;
                load0002();
                Session["re_url"] = Request.UrlReferrer.ToString();
                string LoginName = "UNKNOW";
                if (Session["LoginName"] != null)
                {

                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName, bty);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string mm = mo.ToString();
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                //JObject res = JObject.Parse(JArray.Parse(mo["BAUTHOR"].ToString())[0].ToString());
                JArray ma_author = new JArray();
                JArray ma_cate = new JArray();


                ma_author = JArray.Parse(mo["AUTHOR"].ToString());

                ma_cate = JArray.Parse(mo["CATEGORY"].ToString());

                ViewBag.AUTO_ID = id;//danh cho com men
                JObject mo_comment = MyControllers.GetListComment(id, "4");
                mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                JArray ma_LIKE = JArray.Parse(mo["LIKE"].ToString());
                //JArray book_bui = JArray.Parse(mo["CART"].ToString());
                if (!string.IsNullOrEmpty(mo["CART"].ToString()) && mo["CART"].ToString() != "[]")
                {
                    JObject packge_book = new JObject();
                    packge_book = JObject.Parse(JArray.Parse(mo["CART"].ToString())[0].ToString());
                    string CART_CODE = packge_book["CART_CODE"] == null ? "" : packge_book["CART_CODE"].ToString();
                    if (CART_CODE == "OWNER")
                    {
                        buy = 1;
                    }

                }
                if (LoginName != "UNKNOW")
                {
                    if (ma_LIKE.Count > 0)
                    {
                        like = 0;
                    }
                }
                ViewBag.info = info;
                ViewBag.ma_author = ma_author;
                ViewBag.ma_cate = ma_cate;
                ViewBag.mtab = m;
                ViewBag.ma_comment = ma_comment;
                ViewBag.bookId = id;
                ViewBag.btyles = bty;
                ViewBag.buy = buy;
                ViewBag.Like = like;
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
                //0:book=4; video th=2; new=3; video luat =1;
                JObject mo = MyControllers.AddComment(AUTO_ID, "4", "aFun", HttpUtility.HtmlEncode(commentText), LoginName);
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

        public ActionResult Read(string id)
        {
            try
            {
          
                Session["BOOK_DUM"] = id;
                string url_pathbook = "";
                string urlFolder = "";
                string filename = "";
                string TXNAME = "";
                string LoginName = "0";
                string de_content = "";
                string de_key = "";
                string PRICE = "0";
                string ISHOT = "";
                JObject mlogstream = new JObject();
                JObject mulog = new JObject();
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string m0o = mo.ToString();
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                JObject blink = JObject.Parse(JArray.Parse(mo["LINK"].ToString())[0].ToString());
                //check trang thai cho doc
                JObject packge_book = new JObject();
                JObject packge_acc = new JObject();
                JArray ma = new JArray();
                int PLUS = -1;
                string CART_CODE, SERVICE_CODE, REMAIN;
                CART_CODE = SERVICE_CODE = REMAIN = "";
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    ma = JArray.Parse(res["RPATH"].ToString());
                }
                if (!string.IsNullOrEmpty(mo["CART"].ToString()) && mo["CART"].ToString() != "[]")
                {
                    packge_book = JObject.Parse(JArray.Parse(mo["CART"].ToString())[0].ToString());
                    CART_CODE = packge_book["CART_CODE"] == null ? "" : packge_book["CART_CODE"].ToString();
                    REMAIN = packge_book["REMAIN"] == null ? "" : packge_book["REMAIN"].ToString();
                }

                if (!string.IsNullOrEmpty(mo["SERVICE"].ToString()) && mo["SERVICE"].ToString() != "[]")
                {
                    packge_acc = JObject.Parse(JArray.Parse(mo["SERVICE"].ToString())[0].ToString());
                    SERVICE_CODE = packge_acc["SERVICE_CODE"] == null ? "" : packge_acc["SERVICE_CODE"].ToString();
                    PLUS = packge_acc["PLUS"] == null ? -1 : Convert.ToInt32(packge_acc["PLUS"].ToString());
                }

                ViewBag.info = info;
                ViewBag.ma = ma;
                i_pageitem = 0;
                if (Request.QueryString["p"] == null)
                {
                    if (!string.IsNullOrEmpty(REMAIN) && REMAIN.Contains("#") && REMAIN.StartsWith("#") == false)
                    {
                        string[] words = REMAIN.Split('#');
                        i_pageIndex = Convert.ToInt32(words[0].ToString());
                        i_pageitem = Convert.ToInt32(words[1].ToString());
                    }
                }
                else
                {
                    RequestParam();
                }
                if (info != null)
                {
                    if (res != null && ma.Count > 0)
                    {
                       // Session["BOOK_DUM"] = id;
                        urlFolder = blink["ENCYP_PATH"].ToString();
                        de_key = blink["ENCYP_KEY"].ToString();
                        filename = ma[i_pageIndex]["CPATH"].ToString();
                        ViewBag.namechapter = ma[i_pageIndex]["CNAME"].ToString();
                        TXNAME = info["TXNAME"].ToString();
                        PRICE = info["PRICE"].ToString();
                        ISHOT = "0";
                        url_pathbook = urlFolder + "/" + filename;
                        if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
                        {
                            url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkLocalserver"].ToString());
                        }
              

                        string contents = Core.CallService2(url_pathbook);
                        de_content = AESLib.decryptAES(contents, de_key);
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(de_content);
                        HtmlDocument docnew = new HtmlDocument();

                        HtmlNodeCollection nodeColection = doc.DocumentNode.SelectNodes("//span|//i|//br");
                        int countPerpart = 20;
                        if (nodeColection != null)
                        {
                            int tong = nodeColection.Count;
                            int du = tong % countPerpart;
                            totalItem = du == 0 ? (tong / countPerpart) : (tong / countPerpart) + 1;
                            if (i_pageitem > totalItem)
                            {
                                i_pageitem = 0;
                            }
                           

                            if (PRICE == "0" || (!string.IsNullOrEmpty(packge_book.ToString()) && (CART_CODE == "OWNER" || CART_CODE == "READING")))
                            {
                                ViewBag.totalPage = ma.Count;
                                ViewBag.i_pageIndex = i_pageIndex ;
                                if (CART_CODE == "OWNER")
                                {
                                    ViewBag.readower = 1;
                                }
                                else if (CART_CODE == "READING")
                                {
                                    ViewBag.readower = 0;
                                }
                                else
                                {
                                    ViewBag.readower = 0;
                                }
                            }
                            else if (CART_CODE != "OWNER" && CART_CODE != "READING" && !string.IsNullOrEmpty(packge_acc.ToString()) && (SERVICE_CODE == "EB") && PLUS >= 0 && ISHOT != "2")
                            {
                                //đánh dấu trang
                                int createStatus = -1;
                                string remain, pExt;
                                remain = pExt = "";
                                remain = "0#3";
                                pExt = "0#3";
                                JObject mu = MyControllers.Markbook(id, LoginName, remain, "0", "0", pExt);
                                createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                                ViewBag.totalPage = ma.Count;
                                ViewBag.i_pageIndex = i_pageIndex;
                            }
                            else
                            {
                                ViewBag.totalPage = 0;
                                ViewBag.i_pageIndex = 0;
                                ViewBag.Demo = "1";
                            }

                            if (i_pageIndex == 0)
                            {
                                if (ViewBag.Demo == "0")
                                {
                                    mulog = MyControllers.Log_ReadDemo(id, LoginName);
                                }
                                else
                                {
                                    mulog = MyControllers.Log_ReadFull(id, LoginName);
                                }
                            }
                            ViewBag.totalItem = totalItem;
                            ViewBag.i_pageitem = i_pageitem;
                            ViewBag.nodeColection = nodeColection;

                        }
                        else
                        {
                            ViewBag.nodeColection = null;
                            ViewBag.i_pageIndex = 0;
                            ViewBag.i_pageitem = 0;
                            ViewBag.totalItem = 0;
                            ViewBag.totalPage = 0;
                            if (i_pageIndex == 0)
                            {
                                mulog = MyControllers.Log_ReadDemo(id, LoginName);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
            return View();
        }


        public ActionResult Read_audio(string id)
        {
            try
            {
                int buy = 0;
                load0002();
                JObject mulog = new JObject();
                ViewBag.SLIDESHOW = null;
                string LoginName = "0"; 
                Session["re_url"] = Request.UrlReferrer.ToString();
                if (Session["LoginName"] != null)
                {
                    
                    LoginName = Session["LoginName"].ToString();
                }
                else
                {
                    ViewBag.msr = "Hãy đăng nhập để xem Tiếp";
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string modd = mo.ToString();
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                JObject blink = JObject.Parse(JArray.Parse(mo["LINK"].ToString())[0].ToString());
                //check trang thai cho doc
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

                    packge_book = JObject.Parse(JArray.Parse(mo["CART"].ToString())[0].ToString());
                    CART_CODE = packge_book["CART_CODE"] == null ? "" : packge_book["CART_CODE"].ToString();
                    REMAIN = packge_book["REMAIN"] == null ? "" : packge_book["REMAIN"].ToString();
                    buy = 1;
                }

                if (!string.IsNullOrEmpty(mo["SERVICE"].ToString()) && mo["SERVICE"].ToString() != "[]")
                {
                    packge_acc = JObject.Parse(JArray.Parse(mo["SERVICE"].ToString())[0].ToString());
                    SERVICE_CODE = packge_acc["SERVICE_CODE"] == null ? "" : packge_acc["SERVICE_CODE"].ToString();
                    PLUS = packge_acc["PLUS"] == null ? -1 : Convert.ToInt32(packge_acc["PLUS"].ToString());
                }
                if (!string.IsNullOrEmpty(packge_book.ToString()) && (CART_CODE == "OWNER" || CART_CODE == "READING"))
                {
                    ViewBag.Full = "1";
                }
                else if (CART_CODE != "OWNER" && CART_CODE != "READING" && !string.IsNullOrEmpty(packge_acc.ToString()) &&  SERVICE_CODE == "EB" && PLUS >= 0 && ISHOT != "2")
                {
                    //đánh dấu trang
                    int createStatus = -1;
                    string remain = "";
                    string time = "";
                    remain = "0#0";
                    time = "0#0";


                    JObject mu = MyControllers.Markbook(id, LoginName, remain, "0", "0", time);
                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                    ViewBag.Full = "1";
                }
                else
                {
                    ViewBag.totalPage = 0;
                    ViewBag.i_pageIndex = 0;
                    ViewBag.Full = null;
                    if (!string.IsNullOrEmpty(packge_acc.ToString()) && (SERVICE_CODE == "EB") && PLUS < 0)
                    {
                        ViewBag.Packge = SERVICE_CODE;
                    }
                }
                if (ViewBag.Full == "1")
                {
                    mulog = MyControllers.Log_ReadFull(id, LoginName);

                }
                else
                {
                    mulog = MyControllers.Log_ReadDemo(id, LoginName);
                }
                JArray ma_author = new JArray();
                JArray ma_cate = new JArray();
                ma_author = JArray.Parse(mo["AUTHOR"].ToString());
                ma_cate = JArray.Parse(mo["CATEGORY"].ToString());

                JObject mo_comment = MyControllers.GetListComment(id);
                mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());

                ViewBag.info = info;
                ViewBag.Linkres = (blink["ENCYP_PATH"].ToString()).Replace("ENCRYP/", "").Trim();
                ViewBag.ma = ma;
                ViewBag.ma_author = ma_author;
                ViewBag.ma_cate = ma_cate;
                ViewBag.ma_comment = ma_comment;
                ViewBag.bookId = id;
                ViewBag.buy = buy;

            }
            catch (Exception)
            {
                ViewBag.Full = null;
            }
            return View();
        }

        public ActionResult Read_video_book(string id)
        {

            try
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                string video_IS = "not";
                load0002();
                JObject mulog = new JObject();
                ViewBag.SLIDESHOW = null;
                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string mmmm = mo.ToString();
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                JObject blink = JObject.Parse(JArray.Parse(mo["LINK"].ToString())[0].ToString());
                //check trang thai cho doc
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
                if (!string.IsNullOrEmpty(packge_book.ToString()) && (CART_CODE == "OWNER" || CART_CODE == "READING"))
                {
                    ViewBag.Full = "1";
                    video_IS = "AFUN";
                }
                else if (CART_CODE != "OWNER" && CART_CODE != "READING" && !string.IsNullOrEmpty(packge_acc.ToString()) && SERVICE_CODE == "EB" && PLUS >= 0 && ISHOT != "2")
                {
                    //đánh dấu trang
                    int createStatus = -1;
                    string remain = "";
                    string time = "";
                    remain = "0#0";
                    time = "0#0";
                  
                    JObject mu = MyControllers.Markbook(id, LoginName, remain, "0", "0", time);
                    createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                    ViewBag.Full = "1";
                    video_IS = "AFUN";
                }
                else
                {
                    ViewBag.totalPage = 0;
                    ViewBag.i_pageIndex = 0;
                    ViewBag.Full = null;
                    if (!string.IsNullOrEmpty(packge_acc.ToString()) && (SERVICE_CODE == "EB") && PLUS < 0)
                    {
                        ViewBag.Packge = SERVICE_CODE;
                       
                    }
                    ViewBag.ISSERVICE = video_IS;
                }
                if (ViewBag.Full == "1")
                {
                    mulog = MyControllers.Log_ReadFull(id, LoginName);

                }
                else
                {
                    mulog = MyControllers.Log_ReadDemo(id, LoginName);
                }
                JArray ma_author = new JArray();
                JArray ma_cate = new JArray();
                ma_author = JArray.Parse(mo["AUTHOR"].ToString());
                ma_cate = JArray.Parse(mo["CATEGORY"].ToString());

                JObject mo_comment = MyControllers.GetListComment(id);
                mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                ViewBag.ISSERVICE = video_IS;
                ViewBag.info = info;
                ViewBag.Linkres = (blink["ENCYP_PATH"].ToString()).Replace("ENCRYP/", "").Trim();
                ViewBag.ma = ma;
                ViewBag.ma_author = ma_author;
                ViewBag.ma_cate = ma_cate;
                ViewBag.ma_comment = ma_comment;
                Session["BOOK_DUM"] = id;
                ViewBag.bookId = id;


            }
            catch (Exception)
            {
                ViewBag.Full = null;
                ViewBag.ISSERVICE = "";

            }
            return View();
        }

        public ActionResult GetContentPart(string id, int part, int currentChapter)
        {
            i_pageIndex = currentChapter;
            i_pageitem = part;
            StringBuilder sb = new StringBuilder();
            try
            {
                string url_pathbook = "";
                string urlFolder = "";
                string filename = "";
                string LoginName = "0";
                string de_content = "";
                string de_key = "";
                if (Session["LoginName"] != null)
                {
                    Session["re_url"] = Request.UrlReferrer.ToString();
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string mmmm = mo.ToString();
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                JObject blink = JObject.Parse(JArray.Parse(mo["LINK"].ToString())[0].ToString());
                //check trang thai cho doc
                JObject packge_book = new JObject();
                JObject packge_acc = new JObject();
                JArray ma = new JArray();
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    ma = JArray.Parse(res["RPATH"].ToString());
                }
                if (info != null)
                {
                    if (res != null && ma.Count > 0)
                    {
                        urlFolder = blink["ENCYP_PATH"].ToString();
                        de_key = blink["ENCYP_KEY"].ToString();
                        filename = ma[i_pageIndex]["CPATH"].ToString();
                        url_pathbook = urlFolder + "/" + filename;
                        if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
                        {
                            url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkLocalserver"].ToString());
                        }
                        string contents = Core.CallService2(url_pathbook);
                        de_content = AESLib.decryptAES(contents, de_key);
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(de_content);
                        HtmlDocument docnew = new HtmlDocument();
                        HtmlNodeCollection nodeColection = doc.DocumentNode.SelectNodes("//span|//i|//br");
                        int countPerpart = 20;
                        if (nodeColection != null)
                        {
                            int tong = nodeColection.Count;
                            int du = tong % countPerpart;
                            totalItem = du == 0 ? (tong / countPerpart) : (tong / countPerpart) + 1;
                            ViewBag.totalPage = ma.Count;
                            ViewBag.i_pageIndex = i_pageIndex + 1;
                            if (part < totalItem)
                            {
                                int tongduyet = du == 0 ? countPerpart * totalItem : tong;
                                int mixduyet = part * countPerpart;
                                int maxduyet = part == (totalItem - 1) ? (du == 0 ? (part + 1) * countPerpart : tong) : (part + 1) * countPerpart;
                                for (int i = mixduyet; i < maxduyet; i++)
                                {
                                    sb.Append(nodeColection[i].OuterHtml);
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
            Response.Write(sb.ToString());
            return null;
        }
        public ActionResult GetTotalPart(string id, int currentChapter)
        {
            i_pageIndex = currentChapter;
            StringBuilder sb = new StringBuilder();
            try
            {
                string url_pathbook = "";
                string urlFolder = "";
                string filename = "";
                string TXNAME = "";
                string LoginName = "0";
                string de_content = "";
                string de_key = "";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetBookById(id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["BOOK"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                JObject blink = JObject.Parse(JArray.Parse(mo["LINK"].ToString())[0].ToString());
                //check trang thai cho doc
                JObject packge_book = new JObject();
                JObject packge_acc = new JObject();
                JArray ma = new JArray();
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    ma = JArray.Parse(res["RPATH"].ToString());
                }
                if (info != null)
                {
                    if (res != null && ma.Count > 0)
                    {
                        urlFolder = blink["ENCYP_PATH"].ToString();
                        de_key = blink["ENCYP_KEY"].ToString();
                        filename = ma[i_pageIndex]["CPATH"].ToString();
                        ViewBag.namechapter = ma[i_pageIndex]["CNAME"].ToString();
                        TXNAME = info["TXNAME"].ToString();
                        url_pathbook = urlFolder + "/" + filename;
                        if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
                        {
                            url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkLocalserver"].ToString());
                        }
                        string contents = Core.CallService2(url_pathbook);
                        de_content = AESLib.decryptAES(contents, de_key);

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(de_content);
                        HtmlDocument docnew = new HtmlDocument();
                        HtmlNodeCollection nodeColection = doc.DocumentNode.SelectNodes("//span|//i|//br");
                        int countPerpart = 20;
                        if (nodeColection != null)
                        {
                            int tong = nodeColection.Count;
                            int du = tong % countPerpart;
                            totalItem = du == 0 ? (tong / countPerpart) : (tong / countPerpart) + 1;
                        }
                        else
                        {
                            totalItem = 0;
                        }
                    }

                }
            }
            catch (Exception)
            {
            }

            Response.Write(totalItem.ToString());
            return null;
        }
        public ActionResult Updatemarkbook(string bookId, string p, string page, string ower, string time)
        {

            try
            {
                if (string.IsNullOrEmpty(p)) p = "1";
                if (string.IsNullOrEmpty(page)) page = "3";

                string LoginName = "0";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                int createStatus = -1;
                if (string.IsNullOrEmpty(ower)) ower = "1";
                string remain = "";
                p = (Convert.ToInt32(p) - 1).ToString();

                remain = p + "#" + page;
                JObject mu = MyControllers.Markbook(bookId, LoginName, remain, ower, "0", time);
                createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                if (createStatus == 0)
                {
                    return Json(new { success = 1 });

                }
                else
                {
                    return Json(new { success = 0 });
                }
            }
            catch (Exception)
            {
                return Json(new { success = 0 });
            }

        }



        #region GetContentPart
        //public ActionResult GetContentPart(string id, int part, int currentChapter)
        //{
        //    i_pageIndex = currentChapter;
        //    i_pageitem = part;
        //    StringBuilder sb = new StringBuilder();
        //    try
        //    {
        //        string url_pathbook = "";
        //        string urlFolder = "";
        //        string filename = "";
        //        string LoginName = "0";
        //        string de_content = "";
        //        string de_key = "";
        //        if (Session["LoginName"] != null)
        //        {
        //            LoginName = Session["LoginName"].ToString();
        //        }
        //        JObject mo = MyControllers.GetBookById(id, LoginName);
        //        mo = JObject.Parse(mo["Body"]["Data"].ToString());
        //        JObject info = JObject.Parse(JArray.Parse(mo["BDETAIL"].ToString())[0].ToString());
        //        JObject res = JObject.Parse(JArray.Parse(mo["BRESOURCE"].ToString())[0].ToString());
        //        JObject blink = JObject.Parse(JArray.Parse(mo["BLINK"].ToString())[0].ToString());
        //        //check trang thai cho doc
        //        JObject packge_book = new JObject();
        //        JObject packge_acc = new JObject();
        //        JArray ma = new JArray();
        //        if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
        //        {
        //            ma = JArray.Parse(res["RPATH"].ToString());
        //        }
        //        if (info != null)
        //        {
        //            if (res != null && ma.Count > 0)
        //            {
        //                urlFolder = blink["ENCYP_PATH"].ToString();
        //                de_key = blink["ENCYP_KEY"].ToString();
        //                filename = ma[i_pageIndex]["CPATH"].ToString();
        //                url_pathbook = urlFolder + "/" + filename;
        //                if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
        //                {
        //                    url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkLocalserver"].ToString());
        //                }
        //                string contents = Core.CallService2(url_pathbook);
        //                de_content = AESLib.decryptAES(contents, de_key);
        //                HtmlDocument doc = new HtmlDocument();
        //                doc.LoadHtml(de_content);
        //                HtmlDocument docnew = new HtmlDocument();
        //                HtmlNodeCollection nodeColection = doc.DocumentNode.SelectNodes("//span|//i|//br");
        //                int countPerpart = 20;
        //                if (nodeColection != null)
        //                {
        //                    int tong = nodeColection.Count;
        //                    int du = tong % countPerpart;
        //                    totalItem = du == 0 ? (tong / countPerpart) : (tong / countPerpart) + 1;
        //                    ViewBag.totalPage = ma.Count;
        //                    ViewBag.i_pageIndex = i_pageIndex + 1;
        //                    if (part < totalItem)
        //                    {
        //                        int tongduyet = du == 0 ? countPerpart * totalItem : tong;
        //                        int mixduyet = part * countPerpart;
        //                        int maxduyet = part == (totalItem - 1) ? (du == 0 ? (part + 1) * countPerpart : tong) : (part + 1) * countPerpart;
        //                        for (int i = mixduyet; i < maxduyet; i++)
        //                        {
        //                            sb.Append(nodeColection[i].OuterHtml);
        //                        }

        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    Response.Write(sb.ToString());
        //    return null;
        //}
        //public ActionResult GetTotalPart(string id, int currentChapter)
        //{
        //    i_pageIndex = currentChapter;
        //    StringBuilder sb = new StringBuilder();
        //    try
        //    {
        //        string url_pathbook = "";
        //        string urlFolder = "";
        //        string filename = "";
        //        string TXNAME = "";
        //        string LoginName = "0";
        //        string de_content = "";
        //        string de_key = "";
        //        if (Session["LoginName"] != null)
        //        {
        //            LoginName = Session["LoginName"].ToString();
        //        }
        //        JObject mo = MyControllers.GetBookById(id, LoginName);
        //        mo = JObject.Parse(mo["Body"]["Data"].ToString());
        //        JObject info = JObject.Parse(JArray.Parse(mo["BDETAIL"].ToString())[0].ToString());
        //        JObject res = JObject.Parse(JArray.Parse(mo["BRESOURCE"].ToString())[0].ToString());
        //        JObject blink = JObject.Parse(JArray.Parse(mo["BLINK"].ToString())[0].ToString());
        //        //check trang thai cho doc
        //        JObject packge_book = new JObject();
        //        JObject packge_acc = new JObject();
        //        JArray ma = new JArray();
        //        if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
        //        {
        //            ma = JArray.Parse(res["RPATH"].ToString());
        //        }
        //        if (info != null)
        //        {
        //            if (res != null && ma.Count > 0)
        //            {
        //                urlFolder = blink["ENCYP_PATH"].ToString();
        //                de_key = blink["ENCYP_KEY"].ToString();
        //                filename = ma[i_pageIndex]["CPATH"].ToString();
        //                ViewBag.namechapter = ma[i_pageIndex]["CNAME"].ToString();
        //                TXNAME = info["TXNAME"].ToString();
        //                url_pathbook = urlFolder + "/" + filename;
        //                if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
        //                {
        //                    url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkLocalserver"].ToString());
        //                }
        //                string contents = Core.CallService2(url_pathbook);
        //                de_content = AESLib.decryptAES(contents, de_key);

        //                HtmlDocument doc = new HtmlDocument();
        //                doc.LoadHtml(de_content);
        //                HtmlDocument docnew = new HtmlDocument();
        //                HtmlNodeCollection nodeColection = doc.DocumentNode.SelectNodes("//span|//i|//br");
        //                int countPerpart = 20;
        //                if (nodeColection != null)
        //                {
        //                    int tong = nodeColection.Count;
        //                    int du = tong % countPerpart;
        //                    totalItem = du == 0 ? (tong / countPerpart) : (tong / countPerpart) + 1;
        //                }
        //                else
        //                {
        //                    totalItem = 0;
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    Response.Write(totalItem.ToString());
        //    return null;
        //}
        //public ActionResult Updatemarkbook(string bookId, string p, string page, string ower)
        //{

        //    try
        //    {
        //        if (string.IsNullOrEmpty(p)) p = "1";
        //        if (string.IsNullOrEmpty(page)) page = "3";

        //        string LoginName = "0";
        //        if (Session["LoginName"] != null)
        //        {
        //            LoginName = Session["LoginName"].ToString();
        //        }
        //        int createStatus = -1;
        //        if (string.IsNullOrEmpty(ower)) ower = "0";
        //        string remain = "";
        //        p = (Convert.ToInt32(p) - 1).ToString();
        //        remain = p + "#" + page;
        //        JObject mu = MyControllers.Markbook(bookId, LoginName, remain, ower, "0");
        //        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
        //        if (createStatus == 0)
        //        {
        //            return Json(new { success = 1 });

        //        }
        //        else
        //        {
        //            return Json(new { success = 0 });
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = 0 });
        //    }

        //}
        #endregion

        public int AddStar(string bookId, string star)
        {
            if (Session["LoginName"] == null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                Response.Redirect("/dang-nhap.html");

                return 0;
            }
            else
            {
                string LoginName = Session["LoginName"].ToString();
                int createStatus = -1;
                if (string.IsNullOrEmpty(star)) star = "5";
                int rate = Convert.ToInt32(star);
                //if (rate < 5)
                //{
                //    rate++;
                //}
                JObject mo = MyControllers.UpdateStar(bookId, rate.ToString(), LoginName);
                createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                return createStatus;
            }
        }
        public string addlikebook(string bookId, string isFa)
        {
            string LoginName = "0";
            if (Session["LoginName"] != null)
            {
                Session["re_url"] = Request.UrlReferrer.ToString();
                LoginName = Session["LoginName"].ToString();
            }
            if (string.IsNullOrEmpty(isFa)) isFa = "1";
            int createStatus = -1;
            JObject mu = MyControllers.AddBookLike(bookId, LoginName, isFa);
            createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
            if (createStatus == 0)
            {
                return "1";
            }
            else if (createStatus == 2)
            {
                return "2";
            }
            else
            {
                return "-1";
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
            if (Request.QueryString["ccode"] != null)
            {
                _catecode = Request.QueryString["ccode"].ToString();
            }
            if (Request.QueryString["type"] != null)
            {
                int.TryParse(Request.QueryString["type"], out type);
            }
            if (Request.QueryString["btype"] != null)
            {
                int.TryParse(Request.QueryString["btype"], out btype);
            }

            if (Request.QueryString["mcode"] != null)
            {
                _modCode = Request.QueryString["mcode"].ToString();
            }
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
                        if (i_pageIndex < 0)
                        {
                            i_pageIndex = 0;
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception)
                {

                }

            }

        }
    }
}
