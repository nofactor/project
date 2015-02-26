using PhungNoiProject.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data;

namespace PhungNoiProject.Controllers
{
    public class RecipeController : Controller
    {

        //  GET: /Recipe/

        phungnoiDBEntities db = new phungnoiDBEntities();
        List<Product> products;
        [HttpGet]
        public ActionResult Index()
        {
            products = db.Product.ToList();


            return View("Index", products);
        }

        public ActionResult Get(int id=0)
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Delete(int id=0)
        {
            Ingredient ingre = db.Ingredient.Find(id);
           List< Recipe > rc = db.Recipe.Where(x => x.ingredID == ingre.IngreID).ToList();
            foreach (var a in rc)
            {
                
                db.Recipe.Remove(a);
                db.SaveChanges();
            }
          
            return  View("Index",db.Product.ToList());
        }
        public ActionResult Insert(int id=0)
        {
            
            List<Ingredient> ingre = db.Ingredient.ToList();

            RecipeAddIngredientModel recipeInsertModel = new RecipeAddIngredientModel();

            recipeInsertModel.productId = id;
            recipeInsertModel.ingredient = ingre;

            return View("Insert", recipeInsertModel);
        }
        public ActionResult InsertConfirm(int productId, int ingredientId)
        {
            if (!db.Recipe.Any(x => x.productID == productId && x.ingredID == ingredientId))
            {
                Recipe recipe = new Recipe();
                Ingredient ingre = db.Ingredient.FirstOrDefault(x => x.IngreID == ingredientId);

                recipe.productID = productId;
                recipe.ingredID = ingredientId;
                recipe.Ingredient = ingre;
                recipe.unitID = 1;
                
                db.Recipe.Add(recipe);
                db.SaveChanges();
            }
            return Edit(productId);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Session["a"] = id;
            Product product = db.Product.Find(id);
            List<Recipe> recipe = db.Recipe.Where(x => x.productID == product.productID).ToList();
            if (recipe.Count == 0)
            {
                Recipe r = new Recipe();
                r.ingredID = 1;
                r.ingredQuantity = 0;
                r.productID = product.productID;
                r.unitID = 1;

                db.Recipe.Add(r);
                db.SaveChanges();
                recipe = db.Recipe.Where(x => x.productID == product.productID).ToList();
            }

            List<SelectListItem> list = db.Units.ToList().Select(x => new SelectListItem
            {
                Text = x.unitName,
                Value = x.unitID.ToString()
            }).ToList();

            if (recipe != null)
            {
                recipe.ForEach(x => x.SelectListItems = list);

       
            recipe.ForEach(x => x.SelectListItems = list);


                return View("EditRecipe", recipe);
            }
            else {
                return View("EditRecipe");
            }
          
        }
        [HttpPost]
        public ActionResult SaveRecipe(List<Recipe> rc)
        {
            if(ModelState.IsValid){

                foreach (Recipe r in rc)
                {
                   var recipe = db.Recipe.FirstOrDefault(x => x.recipeID == r.recipeID);
                   recipe.ingredQuantity = r.ingredQuantity;
                   recipe.unitID = r.unitID;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index",db.Product.ToList());
        }
       
       

    }
}
