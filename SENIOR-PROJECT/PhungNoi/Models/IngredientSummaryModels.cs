using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhungNoiProject.Models
{
    public class IngredientSummaryModel
    {
        public List<IngredientSummaryDetail> ingredientDetails { get; set; }

        public IngredientSummaryModel()
        {
            ingredientDetails = new List<IngredientSummaryDetail>();
        }

    }
}