using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanGauBong.Models
{
    public class TempRating
    {
        public int Star { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [AllowHtml]
        public string Comment { get; set; }
        public string Image { get; set; }
        public DateTime RateDate { get; set; }
    }
}