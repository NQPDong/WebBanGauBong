using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanGauBong.Models
{
    public class MoMoResponse
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderInfo { get; set; }
        public string orderType { get; set; }
        public string transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payUrl { get; set; }
        public string signature { get; set; }
    }
}