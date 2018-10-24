using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using aFun.Models;
using log4net;
using Newtonsoft.Json.Linq;

namespace aFun.Controllers
{
    public class PartnerController : Controller
    {
        //
        // GET: /Header/
        private ILog logger = log4net.LogManager.GetLogger(typeof(PartnerController));
        public ActionResult Index(string u, string p)
        {
            if (!string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(p))
            {
                if ((u == "appandroid" && p == "MKJAKWTYZABC2RTZ") || (u == "appios" && p == "MKJAKWTYZAOKKRTZ") || (u == "aFun" && p == "MKJMTMTYZABC2RTZ"))
                {
                    String msisdn = CheckDevice.GetNumberPhoneName();
                    //msisdn = "841226457881";//"841202154614";//841202154614
                    string ipaddress = Common.GetClientIpAddress();
                    //msisdn = "841202154614";
                    //ipaddress = "113.187.0.0";
                    if (!string.IsNullOrEmpty(msisdn) && msisdn.StartsWith("84"))
                    {
                        int createStatus = -1;
                        //Core.CheckToken();

                        if (Common.accessList3g(ipaddress))
                        {
                            ViewBag.msisdn = msisdn;
                        }
                        else
                        {
                            //log msisdn
                            ViewBag.msisdn = "0";
                        }
                    }
                    else
                    {
                        ViewBag.msisdn = "0";
                    }
                }
                else
                {
                    ViewBag.msisdn = "-1";
                }

            }
            else
            {
                ViewBag.msisdn = "-1";
            }
            return View();
        }


        public ActionResult Msisdn2(string sid, string sc, string cp, string reurl)
        {
            string urlnew = "";
            string ipaddress = Common.GetClientIpAddress();
            if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(sc) && !string.IsNullOrEmpty(cp) && !string.IsNullOrEmpty(reurl))
            {
                if (!string.IsNullOrEmpty(reurl))
                {
                    urlnew = reurl;
                }
                String msisdn = CheckDevice.GetNumberPhoneName();
                //msisdn = "841226457881";//"841202154614";//841202154614
                //msisdn = "841202154614";
                //ipaddress = "113.187.0.0";
                if (!string.IsNullOrEmpty(msisdn) && msisdn.StartsWith("84"))
                {
                    int createStatus = -1;
                    //Core.CheckToken();

                    if (Common.accessList3g(ipaddress))
                    {
                        ViewBag.msisdn = msisdn;
                        urlnew += "?m=" + msisdn;
                        if (!string.IsNullOrEmpty(sid))
                        {
                            urlnew += "&sid=" + sid;
                        }
                        if (!string.IsNullOrEmpty(cp))
                        {
                            urlnew += "&cp=" + cp;
                        }
                        if (!string.IsNullOrEmpty(sc))
                        {
                            urlnew += "&sc=" + sc;
                        }

                        //if (!string.IsNullOrEmpty(rurl))
                        //{
                        //    urlnew += "&rurl=" + rurl;
                        //}
                        logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                        return Redirect(urlnew);
                    }
                    else
                    {
                        //log msisdn
                        urlnew += "?m=" + ipaddress;
                        if (!string.IsNullOrEmpty(sid))
                        {
                            urlnew += "&sid=" + sid;
                        }
                        if (!string.IsNullOrEmpty(cp))
                        {
                            urlnew += "&cp=" + cp;
                        }
                        if (!string.IsNullOrEmpty(sc))
                        {
                            urlnew += "&sc=" + sc;
                        }
                        //if (!string.IsNullOrEmpty(rurl))
                        //{
                        //    urlnew += "&rurl=" + rurl;
                        //}
                        //else
                        //{
                        //    rurl = MakeLink.DefaultURLWap();
                        //    urlnew += "&rurl=" + rurl;
                        //}

                        logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                        return Redirect(urlnew);
                    }
                }
                else
                {
                    urlnew += "?m=" + ipaddress;
                    if (!string.IsNullOrEmpty(sid))
                    {
                        urlnew += "&sid=" + sid;
                    }
                    if (!string.IsNullOrEmpty(cp))
                    {
                        urlnew += "&cp=" + cp;
                    }
                    if (!string.IsNullOrEmpty(sc))
                    {
                        urlnew += "&sc=" + sc;
                    }
                    //if (!string.IsNullOrEmpty(rurl))
                    //{
                    //    urlnew += "&rurl=" + rurl;
                    //}
                    //else
                    //{
                    //    rurl = MakeLink.DefaultURLWap();
                    //    urlnew += "&rurl=" + rurl;
                    //}
                    logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                    return Redirect(urlnew);
                }

            }
            else
            {
                urlnew += "?m=" + ipaddress;
                if (!string.IsNullOrEmpty(sid))
                {
                    urlnew += "&sid=" + sid;
                }
                if (!string.IsNullOrEmpty(cp))
                {
                    urlnew += "&cp=" + cp;
                }
                if (!string.IsNullOrEmpty(sc))
                {
                    urlnew += "&sc=" + sc;
                }
                //if (!string.IsNullOrEmpty(rurl))
                //{
                //    urlnew += "&rurl=" + rurl;
                //}
                //else
                //{
                //    rurl = MakeLink.DefaultURLWap();
                //    urlnew += "&rurl=" + rurl;
                //}
                logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                return Redirect(urlnew);
            }
        }//end Msisdn;

        public ActionResult subAds()
        {
            String msisdn = CheckDevice.GetNumberPhoneName();
            //msisdn = "841226457881";//"841202154614";//841202154614
            string ipaddress = Common.GetClientIpAddress();
            //msisdn = "841202154614";
            //ipaddress = "113.187.0.0";
            if (!string.IsNullOrEmpty(msisdn) && msisdn.StartsWith("84"))
            {
                int createStatus = -1;
                //Core.CheckToken();
                if (Common.accessList3g(ipaddress))
                {
                    HtmlDocument doc = new HtmlDocument();
                    HtmlNodeCollection nodeColection;
                    string contents = "";
                    string comfirmUrl = "";
                    string urlcomfirm = "";
                    urlcomfirm = CoreTelco.getUrlRequesComfirmAds(Session["LoginName"].ToString(), Core.LTH, "1000", "goi_luat_tong_hop", "http://aFun.vn/partner/subAds");
                    //b1:
                    contents = Core.CallService2(urlcomfirm);
                    //contents = Regex.Replace(contents, @"\t|\n|\r", "");
                    doc.LoadHtml(contents);
                    nodeColection = doc.DocumentNode.SelectNodes("//input[@type='hidden']");
                    //string id = nodeColection[0].GetAttributeValue("id", "default");
                    string h_sc = nodeColection[0].GetAttributeValue("value", "default");
                    //string atr = nodeColection[0].Attributes["value"].Value;
                    //string atr2 = nodeColection[0].Attributes["id"].Value;
                    //b2:
                    urlcomfirm = "http://dk.vinaphone.com.vn/confirm.jsp?h_sc=" + h_sc + "";
                    logger.Info("h_sc regUrl: " + urlcomfirm);
                    contents = Core.CallService2(urlcomfirm);
                    contents = Regex.Replace(contents, @"\t|\n|\r", "");
                    //logger.Info("contents backurl: " + contents);
                    doc.LoadHtml(contents);
                    nodeColection = doc.DocumentNode.SelectNodes("//a[@class='btn btn-default']");
                    //string id = nodeColection[0].GetAttributeValue("id", "default");
                    string backurl = nodeColection[0].GetAttributeValue("href", "default");
                    logger.Info("backurl: " + backurl);
                    Response.Redirect("http://aFun.vn/");
                }
                else
                {
                    //log msisdn
                    Response.Redirect("http://aFun.vn/");
                }
            }
            else
            {
                Response.Redirect("http://aFun.vn/");
            }

            return null;
        }

        public ActionResult Msisdn(string sid, string sc, string cp, string reurl, string cf)
        {
            string urlnew = "";
            string regurl = "";
            string ipaddress = Common.GetClientIpAddress();
            string is_cf = "N";
            if (!string.IsNullOrEmpty(cf))
            {
                is_cf = cf;
            }
            try
            {
                if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(sc) && !string.IsNullOrEmpty(cp) && !string.IsNullOrEmpty(reurl))
                {
                    if (!string.IsNullOrEmpty(reurl))
                    {
                        urlnew = reurl;
                    }
                    String msisdn = CheckDevice.GetNumberPhoneName();
                    //msisdn = "841226457881";//"841202154614";//841202154614
                    //msisdn = "841202154614";
                    //ipaddress = "113.187.0.0";
                    if (!string.IsNullOrEmpty(msisdn) && msisdn.StartsWith("84"))
                    {
                        int createStatus = -1;
                        //Core.CheckToken();

                        if (Common.accessList3g(ipaddress))
                        {
                            ViewBag.msisdn = msisdn;
                            urlnew += "?m=" + msisdn;
                            if (!string.IsNullOrEmpty(sid))
                            {
                                urlnew += "&sid=" + sid;
                            }
                            if (!string.IsNullOrEmpty(cp))
                            {
                                urlnew += "&cp=" + cp;
                            }
                            if (!string.IsNullOrEmpty(sc))
                            {
                                urlnew += "&sc=" + sc;
                            }

                            //if (!string.IsNullOrEmpty(rurl))
                            //{
                            //    urlnew += "&rurl=" + rurl;
                            //}
                            logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid + ", cf=" + is_cf);
                            if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(cp) && !string.IsNullOrEmpty(sc))
                            {
                                Core.CheckToken();
                                JObject mocheck = MyControllers.CheckServices(msisdn, sc);
                                string pack = "0";
                                try
                                {
                                    createStatus = Convert.ToInt32(mocheck["Header"]["Code"].ToString());
                                    mocheck = JObject.Parse(mocheck["Body"]["Data"].ToString());
                                    mocheck = JObject.Parse(JArray.Parse(mocheck["SERVICE"].ToString())[0].ToString());
                                    pack = mocheck["PACK"] != null ? mocheck["PACK"].ToString() : "";
                                    logger.Info("CheckServices m=" + msisdn + ";- createStatus: " + createStatus + "; - sc=" + sc + ";- pack= " + pack);
                                }
                                catch (Exception ex)
                                {
                                    createStatus = -1;
                                    pack = "0";
                                    logger.Info("Exception CheckServices m=" + msisdn + ";- sc=" + sc + ";- pack= " + Convert.ToInt32(pack));
                                }
                                JObject mu_log = new JObject();
                                JObject mu = new JObject();
                                if (createStatus == 1101)// Dang su dung goi
                                {
                                    logger.Debug("Đang sử dụng gói m: " + msisdn + "; sc= " + sc + ";- createStatus= " + createStatus);
                                    mu_log = MyControllers.Log_Ads_Update(sid, sc, msisdn, createStatus.ToString(), "Thuê bao đang sử dụng gói", "F");
                                    return Redirect("/");
                                }
                                else if ((createStatus == 0 || createStatus == 1102) && Convert.ToInt32(pack) == 0)//user chua co goi và chưa hủy lần nào(pack=0)
                                {
                                    if (is_cf.ToUpper() == "Y")
                                    {
                                        logger.Debug("Đăng ký mới: sub m: " + msisdn + "; sc= " + sc + ";- createStatus= " + createStatus + "; cf=" + is_cf);
                                        Random rand = new Random();
                                        string trans_id = msisdn + DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next(1, 9999);
                                        if (sc.ToUpper().Trim() == Core.LTH)
                                        {
                                            regurl = CoreTelco.getUrlRequesComfirm(msisdn, Core.LTH, "1000", "goi_luat_tong_hop", trans_id);
                                        }
                                        else
                                        {
                                            regurl = CoreTelco.getUrlRequesComfirm(msisdn, Core.TN, "2000", "goi_trac_nghiem_phap_luat", trans_id);//
                                        }
                                        ViewBag.regurl = regurl;
                                        INFOCPCADS infocpc = new INFOCPCADS();
                                        infocpc.sid = sid;
                                        infocpc.sc = sc;
                                        infocpc.cp = cp;
                                        infocpc.msisdn = msisdn;
                                        infocpc.trans_id = trans_id;
                                        Session["INFOCPCADS"] = infocpc;
                                        //return View();
                                        return Redirect(regurl);
                                    }
                                    else
                                    {
                                        logger.Debug("Đăng ký mới: sub m: " + msisdn + "; sc= " + sc + ";- createStatus= " + createStatus + "; cf=" + is_cf);
                                        if (sc.ToUpper().Trim() == "TN" || sc.ToUpper().Trim() == "TNPL")
                                        {
                                            mu = MyControllers.AddPackgeTN(msisdn, sc, "123", 1, cp);
                                        }
                                        else
                                        {
                                            mu = MyControllers.AddPackgeLTH(msisdn, sc, "123", 1, cp);
                                        }
                                        createStatus = Convert.ToInt32(mu["Header"]["Code"].ToString());
                                        if (createStatus == 0)
                                        {
                                            mu_log = MyControllers.Log_Ads_Update(sid, sc, msisdn, createStatus.ToString(), "Đăng ký gói " + sc, "A");
                                        }
                                        else if (createStatus == 1101)
                                        {
                                            mu_log = MyControllers.Log_Ads_Update(sid, sc, msisdn, createStatus.ToString(), "Đăng ký  gói " + sc + " thất bại: Thuê bao đang sử dụng gói.", "F");
                                        }
                                        else
                                        {
                                            mu_log = MyControllers.Log_Ads_Update(sid, sc, msisdn, createStatus.ToString(), "Đăng ký  gói " + sc + " thất bại: " + ACC_MESSAGE(createStatus, "packge") + ".", "F");
                                        }
                                        logger.Debug("Kết quả đăng ký mới số m: " + msisdn + "; sc= " + sc + ";- createStatus= " + createStatus + "; cf=" + is_cf);
                                        return Redirect("/");
                                    }


                                }
                                else //dang ky lai va hien comfirm if (createStatus == 1102 && Convert.ToInt32(pack) > 0)
                                {
                                    logger.Debug("Đăng ký lại gói m: " + msisdn + "; sc= " + sc + "; pack=" + Convert.ToInt32(pack) + "; cf:" + is_cf);
                                    //mu_log = MyControllers.Log_Ads_Update(sid, sc, m, createStatus.ToString(), "Đăng ký lại " + sc + ".", "F");
                                    Random rand = new Random();
                                    string trans_id = msisdn + DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next(1, 9999);
                                    if (sc.ToUpper().Trim() == Core.LTH)
                                    {
                                        regurl = CoreTelco.getUrlRequesComfirm(msisdn, Core.LTH, "1000", "goi_luat_tong_hop", trans_id);
                                    }
                                    else
                                    {
                                        regurl = CoreTelco.getUrlRequesComfirm(msisdn, Core.TN, "2000", "goi_trac_nghiem_phap_luat", trans_id);//
                                    }
                                    ViewBag.regurl = regurl;
                                    INFOCPCADS infocpc = new INFOCPCADS();
                                    infocpc.sid = sid;
                                    infocpc.sc = sc;
                                    infocpc.cp = cp;
                                    infocpc.msisdn = msisdn;
                                    infocpc.trans_id = trans_id;
                                    Session["INFOCPCADS"] = infocpc;
                                    // return View();
                                    return Redirect(regurl);

                                }

                            }
                            else
                            {
                                return Redirect(urlnew);
                            }

                        }
                        else
                        {
                            //log msisdn
                            urlnew += "?m=" + ipaddress;
                            if (!string.IsNullOrEmpty(sid))
                            {
                                urlnew += "&sid=" + sid;
                            }
                            if (!string.IsNullOrEmpty(cp))
                            {
                                urlnew += "&cp=" + cp;
                            }
                            if (!string.IsNullOrEmpty(sc))
                            {
                                urlnew += "&sc=" + sc;
                            }
                            //if (!string.IsNullOrEmpty(rurl))
                            //{
                            //    urlnew += "&rurl=" + rurl;
                            //}
                            //else
                            //{
                            //    rurl = MakeLink.DefaultURLWap();
                            //    urlnew += "&rurl=" + rurl;
                            //}

                            logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                            return Redirect(urlnew);
                        }
                    }
                    else
                    {
                        urlnew += "?m=" + ipaddress;
                        if (!string.IsNullOrEmpty(sid))
                        {
                            urlnew += "&sid=" + sid;
                        }
                        if (!string.IsNullOrEmpty(cp))
                        {
                            urlnew += "&cp=" + cp;
                        }
                        if (!string.IsNullOrEmpty(sc))
                        {
                            urlnew += "&sc=" + sc;
                        }
                        //if (!string.IsNullOrEmpty(rurl))
                        //{
                        //    urlnew += "&rurl=" + rurl;
                        //}
                        //else
                        //{
                        //    rurl = MakeLink.DefaultURLWap();
                        //    urlnew += "&rurl=" + rurl;
                        //}
                        logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                        return Redirect(urlnew);
                    }

                }
                else
                {
                    urlnew += "?m=" + ipaddress;
                    if (!string.IsNullOrEmpty(sid))
                    {
                        urlnew += "&sid=" + sid;
                    }
                    if (!string.IsNullOrEmpty(cp))
                    {
                        urlnew += "&cp=" + cp;
                    }
                    if (!string.IsNullOrEmpty(sc))
                    {
                        urlnew += "&sc=" + sc;
                    }
                    //if (!string.IsNullOrEmpty(rurl))
                    //{
                    //    urlnew += "&rurl=" + rurl;
                    //}
                    //else
                    //{
                    //    rurl = MakeLink.DefaultURLWap();
                    //    urlnew += "&rurl=" + rurl;
                    //}
                    logger.Info("m=" + ipaddress + "; cp=" + cp + ", sc=" + sc + ", sid=" + sid);
                    return Redirect(urlnew);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ip: " + ipaddress + "; Exception: " + ex.Message);
                return Redirect("/");
            }

        }//end Msisdn;


        private string ACC_MESSAGE(int code, string cmd)
        {
            if (cmd == "packge")
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
                    case 110:
                        return "Thuê bao của Quý khách đang bị khóa vui lòng liên hệ 9090 để được hỗ trợ.";
                    case 111:
                        return "Thuê báo của người được tặng đang bị khóa vui lòng liên hệ 9090 để được hỗ trợ.";
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
                    case 109:
                        return "Quý khách không thể tặng sách cho chính mình.";
                    case 110:
                        return "Thuê bao của Quý khách đang bị khóa vui lòng liên hệ 9090 để được hỗ trợ.";
                    case 111:
                        return "Thuê báo của người được tặng đang bị khóa vui lòng liên hệ 9090 để được hỗ trợ.";
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

    }
}
