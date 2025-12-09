using System.Diagnostics;
using System.Security.Claims;
using BudgetTracker.Models.ViewModels;
using BudgetTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IBudgetRepository _budgetRepository;

    public HomeController(IBudgetRepository budgetRepository)
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
        var transactions = _budgetRepository.GetTransactions(GetUserId())
            .OrderByDescending(t => t.Date)
            .ToList();
        var categories = _budgetRepository.GetCategories();

        var viewModel = new BudgetViewModel
        {
            Transactions = transactions,
            Categories = categories
        };

        return View(viewModel);
    }

    public IActionResult Income()
    {
        var transactions = _budgetRepository.GetTransactions(GetUserId())
            .Where(t => t.TransactionType == Models.TransactionType.Income)
            .OrderByDescending(t => t.Date)
            .ToList();
        var categories = _budgetRepository.GetCategories();

        var viewModel = new BudgetViewModel
        {
            Transactions = transactions,
            Categories = categories
        };

        return View("Index", viewModel);
    }

    public IActionResult Expense()
    {
        var transactions = _budgetRepository.GetTransactions(GetUserId())
            .Where(t => t.TransactionType == Models.TransactionType.Expense)
            .OrderByDescending(t => t.Date)
            .ToList();
        var categories = _budgetRepository.GetCategories();

        var viewModel = new BudgetViewModel
        {
            Transactions = transactions,
            Categories = categories
        };

        return View("Index", viewModel);
    }

    [HttpPost]
    public IActionResult AddTransaction(string name, string date, decimal amount, int transactionType, int categoryId)
    {
        try
        {
            _budgetRepository.AddTransaction(name, date, amount, transactionType, categoryId, GetUserId());
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult DeleteTransaction(int id)
    {
        try
        {
            _budgetRepository.DeleteTransaction(id, GetUserId());
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult UpdateTransaction(int id, string name, string date, decimal amount, int transactionType, int categoryId)
    {
        try
        {
            _budgetRepository.UpdateTransaction(id, name, date, amount, transactionType, categoryId, GetUserId());
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}

