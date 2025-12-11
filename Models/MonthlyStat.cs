namespace BudgetTracker.Models
{
    public class MonthlyStat
    {
        public string MonthStr { get; set; } // Format: "yyyy-MM" or similar from DB
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
