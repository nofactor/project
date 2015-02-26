using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PhungNoiProject.Models;



namespace PhungNoiProject.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private phungnoiDBEntities db = new phungnoiDBEntities();



        //
        // GET: /Order/
        //[Authorize(Roles="Admin")]



        public ActionResult Index(string sortOrder)
        {

            List<Order> lOrder = null;
            if (Request != null && Request.Params["q"] != null)
            {
                string q = Request.Params["q"];
                lOrder = db.Order.Where(x => x.Member.UserName.ToLower().Contains(q.ToLower())).ToList();
            }
            else if (sortOrder != null)
            {

                ////////////////sort//////////////////

                ViewBag.DeliverDateSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.OrderStatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";
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
                return View("Index", pds);
            }
            else
            {


                lOrder = db.Order.OrderBy(x => x.deliveryDate).ToList();
            }


            var order = db.Order.Include(o => o.Member);

            return View("Index", lOrder.OrderByDescending(o => o.deliveryDate));
        }

        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View("Details", order);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0, int orderId = 0)
        {
            List<Product> completedProduct;
            Order order = null;

            if (orderId != 0)
                order = db.Order.FirstOrDefault(x => x.orderID == orderId);

            //if (order != null && order.orderStatus.Value != 2)
            //    return RedirectToAction("Details", "Home", new { id = order.orderID });

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
                return View("Edit", order);
            else
                return View("Edit");

        }

        //
        // GET: /Order/Edit/5

        //public ActionResult Edit(int id = 0)
        //{

        //    Order order = db.Order.Find(id);

        //    if (order == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.user_id = new SelectList(db.Member, "UserId", "UserName", order.memberID);
        //    return View("Edit", order);


        //}

        //
        // POST: /Order/Edit/5

        [HttpPost, ActionName("Edit")]
        public ActionResult Edit(Order ord, string submit)
        {
            List<Product> completedProduct;
            Order order;


            TimeSpan noon = new TimeSpan(24, 0, 0);
            //int hourdiff = span.Hours; 
            if (ord.deliveryDate == null)
            {
                ModelState.AddModelError("deliveryDate", "Please specify delivery date.");
                return Edit(0,0);
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
                        foreach (int id in detailIds)
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

        [HttpPost]
        public ActionResult SetCartItem(int rownumber, int quantity)
        {
            // Remove the item from the cart
            List<Product> cart = (List<Product>)Session["cart"];
            cart[rownumber].quantity = quantity;

            return Json("OK");
        }

        //
        // GET: /Order/Delete/5

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

        public ActionResult print(int id)
        {

            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View("print", order);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}