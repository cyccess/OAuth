using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Service.Common
{
    /// <summary>
    /// 加密 解密服务
    /// </summary>
    public abstract class EncryptHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string Encrypt(string inputString)
        {
            string strEncryptConn = "";
            try
            {
                AESEDSVcrypts.AES_Encrypt(inputString, out strEncryptConn);
            }
            catch
            {
                strEncryptConn = "API出错了";
            }
            return strEncryptConn;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string Decrypt(string inputString)
        {
            string strEncryptConn = "";
            try
            {
                if (!AESEDSVcrypts.AES_Decrypt(inputString, out strEncryptConn))
                {
                    strEncryptConn = "";
                }
            }
            catch
            {
                strEncryptConn = "API出错了";
            }
            return strEncryptConn;
        }
    }
}
