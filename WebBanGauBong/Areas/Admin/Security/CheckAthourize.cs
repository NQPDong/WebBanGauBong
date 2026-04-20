using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebBanGauBong.Areas.Admin.Security
{
    public class CheckAthourize : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var userRoleObject = filterContext.HttpContext.Session["Role"];

            // 1. CHẶN KHI CHƯA ĐĂNG NHẬP (Unauthenticated)
            if (userRoleObject == null || string.IsNullOrWhiteSpace(userRoleObject.ToString()))
            {
                // Chuyển hướng đến trang Login
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Login",
                        area = "Admin"
                    })
                );
                return;
            }

            string userRole = userRoleObject.ToString().Trim();
            string allowedRolesString = this.Roles;

            // 2. CHẶN KHI KHÔNG CÓ QUYỀN (Unauthorized)
            if (!string.IsNullOrEmpty(allowedRolesString))
            {
                string[] allowedRoles = allowedRolesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToArray();

                // Nếu Role của User KHÔNG nằm trong danh sách Role được phép
                if (!allowedRoles.Any(r => r.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
                {
                    // Gán TempData an toàn qua Controller property
                    if (filterContext.Controller != null)
                    {
                        filterContext.Controller.TempData["Error"] = "Bạn không có quyền hạn phù hợp để truy cập chức năng này!";
                    }

                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new
                        {
                            controller = "Dashboard",
                            action = "Index",
                            area = "Admin"
                        }));
                    return;
                }
            }
        }


    }
}