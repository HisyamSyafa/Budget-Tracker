using System;

namespace BudgetTracker.Models.ViewModels
{
    public class MonthlyStatisticViewModel
    {
        public string MonthYear { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
