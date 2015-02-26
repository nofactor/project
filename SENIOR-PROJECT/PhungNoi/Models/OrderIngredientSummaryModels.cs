using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhungNoiProject.Models
{
    public class OrderIngredientSummaryModel
    {
        public Order order { get; set; }
        public List<IngredientSummaryDetail> ingredientDetails { get; set; }

        public OrderIngredientSummaryModel()
        {
            ingredientDetails = new List<IngredientSummaryDetail>();
        }

    }
}