using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhungNoiProject.Models
{
    public class IngredientSummaryPeriodModel
    {
        public List<IngredientSummaryPeriod> ingredientSummaryPeriod { get; set; }

        public IngredientSummaryPeriodModel()
        {
            ingredientSummaryPeriod = new List<IngredientSummaryPeriod>();
        }

        public class IngredientSummaryPeriod
        {
            public DateTime date { get; set; }
            public IngredientSummaryModel ingredientSummary { get; set; }
        }
    }
}