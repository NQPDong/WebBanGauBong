using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Models;
using PagedList.Mvc;
using PagedList;

namespace WebBanGauBong.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        QL_THU_BONG csdl = new QL_THU_BONG();
        public ActionResult Index(int? page)
        {
            ViewBag.TenLoai = "GẤU BÔNG CAO CẤP";
            return View(csdl.Product.ToList().FindAll(t => t.Isenabled == 1).ToPagedList(page ?? 1, 12));
        }

        public ActionResult HienThiMenu()
        {
            ViewBag.KichThuocNut = new List<int> { 20, 30, 32, 33, 35, 37, 38, 40, 42, 43, 45, 47, 48, 50, 52, 53, 54, 55, 57, 58, 60, 63, 65, 68, 70, 72, 75, 78, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 150, 160, 165, 170, 175, 180, 200 };
            return PartialView(csdl.Category.ToList());
        }

        [HttpPost]
        public ActionResult TimKiemNangCao(FormCollection form)
        {
            List<Product> sanPhamHienTai = Session["DanhSachSanPhamHienTai"] != null ? Session["DanhSachSanPhamHienTai"] as List<Product> : csdl.Product.Where(t => t.Isenabled == 1).ToList();
            List<Product> sanPhamTheoLoai = new List<Product>();
            List<Product> sanPhamTheoGia = new List<Product>();
            List<Product> sanPhamTheoSize = new List<Product>();

            List<int> dsSize = Session["Sizes"] != null ? Session["Sizes"] as List<int> : new List<int>();

            var idLoai = form["loai"];
            var minGia = int.Parse(form["min-value"]);
            var maxGia = int.Parse(form["max-value"]);
            var kichThuoc = form["size"] != null ? int.Parse(form["size"]) : 0;
            if (kichThuoc != 0)
            {
                if (dsSize.Contains(kichThuoc)) dsSize.Remove(kichThuoc);
                else dsSize.Add(kichThuoc);
            }

            TempData["GiaMax"] = maxGia;
            TempData["GiaMin"] = minGia;

            Session["Sizes"] = dsSize;

            // Lọc theo loại
            if (idLoai != null)
            {
                List<string> dsLoai = idLoai.Split(',').ToList();
                TempData["CheckedLoai"] = dsLoai;
                foreach (var cat in csdl.Category.ToList().FindAll(t => dsLoai.Contains(t.CategoryID)))
                {
                    sanPhamTheoLoai = sanPhamTheoLoai.Concat(cat.Product.ToList().FindAll(t => t.Isenabled == 1)).ToList();
                }

                sanPhamTheoLoai = sanPhamTheoLoai.Concat(sanPhamHienTai).ToList();
            }
            else
            {
                sanPhamTheoLoai = sanPhamHienTai;
            }

            // Lọc theo giá
            foreach (Product sp in sanPhamTheoLoai)
            {
                foreach (var size in sp.ProductSize.ToList())
                {
                    if (size.Price >= minGia && size.Price <= maxGia)
                    {
                        sanPhamTheoGia.Add(sp);
                        break;
                    }

                }
            }

            // Lọc theo size
            if (dsSize.Count == 0)
            {
                sanPhamTheoSize = sanPhamTheoLoai;
            }
            else
            {
                foreach (Product sp in sanPhamTheoLoai)
                {
                    foreach (ProductSize size in sp.ProductSize.ToList())
                    {
                        if (dsSize.Contains((int)size.SizeName))
                        {
                            sanPhamTheoSize.Add(sp);
                            break;
                        }

                    }
                }
            }

            List<Product> finals = sanPhamTheoSize.Intersect(sanPhamTheoGia).ToList();

            return View("Index", finals.ToPagedList(1, 12));
        }

        public ActionResult TimKiemTheoTen(string name)
        {
            List<Product> ds = csdl.Product.ToList().FindAll(sp => sp.ProductName.ToLower().Trim().Contains(name.ToLower().Trim()) && sp.Isenabled == 1);
            Session["DanhSachSanPhamHienTai"] = null;
            Session["Sizes"] = null;

            return View("Index", ds.ToPagedList(1, 12));
        }

        public ActionResult TimKiemTheoSize(int min, int max)
        {
            List<Product> dsSP = new List<Product>();
            List<int> buttonSizeClicked = new List<int>();
            foreach (var item in csdl.Product.ToList().FindAll(t => t.Isenabled == 1))
            {
                foreach (var size in item.ProductSize.ToList())
                {
                    if (size.SizeName >= min && size.SizeName <= max)
                    {
                        dsSP.Add(item);
                        buttonSizeClicked.Add((int)size.SizeName);
                    }
                }
            }
            Session["DanhSachSanPhamHienTai"] = dsSP;
            Session["Sizes"] = buttonSizeClicked;
            return View("Index", dsSP.ToPagedList(1, 12));
        }

        public ActionResult TimKiemTheoGia(int min, int max)
        {
            TempData["GiaMax"] = max;
            TempData["GiaMin"] = min;

            List<Product> ds = new List<Product>();
            foreach (var item in csdl.Product.ToList().FindAll(t => t.Isenabled == 1))
            {
                foreach (var s in item.ProductSize.ToList())
                {
                    if (s.Price >= min && s.Price <= max)
                    {
                        ds.Add(item);
                        break;
                    }
                }
            }
            Session["DanhSachSanPhamHienTai"] = ds;
            return View("Index", ds.ToPagedList(1, 12));
        }

        public ActionResult TimKiemTheoLoai(string id)
        {
            Category cat = csdl.Category.FirstOrDefault(t => t.CategoryID.Equals(id));
            List<Product> dssp = new List<Product>();
            ViewBag.TenLoai = cat.CategoryName.ToUpper();

            ViewBag.DanhSachLoaiCon = null;
            if (cat.Category1 != null)
            {
                ViewBag.DanhSachLoaiCon = cat.Category1.ToList();
            }
            else if (cat.Category2 != null)
            {
                ViewBag.DanhSachLoaiCon = cat.Category2.Category1.ToList();
            }

            if (cat.Product != null)
            {
                dssp = dssp.Concat(cat.Product.ToList().FindAll(t => t.Isenabled == 1)).ToList();
            }
            if (cat.Category1 != null)
            {
                foreach (var c in cat.Category1)
                {
                    dssp = dssp.Concat(c.Product.ToList().FindAll(t => t.Isenabled == 1)).ToList();
                }
            }

            Session["DanhSachSanPhamHienTai"] = dssp;
            //Session["Sizes"] = null;

            return View("Index", dssp.ToPagedList(1, 12));
        }


        public ActionResult Detail(int id)
        {
            Product sp = csdl.Product.ToList().Find(t => t.ProductID == id);

            ViewBag.ListHinhAnhPhu = sp.ProductImages.ToList();

            List<Category> dsLoai = new List<Category>();
            Category loai = sp.Category.First();
            ViewBag.Loai = loai;
            dsLoai.Add(loai);
            while (loai.Category2 != null)
            {
                dsLoai.Add(loai.Category2);
                loai = loai.Category2;
            }
            dsLoai.Reverse();
            ViewBag.BreadCrumbLoai = dsLoai;
            if (TempData["TempID"] != null && int.Parse(TempData["TempID"].ToString()) == id)
            {
                TempData.Keep("ListTempRating");
            }
            else
            {
                TempData["ListTempRating"] = null;
            }
            return View(sp);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RatingOnSubmit(FormCollection form, int id, HttpPostedFileBase Image)
        {
            List<TempRating> rateList = TempData["ListTempRating"] != null ? TempData["ListTempRating"] as List<TempRating> : new List<TempRating>();
            TempData["TempID"] = id;
            if (ModelState.IsValid)
            {
                string FileName = "";
                string Dir = "/Content/Images/";
                if (Image != null && Image.ContentLength > 0)
                {
                    FileName = Path.GetFileName(Image.FileName);
                    string physicalDir = Server.MapPath(Dir);
                    if (!Directory.Exists(physicalDir))
                    {
                        Directory.CreateDirectory(physicalDir);
                    }
                    string path = Path.Combine(Server.MapPath(Dir), FileName);
                    Image.SaveAs(path);
                }
                TempRating rate = new TempRating();
                if (!string.IsNullOrEmpty(form["rating"]))
                {
                    rate.Star = int.Parse(form["rating"]);
                }
                else
                {
                    rate.Star = 5; // Mặc định 5 sao nếu không chọn (hoặc xử lý lỗi tùy bạn)
                }
                rate.Name = form["name"];
                rate.Email = form["email"];
                rate.Comment = form["comment"];
                rate.Image = FileName;
                rate.RateDate = DateTime.Now;
                rateList.Add(rate);
                TempData["ListTempRating"] = rateList;
            }
            return RedirectToAction("Detail", new { id = id });
        }

    }
}