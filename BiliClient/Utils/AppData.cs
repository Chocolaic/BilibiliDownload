using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.Utils
{
    class AppData
    {
        private static string Key { get
            {
                return "EXAMPLE VERSION";
            } }

        internal static string getValue(string Str)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(Key);
            byte[] toEncryptArray = Convert.FromBase64String(Str);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
