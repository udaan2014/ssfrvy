using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private NORTHWNDEntities db = new NORTHWNDEntities();
        // GET: User
        public ActionResult Index()
        {
            if (Session["userID"] != null)
            {
                var _userID = Session["userID"].ToString();
                ViewData["userID"] = _userID;
                var _userName = db.Logins.Where(u => u.UserID == _userID).Select(u => u.UserName).ToList(); ;
                ViewData["userName"] = _userName[0];
                Session["name"] = _userName[0];
                var categoryChartData = db.Products.GroupBy(p => p.Category.CategoryName).Select(y => new { Name = y.Key, Count = y.Count() });
                string s = "[";
                foreach (var item in categoryChartData)
                {
                    s += "{" + "label:" + '\"' + item.Name + '\"' + ",y:" + item.Count + "},";
                }
                s += "]";
                ViewBag.categoryChartResult = s;


                var supplierChartData = db.Products.GroupBy(p => p.Supplier.CompanyName).Select(y => new { Name = y.Key, Count = y.Count() });
                string r = "[";
                foreach (var item in supplierChartData)
                {
                    r += "{" + "label:" + '\"' + item.Name + '\"' + ",y:" + item.Count + "},";
                }
                r += "]";
                ViewBag.supplierChartResult = r;
                return View();
            }
            else
            {

                return RedirectToAction("Login", "Logins");
            }
        }
        public ActionResult Products()
        {
            ViewData["deleteFlag"] = 0;
            ViewData["addFlag"] = 0;
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            else
            {
                var products = db.Products.ToList();
                ViewBag.products = products;
                //IEnumerable<Product> product = db.Products.ToList();
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
                return View();
            }
        }


        public ActionResult Chart()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            else
            {
                var categoryChartData = db.Products.GroupBy(p => p.Category.CategoryName).Select(y => new { Name = y.Key, Count = y.Count() });
                string s = "[";
                foreach (var item in categoryChartData)
                {
                    s += "{" + "label:" + '\"' + item.Name + '\"' + ",y:" + item.Count + "},";
                }
                s += "]";
                ViewBag.categoryChartResult = s;


                var supplierChartData = db.Products.GroupBy(p => p.Supplier.CompanyName).Select(y => new { Name = y.Key, Count = y.Count() });
                string r = "[";
                foreach (var item in supplierChartData)
                {
                    r += "{" + "label:" + '\"' + item.Name + '\"' + ",y:" + item.Count + "},";
                }
                r += "]";
                ViewBag.supplierChartResult = r;


                return View();
            }
        }

        public ActionResult DeleteProducts(string productID)
        {
            int prodID = int.Parse(productID);
            Product product = db.Products.Find(prodID);
            db.Products.Remove(product);
            if (db.SaveChanges() > 0)
                return Content("Success");
            else
                return Content("Failed");
        }

        [HttpPost]

        public ActionResult Products([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product, string add, string edit)
        {

            //if (ModelState.IsValid)
            if (!string.IsNullOrEmpty(add))
            {
                db.Products.Add(product);
                db.SaveChanges();
                ModelState.Clear();
            }
            else if (!string.IsNullOrEmpty(edit))
            {
                Product productObj = db.Products.Find(product.ProductID);
                productObj.ProductName = product.ProductName;
                productObj.SupplierID = product.SupplierID;
                productObj.CategoryID = product.CategoryID;
                productObj.QuantityPerUnit = product.QuantityPerUnit;
                productObj.UnitPrice = product.UnitPrice;
                productObj.UnitsInStock = product.UnitsInStock;
                productObj.UnitsOnOrder = product.UnitsOnOrder;
                productObj.ReorderLevel = product.ReorderLevel;
                productObj.Discontinued = product.Discontinued;
                db.SaveChanges();

            }
            var products = db.Products.ToList();
            ViewBag.products = products;
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);

            return View(product);

        }


        public ActionResult MyProfile()
        {
            if (Session["userID"] != null)
            {
                var _userID = Session["userID"].ToString();
                ViewData["userID"] = _userID;
                var _userName = db.Logins.Where(u => u.UserID == _userID).Select(u => u.UserName).ToList(); ;
                ViewData["userName"] = _userName[0];
                Session["name"] = _userName[0];
                return View();
            }

            else
            {
                return RedirectToAction("Login", "Logins");
            }

        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "Logins");
        }

    }

}