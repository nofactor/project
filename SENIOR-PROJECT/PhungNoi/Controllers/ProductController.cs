﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using PhungNoiProject.Models;
using System.Web.Helpers;
using System.Configuration;

namespace PhungNoiProject.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private phungnoiDBEntities db = new phungnoiDBEntities();
        List<Product> products = null;

        //
        // GET: /Default1/

        public ActionResult Index(string sortOrder, string Category, string Price)
        {

        


            ////////////////search function//////////////
            if (Request != null && Request.Params["q"] != null)
            {
                string q = Request.Params["q"];
                products = db.Product.Where(x => x.productName.ToLower().Contains(q.ToLower())).ToList();

            }
            else if (Category != null && Price.Contains("price"))
            {

                products = db.Product.Where(x => x.category.ToLower().Contains(Category.ToLower())).ToList();
                return View("Index", products);
            }
            else if (Price != null && Category.Contains("category"))
            {
                products = new List<Product>();
                int p = int.Parse(Price);
                List<Product> productAll = db.Product.ToList();
                foreach (Product pp in productAll)
                {
                    if (pp.price <= p)
                    {
                       
                        products.Add(pp);
                    }
                }

                return View("Index", products);
            }
            else
            {
                ////////////////sort//////////////////
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
                var pds = from p in db.Product
                          select p;
                switch (sortOrder)
                {
                    case "name_desc":
                        pds = pds.OrderByDescending(s => s.productName);
                        break;
                    case "Price":
                        pds = pds.OrderBy(s => s.price);
                        break;
                    case "price_desc":
                        pds = pds.OrderByDescending(s => s.price);
                        break;
                    default:
                        pds = pds.OrderBy(s => s.productName);
                        break;

                } return View("Index", pds.ToList());
            }

            return View("Index",products); ;
        }
        //
        // GET: /Default1/Details/5

        public ActionResult Details(int id = 0)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View("Details", product);
        }

        //
        // GET: /Default1/Create

        public ActionResult Create()
        {
            return View("Create");
        }

        //
        // POST: /Default1/Create

        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {

                Product result = db.Product.Add(product);
                db.SaveChanges();

                WebImage image = null;
                if (Request != null)
                    image = WebImage.GetImageFromRequest();

                if (image != null)
                {
                    string realFileName = result.productID + image.FileName;
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string imageFolder = ConfigurationManager.AppSettings["ImagePath"].ToString();
                    string fullPath = baseDirectory + imageFolder;
                    string relativeImagePath = imageFolder + "\\" + realFileName;

                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    string imagePath = fullPath + "\\" + realFileName;
                    image.Save(imagePath);
                    result.productPicture = relativeImagePath;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(product);
        }

        //
        // GET: /Default1/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View("Edit", product);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                
             

                WebImage image = null;
                if (Request != null)
                    image = WebImage.GetImageFromRequest();

                if (image != null)
                {
                    string realFileName = product.productID + image.FileName;
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string imageFolder = ConfigurationManager.AppSettings["ImagePath"].ToString();
                    string fullPath = baseDirectory + imageFolder;
                    string relativeImagePath = imageFolder + "\\" + realFileName;

                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    string imagePath = fullPath + "\\" + realFileName;
                    image.Save(imagePath);
                    product.productPicture = relativeImagePath;
                    db.SaveChanges();
                }

                products = db.Product.ToList();
                return RedirectToAction("Index", "Product", products);
            }

            return View(product);
        }

        //
        // GET: /Default1/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View("Delete", product);
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
            db.SaveChanges();
            products = db.Product.ToList();
            return View("Index", products);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
