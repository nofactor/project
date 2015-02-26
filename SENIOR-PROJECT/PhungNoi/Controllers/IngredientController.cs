﻿using PhungNoiProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PhungNoiProject.Controllers
{
    public class IngredientController : Controller
    {
        //
        // GET: /Ingredient/
        phungnoiDBEntities db = new phungnoiDBEntities();
        List<Ingredient> ingreList = null;

        public ActionResult Index()
                 {
                     if (Request != null && Request.Params["q"] != null)
                     {
                         string q = Request.Params["q"];
                         ingreList = db.Ingredient.Where(x => x.ingredName.ToLower().Contains(q.ToLower())).ToList();
                     }
                     else
                     {
                         ingreList = db.Ingredient.ToList();
                     }
                     return View("Index", ingreList);
        }

        public ActionResult Details(int id = 0)
        {
            Ingredient ingre = db.Ingredient.Find(id);
            if (ingre == null)
            {
                return HttpNotFound();
            }
            return View("Details", ingre);
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
        public ActionResult Create(Ingredient ingre)
        {
            if (ModelState.IsValid)
            {

                Ingredient result = db.Ingredient.Add(ingre);
                db.SaveChanges();

                WebImage image = null;
                if (Request != null)
                    image = WebImage.GetImageFromRequest();

                if (image != null)
                {
                    string realFileName = result.IngreID + image.FileName;
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
                    result.ingredPicture = relativeImagePath;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(ingre);
        }

        //
        // GET: /Default1/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Ingredient ingre = db.Ingredient.Find(id);
            if (ingre == null)
            {
                return HttpNotFound();
            }
            return View("Edit", ingre);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(Ingredient ingre)
        {
            if (ModelState.IsValid)
            {
                    db.Entry(ingre).State = EntityState.Modified;
                    db.SaveChanges();
                
             

                WebImage image = null;
                if (Request != null)
                    image = WebImage.GetImageFromRequest();

                if (image != null)
                {
                    string realFileName = ingre.IngreID + image.FileName;
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
                    ingre.ingredPicture = relativeImagePath;
                    db.SaveChanges();
                }

                ingreList = db.Ingredient.ToList();
                return RedirectToAction("Index", "Ingredient", ingre);
            }

            return View(
);
        }

        //
        // GET: /Default1/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Ingredient ingre = db.Ingredient.Find(id);
            if (ingre == null)
            {
                return HttpNotFound();
            }
            return View("Delete", ingre);
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {

            Ingredient ingre = db.Ingredient.Find(id);
            db.Ingredient.Remove(ingre);
            db.SaveChanges();
            ingreList = db.Ingredient.ToList();
            return View("Index", ingreList);
        }

       
    }
}


