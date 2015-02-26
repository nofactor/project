﻿using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PhungNoiProject.Models;

namespace PhungNoiProject.Controllers
{
    [Authorize]
    public class OrderDetailController : Controller
    {
        private phungnoiDBEntities db = new phungnoiDBEntities();

        //
        // GET: /OrderDetail/

        public ActionResult Index()
        {
            var oderdetail = db.OderDetail.Include(o => o.Order).Include(o => o.Product);
            return View(oderdetail.ToList());
        }

        //
        // GET: /OrderDetail/Details/5

        public ActionResult Details(int id = 0)
        {
            OderDetail oderdetail = db.OderDetail.Find(id);
            if (oderdetail == null)
            {
                return HttpNotFound();
            }
            return View(oderdetail);
        }

        //
        // GET: /OrderDetail/Create

        public ActionResult Create()
        {
            ViewBag.orderId = new SelectList(db.Order, "id", "id");
            ViewBag.productId = new SelectList(db.Product, "id", "name");
            return View();
        }

        //
        // POST: /OrderDetail/Create

        [HttpPost]
        public ActionResult Create(OderDetail oderdetail)
        {
            if (ModelState.IsValid)
            {
                db.OderDetail.Add(oderdetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.orderId = new SelectList(db.Order, "id", "id", oderdetail.orderID);
            ViewBag.productId = new SelectList(db.Product, "id", "name", oderdetail.productID);
            return View(oderdetail);
        }

        //
        // GET: /OrderDetail/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OderDetail oderdetail = db.OderDetail.Find(id);
            if (oderdetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.orderId = new SelectList(db.Order, "id", "id", oderdetail.orderID);
            ViewBag.productId = new SelectList(db.Product, "id", "name", oderdetail.productID);
            return View(oderdetail);
        }

        //
        // POST: /OrderDetail/Edit/5

        [HttpPost]
        public ActionResult Edit(OderDetail oderdetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oderdetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.orderId = new SelectList(db.Order, "id", "id", oderdetail.orderID);
            ViewBag.productId = new SelectList(db.Product, "id", "name", oderdetail.productID);
            return View(oderdetail);
        }

        //
        // GET: /OrderDetail/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OderDetail oderdetail = db.OderDetail.Find(id);
            if (oderdetail == null)
            {
                return HttpNotFound();
            }
            return View(oderdetail);
        }

        //
        // POST: /OrderDetail/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OderDetail oderdetail = db.OderDetail.Find(id);
            db.OderDetail.Remove(oderdetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}