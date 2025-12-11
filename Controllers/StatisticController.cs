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
            var rawStats = _budgetRepository.GetMonthlyStats(GetUserId());

            var stats = rawStats.Select(s => {
                // Convert "yyyy-MM" to "MonthName Year"
                string monthName = s.MonthStr; 
                if (DateTime.TryParseExact(s.MonthStr, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    monthName = dt.ToString("MMMM yyyy");
                }
                
                return new MonthlyStatisticViewModel
                {
                    MonthYear = monthName,
                    TotalIncome = s.TotalIncome,
                    TotalExpense = s.TotalExpense
                };
            }).ToList();

            return View(stats);
        }
    }
}
