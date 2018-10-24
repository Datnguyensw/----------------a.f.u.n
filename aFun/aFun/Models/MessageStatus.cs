using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aFun.Models
{
    public class ValidateClient
    {
        //Respone client
        public const string EmptyMobile = "Chưa nhập số điện thoại.";
        public const string InvalidMobile = "Số điện thoại không hợp lệ.";
        public const string EmptyEmail = "Chưa nhập Email.";
        public const string InvalidEmail = "Email không hợp lệ.";
        public const string EmptyPassword = "Chưa nhập mật khẩu.";
        public const string InvalidPassword = "Mật khẩu không hợp lệ.";
        public const string InvalidComfirmPassword = "Xác nhận mật khẩu không chính xác.";
        public const string LengthPassword = "Mật khẩu phải có ít nhất 6 ký tự.";
        public const string EmptyCaptcha = "Chưa nhập mã chống spam";
        public const string InvalidCaptcha = "Mã chống spame không chính xác.";
        public const string EmptyCodeOTP = "Chưa nhập mã xác nhận.";
        public const string InvalidCodeOTP = "Mã xác nhận không hợp lệ.";
        public const string EmptyName = "Chưa nhập họ tên.";
        public const string EmptyNameDislay = "Chưa nhập tên hiển thị.";
        public const string EmptyBirthday = "Chưa nhập ngày sinh.";
        public const string InvalidBirthday = "Ngày sinh không hợp lệ(format:dd/mm/yyyy).";

    }
    public class SystemMessage
    {
        public string STRINGSTATUSMESSAGE(int code)
        {
            switch (code)
            {
                case 0:
                    return "Thành công";
                case 10:
                    return "Request hết hạn.";
                case 11:
                    return "Key AES fail";
                case 100:
                    return "Cmd not exists.";
                case 101:
                    return "Parameters fail.";
                case 102:
                    return "Format param fail.";
                case 103:
                    return "Warring.";
                case 104:
                    return "Exception.";
                case 105:
                    return "Not data Body.";
                case 1101:
                    return "Exists in DB.";
                case 1102:
                    return "Not Exist in DB.";
                case 1103:
                    return "Password < 6 .";
                case 1104:
                    return "Email fail.";
                default:
                    return "Hệ thống đang bận. Vui lòng quay lại sau ít phút.";
            }
        }

    }
    public class RegisterMessage
    {
        //Respone server
        public const string Success = "Đăng ký thành công!";
        public const string SuccessExit = "Tài khoản tồn tại.";
        public const string InvalidParameter = "Tham số truyền vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi.";
        public const string SuccessActive = "Kích hoạt tài khoản thành công!";
        public const string ErrorActive = "Kích hoạt thất bại!";
    }
    public class ActiveRegMessage
    {
        //Respone server
        public const string Success = "Kích hoạt tài khoản thành công!";
        public const string SuccessExit = "Tài khoản không tồn tại.";
        public const string InvalidParameter = "Tham số truyền vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi.";
        public const string SuccessActive = "Kích hoạt thành công!";
        public const string ErrorActive = "Kích hoạt thất bại!";
    }
    public class LoginMessage
    {
        public const string Success = "Đăng nhập thành công";
        public const string SuccessExit = "Tài khoản không tồn tại";
        public const string InvalidParameter = "Tài khoản hoặc mật khẩu không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class InfoAccMessage
    {
        public const string Success = "Cập nhật thông tin cá nhân thành công.";
        public const string Error = "Cập nhật thông tin thất bại.";
        public const string InvalidParameter = "Thông tin cập nhật không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class ForgetMessage
    {
        public const string Success = "Lấy lại mật khẩu thành công.";
        public const string Error = "Lấy lại mật khẩu thất bại.";
        public const string InvalidParameter = "Dữ liệu đầu vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class ChangPassMessage
    {
        public const string Success = "Đổi mật khẩu thành công.";
        public const string Error = "Đổi mật khẩu thất bại.";
        public const string InvalidParameter = "Dữ liệu đầu vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class BuyStatusMessage
    {
        public const string Success = "Mua thành công";
        public const string SuccessExit = "Tài khoản tồn tại";
        public const string Error = "Đổi mật khẩu thất bại.";
        public const string InvalidParameter = "Dữ liệu đầu vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class GiffStatusMessage
    {
        public const string Success = "Tặng thành công";
        public const string SuccessExit = "Tài khoản tồn tại";
        public const string Error = "Tặng thất bại.";
        public const string InvalidParameter = "Dữ liệu đầu vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class PackgeStatusMessage
    {
        public const string Success7 = "Mua gói tuần thành công";
        public const string SuccessExit7 = "Hủy gói tuần thành công";
        public const string ErrorExit7 = "Hủy gói tuần thất bại";
        public const string Error7 = "Mua gói tuần thất bại.";

        public const string Success30 = "Mua gói tháng thành công";
        public const string SuccessExit30 = "Hủy gói tháng thành công";
        public const string ErrorExit30 = "Hủy gói tháng thất bại";
        public const string Error30 = "Mua gói tháng thất bại.";

        public const string InvalidParameter = "Dữ liệu đầu vào không hợp lệ.";
        public const string ServerError = "Lỗi trong quá trình thực thi";
    }
    public class MessageCodes
    {
        private static MessageCodes _messageCode;

        public MessageCodes(string cultureCode)
        {
            ServerError = "System has error!";
            Success = "Command executed sucessfully!";
            Restricted = "Command is restricted";
            InvalidRequest = "Invalid request!";
            InvalidToken = "Invalid token string!";
            InvalidSessionId = "Invalid session ID!";
            InvalidParameter = "Invalid parameters!";
            ModuleNotFound = "Module is not found!";
            DeviceNotAllowed = "Device is not allowed!";
            AppNotAllowed = "Appication is not allowed!";
            CommandNotFound = "Command is not found!";
            DataPostBlank = "Data post is blank!";
            DataPostInvalid = "Invalid data post!";
        }

        public static void Init(string cultureCode)
        {
            _messageCode = new MessageCodes(cultureCode);
        }

        public static MessageCodes Instance()
        {
            if (_messageCode == null)
                _messageCode = new MessageCodes("");
            return _messageCode;
        }

        #region Public Properties

        public string ServerError { get; private set; }

        public string Success { get; private set; }

        public string Restricted { get; private set; }

        public string InvalidRequest { get; private set; }

        public string InvalidToken { get; private set; }

        public string InvalidSessionId { get; private set; }

        public string InvalidParameter { get; private set; }

        public string ModuleNotFound { get; private set; }

        public string CommandNotFound { get; private set; }

        public string AppNotAllowed { get; private set; }

        public string DeviceNotAllowed { get; set; }

        public string DataPostBlank { get; private set; }

        public string DataPostInvalid { get; private set; }

        #endregion
    }
}