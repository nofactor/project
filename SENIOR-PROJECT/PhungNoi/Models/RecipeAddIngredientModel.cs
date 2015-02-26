using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhungNoiProject.Models
{
    public class RecipeAddIngredientModel
    {
        public int productId;
        public List<Ingredient> ingredient { get; set; }

        public RecipeAddIngredientModel()
        {
            ingredient = new List<Ingredient>();
        }

    }
}
