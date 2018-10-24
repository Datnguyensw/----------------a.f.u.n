using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lemon.wapgw.cryptengine
{
    public static  class AESLib
    {
        public static String encryptAES(String data, String aeskey)
        {
            try
            {
                //1.AES
                string crypto = EncryptedString.EncryptString(data, aeskey);
                return crypto;
            }
            catch (Exception exception)
            {
                return "";
            }
        }

        public static String decryptAES(String encryptdata, String aeskey)
        {
            try
            {
                //2.AES
                String decrypt = EncryptedString.DecryptString(encryptdata, aeskey);
                return decrypt;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }

    }
}
