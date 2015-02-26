using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhungNoiProject.Models
{
    public class IngredientSummaryDetail
    {
        public Ingredient ingredient;
        public float amount;

        public IngredientSummaryDetail()
        {
            amount = 0;
        }
    }
}