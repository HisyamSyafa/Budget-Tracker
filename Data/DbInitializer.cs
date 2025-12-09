using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace BudgetTracker.Data
{
    public class DbInitializer
    {
        private readonly IConfiguration _configuration;

        public DbInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize()
        {
            using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Create Users table if not exists
                var createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Username VARCHAR(255) NOT NULL UNIQUE,
                        PasswordHash VARCHAR(255) NOT NULL
                    );";
                connection.Execute(createUsersTable);

                // Add UserId to Transactions if not exists
                // Note: Checking column existence in MySQL
                var checkColumn = "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'UserId'";
                var columnExists = connection.ExecuteScalar<int>(checkColumn);

                if (columnExists == 0)
                {
                    var addColumn = "ALTER TABLE Transactions ADD COLUMN UserId INT DEFAULT NULL;";
                    connection.Execute(addColumn);
                    
                    // Optional: Add FK constraint
                    // var addFk = "ALTER TABLE Transactions ADD CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(Id);";
                    // connection.Execute(addFk);
                }
            }
        }
    }
}
