using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Models;
using WebBanGauBong.Services;

namespace WebBanGauBong.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        QL_THU_BONG csdl = new QL_THU_BONG();
        public ActionResult CartPage()
        {
            ShoppingCart cart;
            if (Session["User"] != null)
            {
                Users user = Session["User"] as Users;
                cart = csdl.ShoppingCart.FirstOrDefault(t => t.UserID == user.UserID);
                Session["Cart"] = cart;
            }
            else
            {
                return RedirectToAction("LoginPage", "User");
            }
            return PartialView(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int productSizeID, int quantity = 1)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("LoginPage", "User");
            }

            ShoppingCart cart = Session["Cart"] as ShoppingCart;
            ShoppingCartItem sp = csdl.ShoppingCartItem.FirstOrDefault(t => t.ProductSizeID == productSizeID && t.ShoppingCartID == cart.ShoppingCartID);
            try
            {
                if (sp == null)
                {
                    ProductSize pd = csdl.ProductSize.FirstOrDefault(t => t.ProductSizeID == productSizeID);
                    sp = new ShoppingCartItem
                    {
                        ShoppingCartID = cart.ShoppingCartID,
                        ProductSizeID = productSizeID,
                        Quantity = quantity,
                        UnitPrice = pd.Price
                    };

                    csdl.ShoppingCartItem.Add(sp);
                }
                else
                {
                    sp.Quantity += quantity;
                    //csdl.Entry(sp).State = EntityState.Modified;
                }
                csdl.SaveChanges();

                // Cập nhật lại session cart
                Session["Cart"] = csdl.ShoppingCart.FirstOrDefault(t => t.ShoppingCartID == cart.ShoppingCartID);

                // Cờ hiệu
                TempData["ShowCart"] = true;

                return RedirectToAction("Detail", "Product", new { id = sp.ProductSize.Product.ProductID });
            }
            catch (Exception e)
            {
                ViewBag.Error = $"Lỗi thêm không thành công\n {e.Message}";
                return RedirectToAction("Detail", "Product", new { id = sp.ProductSize.Product.ProductID });
            }
        }

        public ActionResult UpdateQuantity(int productSizeID, int type) // 1 tang, -1 giam
        {
            ShoppingCart cart = Session["Cart"] as ShoppingCart;
            ShoppingCartItem sp = csdl.ShoppingCartItem.FirstOrDefault(t => t.ProductSizeID == productSizeID && t.ShoppingCartID == cart.ShoppingCartID);

            try
            {
                if (type == 1)
                {
                    sp.Quantity += 1;
                }
                else
                {
                    sp.Quantity -= 1;
                    if (sp.Quantity <= 0)
                    {
                        csdl.ShoppingCartItem.Remove(sp);
                    }
                }
                csdl.SaveChanges();

                // Cập nhật lại session cart
                Session["Cart"] = csdl.ShoppingCart.FirstOrDefault(t => t.ShoppingCartID == cart.ShoppingCartID);

                // Cờ hiệu
                TempData["ShowCart"] = true;

                return RedirectToAction("Detail", "Product", new { id = sp.ProductSize.Product.ProductID });
            }
            catch (Exception e)
            {
                ViewBag.Error = $"Lỗi thay đổi không thành công\n {e.Message}";
                return RedirectToAction("Index", "Product");
            }
        }

        public ActionResult DeleteProductInCart(int productSizeID)
        {
            ShoppingCart cart = Session["Cart"] as ShoppingCart;
            ShoppingCartItem sp = csdl.ShoppingCartItem.FirstOrDefault(t => t.ProductSizeID == productSizeID && t.ShoppingCartID == cart.ShoppingCartID);

            try
            {

                csdl.ShoppingCartItem.Remove(sp);

                csdl.SaveChanges();

                // Cập nhật lại session cart
                Session["Cart"] = csdl.ShoppingCart.FirstOrDefault(t => t.ShoppingCartID == cart.ShoppingCartID);

                // Cờ hiệu
                TempData["ShowCart"] = true;

                return RedirectToAction("Index", "Product");
            }
            catch (Exception e)
            {
                ViewBag.Error = $"Lỗi xóa không thành công\n {e.Message}";
                return RedirectToAction("Index", "Product");
            }
        }

        [HttpPost]
        public ActionResult DatHangOnSubmit(FormCollection form)
        {

            string name = string.IsNullOrEmpty(form["nameTang"].ToString()) == false ? form["nameTang"].ToString() : form["name"].ToString();
            string sdt = string.IsNullOrEmpty(form["sdtTang"].ToString()) == false ? form["sdtTang"].ToString() : form["sdt"].ToString();
            string diaChi = form["diaChi"].ToString();
            decimal goiQua = string.IsNullOrEmpty(form["goiQua"].ToString()) == false ? decimal.Parse(form["goiQua"].ToString()) : 0;
            string thanhToan = form["thanhToan"].ToString();
            string khuyenMai = string.IsNullOrEmpty(form["khuyenMai"]) == false ? form["khuyenMai"].ToString() : "noDiscount";

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(diaChi))
            {
                TempData["ShowCart"] = true;
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return RedirectToAction("Index", "Product");
            }

            if (name.Length > 60 || sdt.Length > 20 || diaChi.Length > 255)
            {
                TempData["ShowCart"] = true;
                ViewBag.Error = "Thông tin nhập không hợp lệ";
                return RedirectToAction("Index", "Product");
            }

            try
            {
                Users user = Session["User"] as Users;
                ShoppingCart cart = csdl.ShoppingCart.FirstOrDefault(t => t.UserID == user.UserID);
                if (cart == null || cart.ShoppingCartItem.Count == 0)
                {
                    ViewBag.Error = "Giỏ hàng trống";
                    TempData["ShowCart"] = true;
                    return RedirectToAction("Index", "Product");
                }

                Orders order = new Orders();
                order.UserID = user.UserID;
                order.OrderDate = DateTime.Now;
                order.Address = diaChi;
                order.Status = "Đang chờ xác nhận";
                order.UserPaymentMethod = thanhToan;
                order.SDT = sdt;
                order.NameUser = name;

                csdl.Orders.Add(order);
                csdl.SaveChanges();

                foreach (var item in cart.ShoppingCartItem.ToList())
                {
                    OrderDetail od = new OrderDetail();
                    od.OrderID = order.OrderID;
                    od.ProductSizeID = item.ProductSizeID;
                    od.Quantity = item.Quantity;
                    od.UnitPrice = item.UnitPrice;

                    csdl.OrderDetail.Add(od);

                    ProductSize pd = csdl.ProductSize.FirstOrDefault(t => t.ProductSizeID == od.ProductSizeID);
                    if (od.Quantity <= pd.StockQuantity)
                    {
                        pd.StockQuantity -= od.Quantity;
                    }
                    else
                    {
                        pd.StockQuantity = 0;
                    }

                        csdl.ShoppingCartItem.Remove(item);
                }

                order.Discount = 0;
                if (khuyenMai.Equals("giam50") && order.TamTinh() > 1000000)
                {
                    order.Discount = 50000 - goiQua;
                }
                else if (khuyenMai.Equals("giam30") && order.TamTinh() > 300000)
                {
                    order.Discount = 30000 - goiQua;
                }

                csdl.SaveChanges();

                // Cờ hiệu
                TempData["ShowOrder"] = true;
                TempData["ShowCart"] = false;

                Session["Order"] = order;
                Session["Cart"] = csdl.ShoppingCart.FirstOrDefault(t => t.ShoppingCartID == cart.ShoppingCartID);

                TempData["OrderSuccess"] = "Đặt hàng thành công!";
                return RedirectToAction("Index", "Product");
            }
            catch(Exception e)
            {
                // Cờ hiệu
                TempData["ShowOrder"] = false;
                TempData["ShowCart"] = true;
                TempData["OrderError"] = $"Đặt hàng thất bại {e.Message}";
                return RedirectToAction("Index", "Product");
            }
        } 

        public ActionResult OrderPage()
        {
            TempData["ShowCart"] = false;
            Orders orderSession = Session["Order"] as Orders;
            if (orderSession != null)
            {
                Orders order = csdl.Orders.FirstOrDefault(t => t.OrderID == orderSession.OrderID);
                return PartialView(order);
            }
            return PartialView(new Orders { OrderDetail = new List<OrderDetail>() });
        }

        public ActionResult OrderHistory()
        {
            Users user = Session["User"] as Users;
            if (user == null)
            {
                return RedirectToAction("LoginPage", "User");
            }
            List<Orders> orderHistory = csdl.Orders.ToList().FindAll(t => t.UserID == user.UserID).OrderByDescending(t => t.OrderDate).ToList();
            if (orderHistory == null)
            {
                return View(new List<Orders>());
            }
            return View(orderHistory);
        }

        public ActionResult OrderDetail(int orderID)
        {
            Orders od = csdl.Orders.FirstOrDefault(t => t.OrderID == orderID);
            return PartialView(od);
        }

        public ActionResult OrderDetailPage(int orderID)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("LoginPage", "User");
            }

            Orders od = csdl.Orders.FirstOrDefault(t => t.OrderID == orderID);
            return View(od);
        }

        //[HttpPost]
        //public async Task<ActionResult> PaymentWithMoMo(int orderId, long amount)
        //{
            
        //    string partnerCode = ConfigurationManager.AppSettings["MoMoPartnerCode"];
        //    string accessKey = ConfigurationManager.AppSettings["MoMoAccessKey"];
        //    string secretKey = ConfigurationManager.AppSettings["MoMoSecretKey"];

           
        //    string orderInfo = "Thanh toán đơn hàng #" + orderId;
        //    string requestId = Guid.NewGuid().ToString(); // Mã yêu cầu duy nhất

            
        //    string payUrl = await MoMoService.CreatePaymentRequest(partnerCode, accessKey, secretKey, amount, orderId.ToString(), requestId, orderInfo);

            
        //    return Redirect(payUrl);
        //}

        //public ActionResult MoMoReturn(MoMoResponse response)
        //{
        //    if (response.resultCode == 0) // resultCode = 0 là thành công
        //    {
        //        ViewBag.Message = "Thanh toán đơn hàng thành công!";
        //        // Cập nhật Database tại đây
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Thanh toán thất bại: " + response.message;
        //    }
        //    return View();
        //}


    }
}