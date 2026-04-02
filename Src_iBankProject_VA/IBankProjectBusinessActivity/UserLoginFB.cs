using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using System.Configuration;
using eCATBusinessServiceProtocol;
using System.Threading;
using UIServiceProtocol;
using FaceRecognizeServiceProtocol;
using IBankProjectBusinessActivityBase;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{9C6C06A8-2C3A-4FDB-98B9-09535E77E6B2}",
                NodeNameOfConfiguration = "UserLoginFB",
                Name = "UserLoginFB",
                Author = "")]
    public class UserLoginFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new UserLoginFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected UserLoginFB()
        {

        }
        #endregion

        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            object obj = null;
            string strpublicKey = string.Empty;
            
            m_objContext.TransactionDataCache.Get("FB_publicKey", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FB_publicKey is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strpublicKey = obj as string;

            //Log.Action.LogDebugFormat("strpublicKey:{0}", strpublicKey);

            string password = RSAPublicKeyEncrypt(strpublicKey, "admin123");

            ProjVTMContext.CardHolderDataCache.Set("FB_userName", "admin", GetType());
            ProjVTMContext.CardHolderDataCache.Set("FB_userPassword", password, GetType());

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }


        /// <summary>    
        /// RSA公钥pem-->XML格式转换， 
        /// </summary>    
        /// <param name="publicKey">pem公钥</param>    
        /// <param name="content">要加密的内容：比如密码</param>  
        /// <returns></returns>    
        public string RSAPublicKeyEncrypt(string publicKey, string content)
        {
            Log.Action.LogDebug("Run RSAPublicKeyEncrypt ");
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            string XML = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(XML);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            rsa.Dispose();
            return Convert.ToBase64String(cipherbytes);
        }
    }
}
