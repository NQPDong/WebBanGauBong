using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Areas.Admin.Security;
using WebBanGauBong.Models;
using PagedList;
using PagedList.Mvc;

namespace WebBanGauBong.Areas.Admin.Controllers
{
    [CheckAthourize(Roles = "Admin")]
    public class OrderController : Controller
    {
        // GET: Admin/Order
        QL_THU_BONG csdl = new QL_THU_BONG();
        public ActionResult OrderPage(int? page)
        {
            List<Orders> ds = csdl.Orders.ToList();
            return View(ds.ToPagedList(page ?? 1, 10));
        }

        public ActionResult _ModalSuaOrder(int orderID)
        {
            Orders od = csdl.Orders.FirstOrDefault(t => t.OrderID == orderID);
            return PartialView(od);
        }

        public ActionResult UpdateOrderStatus(Orders order)
        {
            try
            {
                Orders od = csdl.Orders.FirstOrDefault(t => t.OrderID == order.OrderID);
                od.Status = order.Status;
                csdl.SaveChanges();

                TempData["UpdateOrderSuccess"] = $"Cập nhật đơn hàng thành công";

                return RedirectToAction("OrderPage");
            }
            catch(Exception e)
            {
                TempData["UpdateOrderError"] = $"Cập nhật đơn hàng thất bại \n {e.Message}";
                return RedirectToAction("OrderPage");
            }
        }

        public ActionResult TimKiemDonHang(string keyword, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                List<Orders> ds = csdl.Orders.ToList().FindAll(t => (t.OrderID.ToString().Equals(keyword) || t.Users.Name.ToLower().Contains(keyword.ToLower())));
                return View("OrderPage", ds.ToPagedList(1, 10));
            }
            else
            {
                List<Orders> ds = csdl.Orders.ToList().FindAll(t => (t.OrderID.ToString().Equals(keyword) || t.Users.Name.ToLower().Contains(keyword.ToLower()) && t.Status.Equals(status)));
                return View("OrderPage", ds.ToPagedList(1, 10));
            }
        }


    }


}