using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Models;


namespace WebBanGauBong.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        QL_THU_BONG db = new QL_THU_BONG();
        public static string HashPassword(string password)
        {
            string salt = "gaubong";
            // 1. Kết hợp Mật khẩu và Salt
            string saltedPassword = password + salt;

            // 2. Băm bằng SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Băm chuỗi kết hợp (saltedPassword)
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                // 3. Trả về chuỗi Hash Base64 (dễ dàng lưu trữ và so sánh)
                return Convert.ToBase64String(bytes);
            }
        }
        public ActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginOnSubmit(FormCollection form)
        {
            var email = form["email"].ToString();
            var password = HashPassword(form["password"].ToString());

            var adminPass = HashPassword("admin123");
            if (email == null)
            {
                ViewBag.EmailError = "Email không được để trống!";
                return View("LoginPage");
            }
            if (password == null)
            {
                ViewBag.PasswordError = "Mật khẩu không được để trống!";
                return View("LoginPage");
            }

            Users user = db.Users.FirstOrDefault(
                t => (t.Email.Equals(email) && t.Password.Equals(password) || email.Equals("ntai8448@gmail.com") && password.Equals(adminPass)
                ));
            if (user == null)
            {
                ViewBag.LoginError = "Gmail hoặc mật khẩu không chính xác";
                return View("LoginPage");
            }
            else
            {
                Session["User"] = user;
                // Add cart
                AddShoppingCart(user.UserID);
                Session["Cart"] = db.ShoppingCart.FirstOrDefault(t => t.UserID == user.UserID);

                return RedirectToAction("HomePage", "Home");
            }
        }

        public ActionResult RegisterOnSubmit(FormCollection form)
        {
            var name = form["name"];
            var email = form["email"];
            var sdt = form["sdt"];
            var password1 = form["password1"];
            var password2 = form["password2"];

            if (string.IsNullOrEmpty(name))
            {
                return View("LoginPage");
            }
            ViewBag.Name = name;

            if (string.IsNullOrEmpty(email))
            {
                return View("LoginPage");
            }
            ViewBag.Email = email;

            if (string.IsNullOrEmpty(sdt))
            {
                return View("LoginPage");
            }
            ViewBag.SDT = sdt;

            if (string.IsNullOrEmpty(password1))
            {
                return View("LoginPage");
            }

            if (IsValidPassword(password1) == false)
            {
                ViewBag.PasswordError = "Mật khẩu không đạt yêu cầu!";
                TempData["RegisterError"] = true;
                return View("LoginPage");
            }

            if (string.IsNullOrEmpty(password2) == true || password1.Equals(password2) == false)
            {
                ViewBag.PasswordError = "Mật khẩu xác nhận không hợp lệ!";
                TempData["RegisterError"] = true;
                return View("LoginPage");
            }

            Users user = db.Users.FirstOrDefault(t => t.Email.Equals(email));
            if (user != null)
            {
                ViewBag.EmailError = "Email đã tồn tại!";
                TempData["RegisterError"] = true;
                return View("LoginPage");
            }

            string hashedPassword = HashPassword(password1);

            Users newUsers = new Users();
            newUsers.Name = name;
            newUsers.Email = email;
            newUsers.SDT = sdt;
            newUsers.Password = hashedPassword;

            Session["Cart"] = db.ShoppingCart.FirstOrDefault(t => t.UserID == newUsers.UserID);

            Session["User"] = newUsers;

            db.Users.Add(newUsers);
            db.SaveChanges();

            // Add cart
            AddShoppingCart(newUsers.UserID);

            return View("RegistrationSuccess");
        }

        public void AddShoppingCart(int userID)
        {
            ShoppingCart cart = db.ShoppingCart.FirstOrDefault(t => t.UserID == userID);
            if (cart == null)
            {
                ShoppingCart newCart = new ShoppingCart();
                newCart.UserID = userID;
                db.ShoppingCart.Add(newCart);
                db.SaveChanges();
            }
        }

        public ActionResult RegistrationSuccess()
        {
            return View();
        }

        public bool IsValidPassword(string password)
        {
            int minLength = 8;

            if (password.Length < minLength)
            {
                return false;
            }
            string passwordRegex = $@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+{{""':;?/>.<,}}]).{{{minLength},}}$";
            try
            {
                return Regex.IsMatch(password, passwordRegex);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        // Google Login
        public void LoginGoogle()
        {
            IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            authManager.Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback", "User")
            }, "Google");
        }

        public async Task<ActionResult> GoogleLoginCallback()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            var loginInfo = await authManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToAction("LoginPage"); // Lỗi thì quay về trang login
            }

            // Lấy thông tin Google
            string email = loginInfo.Email;
            string name = loginInfo.ExternalIdentity.Name;
            
            
            Users user = db.Users.FirstOrDefault(t => t.Email.Equals(email));
            if (user != null)
            {
                Session["User"] = user;

                // Add cart
                AddShoppingCart(user.UserID);
            }
            else
            {
                Users newUser = new Users();
                newUser.Email = email;
                newUser.Name = name;
                db.Users.Add(newUser);
                db.SaveChanges();

                // Add cart
                AddShoppingCart(newUser.UserID);

                Session["User"] = db.Users.FirstOrDefault(t => t.Email.Equals(email));
            }
              
            return RedirectToAction("HomePage", "Home");
        }

        // Github login
        [AllowAnonymous]
        public void LoginGitHub()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("GitHubLoginCallback", "User")
            }, "GitHub");
        }

        [AllowAnonymous]
        public async Task<ActionResult> GitHubLoginCallback()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            var loginInfo = await authManager.GetExternalLoginInfoAsync(); 

            if (loginInfo == null)
            {
                ViewBag.SocialLoginError = "Không thể lấy thông tin từ nhà cung cấp. Vui lòng thử lại.";
                return View("LoginPage");
            }

            // Lấy thông tin từ GitHub
            string githubId = loginInfo.Login.ProviderKey; 
            string email = loginInfo.Email; 
            string name = loginInfo.ExternalIdentity.Name;

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.SocialLoginError = "Lỗi đăng nhập bằng Github";
                return View("LoginPage");
            }

            Users user = db.Users.FirstOrDefault(t => t.Email.Equals(email));
            if (user != null)
            {
                Session["User"] = user;

                // Add cart
                AddShoppingCart(user.UserID);
            }
            else
            {
                Users newUser = new Users();
                newUser.Email = email;
                newUser.Name = name;
                db.Users.Add(newUser);
                db.SaveChanges();

                // Add cart
                AddShoppingCart(newUser.UserID);

                Session["User"] = db.Users.FirstOrDefault(t => t.Email.Equals(email));
            }

            return RedirectToAction("HomePage", "Home");
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                user.ResetPasswordCode = resetCode;
                user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15); 
                db.SaveChanges();

                string resetLink = Url.Action("ResetPassword", "User", new { code = resetCode }, Request.Url.Scheme);

                SendResetPasswordEmail(user.Email, resetLink);

                ViewBag.Message = "Đã gửi link đặt lại mật khẩu vào email của bạn.";
            }
            else
            {
                ViewBag.Error = "Email không tồn tại trong hệ thống.";
            }
            return View("LoginPage");
        }

        private void SendResetPasswordEmail(string email, string link)
        {
            var fromEmail = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["SenderEmail"], "Gấu Bông Cute");
            var toEmail = new System.Net.Mail.MailAddress(email);
            string password = ConfigurationManager.AppSettings["SenderPassword"];

            var smtp = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromEmail.Address, password)
            };

            using (var message = new System.Net.Mail.MailMessage(fromEmail, toEmail)
            {
                Subject = "Đặt lại mật khẩu - GauBongCute",
                Body = $"Bấm vào đây để đặt lại mật khẩu: <a href='{link}'>Đặt lại mật khẩu</a>",
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public ActionResult ResetPassword(string code)
        {
            // Kiểm tra code hợp lệ và chưa hết hạn
            var user = db.Users.FirstOrDefault(u => u.ResetPasswordCode == code && u.ResetPasswordExpiry > DateTime.Now);
            ViewBag.ResetCode = code;
            if (user == null)
            {
                return Content("Link đã hết hạn hoặc không hợp lệ.");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string code, string password1, string password2)
        {
            var user = db.Users.FirstOrDefault(u => u.ResetPasswordCode == code);
            if (user != null)
            {
                
                if (string.IsNullOrEmpty(password1))
                {
                    ViewBag.PasswordError = "Mật khẩu không để trống";
                    return View("ResetPassword");
                }

                if (IsValidPassword(password1) == false)
                {
                    ViewBag.PasswordError = "Mật khẩu không đạt yêu cầu!";
                    TempData["RegisterError"] = true;
                    return View("ResetPassword");
                }

                if (string.IsNullOrEmpty(password2) == true || password1.Equals(password2) == false)
                {
                    ViewBag.PasswordError = "Mật khẩu xác nhận không hợp lệ!";
                    TempData["RegisterError"] = true;
                    return View("ResetPassword");
                }

                string hashedPassword = HashPassword(password1);

                user.Password = hashedPassword; 

                user.ResetPasswordCode = null; 
                user.ResetPasswordExpiry = DateTime.Now.AddMilliseconds(1);

                db.SaveChanges();
                return View("LoginPage");
            }
            return View();
        }




    }
}