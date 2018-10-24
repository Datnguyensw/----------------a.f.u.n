using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using lemon.wapgw.cryptengine;
using System.Globalization;
using System.Web.Mvc;
using log4net;
namespace aFun.Models
{
    public static class Common
    {
        public static string GetClientIpAddress()
        {
            string ip;
            try
            {
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",", StringComparison.Ordinal) > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                    ip = HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                ip = "0.0.0.0";
            }
            return ip;
        }
        public static string[] booktype = { "", "Sách chọn lọc", "Sách miễn phí", "Sách bán chạy", "Sách nổi bật", "Sách tuyển chọn", "Sách mới" };
        public static string[] booknume = { "01", "02", "03", "04", "05" };//{ "BOOKTOP", "BOOKNEW", "BOOKFREE", "BOOKDOWNLOAD", "" };
        public static string[] catetype = { "0055", "0056", "0058", "0053", "0041", "0001" };
        public static string[] cnumtype = { "0", "1", "2", "3", "4", "5", "6" };
        public static string[] newscate = { "", "Chân dung", "Phê bình", "Điểm tin" };
        public static bool ValidatePhoneAfun(string numberphone) 
        {
          //  (096, 097, 098, 0162, 0163, 0164, 0165, 0166, 0167, 0168, 0169),
            numberphone = "096";//&& numberphone.Length >= 10 && numberphone.Length <= 11
            if (numberphone.StartsWith("096") || numberphone.StartsWith("097") || numberphone.StartsWith("098") || numberphone.StartsWith("0162") || numberphone.StartsWith("0163") || numberphone.StartsWith("0166") || numberphone.StartsWith("0167") || numberphone.StartsWith("0168") || numberphone.StartsWith("0169") && numberphone.Length >= 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ValidateViettel(String phone)
        {
            string reg = @"^(84)((9[678]([0-9]){7})|(16[1-9]([0-9]){7}))$";
           string reg1 = @"^(0)((9[678]([0-9]){7})|(16[1-9]([0-9]){7}))$";
            //string reg = @"^(84)((9[68]([0-9]){7})|(12[01268]([0-9]){7}))$";
            //string reg1 = @"^(0)((9[68]([0-9]){7})|(12[01268]([0-9]){7}))$";
            //string reg2= @"^(84)((16[69]([0-9]){7})|(12[01268]([0-9]){7}))$";
            //string reg3 = @"^(0)((16[69]([0-9]){7})|(12[01268]([0-9]){7}))$";

            if (Regex.IsMatch(phone, reg) || Regex.IsMatch(phone, reg1) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static String formatCuryment(String price)
        {
            //string reg = @"^(84)((9[03]([0-9]){7})|(12[01268]([0-9]){7}))$";
            String temp = price;
            string tempsub = "";
            temp = temp.Replace(",", ""); temp = temp.Replace("'", ""); temp = temp.Replace("\"", ""); temp = temp.Replace(".", "");
            try
            {
                if (Convert.ToInt32(temp) > 0)
                {
                    tempsub = temp.Substring(0, temp.Length - 3) + "." + temp.Substring(temp.Length - 3, 3) + " VNĐ";
                }
                else
                {
                    tempsub = "Miễn phí";
                }
                temp = tempsub;
            }
            catch (Exception)
            {
                temp = "";
            }
            return temp;
        }
        public static String formatPhone(String phone, int formatType)
        {
            String temp = phone;
            switch (formatType)
            {
                case 0:
                    if (temp.StartsWith("9"))
                    {
                        temp = "84" + temp;
                    }
                    else if (temp.StartsWith("0"))
                    {
                        temp = "84" + temp.Substring(1);
                    } // else startsWith("84")
                    else if (temp.StartsWith("1") && temp.Length == 10)
                    {
                        temp = "84" + temp;
                    }
                    break;
                case 1:
                    if (temp.StartsWith("84"))
                    {
                        temp = temp.Substring(2);
                    }
                    else if (temp.StartsWith("0"))
                    {
                        temp = temp.Substring(1);
                    } // else startsWith("9")
                    break;
                case 2:
                    if (temp.StartsWith("84"))
                    {
                        temp = "0" + temp.Substring(2);
                    }
                    else if (temp.StartsWith("9"))
                    {
                        temp = "0" + temp;
                    } // else startsWith("09")
                    break;
                default:
                    return null;
            }
            return temp;
        }
        public static bool accessList3g(String ip)
        {
            string IP_ACCESS = System.Configuration.ConfigurationManager.AppSettings["IP_ACCESS"].ToString();
            String accessList = IP_ACCESS;
            String[] ipList = accessList.Split(',');
            bool result = false;
            foreach (var item in ipList)
            {
                if (ip.StartsWith(item.Trim()))
                {
                    result = true;
                }
            }
            return result;
        }
        public static bool Check_HttpWebRequest(String path_url)
        {
            try
            {
                // Creates an HttpWebRequest for the specified URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(path_url);
                myHttpWebRequest.Timeout = 5000;
                // Sends the HttpWebRequest and waits for a response.
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                // Releases the resources of the response.
                myHttpWebResponse.Close();

            }
            catch (WebException e)
            {

                Console.WriteLine("\r\nWebException Raised. The following error occured : {0}", e.Status);
                return false;
            }

        }
        public static bool CheckDate(String strdate)
        {
            DateTime fromDateValue;
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(strdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static List<SelectListItem> ListbType()
        {
            List<SelectListItem> lstbType = new List<SelectListItem>();
            lstbType.Add(new SelectListItem() { Text = "Law", Value = "1" });
            lstbType.Add(new SelectListItem() { Text = "Audio", Value = "2" });
            lstbType.Add(new SelectListItem() { Text = "ECOMICS", Value = "3" });
            lstbType.Add(new SelectListItem() { Text = "MAGAZINE", Value = "4" });
            lstbType.Add(new SelectListItem() { Text = "DOCUMENT", Value = "5" });
            return lstbType;
        }
        private const string chars_pass = "ABCDEFGHJKLMNPRSTUVXYZ23456789#@$!";
        private static string GenRandomText(int textLength)
        {
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars_pass, textLength)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }
        public static string CreatePass()
        {
            string password = Common.GenRandomText(6);
            return password;
        }
        public static bool CheckView(HttpCookie cookread)
        {
            //HttpCookie cookread = new HttpCookie();
            string countview = cookread.Value;
            if (Convert.ToInt32(countview) <= 2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
    public static class Core
    {

        public static String ServerPath = System.Configuration.ConfigurationManager.AppSettings["LinkRes"].ToString();
        public static String headerphone = System.Configuration.ConfigurationManager.AppSettings["headerphone"].ToString();
        public static String LTH = System.Configuration.ConfigurationManager.AppSettings["LTH"].ToString();
        public static String TN = System.Configuration.ConfigurationManager.AppSettings["TP"].ToString();
        public static String EB = System.Configuration.ConfigurationManager.AppSettings["EB"].ToString();
        public static String VD = System.Configuration.ConfigurationManager.AppSettings["VD"].ToString();
        public static String URL_VD = System.Configuration.ConfigurationManager.AppSettings["URL_VD"].ToString();
        public static String URL_EB = System.Configuration.ConfigurationManager.AppSettings["URL_EB"].ToString();
        public static String URL_TP = System.Configuration.ConfigurationManager.AppSettings["URL_TP"].ToString();
        public static String URL_LTH = System.Configuration.ConfigurationManager.AppSettings["URL_LTH"].ToString();
        public static String LinkReg = System.Configuration.ConfigurationManager.AppSettings["LinkReg"].ToString();
        
        public static String key = "Lem0n@014Mbo0k12";//"ybZMrzm9zS0J19D+WiGmDWeISXJQXypRajJ4lRoSbCs=";
        public static CoreData.ServiceCore coreData = new CoreData.ServiceCore();
        public static string token = "";
        public static int is3g = 0;

        public static string jsonHeader(string cmd)
        {
            string ipaddress = "113.187.0.0";//Common.GetClientIpAddress();
            string s = "{'cmd':'" + cmd + "','ip':'" + ipaddress + "','agent':'WAP','token':'" + token + "'}";
            return s;
        }
        public static string jsonFooter = "{'user':'abc','password':'123'}";
        public static void CheckToken()
        {
            if (String.IsNullOrEmpty(token))
            {
                string s = coreData.ProService(jsonHeader("0001"), "", jsonFooter);
                JObject j = JObject.Parse(s);
                j = JObject.Parse(j["Footer"].ToString());
                token = j["Token"].ToString();
            }
        }
        public static string CallService2(string linkbooks)
        {
            //ILog logger = log4net.LogManager.GetLogger(typeof(Core));
            //logger.Info("log linkbooks:" + linkbooks.ToString());
            try
            {
                //string urlLocalPost = txtUrl.Text;
                string Url = linkbooks;
                WebRequest webRequest = WebRequest.Create(Url);
                WebResponse response = webRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                //logger.Info("log CallService2: ok");
                string momy = streamRead.ReadToEnd();
                return momy;
            }
            catch (Exception e)
            {
                e.ToString();
                //log content html
                //logger.Error("log CallService2:" + e.ToString());

                return "Time Out To Connect";
            }
            //log content html

        }
        #region MOD_CODE


        //
        public static string TXNEW = "0067";
        public static string TXVIDEO = "0012";
        public static string TXQUIZ = "0200";
        public static string TXAUDIO = "0011";
        public static string TXLAW = "0002";
        public static string TXPOST = "0083";

        public static string TXQUESTION = "0084";
        public static string A_TXHOME = "0000";
        public static string A_TXBOOK = "0001";
        public static string A_TXLUAT = "0080";
        public static string A_TXVIDEO = "0012";
        public static string A_TXQUIZ = "0200";
        public static string A_TXHELP = "0084";
        public static string A_TXINFO = "0083";
        public static string A_TXSHARE = "0000";
        public static string A_TXDOWLOAD = "0000";
        #endregion
        #region CATE_CODE
        public static string A_HOME = "0001";
        public static string A_BOOK = "0004";
        public static string A_LUAT = "0003";
        public static string A_QUIZ = "0005";
        public static string A_VIDEO = "0002";
        public static string A_HELP = "0006";
        public static string A_INFO = "0007";
        public static string A_SHARE = "0009";
        public static string A_DOWLOAD = "0008";

        public static string C_NEW = "0005";
        public static string C_VIDEO = "0002";
        public static string C_QUIZ = "0005";
        public static string C_AUDIO = "0058";
        public static string C_RELAX = "0041";
        public static string C_BOOK= "0004";
        public static string C_LAW = "0001";
        public static string C_QUESTION = "0057";
        public static string C_ABOUT = "0052";
        #endregion
        #region CATE_CODE
        public static int row_view = 0;
        public static string name_object = "";
        #endregion
    }

    public static class MyControllers
    {
        private static ILog logger = log4net.LogManager.GetLogger(typeof(MyControllers));
        #region OPP get ojob

        public static JObject Get_cate_item(string cmd, string ctype, string btype)
        {
            JObject mo = null;
            try
            {
                string body = "{'cType':'" + ctype + "','bType':'"+btype+"'}";
                string result = Core.coreData.ProService(Core.jsonHeader(cmd), body, Core.jsonFooter);
                mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception e)
            {
                string ef = e.ToString();
                return mo;
            }
        }
        public static JObject Get_cate_item_0002(string ctype, string btype,string ccode)
        {
            JObject mo = null;
            try
            {
                string body = "{'cType':'" + ctype + "','bType':'" + btype + "','cateCode':'" + ccode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("0002"), body, Core.jsonFooter);
                mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception e)
            {
                string ef = e.ToString();
                return mo;
            }
        }
        #endregion
        #region OPP get Cate 3014
//        type: mới=1; chọn lọc=2; miễn phí=4; cateCode=Thể loại; modCode=cType ( Video='0002'; LAW= '0003'; BOOK='0004')
//bType:1 = sách điện tử / 2 = Truyện tranh / 3 = sách nói / 4 = video ( -  sách -) / 5 = tin tức / 6 = video / 7 = văn bản
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">page:0</param>
        /// <param name="ps">pageSize:10</param>
        /// <param name="cCode">cateCode:0002</param>
        /// <param name="mcode">modCode:32</param>
        /// <param name="btype">1</param>
        /// <param name="type">1</param>
        /// <returns></returns>
        public static JObject get_3014(int p,int ps,string cCode,string mcode,string btype,int type)
        {
            JObject mo = null;
            try
            {//
            //   {'page':'0','pageSize':'10','cateCode':'00','modCode':'0003','bType':'7','type':'1'}
                //{'page':'0','pageSize':'2','cateCode':'ALL','modCode':'0003','bType':'6','type':'1'}
                string body = "{'page':'" + p + "','pageSize':'" + ps + "','cateCode':'" + cCode + "','modCode':'" + mcode + "','bType':'" + btype + "','type':'" + type + "'}";
               
                string result = Core.coreData.ProService(Core.jsonHeader("3014"), body, Core.jsonFooter);
                mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception e)
            {
                string ef = e.ToString();
                return mo;
            }
         }

        #endregion


        public static JObject Buyvideo(string userName, string bookId, string otp, int is3g = 0)
        {
            try
            {
                userName = Common.formatPhone(userName, 0);
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'bookId':'" + bookId + "','is3G':'" + is3g + "', 'otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2071"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Giffvideo(string userName, string userGift, string bookId, string otp, int is3g = 0)
        {
            try
            {
                //userName = Common.formatPhone(userName, 0);
                //userGift = Common.formatPhone(userGift, 0);
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'userGift':'" + userGift + "','bookId':'" + bookId + "','is3G':'" + is3g + "', 'otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2072"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        

        #region OPP get List New 3050
        
        public static JObject get_3050(int p, int ps, string cCode)
        {
            JObject mo = null;
            try
            {
                //{'page':'0','pageSize':'10','cateCode':'ALL','modCode':'0004','bType':'1','type':'0'}
                string body = "{'page':'" + p + "','pageSize':'" + ps + "','cateCode':'" + cCode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3050"), body, Core.jsonFooter);
                mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception e)
            {
                string ef = e.ToString();
                return mo;
            }
        }

        #endregion


        #region OPP get  book 3016

        public static JObject get_3016(int p, int ps, string cCode,int btype,int type)
        {
            JObject mo = null;
            try
            {
                string body = "{'page':'" + p + "','pageSize':'" + ps + "','cateCode':'" + cCode + "','bType':'" + btype + "','type':'"+type+"'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3016"), body, Core.jsonFooter);
                mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception e)
            {
                string ef = e.ToString();
                return mo;
            }
        }

        #endregion


        #region Book
        public static JObject UpdateStar(string bookId, string star, string userName)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'bookId':'" + bookId + "','star':'" + star + "','userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3061"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetBookByTabCate(string btype = "ALL", string catecode = "ALL", string tab = "0", int page = 0, int pagesize = 10, string authorId = "0", string pubId = "0", string supplierId = "0")
        {
            try
            {
                string body = "{'cateCode':'" + catecode + "','type':'" + tab + "','page':'" + page + "','pageSize':'" + pagesize + "','authorId':'" + authorId + "','pubId':'" + pubId + "','supplierId':'" + supplierId + "','bType':'" + btype + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3014"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetBookById(string id = "0", string userName = "0",string btyle="1")
        {
            try
            {
                string body = "{'bookId':'" + id + "','userName':'" + userName + "','bType':'" + btyle + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2014"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetListComment_book(string bookId = "0", int page = 0, int pagesize = 10)
        {
            try
           {
               string body = "{'bookId':'" + bookId + "','page':'" + page + "','pageSize':'" + pagesize + "'}";
               string result = Core.coreData.ProService(Core.jsonHeader("3027"), body, Core.jsonFooter);
               JObject mo = JObject.Parse(result);
               return mo;
          }
           catch (Exception)
          {
              return null;
        }
        }
        public static JObject AddComment(string bookId, string title, string content, string userName)
        {
            try
            {
                //{'autoId':'123','title':'ad','content':'abc','userName':'84988954651','ctype':'4'}
                string body = "{'autoId':'" + bookId + "','title':'" + title + "','content':'" + content + "','userName':'" + userName + "','ctype':'4'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3028"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Markbook(string bookId, string userName, string remain, string owner, string type,string time)
        {
            try
            {
                //ower: 0: thuoc dang doc, thue, 1: thu vien
                //type: 0: add, 1: get
                string body = "{'bookId':'" + bookId + "','userName':'" + userName + "','remain':'" + remain + "','font':'1000','owner':'" + owner + "','type':'" + type + "','pExt':'" + time + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3070"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject AddBookLike(string bookId, string userName, string isFa = "1")
        {
            try
            {
                //isFa: 1: like, 0: not like
                string body = "{'bookId':'" + bookId + "', 'userName':'" + userName + "','isFa':'" + isFa + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3063"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static JObject BuyBook(string userName, string bookId, string otp, int is3g = 0)
        {
            try
            {
                userName = Common.formatPhone(userName, 0);
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'bookId':'" + bookId + "','is3G':'" + is3g + "', 'otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2018"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GiffBook(string userName, string userGift, string bookId, string otp, int is3g = 0)
        {
            try
            {
                //userName = Common.formatPhone(userName, 0);
                //userGift = Common.formatPhone(userGift, 0);
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'userGift':'" + userGift + "','bookId':'" + bookId + "','is3G':'" + is3g + "', 'otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2026"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        #endregion

        
        #region Home
        public static JObject GetDataHome()
        {
            try
            {
                string result = Core.coreData.ProService(Core.jsonHeader("0000"), "", Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static void CheckToken()
        {
            if (String.IsNullOrEmpty(Core.token))
            {
                string s = Core.coreData.ProService(Core.jsonHeader("0001"), "", Core.jsonFooter);
                JObject j = JObject.Parse(s);
                j = JObject.Parse(j["Footer"].ToString());
                Core.token = j["Token"].ToString();
            }
        }
        public static JObject GenOTP(string userName, string type)
        {
            try
            {
                //Core.CheckToken();
                //type: 0.Dang ky, 1.quen mat khau, 2. opt mua book, 3. opt tang book, 4. mua goi, 5. tang goi
                string body = "{'userName':'" + userName + "', 'type':'" + type + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("0003"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetDataHomeQuick()
        {
            try
            {
                string result = Core.coreData.ProService(Core.jsonHeader("0004"), "{}", Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetSysvarByCode(string txcode)
        {
            try
            {
                string body = "{'code':'" + txcode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3075"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Commentsendemail(string userName, string fullName, string email, string content)
        {
            try
            {
                string body = "{'userName':'" + userName + "','fullName':'" + fullName + "','title':'y/c book','email':'" + email + "','content':'" + content + "','type':'1', 'star':'0'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1028"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Star_App(string userName, string fullName, string email, string content, string star)
        {
            try
            {
                string body = "{'userName':'" + userName + "','fullName':'" + fullName + "','title':'y/c book','email':'" + email + "','content':'" + content + "','type':'0', 'star':'" + star + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1027"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Search
        public static JObject GetSearchAdvanBook(string word, string btype = "ALL", string catecode = "ALL", int page = 0, int pagesize = 10, string authorId = "0", string pubId = "0", string supplierId = "0")
        {
            try
            {
                string body = "{'cateCode':'" + catecode + "','word':'" + word + "','page':'" + page + "','pageSize':'" + pagesize + "','authorId':'" + authorId + "','pubId':'" + pubId + "','supplierId':'" + supplierId + "','bType':'" + btype + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3026"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetSearchAjax(string key,int top) {
            try
            {
                string body = "{'word':'"+key+"','top':'"+top+"'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3006"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetSearchQuick(string word)
        {
            try
            {
                string body = "{'word':'" + word + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3006"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetSearchAdvan(string word, string fdate = "", string tdate = "", string cateCode = "", string docType = "", string isSuer = "")
        {
            try
            {
                string body = "{'word':'" + word + "','fdate':'" + fdate + "','tdate':'" + tdate + "','cateCode':'" + cateCode + "','docType':'" + docType + "','isSuer':'" + docType + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3026"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Category
        public static JObject GetCateInfo(string cateCode = "ALL")
        {
            try
            {
                string body = "{'cateCode':'" + cateCode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3036"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetListChildCate(string cateCode = "ALL")
        {
            try
            {
                string body = "{'cateCode':'" + cateCode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3051"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public static JObject GetListCate(int page = 0, int pagesize = 10, string ctype = "ALL")
        {
            try
            {
                string body = "{'ctype':'" + ctype + "','page':'" + page + "','pageSize':'" + pagesize + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3060"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Law
        public static JObject GetTopLaw(string modcode, string cateCode = "ALL", int pagesize = 5)
        {
            try
            {
                string body = "{'top':'" + pagesize + "','modCode':'" + modcode + "','cateCode':'" + cateCode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3015"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetLawByCate(string modcode, string catecode = "ALL", int page = 0, int pagesize = 10, string type = "0")
        {
            try
            {
                string body = "{'page':'" + page + "','pageSize':'" + pagesize + "','cateCode':'" + catecode + "','modCode':'" + modcode + "','type':'" + type + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3014"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetLawById(string modCode, string lawId = "0", string userName = "UNKNOW")
        {
            try
            {
                string body = "{'lawId':'" + lawId + "','userName':'" + userName + "','modCode':'" + modCode + "','bType':'0'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3013"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetLawById_btyle(string modCode, string lawId = "0", string userName = "UNKNOW",string bty="")
        {
            try
            {
                string body = "{'lawId':'" + lawId + "','userName':'" + userName + "','modCode':'" + modCode + "','bType':'" + bty + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3013"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetListComment(string autoId = "0", string ctype = "0", int page = 0, int pagesize = 10)
        {
            try
            {
                string body = "{'autoId':'" + autoId + "','ctype':'" + ctype + "','page':'" + page + "','pageSize':'" + pagesize + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3027"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject AddComment(string autoId, string ctype, string title, string content, string userName)
        {
            try
            {
                string body = "{'autoId':'" + autoId + "','ctype':'" + ctype + "','title':'" + title + "','content':'" + content + "','userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3028"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region News
        public static JObject GetListNews(string cateCode = "ALL", int page = 0, int pagesize = 10)
        {
            try
            {
                string body = "{'page':'" + page + "','pageSize':'" + pagesize + "','cateCode':'" + cateCode + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3050"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetNewById(string id)
        {
            try
            {
                string body = "{'newId':'" + id + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3052"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Quiz
        public static JObject getQuestion(string userName)
        {
            try
            {
                string body = "{'userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3076"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject answerQuestion(string userName, string answer)
        {
            try
            {
                string body = "{'userName':'" + userName + "','answer':'" + answer + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3077"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetBonus(int page = 0, int pagesize = 10)
        {
            try
            {
                string body = "{'page':'" + page + "','pageSize':'" + pagesize + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3083"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GetCharts(string userName)
        {
            try
            {
                string body = "{'userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3084"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Question
        public static JObject GetQA(string userName, int page = 0, int pagesize = 10)
        {
            try
            {
                string body = "{'userName':'" + userName + "','page':'" + page + "','pageSize':'" + pagesize + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3080"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject getQAById(string autoId)
        {
            try
            {
                string body = "{'autoId':'" + autoId + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3081"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject AddQA(string title, string content, string userName = "UNKNOW")
        {
            try
            {
                string body = "{'title':'" + title + "','content':'" + content + "','userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3082"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Acc
        public static JObject Register(string userName, string password, int is3g = 0)
        {
            try
            {
                //userName = Common.formatPhone(userName, 0);
                string passpower = Common.CreatePass();
                password = is3g == 1 ? passpower : password;
                string body = "{'userName':'" + userName + "', 'password':'" + password + "','is3G':'" + is3g + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1001"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject ActiveReg(string userName, string opt)
        {
            try
            {

                //Core.CheckToken();
                userName = Common.formatPhone(userName, 0);
                string body = "{'userName':'" + userName + "','codeUser':'" + opt + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1002"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Login(string userName, string password, int is3g = 0)
        {
            try
            {
                //userName = Common.formatPhone(userName, 0);
                string body = "{'userName':'" + userName + "', 'password':'" + password + "','is3G':'" + is3g + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1003"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject InfoAcc(string userName, string userCode)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1005"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject UpdateAcc(string userName, string userCode, string fullName, string sex, string birthDay)
        {
            try
            {
                //Core.CheckToken();

                string body = "{'userName':'" + userName + "','fullName':'" + fullName + "','sex':'" + sex + "','birthDay':'" + birthDay + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1004"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject ChangPass(string userName, string passNew, string passOld)
        {
            try
            {
                //Core.CheckToken();
                //passNew = AESLib.encryptAES(passNew, Core.key); //ma hoa AES 
                //passNew = MD5Crypt.getMD5String(passNew); //ma hoa MD5
                //passOld = AESLib.encryptAES(passOld, Core.key); //ma hoa AES 
                //passOld = MD5Crypt.getMD5String(passOld); //ma hoa MD5
                string body = "{'userName':'" + userName + "', 'passNew':'" + passNew + "','passOld':'" + passOld + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1006"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GenOTPForget(string userName)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1007"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject Forgets(string userName, string otp)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("1107"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject CheckServices(string userName, string packge)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'serviceCode':'" + packge + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2040"), body, Core.jsonFooter);
                //logger.Info("result= " + result);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject ListPackge(string userName = "0", string isPackage = "2")
        {
            try
            {
                //Core.CheckToken();
                string body = "{'isPackage':'" + isPackage + "','userName':'" + userName + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3067"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //  EBook . VIDEO . Trieu phu , Luaat  .dK . 2030 .. huy 2031
        public static JObject AddPackgeTN(string userName, string packge, string otp = "123", int is3g = 0, string cpname = "admin")
        {
            try
            {

                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'serviceCode':'" + packge + "', 'is3G':'" + is3g + "','otp':'" + otp + "','cpname':'" + cpname + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2030"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public static JObject CancelPackgeTN(string userName, string packge)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'serviceCode':'" + packge + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2031"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        // dung chung co ca Book va
        public static JObject AddPackgeLTH(string userName, string packge, string otp = "123", int is3g = 0, string cpname = "admin")
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'serviceCode':'" + packge + "', 'is3G':'" + is3g + "','otp':'" + otp + "','cpname':'" + cpname + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2030"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject CancelPackgeLTH(string userName, string packge)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'serviceCode':'" + packge + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2031"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject PackgeSMS(string userName, string packge, string active = "")
        {
            try
            {

                //Core.CheckToken();
                string body = "{'userName':'" + userName + "', 'serviceCode':'" + packge + "', 'active':'" + active + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3085"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject GiffPackgeTN(string userName, string userGift, string packge, string otp, int is3g = 0)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','userGift':'" + userGift + "','serviceCode':'" + packge + "', 'is3G':'" + is3g + "','otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2034"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject GiffPackgeLTH(string userName, string userGift, string packge, string otp, int is3g = 0)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','userGift':'" + userGift + "','serviceCode':'" + packge + "', 'is3G':'" + is3g + "','otp':'" + otp + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2039"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject History(string userName, string fromday, string endday, string packge = "ALL", int page = 0, int pagesize = 10)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','frDate':'" + fromday + "', 'toDate':'" + endday + "','page':'" + page + "','pageSize':'" + pagesize + "', 'serviceCode':'" + packge + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2005"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);

                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region "Thu vien"
        public static JObject ListBookofRead(string userName = "0", string owner = "0", int page = 0, int pagesize = 30)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','page':'" + page + "','pageSize':'" + pagesize + "','owner':'" + owner + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3065"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject ListBookTran(string userName = "0", string owner = "1", int page = 0, int pagesize = 10)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','page':'" + page + "','pageSize':'" + pagesize + "','owner':'" + owner + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3066"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject ListVideoTran(string userName = "0", string owner = "1", int page = 0, int pagesize = 10)
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','page':'" + page + "','pageSize':'" + pagesize + "','owner':'" + owner + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3088"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject ListBookLike(string userName = "0", int page = 0, int pagesize = 30)
        {
            try
            {
                string body = "{'userName':'" + userName + "','page':'" + page + "','pageSize':'" + pagesize + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3064"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region "Save Log KPI"
        //log Log_cprequest (nhan dien so + charge viettel)
        public static JObject LogUrlmps(string cprequest, string txurl, string isurl)
        {
            //isURL
            try
            {
                if (string.IsNullOrEmpty(cprequest) || cprequest == "0") cprequest = "UNKNOW";
                if (string.IsNullOrEmpty(txurl) || txurl == "#") txurl = MakeLink.DefaultURLWap();
                if (string.IsNullOrEmpty(isurl) || isurl == "0") isurl = "M";//M: nh?n di?n s?, C: charge sub
                //Core.CheckToken();
                string body = "{'cprequest':'" + cprequest + "','txurl':'" + txurl + "','isurl':'" + isurl + "'}";
                logger.Info("LogUrlmps body: " + body);
                string result = Core.coreData.ProService(Core.jsonHeader("7001"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception ex)
            {
                logger.Info("LogUrlmps Exception =" + ex.Message);
                return null;
            }
        }
        public static JObject Log_MSISDN(string userName = "0", string status = "A")
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','status':'" + status + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2048"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject Log_Streaming(string userName = "0", string txurl = "", string status = "A")
        {
            try
            {
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','txurl':'" + txurl + "','status':'" + status + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2047"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Log_ReadFull(string bookId, string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || userName == "0") userName = "UNKNOW";
                //Core.CheckToken();
                string body = "{'bookId':'" + bookId + "','userName':'" + userName + "','type':'1'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3071"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Log_ReadDemo(string bookId, string userName)
        {

            try
            {
                if (string.IsNullOrEmpty(userName) || userName == "0") userName = "UNKNOW";
                //Core.CheckToken();
                string body = "{'bookId':'" + bookId + "','userName':'" + userName + "','type':'0'}";
                string result = Core.coreData.ProService(Core.jsonHeader("3072"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //log comfirm Viettel portal
        public static JObject Log_ComfirmViettelPortal(string userName, string packge, string status)
        {

            try
            {
                if (string.IsNullOrEmpty(userName) || userName == "0") userName = "UNKNOW";
                //Core.CheckToken();
                string body = "{'userName':'" + userName + "','status':'" + status + "','serviceCode':'" + packge + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("7024"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Search
        public static JObject GetListAdvan()
        {
            try
            {
                string body = "{}";
                string result = Core.coreData.ProService(Core.jsonHeader("3090"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region "Get 2g"
        public static string check2g(string username, string password)
        {
            JObject mlogmisdn = new JObject();
            String msisdn2 = "";//CheckDevice.GetNumberPhoneName();
            //msisdn2 = "841226457881";//"841202154614";//841202154614
            if (!string.IsNullOrEmpty(msisdn2) && msisdn2 == "200")
            {
                HttpContext.Current.Session["msisdn2"] = msisdn2;
            }
            else
            {
                HttpContext.Current.Session["msisdn"] = null;
                //log msisdn
                //mlogmisdn = MyControllers.Log_MSISDN(ipaddress, "F");
            }
            return msisdn2;
        }
        #endregion

        #region "CPC ADS"
        public static JObject Log_Ads_Add(string ads_url, string re_url, string cpname, string pcode, string sessionid, string mobile, string oper, string amount, string errorcode, string errordes, string agent, string status)
        {
            try
            {
                //if (string.IsNullOrEmpty(userName) || userName=="0") userName = "UNKNOW";
                //Core.CheckToken();
                string body = "{'ads_url':'" + ads_url + "','re_url':'" + re_url + "','cpname':'" + cpname + "','pcode':'" + pcode + "','sessionid':'" + sessionid + "','mobile':'" + mobile + "','oper':'" + oper + "','amount':'" + amount + "','errorcode':'" + errorcode + "','errordes':'" + errordes + "','agent':'" + agent + "','status':'" + status + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2063"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static JObject Log_Ads_Update(string sessionid, string pcode, string mobile, string errorcode, string errordes, string status)
        {
            try
            {
                //if (string.IsNullOrEmpty(userName) || userName=="0") userName = "UNKNOW";
                //Core.CheckToken();
                string body = "{'sessionid':'" + sessionid + "','pcode':'" + pcode + "','mobile':'" + mobile + "','errorcode':'" + errorcode + "','errordes':'" + errordes + "','status':'" + status + "'}";
                string result = Core.coreData.ProService(Core.jsonHeader("2064"), body, Core.jsonFooter);
                JObject mo = JObject.Parse(result);
                return mo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

    }

    public static class MakeLink
    {

        #region "Ebook"
        public static string MakeLinkEBook(string name, string id,string bty)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-xem-" + id +"-b-"+bty+".html";
        }
        public static string MakeLinkREADBook_audio(string name, string id)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-play-" + id + ".html";
        }
        public static string MakeLinkEREADBook_video(string name, string id)
        {
            return "/sach-video/" + Make_link(name.Trim()) + "-open-" + id + ".html";
        }
        public static string MakeLinkREADBook_image(string name, string id)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-view-" + id + ".html";
        }
        
        
        public static string MakeLinkEBookTab(string name, string id, string t)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-xem-" + id + "-" + t + ".html";
        }
        public static string MakeLinkERead(string name, string id)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-doc-" + id + ".html";
        }
        public static string MakeLinkEReadPage(string name, string id)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-doc-" + id;
        }
        public static string MakeTabEbook(string name, string id)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-t-" + id;
        }
        public static string MakeTabEbookPage(string name, string id)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-t-" + id;
        }
        public static string MakeCateEbook(string name, string id)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-c-" + id;
        }
        public static string MakeCateEbookPage(string name, string id)
        {
            return "/sach-dien-tu/" + Make_link(name.Trim()) + "-c-" + id;
        }
        public static string MakeCateTabEbook(string catename, string name, string catecode, string t)
        {
            return "/sach-dien-tu/" + Make_link(catename.Trim()) + "/" + Make_link(name.Trim()) + "-c-" + catecode + "-" + t;
        }
        public static string MakeA1Book_go_list(string name_cate, string btype, string id_hot, string name_hot) 
        {///id_hot =noi bat , moi nhat ,... 
         ///
            return "/book/" + Make_link(name_cate.Trim()) + "/" + Make_link(name_hot.Trim()) + "-a-" + btype + "-v-" + id_hot + ".html";
        }
        public static string MakeA1Book_go_list_cate(string name_cate, string btype, string id_hot, string name_hot,string cate_code)
        {
            return "/book/" + Make_link(name_cate.Trim()) + "/" + Make_link(name_hot.Trim()) + "-a-" + btype + "-v-" + id_hot + "-cate-" + cate_code + ".html";
        }

        #endregion

        #region Audiobook
        //Audiobook
        public static string MakeLinkAudioBook(string name, string id)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-xem-" + id + ".html";
        }
        public static string MakeLinkAudioBookTab(string name, string id, string t)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-xem-" + id + "-" + t + ".html";
        }

        public static string MakeLinkAudioRead(string name, string id)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-doc-" + id + ".html";
        }
        public static string MakeLinkAudioReadPage(string name, string id)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-doc-" + id;
        }
        public static string MakeTabAudiobook(string name, string id)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-t-" + id;
        }
        public static string MakeCateAudiobook(string name, string id)
        {
            return "/sach-noi/" + Make_link(name.Trim()) + "-c-" + id;
        }
        public static string MakeCateTabAudiobook(string catename, string name, string catecode, string t)
        {
            return "/sach-noi/" + Make_link(catename.Trim()) + "/" + Make_link(name.Trim()) + "-c-" + catecode + "-" + t;
        }
        #endregion

        #region truyen tranh
        //Ecomics
        public static string MakeTabPicturebook(string name, string id)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-t-" + id;
        }
        public static string MakeCatePicturebook(string name, string id)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-c-" + id;
        }
        public static string MakeCateTabPicturebook(string catename, string name, string catecode, string t)
        {
            return "/truyen-tranh/" + Make_link(catename.Trim()) + "/" + Make_link(name.Trim()) + "-c-" + catecode + "-" + t;
        }
        public static string MakeLinkPicturebook(string name, string id)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-xem-" + id + ".html";
        }
        public static string MakeLinkPicturebookTab(string name, string id, string t)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-xem-" + id + "-" + t + ".html";
        }
        public static string MakeLinkPictureRead(string name, string id)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-view-" + id + ".html";
        }
        public static string MakeLinkPictureReadPage(string name, string id)
        {
            return "/truyen-tranh/" + Make_link(name.Trim()) + "-view-" + id;
        }
        //
        #endregion
        //Law
        #region Luat+audio
        public static string MakeCateLaw(string name, string catecode)
        {
            return "/van-ban/" + Make_link(name.Trim()) + "-" + catecode;
        }
        public static string MakeLinkLaw(string namecate, string name, string id)
        {
            return "/van-ban/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + ".v" + id + ".html";
        }
        // law go new
        public static string Make_cate_Law_news_n1(string namecate, string cate_code)
        {
            return "/phap-luat/" + Make_link(namecate.Trim()) + "-cn-" + cate_code + "";
        }
        public static string Make_cate_Law_video_n1(string catename, string cate_code,string name,string type)
        {
            //return "/video-phap-luat/" + Make_link(namecate.Trim()) + "-cn-" + cate_code + "";
            return "/phap-luat/" + Make_link(catename.Trim()) + "/"+Make_link(name.Trim()) + "-" + cate_code + "-" + type + ".html";
        }
        public static string MakeLinkLawRead(string namecate, string name, string id)
        {
            return "/van-ban/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-doc-" + id + ".html";
        }
        public static string MakeLinkLawReadPage(string namecate, string name, string id)
        {
            return "/van-ban/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-doc-" + id;
        }
        //Audio
        public static string MakeCateAudio(string name, string catecode)
        {
            return "/radio/" + Make_link(name.Trim()) + "-" + catecode;
        }
        public static string MakeLinkAudio(string namecate, string name, string id)
        {
            return "/radio/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-" + id + ".html";
        }
        
        #endregion
        //video
        #region video
        public static string MakeCateVideo(string name, string catecode)
        {
            return "/video/" + Make_link(name.Trim()) + "-" + catecode;
        }

        public static string MakeCateVideo_tyle(string catename,string name, string catecode,string type)
        {
            return "/video/" + Make_link(catename.Trim()) +"/" + Make_link(name.Trim()) + "a" + catecode + "fun" + type + ".html";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"> VD Am nhac</param>
        /// <param name="catecode">0001</param>
        /// <param name="id_hot">1</param>
        /// <param name="name_hot"> moi nhat</param>
        /// <returns></returns>
        public static string MakeA1Video_go_list(string name, string catecode,string id_hot,string name_hot)
        {///id_hot =noi bat , moi nhat ,...
            return "/video/" + Make_link(name.Trim()) + "/" + catecode+"/"+ Make_link(name_hot.Trim()) +"-v-"+id_hot+".html";
        }

        public static string MakeVideo_go_detail(string name_video, string id_video)
        {///id_hot =noi bat , moi nhat ,...
            return "/video/" + Make_link(name_video.Trim()) + "v" + id_video +".html";
        }


        public static string MakeLinkVideo(string namecate, string name, string id)
        {
            //return "/video/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-" + id + ".html";
            return "/video/" + Make_link(name.Trim()) + "v" + id + ".html";
        }
        #endregion //phap-luat-video

        //tin tức
        #region
        public static string MakeCateNews(string name, string catecode)
        {
            return "/" + Make_link(name.Trim()) + "-cn-" + catecode;
        }
        public static string MakeNews(string namecate, string name, string id)
        {
            return "/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "view-" + id + ".html";
        }
        public static string Make_go_law(string namecate, string name, string id)
        {
            return "/van-ban/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-afun-" + id + ".html";
        }
        public static string Make_go_Video_law(string namecate, string name, string id)
        {
            //return "/video/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-" + id + ".html";
            return "/phap-luat-video/" + Make_link(name.Trim()) + "v" + id + ".html";
        }

        public static string Make_go_list_law(string namecate, string name, string id)
        {
            return "/van-ban/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-list-" + id + ".html";

        }
         public static string Make_detai_law(string namecate, string name, string id)
        {
            return "/van-ban/" + Make_link(namecate.Trim()) + "/" + Make_link(name.Trim()) + "-" + id + ".html";
        }
        //hoi dap
        public static string MakeQACate(string name, string catecode)
        {
            return "/hoi-dap/" + Make_link(name.Trim()) + "-" + catecode;
        }
        public static string MakeQA(string name, string id)
        {
            return "/hoi-dap/" + Make_link(name.Trim()) + "-" + id + ".html";
        }
        public static string URLLogin()
        {
            return MakeLink.DefaultURLWap() + "/dang-nhap.html";
        }
        public static string URLMsg()
        {
            return MakeLink.DefaultURLWap() + "/thong-bao.html";
        }
        public static string URLMsgBox()
        {
            return MakeLink.DefaultURLWap() + "/thong-bao-goi.html";
        }
        public static string URLRegister()
        {
            return MakeLink.DefaultURLWap() + "/dang-ky.html";
        }
        public static string URLRegister3g()
        {
            return MakeLink.DefaultURLWap() + "/dang-ky-3g.html";
        }
        public static string DefaultURLMedia()
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["LinkRes"]; ;
        }
        public static string DefaultURLWeb()
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["LinkWeb"];
        }
        public static string DefaultURLWap()
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["LinkWap"];
        }
        public static string ConvertVN(string chucodau)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ'";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY-";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = chucodau.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(chucodau[index]);
                chucodau = chucodau.Replace(chucodau[index], ReplText[index2]);
            }
            return chucodau;
        }
        private static string Make_link(string name)
        {
            string s = ConvertVN(name); s = s.Trim();
            s = s.Replace(" ", "-"); s = s.Replace(",", "-"); s = s.Replace("'", "-"); s = s.Replace("\"", "-");
            s = Regex.Replace(s, @"[^A-Za-z0-9_\.~,]+", "-");
            return s.ToLower();
        }
        public static string Page_MakeLink(string name)
        {
            return Make_link(ConvertVN(name));
        }
        #endregion
        public static string url_image(string url,string w,string h,string zc){
            string[] us = Regex.Split(url, ":");
            if (w == ""||h == "") {
                w = Iconfig.face_w;
                h = Iconfig.face_h;
            }
            string urld = "http://afun.vn/thumb/" + h + "/" + w + "/img/"+us[1].ToString();
            return urld;
        }

    }

    public class MyPage
    {
        public string NameURL { get; set; }
        public string PageParameter { get; set; }
        public int PageCurrent { get; set; }
        public int TotalRecord { get; set; }
    }
    public class CheckDevice
    {
        private static string[] arrMobi = new string[] { "midp", "j2me", "avantg", "ipad", "iphone", "docomo", "novarra", "palmos", "palmsource", "240x320", "opwv", "chtml", "pda", "windows ce", "mmp/", "mib/", "symbian", "wireless", "nokia", "hand", "mobi", "phone", "cdm", "up.b", "audio", "sie-", "sec-", "samsung", "htc", "mot-", "mitsu", "sagem", "sony", "alcatel", "lg", "erics", "vx", "nec", "philips", "mmm", "xx", "panasonic", "sharp", "wap", "sch", "rover", "pocket", "benq", "java", "pt", "pg", "vox", "amoi", "bird", "compal", "kg", "voda", "sany", "kdd", "dbt", "sendo", "sgh", "gradi", "jb", "dddi", "moto", "opera mobi", "opera mini", "android", "tablet" };

        private static string[] arrWindowphone = new string[] { "Windows Phone", "iemobile", "WPDesktop", "Lumia", "XBLWP7", "ZuneWP7", "MSIE" };

        private static string[] arrMAC = new string[] { "Mac Os", "Mobile Safari" };

        private static string[] mobileDevices = new string[] { "midp", "j2me", "avantg", "ipad", "iphone"
            , "docomo", "novarra", "palmos", "palmsource", "240x320", "opwv", "chtml"
            , "pda", "windows ce", "mmp/", "mib/", "symbian", "wireless", "nokia"
            , "hand", "mobi", "phone", "cdm", "up.b", "audio", "sie-", "sec-"
            , "samsung", "htc", "mot-", "mitsu", "sagem", "sony", "alcatel", "lg", "erics", "vx"
            , "nec", "philips", "mmm", "xx", "panasonic", "sharp", "wap", "sch", "rover", "pocket"
            , "benq", "java", "pt", "pg", "vox", "amoi", "bird", "compal", "kg", "voda", "sany", "kdd"
            , "dbt", "sendo", "sgh", "gradi", "jb", "dddi", "moto", "opera mobi", "opera mini", "android","tablet" };
        public static bool IsMobileDevice(string userAgent)
        {
            // TODO: null check
            userAgent = userAgent.ToLower();
            return arrMobi.Any(x => userAgent.Contains(x));
        }
        public static bool IsWindowphone(string userAgent)
        {
            // TODO: null check
            userAgent = userAgent.ToLower();
            return arrWindowphone.Any(x => userAgent.Contains(x));
        }
        public static bool IsMAC(string userAgent)
        {
            // TODO: null check
            userAgent = userAgent.ToLower();
            return arrMAC.Any(x => userAgent.Contains(x));
        }
        public static void login3g()
        {
            JObject mlogmisdn = new JObject();
            String msisdn = CheckDevice.GetNumberPhoneName();
            //msisdn = "841226457881";//"841202154614";//841202154614
            string ipaddress = Common.GetClientIpAddress();
            //msisdn = "84934413268"; 
            //ipaddress = "113.187.0.0";
            if (!string.IsNullOrEmpty(msisdn) && msisdn.StartsWith("84"))
            {
                int createStatus = -1;
                int num_pack = 0;
                string SERVICE_CODE, EXPIRE_DATE;
                int PLUS = -1;
                SERVICE_CODE = EXPIRE_DATE = "";
                //Core.CheckToken();
                JObject mo = new JObject();
                if (Common.accessList3g(ipaddress))
                {
                    HttpContext.Current.Session["msisdn"] = msisdn;
                    try
                    {
                        mo = MyControllers.Login(msisdn, "1234567", 1);
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

                                    HttpContext.Current.Session["PACKGE"] = num_pack;
                                }

                            }
                            mo = JObject.Parse(JArray.Parse(mo["USER"].ToString())[0].ToString());
                            HttpContext.Current.Session["LoginCode"] = mo["USER_CODE"].ToString();
                            HttpContext.Current.Session["LoginName"] = mo["USER_NAME"].ToString();
                            HttpContext.Current.Session["LoginDisplay"] = mo["FULL_NAME"].ToString();
                        }
                        //log msisdn
                        mlogmisdn = MyControllers.Log_MSISDN(msisdn, "A");
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    //log msisdn
                    mlogmisdn = MyControllers.Log_MSISDN(msisdn, "F");
                }
            }
            else
            {
                HttpContext.Current.Session["msisdn"] = null;
                //log msisdn
                mlogmisdn = MyControllers.Log_MSISDN(ipaddress, "F");
            }
        }

        public static void register3g(string phonenumber)
        {
            try
            {
                int createStatus = -1;
                string passpower = Common.CreatePass();
                JObject mo_reg = MyControllers.Register(phonenumber, passpower, 1);
                createStatus = Convert.ToInt32(mo_reg["Header"]["Code"].ToString());
                if (createStatus == 0)
                {
                    createStatus = -1;
                    JObject mo = MyControllers.Login(phonenumber, passpower, 1);
                    createStatus = Convert.ToInt32(mo["Header"]["Code"].ToString());
                    if (createStatus == 0)
                    {
                        mo = JObject.Parse(mo["Body"]["Data"].ToString());
                        if (mo["SERVICE"] != null && !mo["SERVICE"].ToString().StartsWith("[]"))
                        {
                            JObject mo_ser = JObject.Parse(JArray.Parse(mo["SERVICE"].ToString())[0].ToString());
                            //HttpContext.Current.Session["PACKGE"] = mo_ser["SERVICE_CODE"].ToString();
                        }
                        mo = JObject.Parse(JArray.Parse(mo["USER"].ToString())[0].ToString());
                        HttpContext.Current.Session["LoginCode"] = mo["USER_CODE"].ToString();
                        HttpContext.Current.Session["LoginName"] = mo["USER_NAME"].ToString();
                        HttpContext.Current.Session["LoginDisplay"] = mo["FULL_NAME"].ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void check3g()
        {
            JObject mlogmisdn = new JObject();
            String msisdn = CheckDevice.GetNumberPhoneName();
            //msisdn = "841226457881";//"841202154614";//841202154614
            string ipaddress = Common.GetClientIpAddress();
            if (!string.IsNullOrEmpty(msisdn) && msisdn.StartsWith("84"))
            {
                if (Common.accessList3g(ipaddress))
                {
                    HttpContext.Current.Session["msisdn"] = msisdn;
                    //log msisdn
                    mlogmisdn = MyControllers.Log_MSISDN(msisdn, "A");
                }
                else
                {
                    HttpContext.Current.Session["msisdn"] = null;
                    //log msisdn
                    mlogmisdn = MyControllers.Log_MSISDN(msisdn, "F");
                }
            }
            else
            {
                HttpContext.Current.Session["msisdn"] = null;
                //log msisdn
                mlogmisdn = MyControllers.Log_MSISDN(ipaddress, "F");
            }
        }

        public static string GetNumberPhoneName()
        {
            string MobilePhone = "VMS";
            String _IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //String _IP = "123.24.231.113";
            MobilePhone = System.Web.HttpContext.Current.Request.Headers["msisdn"];
            if (!string.IsNullOrEmpty(MobilePhone))
            {
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('1='" + MobilePhone + "')</script>");
                return System.Web.HttpContext.Current.Request.Headers["msisdn"];
            }
            else if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["X-WAP-MSISDN"]))
            {
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('2='" + System.Web.HttpContext.Current.Request.Headers["X-WAP-MSISDN"] + "')</script>");
                return System.Web.HttpContext.Current.Request.Headers["X-WAP-MSISDN"];
            }
            else if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["HTTP_MSISDN"]))
            {
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('3='" + System.Web.HttpContext.Current.Request.Headers["HTTP_MSISDN"] + "')</script>");
                return System.Web.HttpContext.Current.Request.Headers["HTTP_MSISDN"];
            }
            else if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["MSISDN"]))
            {
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('4='" + System.Web.HttpContext.Current.Request.Headers["MSISDN"] + "')</script>");
                return System.Web.HttpContext.Current.Request.Headers["MSISDN"];
            }
            else if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["HTTP_X_WAP_MSISDN"]))
            {
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('5='" + System.Web.HttpContext.Current.Request.Headers["msisdn"] + "')</script>");
                return System.Web.HttpContext.Current.Request.Headers["HTTP_X_WAP_MSISDN"];
            }
            else if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["X-Wap-MSISDN"]))
            {
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('6='" + System.Web.HttpContext.Current.Request.Headers["X-Wap-MSISDN"] + "')</script>");
                return System.Web.HttpContext.Current.Request.Headers["X-Wap-MSISDN"];
            }
            else
                //HttpContext.Current.Response.Write(@"<script language='javascript'>alert('7=else')</script>");
                return null;

        }

    }

    public class CoreTelco
    {
        private static ILog logger = log4net.LogManager.GetLogger(typeof(CoreTelco));
        public static string securepass = "WbNEss8sxm1MzEvi";//System.Configuration.ConfigurationManager.AppSettings["securepass"].ToString();
        public static string getUrlRequesComfirm(string msisdn, string package, string price, string infopkg, string trans_id="")
        {
            string urlcomfirm, backurl, sp_id;
            urlcomfirm  = backurl = sp_id = "";
            Random rand = new Random();

            //VMS cung cấp + data input
            sp_id = "015";
            urlcomfirm = System.Configuration.ConfigurationManager.AppSettings["linkrequestcomfirm"].ToString();
            backurl = System.Configuration.ConfigurationManager.AppSettings["linkbackurlcomfirm"].ToString();
            //backurl = HttpUtility.UrlEncode(backurl);
            if (string.IsNullOrEmpty(trans_id))
            {
                trans_id = msisdn + DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next(1, 9999);
            }
            string datarequest = "";
            datarequest = trans_id + "&" + package + "&" + price + "&" + backurl + "&" + infopkg;
            //logger.Info("datarequest =" + datarequest);

            // Xy ly ma hoa AES 128
            String datacrypt = AESLib.encryptAES(datarequest, CoreTelco.securepass);
            //logger.Info("datacrypt =" + datacrypt);

            urlcomfirm = urlcomfirm + "?sp_id=" + sp_id + "&link=" + datacrypt;
            //logger.Info("urlcomfirm =" + urlcomfirm);

            return urlcomfirm;

        }
        public static string getUrlRequesComfirmAds(string msisdn, string package, string price, string infopkg, string backurl)
        {
            string urlcomfirm, trans_id, sp_id;
            urlcomfirm = trans_id  = sp_id = "";
            Random rand = new Random();

            //VMS cung cấp + data input
            sp_id = "015";
            urlcomfirm = System.Configuration.ConfigurationManager.AppSettings["linkrequestcomfirm"].ToString();
            //backurl = System.Configuration.ConfigurationManager.AppSettings["linkbackurlcomfirm"].ToString();
            //backurl = HttpUtility.UrlEncode(backurl);
            trans_id = msisdn + DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next(1, 9999);

            string datarequest = "";
            datarequest = trans_id + "&" + package + "&" + price + "&" + backurl + "&" + infopkg;
            //logger.Info("datarequest =" + datarequest);

            // Xy ly ma hoa AES 128
            String datacrypt = AESLib.encryptAES(datarequest, CoreTelco.securepass);
            //logger.Info("datacrypt =" + datacrypt);

            urlcomfirm = urlcomfirm + "?sp_id=" + sp_id + "&link=" + datacrypt;
            //logger.Info("urlcomfirm =" + urlcomfirm);

            return urlcomfirm;

        }

    }

    public class ADSCPC
    {

    }
    public class Iconfig
    {
        public static string P_video = System.Configuration.ConfigurationSettings.AppSettings.Get("P_VIDEO");
        public static string P_law = System.Configuration.ConfigurationSettings.AppSettings.Get("P_LAW");
        public static string P_book = System.Configuration.ConfigurationSettings.AppSettings.Get("P_BOOK");
        public static string P_quiz = System.Configuration.ConfigurationSettings.AppSettings.Get("P_QUIZ");
        public static string P_new = System.Configuration.ConfigurationSettings.AppSettings.Get("P_NEW");
        public static string time = System.Configuration.ConfigurationSettings.AppSettings.Get("TIME");
        public static string face_h = System.Configuration.ConfigurationSettings.AppSettings.Get("H");
        public static string face_w = System.Configuration.ConfigurationSettings.AppSettings.Get("W");
        public int PageCurrent;

        public int TotalRecord { get; set; }
    }



    //MSISDN CHARGE SUB VIETTEL
    public class ResponeUrlVTL
    {
        private static ILog logger = log4net.LogManager.GetLogger(typeof(ResponeUrlVTL));

        public static String getUrlDetect(String sub, String source)//, String sessionid
        {
            String result = "";

            try
            {
                String urlReq = "";
                //if (!string.IsNullOrEmpty(ad))
                //{
                //    urlReq = ClassBase.linkdetect + "?SUB=" + sub + "&SOURCE=" + source + "&ad=1";//"&SESS=" + sessionid + 
                //}
                //else
                //{
                urlReq = System.Configuration.ConfigurationManager.AppSettings["linkdetect"].ToString();
                urlReq += "?SUB=" + sub + "&SOURCE=" + source;
                //}
                var request = (HttpWebRequest)WebRequest.Create(urlReq);
                using (WebResponse response = request.GetResponse())
                {
                    // Success
                    // Get the stream containing content returned by the server.
                    Stream dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                    //logger.Info("getUrlCharge REQUEST:" + urlReq + " - RESPONSE:" + responseFromServer);
                    // Display the content.
                    result = responseFromServer;
                    // Cleanup the streams and the response.
                    reader.Close();
                    dataStream.Close();
                    response.Close();
                }
            }
            catch (WebException exception)
            {
                using (WebResponse response = exception.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                        Console.WriteLine(streamReader.ReadToEnd());
                    result = "";
                }
            }
            return result;
        }

        public static String getUrlChargeSub(String cmd, String sub, String subcp, String item, String cate, String price, String cont, String mobile, String source)
        {
            String result = "";

            try
            {
                String urlReq = System.Configuration.ConfigurationManager.AppSettings["linkcharge"].ToString();
                urlReq += "?CMD=" + cmd + "&SUB=" + sub + "&SUB_CP=" + subcp + "&ITEM=" + item + "&CATE=" + cate + "&PRICE=" + price + "&CONT=" + cont + "&MOBILE=" + mobile + "&SOURCE=" + source;
                logger.Info("data inpup: " + urlReq);
                var request = (HttpWebRequest)WebRequest.Create(urlReq);
                using (WebResponse response = request.GetResponse())
                {
                    // Success
                    // Get the stream containing content returned by the server.
                    Stream dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                    //logger.Info("getUrlChargeSub REQUEST:" + urlReq + " - RESPONSE:" + responseFromServer);
                    // Display the content.
                    result = responseFromServer;
                    // Cleanup the streams and the response.
                    reader.Close();
                    dataStream.Close();
                    response.Close();
                }
            }
            catch (WebException exception)
            {
                using (WebResponse response = exception.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                        Console.WriteLine(streamReader.ReadToEnd());
                    result = "";
                }
            }
            return result;
        }

        public static String getUrlMpsResponse(String urlDesc)
        {
            String result = "";

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(urlDesc);
                using (WebResponse response = request.GetResponse())
                {
                    // Success
                    // Get the stream containing content returned by the server.
                    Stream dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                    // Display the content.
                    result = responseFromServer;
                    //logger.Info("getUrlMpsResponse REQUEST:" + urlDesc + " - RESPONSE:" + responseFromServer);

                    // Cleanup the streams and the response.
                    reader.Close();
                    dataStream.Close();
                    response.Close();
                }
            }
            catch (WebException exception)
            {
                using (WebResponse response = exception.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    //logger.Error("Error code:" + exception.Message);
                    //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                        Console.WriteLine(streamReader.ReadToEnd());
                    result = "";
                }
            }
            return result;
        }

    }
    

}