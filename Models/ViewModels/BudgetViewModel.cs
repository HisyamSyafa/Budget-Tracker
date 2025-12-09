using System;

namespace BudgetTracker.Models.ViewModels
{
    public class BudgetViewModel
    {
        public List<Transaction>? Transactions { get; set; }
        public List<Category>? Categories { get; set; }
    }
}

