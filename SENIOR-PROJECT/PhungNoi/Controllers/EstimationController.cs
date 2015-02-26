using PhungNoiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhungNoiProject.Controllers
{
    public class EstimationController : Controller
    {
        //
        // GET: /Estimation/
         phungnoiDBEntities db = new phungnoiDBEntities();

         public ActionResult DailyIngredientReport(DateTime startDate, DateTime endDate)//transaction of ingredient per day
        {

            summaryIngredientByPeriod(startDate,endDate);

            return View("DailyIngredient");
        }

        public ActionResult MonthlyIngredientReport(Order order)
        {
            return View("MounthlyIngredient");
        }
        public ActionResult WeeklyIngredientReport(Order order)
        {
            return View("WeeklyIngredient");

        }
        public ActionResult YearIngredientReport(Order order)
        {
            return View("YearIngredient");
        }

        ///order report////
        public ActionResult IndividualOrderReport(int id)
        {
            return View("IndividualOrderReport");
        }
        public ActionResult TransactionOrderReport()
        {
            return View("TransactionOrderReport");
        }

    
        /// ingredient report for one ingredient

        public ActionResult DailyOneIngredientReport(int id)
        {
            return View("TransactionOrderReport");
        }
        public ActionResult WeeklyOneIngredientReport(int id)
        {
            return View("TransactionOrderReport");
        }
        public ActionResult MonthOneIngredientReport(int id)
        {
            return View("TransactionOrderReport");
        }
        public ActionResult YearOneIngredientReport(int id)
        {
            return View("TransactionOrderReport");
        }



        private ProductIngredientSummaryModel summaryIngredientOfProductByPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            var product = db.Product.FirstOrDefault(x => x.productID == productId);
            if (product != null)
            {
                var recipes = db.Recipe.Where(x => x.productID == product.productID);

                int orderSummary = db.Order.Where(x => x.orderDate.Value >= startDate && x.orderDate <= endDate)
                    .SelectMany(x =>x.OderDetail).Where(x => x.productID == product.productID)
                    .Sum(x => x.productQuantity);

                ProductIngredientSummaryModel results = new ProductIngredientSummaryModel();
                results.product = product;

                foreach (Recipe r in recipes)
                {
                    IngredientSummaryDetail detail = new IngredientSummaryDetail();

                    detail.ingredient = r.Ingredient;
                    detail.amount = (float)(r.ingredQuantity.Value * orderSummary);

                    results.ingredientDetails.Add(detail);
                }

                return results;
            }

            return null;
        }

        private ProductIngredientSummaryModel summaryIngredientOfProductByPeriodAndMember(int memberId, int productId, DateTime startDate, DateTime endDate)
        {
            var product = db.Product.FirstOrDefault(x => x.productID == productId);
            if (product != null)
            {
                var recipes = db.Recipe.Where(x => x.productID == product.productID);

                int orderSummary = db.Order.Where(x => x.orderDate.Value >= startDate && x.orderDate <= endDate && x.memberID == memberId)
                    .SelectMany(x => x.OderDetail).Where(x => x.productID == product.productID)
                    .Sum(x => x.productQuantity);

                ProductIngredientSummaryModel results = new ProductIngredientSummaryModel();
                results.product = product;

                foreach (Recipe r in recipes)
                {
                    IngredientSummaryDetail detail = new IngredientSummaryDetail();

                    detail.ingredient = r.Ingredient;
                    detail.amount = (float)(r.ingredQuantity.Value * orderSummary);

                    results.ingredientDetails.Add(detail);
                }

                return results;
            }

            return null;
        }

        private IngredientSummaryModel summaryIngredientByPeriod(DateTime startDate, DateTime endDate)
        {
            var products = db.Order.Where(x => x.orderDate >= startDate && x.orderDate <= endDate).SelectMany(x =>x.OderDetail).Select(x => x.Product);
            IngredientSummaryModel result = new IngredientSummaryModel();
            foreach(Product product in products)
            {
                ProductIngredientSummaryModel models = summaryIngredientOfProductByPeriod(product.productID, startDate, endDate);
                foreach (var detail in models.ingredientDetails)
                {
                    if (result.ingredientDetails.Any(x => x.ingredient.IngreID == detail.ingredient.IngreID))
                    {
                        result.ingredientDetails.FirstOrDefault(x => x.ingredient.IngreID == detail.ingredient.IngreID).amount += detail.amount;
                    }
                    else
                    {
                        result.ingredientDetails.Add(detail);
                    }
                }
            }
            return result;
        }

        private IngredientSummaryModel summaryIngredientByPeriodAndMember(int memberId, DateTime startDate, DateTime endDate)
        {
            var products = db.Order.Where(x => x.orderDate >= startDate && x.orderDate <= endDate).SelectMany(x => x.OderDetail).Select(x => x.Product);
            IngredientSummaryModel result = new IngredientSummaryModel();
            foreach (Product product in products)
            {
                ProductIngredientSummaryModel models = summaryIngredientOfProductByPeriodAndMember(memberId,product.productID, startDate, endDate);
                foreach (var detail in models.ingredientDetails)
                {
                    if (result.ingredientDetails.Any(x => x.ingredient.IngreID == detail.ingredient.IngreID))
                    {
                        result.ingredientDetails.FirstOrDefault(x => x.ingredient.IngreID == detail.ingredient.IngreID).amount += detail.amount;
                    }
                    else
                    {
                        result.ingredientDetails.Add(detail);
                    }
                }
            }
            return result;
        }

        private IngredientSummaryPeriodModel summaryIngredient_DayByDay_Period(DateTime startDate, DateTime endDate)
        {
            DateTime runner = startDate.Date;
            IngredientSummaryPeriodModel result = new IngredientSummaryPeriodModel();
            while (runner <= endDate)
            {
                var item = summaryIngredientByPeriod(runner, runner);

                PhungNoiProject.Models.IngredientSummaryPeriodModel.IngredientSummaryPeriod summariedItem = new IngredientSummaryPeriodModel.IngredientSummaryPeriod();

                summariedItem.date = runner.Date;
                summariedItem.ingredientSummary = item;

                result.ingredientSummaryPeriod.Add(summariedItem);

                runner = runner.AddDays(1);
            }
            return null;
        }
    
    }
}
