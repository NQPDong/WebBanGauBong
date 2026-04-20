using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Areas.Admin.Security;
using WebBanGauBong.Models;

namespace WebBanGauBong.Areas.Admin.Controllers
{
    [CheckAthourize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        QL_THU_BONG csdl = new QL_THU_BONG();
        // GET: Admin/Dashboard
        public ActionResult DashboardPage()
        {
            ViewBag.TongSanPham = csdl.Product.Count();
            ViewBag.TongDonHang = csdl.Orders.Count();
            ViewBag.TongKhachHang = csdl.Users.Count();
            ViewBag.TongDoanhThu = csdl.Orders.Sum( t =>
                (t.OrderDetail.Sum(d => d.Quantity * d.UnitPrice)) - (t.Discount ?? 0)
                );
            return View();
        }
    }


}