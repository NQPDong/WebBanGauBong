using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebBanGauBong.Services
{
    public class MoMoService
    {
        public static async Task<string> CreatePaymentRequest(string partnerCode, string accessKey, string secretKey, long amount, string orderId, string requestId, string orderInfo)
        {
            
            string endpoint = ConfigurationManager.AppSettings["MoMoEndpoint"];
            string returnUrl = ConfigurationManager.AppSettings["MoMoReturnUrl"];
            string notifyUrl = ConfigurationManager.AppSettings["MoMoNotifyUrl"];
            string requestType = "captureWallet"; 
           
            string rawData = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={notifyUrl}" +
                             $"&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}" +
                             $"&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";

          
            string signature = GenerateSignature(rawData, secretKey);

          
            var message = new
            {
                partnerCode,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                extraData = "",
                requestType,
                signature,
                lang = "vi"
            };

           
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                return result.payUrl; 
            }
        }

        
        private static string GenerateSignature(string text, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(text);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }

    }
}