using System;
using System.Data;
using BudgetTracker.Models;
using MySql.Data.MySqlClient;
using Dapper;

namespace BudgetTracker.Repositories
{
	public interface IBudgetRepository
	{
		List<Transaction> GetTransactions();
		List<Category> GetCategories();
		void AddTransaction(string name, string date, decimal amount, int transactionType, int categoryId, int userId);
		void DeleteTransaction(int id, int userId);
		void UpdateTransaction(int id, string name, string date, decimal amount, int transactionType, int categoryId, int userId);
		void RegisterUser(User user);
		User GetUserByUsername(string username);
		// Update GetTransactions to filter by userId
		List<Transaction> GetTransactions(int userId);
		List<MonthlyStat> GetMonthlyStats(int userId);
	}

	public class BudgetRepository: IBudgetRepository
	{
		private readonly IConfiguration _configuration;

		public BudgetRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public List<Transaction> GetTransactions(int userId)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query =
					@"SELECT t.Amount, t.CategoryId, t.`Date`, t.Id, t.TransactionType, t.Name, c.Name AS Category
                      FROM Transactions AS t
                      LEFT JOIN Category AS c
                      ON t.CategoryId = c.Id
                      WHERE t.UserId = @UserId;";

				var allTransactions = connection.Query<Transaction>(query, new { UserId = userId });

				return allTransactions.ToList();
			}
		}

		public List<MonthlyStat> GetMonthlyStats(int userId)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				// Assuming Date is stored as 'yyyy-MM-dd' string or similar. 
				// aggregating by the first 7 chars (yyyy-MM).
				var query = @"
                    SELECT 
                        SUBSTRING(`Date`, 1, 7) AS MonthStr, 
                        SUM(CASE WHEN TransactionType = 1 THEN Amount ELSE 0 END) AS TotalIncome,
                        SUM(CASE WHEN TransactionType = 0 THEN Amount ELSE 0 END) AS TotalExpense
                    FROM Transactions 
                    WHERE UserId = @UserId
                    GROUP BY SUBSTRING(`Date`, 1, 7)
                    ORDER BY MonthStr DESC";

				return connection.Query<MonthlyStat>(query, new { UserId = userId }).ToList();
			}
		}

        // Keep existing method for backward compatibility if needed, or remove it. 
        // Since interface changed, we can remove it or implement it to return empty or all (admin).
        // For now, I'll remove the parameterless one as interface requires the one with userId.
        public List<Transaction> GetTransactions() => throw new NotImplementedException();

		public void DeleteTransaction(int id, int userId)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query = "DELETE FROM Transactions WHERE Id = @Id AND UserId = @UserId";
				connection.Execute(query, new { Id = id, UserId = userId });
			}
		}

		public void UpdateTransaction(int id, string name, string date, decimal amount, int transactionType, int categoryId, int userId)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query = @"UPDATE Transactions 
							  SET Name = @Name, 
								  `Date` = @Date, 
								  Amount = @Amount, 
								  TransactionType = @TransactionType, 
								  CategoryId = @CategoryId 
							  WHERE Id = @Id AND UserId = @UserId";
				connection.Execute(query, new { Id = id, Name = name, Date = date, Amount = amount, TransactionType = transactionType, CategoryId = categoryId, UserId = userId });
			}
		}

		public List<Category> GetCategories()
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query = "SELECT Id, Name FROM Category ORDER BY Name";
				var categories = connection.Query<Category>(query);
				return categories.ToList();
			}
		}

		public void AddTransaction(string name, string date, decimal amount, int transactionType, int categoryId, int userId)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query = @"INSERT INTO Transactions (Name, `Date`, Amount, TransactionType, CategoryId, UserId) 
							  VALUES (@Name, @Date, @Amount, @TransactionType, @CategoryId, @UserId)";
				connection.Execute(query, new { Name = name, Date = date, Amount = amount, TransactionType = transactionType, CategoryId = categoryId, UserId = userId });
			}
		}

		public void RegisterUser(User user)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
				connection.Execute(query, user);
			}
		}

		public User GetUserByUsername(string username)
		{
			using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				var query = "SELECT * FROM Users WHERE Username = @Username";
				return connection.QuerySingleOrDefault<User>(query, new { Username = username });
			}
		}
	}
}

