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
using log4net;
using System.IO;

namespace aFun.Controllers
{
    public class LawController : Controller
    {
        private ILog logger = log4net.LogManager.GetLogger(typeof(LawController));
        // GET: /Book/
        string i_tab = Common.booknume[0];
        string btype = Common.catetype[0];
        int i_pageIndex = 0;
        string i_go_c1 = "";
        int i_pagesize = Convert.ToInt32(Iconfig.P_law.ToString());//int.TryParse(Iconfig.P_law.ToString());
        int i_pageitem = 0,type=1;
        string i_category = "ALL";
        int totalItem = 3;
        int totalPage = 0;
        string m_cate = "ALL";
        string _catecode = "ALL";
        string _modecode="";
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
            //ViewBag.SearchLaw = "law";
            try
            {
                JObject mo_cate = MyControllers.GetListChildCate(Core.A_LUAT);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());

                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());

                JObject cate_gory = MyControllers.Get_cate_item("0002", "0003", "7");//0
                JObject video = MyControllers.Get_cate_item("0002", "0003", "6");//0
                JObject news= MyControllers.Get_cate_item("0002", "0003", "5");//0
                JObject array_al = JObject.Parse(cate_gory["Body"]["Data"].ToString());//0
                JObject array_al_video = JObject.Parse(video["Body"]["Data"].ToString());//0
                video = JObject.Parse(video["Body"]["Data"].ToString());
                cate_gory = JObject.Parse(cate_gory["Body"]["Data"].ToString());//1
                news = JObject.Parse(news["Body"]["Data"].ToString());//1


                string b = cate_gory.ToString();
                string c = news.ToString();
                string d = video.ToString();


                JArray ma_ = JArray.Parse(cate_gory["CATEGORY"].ToString());//2
                JArray parent = JArray.Parse(video["PARENT"].ToString());//2ORDER
                JArray order = JArray.Parse(video["ORDER"].ToString());//2ORDER 

                ///???  lấy cho tin tức 
                JArray ma_news = JArray.Parse(news["CATEGORY"].ToString());//2 // cate cho tin (* tin ong nươc * QT...)

                ViewBag.ma_cate = ma_cate;
                string ms = parent.ToString();
                string msd = ma_.ToString();
                string m = mo_cate.ToString();
                ViewBag.MENULEFT = cate_gory["MENU"] == null ? null : JArray.Parse(cate_gory["MENU"].ToString());
                ViewBag.cate_gory = ma_;//
                ViewBag.item_cate = parent;///
                ViewBag.list_oder = order;///
                ViewBag.list_arr = array_al;///
                ViewBag.ma_news_ = ma_news;///
                ViewBag.ma_arr_video = array_al_video;///

            }
            catch (Exception)
            {
            }
            return View();

        }
        //văn bản Luat
        public ActionResult Item_cate() {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            //ViewBag.SearchLaw = "law";
            RequestParam();
            try
            {
                //JObject mo_cate = MyControllers.GetListChildCate(Core.A_LUAT);
                //mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                //JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());

                JObject cate_gory = MyControllers.Get_cate_item_0002("0003", "7",i_go_c1);//0
                //JObject news = MyControllers.Get_cate_item("0002", "0003", "5");//0
                JObject array_al = JObject.Parse(cate_gory["Body"]["Data"].ToString());
                cate_gory = JObject.Parse(cate_gory["Body"]["Data"].ToString());//1

                string bdd = cate_gory.ToString(); 

                JArray ma_ = JArray.Parse(cate_gory["CATEGORY"].ToString());//2
                JArray parent = JArray.Parse(cate_gory["PARENT"].ToString());//2ORDER
                JArray order = JArray.Parse(cate_gory["ORDER"].ToString());//2ORDER
                if (cate_gory["CATEGORY"] != null)
                {

                  //  ViewBag.ma_cate = ma_cate;
                    string ms = parent.ToString();
                    string msd = ma_.ToString();
                  //  string m = mo_cate.ToString();
                    ViewBag.MENULEFT = cate_gory["MENU"] == null ? null : JArray.Parse(cate_gory["MENU"].ToString());
                    ViewBag.cate_gory = ma_;//
                    ViewBag.item_cate = parent;///
                    ViewBag.list_oder = order;///
                    ViewBag.list_arr = array_al;///
                }
                    //dq  khong co thi lis phan trang type: mới=1; chọn lọc=2; miễn phí=4; cateCode=Thể loại; modCode=cType ( Video='0002'; LAW= '0003'; BOOK='0004')
                else {
                    
                }
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult List_by_item(){

         //   <action type="Rewrite" url="/Law/List_by_item?ccode={R:3}&amp;pg={R:4}" />
      
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try {//{'page':'0','pageSize':'10','cateCode':'00','modCode':'0003','bType':'7','type':'1'}
                //{'page':'0','pageSize':'10','cateCode':'0003','modCode':'','bType':'0004','type':'7'} "0003", _catecode, "0004", 7
                JObject lisdata = MyControllers.get_3014(i_pageIndex, i_pagesize,_catecode,"0003","7",1);//0
              
            }catch(Exception){
            }
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo_cate = MyControllers.GetListChildCate(_catecode);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                string mmd = mo_cate.ToString();
                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                if (ma_cate == null || ma_cate.Count <= 1)
                {
                    //van-ban/trang3/hinh-su-list-0034.html
                    try
                    {
                        
                        JObject mo = MyControllers.get_3014(i_pageIndex, i_pagesize, _catecode, "0003", "7", type);//0
                       
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
                       
                       
                        ViewBag.URL_PAGEn2 = "/" +MakeLink.Page_MakeLink(ma[0]["CATE_NAME"].ToString()) + "-list-" + ma[0]["CATE_CODE"].ToString()+".html";
                        ViewBag.URL_PAGEn1 = "/van-ban/trang";

                        ViewBag._tpage = Iconfig.P_law;
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

        public ActionResult List_video_by_item()
        {

            //   <action type="Rewrite" url="/Law/List_by_item?ccode={R:3}&amp;pg={R:4}" />

            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            try
            {//{'page':'0','pageSize':'10','cateCode':'00','modCode':'0003','bType':'7','type':'1'}
                //{'page':'0','pageSize':'10','cateCode':'0003','modCode':'','bType':'0004','type':'7'} "0003", _catecode, "0004", 7
                JObject lisdata = MyControllers.get_3014(i_pageIndex, i_pagesize, _catecode, "0002", "7", type);//0

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
                        JObject mo = MyControllers.get_3014(i_pageIndex, i_pagesize, _catecode, "0003", "6", type);//0
                        mo = JObject.Parse(mo["Body"]["Data"].ToString());
                        JArray ma = JArray.Parse(mo["LAW"].ToString());
                        string abc = mo.ToString();
                        mo = JObject.Parse(JArray.Parse(mo["RESULT"].ToString())[0].ToString());
                        JObject catevideo = MyControllers.Get_cate_item("0002", "0003", "6");//0
                        catevideo = JObject.Parse(catevideo["Body"]["Data"].ToString());
                        JArray cate_gory = JArray.Parse(catevideo["CATEGORY"].ToString());

                        ViewBag.ma = ma;
                        ViewBag.video_cate = cate_gory;
                        ViewBag.CurrentPage = i_pageIndex + 1;
                        ViewBag.TotalRecord = Convert.ToInt32(mo["total"].ToString());
                        string t = mo["total"].ToString();
                        ViewBag._catecode = _catecode;
                       
                        if (_catecode != "ALL")
                        {
                            ViewBag.CATENAME = ma[0]["CATE_NAME"].ToString();
                        }
                        else
                        {
                            if (type == 1) { ViewBag.CATENAME = "VIDEO MỚI CẬP NHẬT"; }
                            if (type == 2) { ViewBag.CATENAME = "VIDEO CHỌN LỌC"; }
                            if (type == 4) { ViewBag.CATENAME = "VIDEO MIỄN PHÍ"; }
                        }
                        //
                        ViewBag._tpage = Iconfig.P_video;
                        ViewBag.URL_PAGEn2 = "/" + MakeLink.Page_MakeLink("Video") + "-" + m_cate + "-" + type + ".html";
                        ViewBag.URL_PAGEn1 = "/phap-luat/trang";
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
            ViewBag.SearchLaw = "law";
            try
            {
                LoadDataHomeQuick();
                RequestParam();
                JObject mo_cate = MyControllers.GetListChildCate(i_category);
                mo_cate = JObject.Parse(mo_cate["Body"]["Data"].ToString());
                JArray ma_cate = JArray.Parse(mo_cate["CATEGORY"].ToString());
                if (ma_cate == null || ma_cate.Count <= 1)
                {
                    try
                    {
                        JObject mo = MyControllers.GetLawByCate(Core.TXLAW, i_category, i_pageIndex, i_pagesize);
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
        public ActionResult Details(string id)
        {
            if (Session["msisdn"] == null && Session["LoginName"] == null)
            {
                CheckDevice.login3g();
            }
            ViewBag.SearchLaw = "law";
            try
            {
                LoadDataHomeQuick();
                string LoginName = "UNKNOW";
                string urlFolder = "";
                if (Session["LoginName"] != null)
                {
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetLawById("0003", id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                mo = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                string msd = mo.ToString();
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    urlFolder = res["SRC_PATH"].ToString();
                }

                ViewBag.mo = mo;
                i_category = mo["CATE_CODE"].ToString();
                ViewBag.urlFolder = urlFolder;
                JObject mo_other = MyControllers.GetLawByCate(Core.TXLAW, i_category, 0, 10);
                mo_other = JObject.Parse(mo_other["Body"]["Data"].ToString());
                JArray ma_other = JArray.Parse(mo_other["LAW"].ToString());
                ViewBag.ma_other = ma_other;
            }
            catch (Exception)
            {
            }
            return View();
        }
        public ActionResult Read(string id)
        {
            if (Session["LoginName"] == null)
            {
                string Url = Server.UrlEncode(Request.RawUrl.ToString());
                Session["re_url"] = Request.RawUrl.ToString();
                return Redirect("/dang-nhap.html/red"+Url);
            }
            else
            {
                try
                {
                    //string useragent = Request.UserAgent;
                    //logger.Info(" log useragent:" + useragent);
                    //if (!aFun.Models.CheckDevice.IsMobileDevice(useragent))
                    //{
                    //    Response.Redirect("http://aFun.vn" + Request.Url.AbsolutePath);
                    //}

                    string url_pathbook = "";
                    string urlFolder = "";
                    string filename = "";
                    string TXNAME = "";
                    string LoginName = "0";
                    string de_content = "";
                    string de_key = "";
                    string SERVICE_CODE, EXPIRE_DATE;
                    int PLUS = -1;
                    SERVICE_CODE = EXPIRE_DATE = "";

                    JObject mlogstream = new JObject();
                    JObject mulog = new JObject();
                    if (Session["LoginName"] != null)
                    {
                        LoginName = Session["LoginName"].ToString();
                    }
                    //logger.Info(" (1) LoginName:" + LoginName);
                    JObject mo = MyControllers.GetLawById("0003", id, LoginName);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    //check goi
               
                    JArray ser = JArray.Parse(mo["SERVICE"].ToString());
                    if (ser != null)
                    {
                        foreach (var item in ser)
                        {
                            if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "LTH")
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
                    JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                    JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                    JArray ma = new JArray();
                    string mos = mo.ToString();
                    try
                    {
                        if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                        {
                            ma = JArray.Parse(res["RPATH"].ToString());
                            urlFolder = res["PPATH"].ToString();
                        }
                    }catch(Exception e){
                        string ea = e.ToString();
                    }

                    ViewBag.info = info;
                    ViewBag.ma = ma;
                    i_pageitem = 0;
                    RequestParam();
            
                    if (info != null)
                    {
                        if (res != null && ma.Count > 0)
                        {
                            filename = ma[i_pageIndex]["CPATH"].ToString();
                            ViewBag.namechapter = ma[i_pageIndex]["CNAME"].ToString();
                            TXNAME = info["TXNAME"].ToString();
                            url_pathbook = urlFolder + "/" + filename;
                            //logger.Info(" (3) url_pathbook:" + url_pathbook);

                            if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
                            {

                                string LinkReadClient = System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString();
                                string LinkServerLocal = System.Configuration.ConfigurationManager.AppSettings["LinkServerLocal"].ToString();

                                url_pathbook = url_pathbook.Replace(LinkReadClient, LinkServerLocal);
                            }
                          
                            string contents = Core.CallService2(url_pathbook);
                            //de_content = AESLib.decryptAES(contents, de_key);
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(contents);
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
                                ViewBag.totalPage = ma.Count;
                                ViewBag.i_pageIndex = i_pageIndex;
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
                            }
                            mulog = MyControllers.Log_ReadFull(id, LoginName);
                        }

                    }
                    return View();
                }
                else if (SERVICE_CODE == "TN" && PLUS < 0)
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
                catch (Exception ex)
                {
                    logger.Info(" end :" + ex.Message);
                }
                return View();
            }
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
                    LoginName = Session["LoginName"].ToString();
                }
                JObject mo = MyControllers.GetLawById("0003", id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                //check trang thai cho doc
                JObject packge_book = new JObject();
                JObject packge_acc = new JObject();
                JArray ma = new JArray();
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    ma = JArray.Parse(res["RPATH"].ToString());
                    urlFolder = res["PPATH"].ToString();
                }
                if (info != null)
                {
                    if (res != null && ma.Count > 0)
                    {
                        filename = ma[i_pageIndex]["CPATH"].ToString();
                        url_pathbook = urlFolder + "/" + filename;
                        if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
                        {
                            url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkServerLocal"].ToString());

                        }
                        string contents = Core.CallService2(url_pathbook);
                        //de_content = AESLib.decryptAES(contents, de_key);
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(contents);
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
                        else
                        {
                            sb.Append("Dữ liệu đang cập nhật...");
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                logger.Info(" end 2 :" + ex.Message);
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
                JObject mo = MyControllers.GetLawById(Core.TXLAW, id, LoginName);
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                //check trang thai cho doc
                JObject packge_book = new JObject();
                JObject packge_acc = new JObject();
                JArray ma = new JArray();
                if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                {
                    ma = JArray.Parse(res["RPATH"].ToString());
                    urlFolder = res["PPATH"].ToString();
                }
                if (info != null)
                {
                    if (res != null && ma.Count > 0)
                    {
                        filename = ma[i_pageIndex]["CPATH"].ToString();
                        ViewBag.namechapter = ma[i_pageIndex]["CNAME"].ToString();
                        TXNAME = info["TXNAME"].ToString();
                        url_pathbook = urlFolder + "/" + filename;
                        if (System.Configuration.ConfigurationManager.AppSettings["ConfigLinkLocal"] == "1")
                        {
                            url_pathbook = url_pathbook.Replace(System.Configuration.ConfigurationManager.AppSettings["LinkReadClient"].ToString(), System.Configuration.ConfigurationManager.AppSettings["LinkServerLocal"].ToString());
                        }
                        string contents = Core.CallService2(url_pathbook);
                        //de_content = AESLib.decryptAES(contents, de_key);

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(contents);
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
            catch (Exception ex)
            {
                logger.Info(" end 3 :" + ex.Message);
            }

            Response.Write(totalItem.ToString());
            return null;
        }
        public ActionResult Details_video(string id) 
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

                JObject mo = MyControllers.GetLawById_btyle("0003", id, LoginName,"6");
                mo = JObject.Parse(mo["Body"]["Data"].ToString());
                string vmm = mo.ToString();
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
                            if (item["SERVICE_CODE"] != null && item["SERVICE_CODE"].ToString() == "LTH")
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

                        ViewBag.AUTO_ID = id;
                        JObject mo_comment = MyControllers.GetListComment(id,"1");
                        mo_comment = JObject.Parse(mo_comment["Body"]["Data"].ToString());
                        JArray ma_comment = JArray.Parse(mo_comment["COMMENT"].ToString());
                        ViewBag.ma_comment = ma_comment;
                        return View();
                    }
                    else if (SERVICE_CODE == "TN" && PLUS < 0)
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
                    ViewBag.info = info;
                    ViewBag.res = res;
                    ViewBag.ma_other = ma_other;
                    i_category = info["CATE_CODE"].ToString();

                    ViewBag.AUTO_ID = id;
                    JObject mo_comment = MyControllers.GetListComment(id,"1");
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
        public ActionResult Download(String id)
        {
            string path = "";
            string foldel = "";
            try
            {
                string LoginName = "UNKNOW";
                if (Session["LoginName"] != null && Session["PACKGE"]!=null)
                {
                    LoginName = Session["LoginName"].ToString();
                    JObject mo = MyControllers.GetLawById("0003", id, LoginName);
                    mo = JObject.Parse(mo["Body"]["Data"].ToString());
                    JObject info = JObject.Parse(JArray.Parse(mo["LAW"].ToString())[0].ToString());
                    JObject res = JObject.Parse(JArray.Parse(mo["RESOURCE"].ToString())[0].ToString());
                    //check trang thai cho doc
                    JObject packge_book = new JObject();
                    JObject packge_acc = new JObject();
                    JArray ma = new JArray();
                    if (!string.IsNullOrEmpty(res["RPATH"].ToString()) && res["RPATH"].ToString() != "[]")
                    {
                        foldel = res["PPATH"].ToString();
                        path = foldel + res["SRC_PATH"].ToString();
                    }
                    return Redirect(path);
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
        public ActionResult Search()
        {
            LoadDataHomeQuick();
            ViewBag.SearchLaw = "law";
            //List<SelectListItem> listTimes = new List<SelectListItem>();
            List<SelectListItem> listDocType = new List<SelectListItem>();
            List<SelectListItem> listIssuers = new List<SelectListItem>();
            List<SelectListItem> listCate = new List<SelectListItem>();

            //listTimes.Add(new SelectListItem() { Value = "A", Text = "Tất cả" });
            //listTimes.Add(new SelectListItem() { Value = "1", Text = "Đang hiệu lực" });
            //listTimes.Add(new SelectListItem() { Value = "2", Text = "Sắp có hiệu lực" });
            //listTimes.Add(new SelectListItem() { Value = "3", Text = "Sắp hết hiệu lực" });
            //listTimes.Add(new SelectListItem() { Value = "4", Text = "Hết hiệu lực" });
            //listTimes.Add(new SelectListItem() { Value = "5", Text = "Hết hiệu lực một phần" });
            //ViewBag.listTimes = listTimes;

            JObject mo = MyControllers.GetListAdvan();
            JArray ma_doc = new JArray();
            JArray ma_issuser = new JArray();
            JArray ma_cate = new JArray();
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            ma_doc = JArray.Parse(mo["DOCTYPE"].ToString());
            ma_issuser = JArray.Parse(mo["ISSUER"].ToString());
            ma_cate = JArray.Parse(mo["CATEGORY"].ToString());
            listDocType.Add(new SelectListItem() { Text = "Tất cả", Value = "A" });
            foreach (var itemdoctype in ma_doc)
            {
                listDocType.Add(new SelectListItem() { Text = itemdoctype["DOC_NAME"].ToString(), Value = itemdoctype["DOC_CODE"].ToString() });
            }
            listIssuers.Add(new SelectListItem() { Text = "Tất cả", Value = "A" });
            foreach (var itemIssuers in ma_issuser)
            {

                listIssuers.Add(new SelectListItem() { Text = itemIssuers["TXNAME"].ToString(), Value = itemIssuers["INDUSTRY_CODE"].ToString() });
            }

            listCate.Add(new SelectListItem() { Text = "Tất cả", Value = "A" });
            foreach (var itemcate in ma_cate)
            {
                listCate.Add(new SelectListItem() { Text = itemcate["TXNAME"].ToString(), Value = itemcate["CATE_CODE"].ToString() });
            }

            SearchObjectModel modelview = new SearchObjectModel();
            modelview.keyword = "";
            modelview.StartDate = "";
            modelview.EndDate = "";
            modelview.listDoctype = listDocType;
            modelview.listIssusers = listIssuers;
            modelview.listCate = listCate;
            //modelview.listTimes = listTimes;
            ViewBag.CurrentPage = i_pageIndex + 1;

            return View(modelview);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchObjectModel modelview)
        {
            LoadDataHomeQuick();
            ViewBag.SearchLaw = "law";
            if (string.IsNullOrEmpty(modelview.keyword)) modelview.keyword = "";
            if (string.IsNullOrEmpty(modelview.StartDate)) modelview.StartDate = "";
            if (string.IsNullOrEmpty(modelview.EndDate)) modelview.EndDate = "";
            if (string.IsNullOrEmpty(modelview.SelectedCate)) modelview.SelectedCate = "";
            if (string.IsNullOrEmpty(modelview.SelectedIssusers)) modelview.SelectedIssusers = "";
            if (string.IsNullOrEmpty(modelview.SelectedDoctype)) modelview.SelectedDoctype = "";

            JObject mo_ser = MyControllers.GetSearchAdvan(modelview.keyword, modelview.StartDate, modelview.EndDate, modelview.SelectedDoctype, modelview.SelectedIssusers, modelview.SelectedCate);
            mo_ser = JObject.Parse(mo_ser["Body"]["Data"].ToString());
            JArray ma_ser = JArray.Parse(mo_ser["LAW"].ToString());
            ViewBag.ma_ser = ma_ser;
            //List<SelectListItem> listTimes = new List<SelectListItem>();
            List<SelectListItem> listDocType = new List<SelectListItem>();
            List<SelectListItem> listIssuers = new List<SelectListItem>();
            List<SelectListItem> listCate = new List<SelectListItem>();

            JObject mo = MyControllers.GetListAdvan();
            JArray ma_doc = new JArray();
            JArray ma_issuser = new JArray();
            JArray ma_cate = new JArray();
            mo = JObject.Parse(mo["Body"]["Data"].ToString());
            ma_doc = JArray.Parse(mo["DOCTYPE"].ToString());
            ma_issuser = JArray.Parse(mo["ISSUER"].ToString());
            ma_cate = JArray.Parse(mo["CATEGORY"].ToString());
            listDocType.Add(new SelectListItem() { Text = "Tất cả", Value = "A" });
            foreach (var itemdoctype in ma_doc)
            {
                listDocType.Add(new SelectListItem() { Text = itemdoctype["DOC_NAME"].ToString(), Value = itemdoctype["DOC_CODE"].ToString() });
            }
            listIssuers.Add(new SelectListItem() { Text = "Tất cả", Value = "A" });
            foreach (var itemIssuers in ma_issuser)
            {

                listIssuers.Add(new SelectListItem() { Text = itemIssuers["TXNAME"].ToString(), Value = itemIssuers["INDUSTRY_CODE"].ToString() });
            }

            listCate.Add(new SelectListItem() { Text = "Tất cả", Value = "A" });
            foreach (var itemcate in ma_cate)
            {
                listCate.Add(new SelectListItem() { Text = itemcate["TXNAME"].ToString(), Value = itemcate["CATE_CODE"].ToString() });
            }

            modelview.keyword = "";
            modelview.StartDate = "";
            modelview.EndDate = "";
            modelview.listDoctype = listDocType;
            modelview.listIssusers = listIssuers;
            modelview.listCate = listCate;
            //modelview.listTimes = listTimes;
            ViewBag.CurrentPage = i_pageIndex + 1;

            return View(modelview);
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
                JObject mo = MyControllers.AddComment(AUTO_ID, "3", "aFun", HttpUtility.HtmlEncode(commentText), LoginName);
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
            if (Request.QueryString["v"] != null)
            {
                i_go_c1 =   Request.QueryString["v"].ToString();
            }
            if (Request.QueryString["ccode"] != null)
            {
                m_cate=_catecode = Request.QueryString["ccode"].ToString();
                if (_catecode == "0") {
                    _catecode = "ALL";
                }
            }
            if (Request.QueryString["mcode"] != null)
            {
                _modecode = Request.QueryString["mcode"].ToString();
            }
            if (Request.QueryString["type"] != null)
            {
                int.TryParse(Request.QueryString["type"], out type);
            }
           // string slipc = Request.QueryString["page"].ToString();
            if (Request.QueryString["page"] != null)
            {
                try
                {
                    string slipc = Request.QueryString["page"].ToString();
                    string[] tokens = slipc.Split('g');

                    if (tokens.Length > 0)
                    {
                        string last = tokens[1].ToString();
                        if (last == "") {
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
