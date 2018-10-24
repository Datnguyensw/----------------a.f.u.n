using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lemon.wapgw.cryptengine
{
   public static class SecurityUtil
    {
        //This is the public key
        private const string public_key =
            "<RSAKeyValue><Modulus>poS8u8kjqGVUbYl0bU9jc+ztmwD2Ybvm6vjKxQOAbyTBabBHD7ti9INF5hdyswCnjjPSIWQo+61s0VQbaIL7TmTY769Q+550YTnhjji+s3PLWYmSpfXcHdyVDfrGv18z1Q25/mDd7Mtdn0YxN8IAo5tWUYBqiaiUMZGSQYZ6gjs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        //This is the private and public key.
        private const String private_public_key =
            "<RSAKeyValue><Modulus>poS8u8kjqGVUbYl0bU9jc+ztmwD2Ybvm6vjKxQOAbyTBabBHD7ti9INF5hdyswCnjjPSIWQo+61s0VQbaIL7TmTY769Q+550YTnhjji+s3PLWYmSpfXcHdyVDfrGv18z1Q25/mDd7Mtdn0YxN8IAo5tWUYBqiaiUMZGSQYZ6gjs=</Modulus><Exponent>AQAB</Exponent><P>2F5uvQhney5wUoKlGrCFvs6liyESMaSf5OT6At7czz/f1YLRdmQjrH1Njg3rwxlVaJWYvPDVBhkZSWeb7b/dlQ==</P><Q>xQTQSTF5dHPPMZCx2yCLdhOCUEVl4NJsSl/YItJWJThBooezLx/Jro8mTEkG6fGZlbWzITDbH3oA7Kx7eA/Mjw==</Q><DP>IFddLhxHe34hg/PLQgYIt9Cjpfw3f/wfelNPm4Y8cy4VAxEexheJzYqdQRKLnwn2Xj+eKP/Gl7bAtNwrzonTUQ==</DP><DQ>vfOAJbFvmhk2AGGzOMPN8g+pKl9edD4sbiCuMBR/Pe9ZSKxw09RgDM6zbAVUhyWKoE7I4A1MlrJ4RbeeawlAfQ==</DQ><InverseQ>PpaxdTBbM5nfBsBUWoB7/Q4BMe8bbAFo13sgECSzkkUmpGjschemmQMAg6GwcPH/weise+ap8vWTkrZFiVpMyw==</InverseQ><D>RF8KE1yyDGmWsecXa+6F2kp1AQjzIQwC84W11BDtAjWoSJn9g4MgTV89Kw2DpeXl5mh2pgk3HqeFn6mWG2J0hoD7XM8A0o7nbrn0cxJC+jQ0kZZ/Q1Qz4n80C20hjOciLMmQJD595HkciuSc2XKStFcq+GQ63zyADTQe6HNDUrk=</D></RSAKeyValue>";
        private static RSALib rsaLib = new RSALib();

        public static String encrypt(String data, String aeskey)
        {
            try
            {
                //1.AES
                string crypto = EncryptedString.EncryptString(data, aeskey);
                //2.RSA
                byte[] CTX = rsaLib.encrypt(crypto, private_public_key);
                String cipherText = Convert.ToBase64String(CTX);
                return cipherText;
            }
            catch (Exception exception)
            {
                return "";
            }
        }

       public static String decrypt(String encryptdata, String aeskey)
        {
            try
            {
                //1.RSA
                byte[] CTX = bas64toByte(encryptdata);
                String aescrypt = rsaLib.decrypt(CTX, private_public_key);

                //2.AES
                String decrypt = EncryptedString.DecryptString(aescrypt, aeskey);
                return decrypt;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
          
        }


       public static byte[] bas64toByte(String encryptdata)
       {
           byte[] binaryData;
           try
           {
               binaryData =
                  System.Convert.FromBase64String(encryptdata);
               return binaryData;
           }
           catch (System.ArgumentNullException)
           {
               System.Console.WriteLine("Base 64 string is null.");
               return null;
           }
           catch (System.FormatException)
           {
               System.Console.WriteLine("Base 64 string length is not " +
                  "4 or is not an even multiple of 4.");
               return null;
           }
       }


    }
}
