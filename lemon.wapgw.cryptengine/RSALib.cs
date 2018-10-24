using ArpanTECH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace lemon.wapgw.cryptengine
{
    public class RSALib
    {
     
        public byte[] encrypt(String toEncrypt, String keyInfo)
        {
            RSAx rsax = new RSAx(keyInfo, 1024);
            // Private key encryption
            byte[] CTX = rsax.Encrypt(Encoding.UTF8.GetBytes(toEncrypt), true, true);
            return CTX;
        }
        public String decrypt(byte[] CTX, String keyInfo)
        {
            RSAx rsax = new RSAx(keyInfo, 1024);
            // Public key decryption
            byte[] PTX = rsax.Decrypt(CTX, false, true);
            string plaintext = Encoding.UTF8.GetString(PTX);
            return plaintext;
        }

    }
}