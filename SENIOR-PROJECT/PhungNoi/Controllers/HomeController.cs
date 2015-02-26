﻿using PhungNoiProject.Filters;
using PhungNoiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;



namespace PhungNoiProject.Controllers
{

    // [Authorize]

    [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        private phungnoiDBEntities db = new phungnoiDBEntities();


        public ActionResult Index()
        {

            return View("Index");
        }


        [HttpGet]
        public ActionResult Cart(int id = 0, int orderId = 0)
        {
            List<Product> completedProduct;
            Order order = null;

            if (orderId != 0)
                order = db.Order.FirstOrDefault(x => x.orderID == orderId);

            if (order!= null && order.orderStatus.Value != 2)
                return RedirectToAction("Details", "Home", new { id = order.orderID });

            Session["currentOrderId"] = 0;

            if (Session["cart"] == null)
            {
                completedProduct = new List<Product>();

                if (order != null)
                {

                    Session["currentOrderId"] = orderId;
                    List<Product> products = new List<Product>();
                    foreach (OderDetail detail in order.OderDetail)
                    {
                        detail.Product.quantity = detail.productQuantity;
                        products.Add(detail.Product);
                    }
                    Session["cart"] = products;
                    completedProduct = (List<Product>)Session["cart"];
                }
            }
            else
                completedProduct = (List<Product>)Session["cart"];




            Product tproduct = db.Product.Find(id);
            if (tproduct != null)
            {
                Product orderedItem = completedProduct.FirstOrDefault(x => x.productID == id);
                if (orderedItem != null)
                {
                    orderedItem.quantity += 1;
                }
                else
                {
                    tproduct.quantity = 1;
                    completedProduct.Add(tproduct);
                }
            }

            if (Session != null)
            {
                Session["cart"] = completedProduct;
            }



            if (order != null)
                return View("Cart", order);
            else
                return View("Cart");

        }

        [HttpPost]
        public ActionResult SetCartItem(int rownumber, int quantity)
        {
            // Remove the item from the cart
            List<Product> cart = (List<Product>)Session["cart"];
            cart[rownumber].quantity = quantity;

            return Json("OK");
        }

        [HttpPost, ActionName("Cart")]
        public ActionResult SendCart(Order ord, string submit)
        {
            List<Product> completedProduct;
            Order order;


            TimeSpan noon = new TimeSpan(24, 0, 0);
            //int hourdiff = span.Hours; 
            if (ord.deliveryDate == null)
            {
                ModelState.AddModelError("deliveryDate", "Please specify delivery date.");
                return Cart();
            }

            bool isNew = true;

            if (ord != null)
            {
                order = db.Order.FirstOrDefault(x => x.orderID == ord.orderID); ;
                isNew = false;

                if (order == null)
                {
                    order = new Order();
                    isNew = true;
                }
            }
            else
                order = new Order();

            completedProduct = (List<Product>)Session["cart"];
            Member profile = null;
            Member user = null;

            DateTime dnow = DateTime.Now.Date;
            TimeSpan span = (TimeSpan)(ord.deliveryDate - dnow);
            int daydiff = span.Days;

            switch (submit)
            {
                case "Submit":


                    if (daydiff <= 0)
                    {
                        ModelState.AddModelError("", "Invalid Code verified.");
                        return View();
                    }
                    if (DateTime.Now.TimeOfDay >= noon)
                    {
                        ModelState.AddModelError("", "Invalid 2 Code verified.");
                        return View();
                    }

                    if (Session["cart"] != null && completedProduct.Count > 0)
                    {
                        List<int> detailIds = order.OderDetail.Select(x => x.detailID).ToList();
                        foreach (int id in detailIds)
                        {
                            var detail = db.OderDetail.FirstOrDefault(x => x.detailID == id);

                            order.OderDetail.Remove(detail);
                            db.OderDetail.Remove(detail);

                        }

                        if (!isNew)
                            db.SaveChanges();

                        order.OderDetail = new List<OderDetail>();
                        profile = db.Member.FirstOrDefault(x => x.UserName == this.User.Identity.Name);
                        order.Member = profile;
                        order.orderStatus = 1; // save
                        foreach (Product product in completedProduct)
                        {
                            if (product.quantity > 0)
                            {
                                OderDetail orderDetail = new OderDetail();
                                orderDetail.productID = product.productID;
                                orderDetail.orderID = order.orderID;
                                orderDetail.productQuantity = product.quantity;
                                order.OderDetail.Add(orderDetail);
                            }
                        }
                        order.orderDate = DateTime.Now;
                        order.deliveryDate = ord.deliveryDate;
                        order.feedBack = ord.feedBack;

                        if (isNew)
                            db.Order.Add(order);
                        db.SaveChanges();
                    }
                    else
                    {
                        return View("Cart", "Home");
                    }

                    Session.Remove("cart");
                    user = db.Member.FirstOrDefault(x => x.UserName == this.User.Identity.Name);
                    return View("History", user.Order);


                case "Save":
                    if (Session["cart"] != null && completedProduct.Count > 0)
                    {
                        //order.OderDetail.Clear();
                        List<int> detailIds = order.OderDetail.Select(x => x.detailID).ToList();
                        foreach ( int id in detailIds)
                        {
                            var detail = db.OderDetail.FirstOrDefault(x => x.detailID == id);

                            order.OderDetail.Remove(detail);
                            db.OderDetail.Remove(detail);
                            
                        }

                        if (!isNew)
                            db.SaveChanges();
                        //order.OderDetail.Clear();
                        //if (!isNew)
                        //        db.SaveChanges();
                        //order.OderDetail = new List<OderDetail>();
                        profile = db.Member.FirstOrDefault(x => x.UserName == this.User.Identity.Name);
                        order.Member = profile;
                        order.orderStatus = 2; // Draft
                        foreach (Product product in completedProduct)
                        {
                            if (product.quantity > 0)
                            {
                                OderDetail orderDetail = new OderDetail();
                                orderDetail.productID = product.productID;
                                orderDetail.orderID = order.orderID;

                                orderDetail.productQuantity = product.quantity;
                                order.OderDetail.Add(orderDetail);
                            }
                        }
                        order.orderDate = DateTime.Now;
                        order.deliveryDate = ord.deliveryDate;
                        order.feedBack = ord.feedBack;

                        if (isNew)
                            db.Order.Add(order);

                        db.SaveChanges();
                    }
                    else
                    {
                        return View("Product", "Home");
                    }

                    Session.Remove("cart");
                    user = db.Member.FirstOrDefault(x => x.UserName == this.User.Identity.Name);
                    return View("History", user.Order);
                default:
                    return null;
            }



        }

        [HttpGet]
        public ActionResult Product(string sortOrder, string Category, string Price)
        {

            List<Product> products = null;
            ViewBag.CurrentSort = sortOrder;



            ////////////////search function//////////////

            if (Request != null && Request.Params["q"] != null)
            {
                string q = Request.Params["q"];
                products = db.Product.Where(x => x.productName.ToLower().Contains(q.ToLower())).ToList();

            }
            else if (Category != null && Price.Contains("price"))
            {

                products = db.Product.Where(x => x.category.ToLower().Contains(Category.ToLower())).ToList();
                return View("Product", products);
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

                return View("Product", products);
            }

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

            }
            return View("Product", pds.ToList());
            /////////////////////////////////////////////////


            //////////////////paging////////////////

            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}

            //ViewBag.CurrentFilter = searchString;



            //if (Session != null && Session["cart"] != null)
            //{
            //    foreach (Product product in (List<Product>)Session["cart"])
            //    {
            //        if (product.quantity > 0)
            //        {
            //            Product p = products.FirstOrDefault(x => x.id == product.id);
            //            if (p != null)
            //            {
            //                p.quantity = product.quantity;
            //            }
            //        }
            //    }
            //}

            //int pageSize = 3;
            ////int pageNumber = (page ?? 1);
            //ProductsListViewModel model = new ProductsListViewModel
            //{
            //    products = db.Product
            //    .OrderBy(p => p.id)
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize),
            //    PagingInfo = new PagingInfo
            //    {
            //        CurrentPage = page,
            //        ItemsPerPage = pageSize,
            //        TotalItems = db.Product.Count()
            //    }
            //};



        }

        [HttpPost]
        public ActionResult Product(List<Product> products)
        {
            List<Product> completedProduct;

            if (Session["cart"] == null)
            {
                completedProduct = new List<Product>();
            }
            else
                completedProduct = (List<Product>)Session["cart"];

            foreach (Product product in products)
            {
                if (product.quantity > 0)
                {
                    Product tproduct = db.Product.Find(product.productID);
                    if (tproduct != null)
                    {
                        Product orderedItem = completedProduct.FirstOrDefault(x => x.productID == product.productID);
                        if (orderedItem != null)
                            orderedItem.quantity = product.quantity;
                        else
                        {
                            tproduct.quantity = product.quantity;
                            completedProduct.Add(tproduct);
                        }
                    }
                }
            }
            if (Session != null)
            {
                Session["cart"] = completedProduct;
            }





            //   System.Diagnostics.Debug.Write(member.UserType);
            //return View("Cart");




            return RedirectToAction("Cart", "Home");



            //if (Session["cart"] == null)
            //    completedProduct = new List<Product>();
            //else
            //    completedProduct = (List<Product>)Session["cart"];

            //foreach (Product product in products)
            //{
            //    if (product.quantity > 0)
            //    {
            //        Product tproduct = db.Product.Find(product.id);
            //        if (tproduct != null)
            //        {
            //            Product orderedItem = completedProduct.FirstOrDefault(x => x.id == product.id);
            //            if (orderedItem != null)
            //                orderedItem.quantity = product.quantity;
            //            else
            //            {
            //                tproduct.quantity = product.quantity;
            //                completedProduct.Add(tproduct);
            //            }
            //        }

            //    }
            //}
            //if (Session != null)
            //{
            //    Session["cart"] = completedProduct;
            //}





            //   System.Diagnostics.Debug.Write(member.UserType);
            //return View("Cart");

        }







        public class OrderItem
        {
            public int id;
            public string name;
            public string qty;
        }






        public ActionResult History()
        {

            Member user = db.Member.FirstOrDefault(x => x.UserName == this.User.Identity.Name);
            return View("History", user.Order);
        }

        //[HttpPost,ActionName("History")]
        //public ActionResult RepleteOrder(int id)
        //{

        //    OderDetail list = db.OderDetail.FirstOrDefault(x => x.orderId == id);

        //    foreach(OderDetail x in list){


        //    }

        //    if (Session["cart"] == null)
        //        order. = new List<Product>();
        //    else
        //        order = (List<Product>)Session["cart"];
        //    return View();

        //}

        public ActionResult Details(int id)
        {
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View("Details", order);
        }

        public ActionResult Delete(int id = 0)
        {
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View("Delete", order);
        }

        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult CopyOrder(int orderId)
        {
            Order order = db.Order.FirstOrDefault(x => x.orderID == orderId);

            List<Product> products = new List<Product>();

            foreach (OderDetail o in order.OderDetail)
            {
                o.Product.quantity = o.productQuantity;
                products.Add(o.Product);
            }

            Session["cart"] = products;

            return RedirectToAction("Cart", "Home");
        }




        public ActionResult About()
        {
            return View("About");
        }

        public ActionResult Contact()
        {


            return View("Contact");
        }

    }


}