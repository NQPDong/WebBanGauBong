using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using WebBanGauBong.Areas.Admin.Security;
using WebBanGauBong.Models;
using PagedList;
using PagedList.Mvc;

namespace WebBanGauBong.Areas.Admin.Controllers
{
    [CheckAthourize(Roles = "Admin")]
    public class ProductController : Controller
    {
        // GET: Admin/Product
        QL_THU_BONG csdl = new QL_THU_BONG();
        public ActionResult ProductPage(int? page)
        {
            List<Product> ds = csdl.Product.ToList();
            return View(ds.ToPagedList(page ?? 1, 10));
        }

        public ActionResult _ModalThemSanPham()
        {
            ViewBag.DanhSachLoai = csdl.Category.ToList();
            return PartialView();
        }

        [HttpPost]
        public ActionResult TimKiemSanPham(string keyword, string status)
        {
            int IsEnabled = int.Parse(status);
            List<Product> ds = null;
            if (IsEnabled == 2)
            {
                ds = csdl.Product.ToList().FindAll(t => t.ProductName.ToLower().Trim().Contains(keyword.ToLower().Trim()) || t.ProductID.ToString().Equals(keyword));
            }
            else
            {
                ds = csdl.Product.ToList().FindAll(t => (t.ProductName.ToLower().Trim().Contains(keyword.ToLower().Trim()) || t.ProductID.ToString().Equals(keyword)) && t.Isenabled == IsEnabled);
            }

            return View("ProductPage", ds.ToPagedList(1, 10));
        }

        public ActionResult DisableSanPham(int productID, int type)
        {
            try
            {
                // Disable san pham
                Product product = csdl.Product.FirstOrDefault(t => t.ProductID == productID);
                product.Isenabled = type;

                // Xoa san pham co trong gio hang
                List<ShoppingCartItem> cartItems = csdl.ShoppingCartItem.ToList().FindAll(t => t.ProductSize.ProductID == productID);
                csdl.ShoppingCartItem.RemoveRange(cartItems);

                // Cancel san pham 
                List<Orders> lsOrder = csdl.Orders.ToList().FindAll(t => t.Status.Equals("Đang chờ xác nhận") && t.OrderDetail.Any(sp => sp.ProductSize.ProductID == productID));
                foreach (var item in lsOrder)
                {
                    Orders od = csdl.Orders.FirstOrDefault(t => t.OrderID == item.OrderID);
                    od.Status = "Đã hủy";
                }

                csdl.SaveChanges();

                TempData["DisableSuccess"] = $"Disable sản phẩm mã {productID} thành công";
                return RedirectToAction("ProductPage");
            }
            catch (Exception e)
            {
                TempData["DisableError"] = $"Disable sản phẩm mã {productID} không thành công \n {e.Message}";
                return RedirectToAction("ProductPage");
            }
        }

        public ActionResult DeleteSanPham(int productID)
        {
            try
            {
                // Kiem tra don hang ton tai san pham
                List<Orders> lsOrder = csdl.Orders.ToList().FindAll(t => (t.OrderDetail.Any(sp => sp.ProductSize.ProductID == productID)));
                if (lsOrder.Count() == 0)
                {
                    // Xoa discount cua san pham
                    List<Discount> lsDiscount = csdl.Discount.ToList().FindAll(t => t.ProductID == productID);
                    csdl.Discount.RemoveRange(lsDiscount);

                    // Xoa image cua san pham
                    List<ProductImages> lsProductImage = csdl.ProductImages.ToList().FindAll(t => t.ProductID == productID);
                    csdl.ProductImages.RemoveRange(lsProductImage);

                    // Xoa rating cua san pham
                    List<Rating> lsRating = csdl.Rating.ToList().FindAll(t => t.ProductID == productID);
                    csdl.Rating.RemoveRange(lsRating);

                    // Xoa san pham co trong gio hang
                    List<ShoppingCartItem> cartItems = csdl.ShoppingCartItem.ToList().FindAll(t => t.ProductSize.ProductID == productID);
                    csdl.ShoppingCartItem.RemoveRange(cartItems);

                    // Xoa size san pham
                    List<ProductSize> lsSize = csdl.ProductSize.ToList().FindAll(t => t.ProductID == productID);
                    csdl.ProductSize.RemoveRange(lsSize);

                    // Xoa san pham
                    Product product = csdl.Product.FirstOrDefault(t => t.ProductID == productID);
                    csdl.Product.Remove(product);

                    csdl.SaveChanges();
                }

                TempData["DeleteSuccess"] = $"Không thể sản phẩm mã {productID} do có đơn hàng";
                return RedirectToAction("ProductPage");
            }
            catch (Exception e)
            {
                TempData["DeleteError"] = $"Xóa sản phẩm mã {productID} không thành công \n {e.Message}";
                return RedirectToAction("ProductPage");
            }
        }

        public ActionResult _ModalSuaSanPham(int productID)
        {
            Product pd = csdl.Product.FirstOrDefault(t => t.ProductID == productID);
            ViewBag.DanhSachLoai = csdl.Category.ToList();
            return PartialView(pd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductOnSubmit(Product product, IEnumerable<HttpPostedFileBase> images, string SelectedCategoryID)
        {
            using (var transaction = csdl.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        csdl.Product.Add(product);
                        csdl.SaveChanges();

                        var newCategory = csdl.Category.Find(SelectedCategoryID);

                        if (newCategory != null)
                        {
                            // Thêm danh mục mới 
                            product.Category.Add(newCategory);
                        }


                        string FileName = "";
                        string path = "";
                        string dir = "/Content/Images/";
                        string physicalDir = Server.MapPath(dir);
                        if (Directory.Exists(physicalDir) == false)
                        {
                            Directory.CreateDirectory(physicalDir);
                        }
                        if (images != null && images.Any())
                        {
                            foreach (var file in images)
                            {
                                if (file != null && file.ContentLength > 0)
                                {
                                    FileName = Path.GetFileName(file.FileName);

                                    path = Path.Combine(physicalDir, FileName);
                                    file.SaveAs(path);

                                    ProductImages pi = new ProductImages();
                                    pi.ProductID = product.ProductID;
                                    pi.ImageURL = FileName;
                                    csdl.ProductImages.Add(pi);
                                }
                            }
                            csdl.SaveChanges();

                            transaction.Commit();
                        }

                    }
                    TempData["AddSuccess"] = "Thêm sản phẩm thành công";
                    return RedirectToAction("ProductPage");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    TempData["AddError"] = $"Thêm sản phẩm thất bại {e.Message}";
                    TempData["ShowModalAddProduct"] = true;
                    return RedirectToAction("ProductPage");
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSanPhamOnSubmit(Product product, IEnumerable<HttpPostedFileBase> images, string SelectedCategoryID)
        {
            using (var transaction = csdl.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra lỗi Model
                    if (!ModelState.IsValid)
                    {
                        
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                        TempData["UpdateError"] = "Dữ liệu không hợp lệ: " + string.Join(", ", errors);
                        TempData["ShowModalUpdateProduct"] = true; 
                        return RedirectToAction("ProductPage");
                    }

                    Product updateProduct = csdl.Product.Include("Category").FirstOrDefault(t => t.ProductID == product.ProductID);

                   
                    if (updateProduct != null)
                    {
                        updateProduct.ProductName = product.ProductName;
                        updateProduct.Isenabled = product.Isenabled;

                        // Category
                        if (!string.IsNullOrEmpty(SelectedCategoryID))
                        {
                            // Xóa tất cả các danh mục hiện tại 
                            updateProduct.Category.Clear();

                            var newCategory = csdl.Category.Find(SelectedCategoryID);

                            if (newCategory != null)
                            {
                                // Thêm danh mục mới 
                                updateProduct.Category.Add(newCategory);
                            }
                        }
                    }

                    
                    // Kiểm tra xem có upload ảnh mới 
                    bool hasNewImage = images != null && images.Any(f => f != null && f.ContentLength > 0);

                    if (hasNewImage)
                    {
                        // Xóa ảnh cũ 
                        var oldImages = csdl.ProductImages.Where(t => t.ProductID == product.ProductID).ToList();
                        csdl.ProductImages.RemoveRange(oldImages);

                        

                        // Thêm ảnh mới
                        string physicalDir = Server.MapPath("/Content/Images/");
                        if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);

                        foreach (var file in images)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(file.FileName);
                               
                                string path = Path.Combine(physicalDir, fileName);
                                file.SaveAs(path);

                                ProductImages pi = new ProductImages
                                {
                                    ProductID = product.ProductID,
                                    ImageURL = fileName
                                };
                                csdl.ProductImages.Add(pi);
                            }
                        }
                    }
                    
                    // Xóa hết size cũ
                    var oldSizes = csdl.ProductSize.Where(t => t.ProductID == product.ProductID).ToList();
                    var formSizes = product.ProductSize ?? new List<ProductSize>();

                    foreach (var item in formSizes)
                    {
                        if (item.ProductSizeID > 0)
                        {
                            // Cập nhật thông tin
                            var sizeInDb = oldSizes.FirstOrDefault(s => s.ProductSizeID == item.ProductSizeID);
                            if (sizeInDb != null)
                            {
                                sizeInDb.SizeName = item.SizeName;
                                sizeInDb.Price = item.Price;
                                sizeInDb.StockQuantity = item.StockQuantity;
                            }
                        }
                        else
                        {
                            // thêm size
                            item.ProductID = product.ProductID;
                            csdl.ProductSize.Add(item);
                        }
                    }

                    //// Thêm size mới 
                    //if (product.ProductSize != null && product.ProductSize.Count > 0)
                    //{
                    //    foreach (var item in product.ProductSize)
                    //    {
                            
                    //        if (!string.IsNullOrEmpty(item.SizeName.ToString()) && item.Price > 0)
                    //        {
                    //            ProductSize ps = new ProductSize
                    //            {
                    //                ProductID = product.ProductID,
                    //                SizeName = item.SizeName,
                    //                Price = item.Price,
                    //                StockQuantity = item.StockQuantity
                    //            };
                    //            csdl.ProductSize.Add(ps);
                    //        }
                    //    }
                    //}

                    csdl.SaveChanges();
                    transaction.Commit();

                    TempData["UpdateSuccess"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction("ProductPage");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    TempData["UpdateError"] = $"Lỗi hệ thống: {e.Message}";
                    TempData["ShowModalUpdateProduct"] = true;
                    return RedirectToAction("ProductPage");
                }
            }
        }


    }
}