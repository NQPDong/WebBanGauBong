using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Areas.Admin.Services;

namespace WebBanGauBong.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        AccountService accountService = new AccountService();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            bool isValid = accountService.Connect(username, password, "Admin");
            if (isValid)
            {
                Session["Role"] = "Admin";
                Session["EmployeeName"] = username;

                return RedirectToAction("DashboardPage", "Dashboard");

            }
            else
            {
                ViewBag.Error = "Lỗi đăng nhập: " + "Vui lòng kiểm tra Username/Password hoặc Server SQL chưa chạy.";

                return View();
            }
        }
        public ActionResult Logout()
        {
            string role = Session["Role"]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(role))
            {
                ViewBag.Error = "Bạn chưa đăng nhập!";
                return View("Login", new { area = "admin" });
            }
            else
            {
                Session.Clear();
                return RedirectToAction("Login", "Account", new { area = "admin" });
            }
        }
    }
}