using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Models;

namespace WebBanGauBong.Controllers
{
    public class HomeController : Controller
    {
        QL_THU_BONG csdl = new QL_THU_BONG();
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HienThiSanPham(string id, string image)
        {
            //ViewBag.Ten = id.ToUpper();
            ViewBag.Anh = image;
            Category loai = csdl.Category.FirstOrDefault(t => t.CategoryID.Equals(id));
            List<Product> dssp = new List<Product>();
            if (loai != null)
            {
                ViewBag.Loai = loai;
                if (loai.Product != null)
                {
                    dssp = dssp.Concat(loai.Product).ToList();
                }
                if (loai.Category1 != null)
                {
                    foreach (var c in loai.Category1)
                    {
                        dssp = dssp.Concat(c.Product).ToList();
                    }
                }
                return PartialView(dssp.ToList().FindAll(t => t.Isenabled == 1));
            }    

            return PartialView(csdl.Product.ToList());
        }

        public ActionResult CardSanPham(int id)
        {
            Product sp = csdl.Product.First(t => t.ProductID == id);
            return PartialView(sp);
        }

        public ActionResult SanPhamNoiBat()
        {
            var ls = csdl.Product.Where(t => t.Isenabled == 1).ToList();
            List<Product> randomLs = ls.OrderBy(t => Guid.NewGuid()).Take(3).ToList();

            return PartialView(randomLs);
        }

    }
}