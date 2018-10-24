using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace aFun.Models
{


    #region "Acc"
    public class RegisterAccModel
    {
        public virtual string CaptchaValue { get; set; }
        
        [Display(Name = "Số điện thoại")]
        public string MobileNumber { get; set; }

        [StringLength(100, ErrorMessage = "{0} phải ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
       
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và nhập lại mật khẩu không khớp nhau.")]
        public string ConfirmPassword { get; set; }

    }
    public class ActiveRegAccModel
    {
        public virtual string CaptchaValue { get; set; }

        public string MobileNumber { get; set; }

        public string Otp { get; set; }
    }
    public class LoginAccModel
    {
        public virtual string CaptchaValue { get; set; }

        [Display(Name = "Số điện thoại")]
        public string MobileNumber { get; set; }

        [StringLength(100, ErrorMessage = "{0} phải ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
    }
    public class ForgetAccModel
    {
        public virtual string CaptchaValue { get; set; }

        public string MobileNumber { get; set; }

        public string CodeOTP { get; set; }
    }
    public class InfoAccModel
    {
        public string Point { get; set; }
        public string TopLevel { get; set; }
        public string Fullname { get; set; }
        public string Birthday { get; set; }
        public int SelectedSex { get; set; }
        public List<SelectListItem> ListSex { get; set; }
    }
    public class ChangePassAccModel
    {
        
        public string Password { get; set; }

        [StringLength(100, ErrorMessage = "{0} phải ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Passwordnew { get; set; }

        [Compare("Passwordnew", ErrorMessage = "Xác nhận mật khẩu không chính xác.")]
        public string Passwordnewconfirm { get; set; }
    }
    public class HistoryAccModel
    {

        public int SelectedFromDayId { get; set; }
        public List<SelectListItem> DaysFrom { get; set; }

        public int SelectedFromMonthId { get; set; }
        public List<SelectListItem> MonthsFrom { get; set; }

        public int SelectedFromYearId { get; set; }
        public List<SelectListItem> YearsFrom { get; set; }

        public int SelectedToDayId { get; set; }
        public List<SelectListItem> DaysTo { get; set; }

        public int SelectedToMonthId { get; set; }
        public List<SelectListItem> MonthsTo { get; set; }

        public int SelectedToYearId { get; set; }
        public List<SelectListItem> YearsTo { get; set; }

        public string SelectedServiceCode { get; set; }
        public List<SelectListItem> ServiceList { get; set; }
    }
    #endregion
    #region "Transaction"
    public class BuyNotLogInViewModel
    {
        public virtual string CaptchaValue { get; set; }

        public string CodeOTP { get; set; }

        public string MobileNumber { get; set; }

        public string BookId { get; set; }

        public string BookName { get; set; }

        public string Price { get; set; }

        public string Btype { get; set; }

    }
    public class GiffNotLogInViewModel
    {
        public virtual string CaptchaValue { get; set; }

        public string CodeOTP { get; set; }

        public string MobileNumber { get; set; }

        public string GiffMobileNumber { get; set; }

        public string BookId { get; set; }

        public string BookName { get; set; }

        public string Price { get; set; }

        public string Btype { get; set; }

    }
    public class PackgeAccModel
    {
        public string UseName { get; set; }
        public string CodeOTP { get; set; }
        public string SelectedPackge { get; set; }
        public List<CheckBoxCate> ListPackge { get; set; }
    }
    public class CheckBoxCate
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public int REMAIN { get; set; }
        public string reUrl { get; set; }
    }
    public class LAW_QAModel
    {
        public virtual string CaptchaValue { get; set; }

        [Required(ErrorMessage = "Quý khách chưa nhập Tiêu đề")]
        [Display(Name = "Tiêu đề")]
        public string TITLE { get; set; }

        //[Required(ErrorMessage = "Quý khách chưa nhập Email")]
        //[Display(Name = "Email")]
        //public string EMAIL { get; set; }

        [Required(ErrorMessage = "Quý khách chưa nhập nội dung")]
        [Display(Name = "Nội dung")]
        public string TXDESC { get; set; }
    }
    public class CommentModel
    {
        public virtual string CaptchaValue { get; set; }

        [Required(ErrorMessage = "Quý khách chưa nhập Tiêu đề")]
        [Display(Name = "Tiêu đề")]
        public string TITLE { get; set; }

        //[Required(ErrorMessage = "Quý khách chưa nhập Email")]
        //[Display(Name = "Email")]
        //public string EMAIL { get; set; }

        [Required(ErrorMessage = "Quý khách chưa nhập nội dung")]
        [Display(Name = "Nội dung")]
        public string TXDESC { get; set; }
    }
    #endregion
    #region "Home"
    public class CallBackHomeModel
    {
        public virtual string CaptchaValue { get; set; }

        [Display(Name = "Họ tên")]
        public string Fullname { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhomeNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Tự đề")]
        public string Title { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Display(Name = "Nhận thông tin")]
        public bool IsSend { get; set; }

    }
    public class ContactHomeModel
    {
        public virtual string CaptchaValue { get; set; }

        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string MobileNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }
    }
    public class SearchObjectModel
    {
        public string keyword { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        //public string SelecltTimes { get; set; }
        //public List<SelectListItem> listTimes { get; set; }
        public string SelectedDoctype { get; set; }
        public List<SelectListItem> listDoctype { get; set; }
        public string SelectedIssusers { get; set; }
        public List<SelectListItem> listIssusers { get; set; }
        public string SelectedCate { get; set; }
        public List<SelectListItem> listCate { get; set; }
    }
    public class SearchAllObjectModel
    {
        public string keyword { get; set; }
    }
    #endregion

#region "ADS CPC"
    public class INFOCPCADS
    {
        public string msisdn { get; set; }
        public string sid { get; set; }
        public string cp { get; set; }
        public string sc { get; set; }
        public string trans_id { get; set; }
    }
#endregion
}