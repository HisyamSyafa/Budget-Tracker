using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Repositories;
using System.Security.Claims;
using BudgetTracker.Models;
using BudgetTracker.Models.ViewModels;
using System.Globalization;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class StatisticController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;

        public StatisticController(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        private int GetUserId()
        {
             var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (string.IsNullOrEmpty(userIdClaim)) return 0;
             return int.Parse(userIdClaim);
        }

        public IActionResult Index()
        {
            var transactions = _budgetRepository.GetTransactions(GetUserId());

            // Process transactions to calculate monthly stats
            var stats = transactions
                .Select(t =>
                {
                    // Parse Date
                    DateTime date;
                    if (!DateTime.TryParse(t.Date, out date))
                    {
                        // Fallback or handle invalid date if necessary, 
                        // for now default to MinValue or Today so it doesn't crash
                        date = DateTime.MinValue; 
                    }

                    // Parse Amount
                    decimal amount;
                    if (!decimal.TryParse(t.Amount, out amount))
                    {
                        amount = 0;
                    }

                    return new { t.TransactionType, Date = date, Amount = amount };
                })
                .Where(x => x.Date != DateTime.MinValue) // Exclude invalid dates
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new MonthlyStatisticViewModel
                {
                    MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    TotalIncome = g.Where(x => x.TransactionType == TransactionType.Income).Sum(x => x.Amount),
                    TotalExpense = g.Where(x => x.TransactionType == TransactionType.Expense).Sum(x => x.Amount)
                })
                .OrderByDescending(x => DateTime.ParseExact(x.MonthYear, "MMMM yyyy", CultureInfo.InvariantCulture))
                .ToList();

            return View(stats);
        }
    }
}
