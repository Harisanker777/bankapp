using Microsoft.Data.Sqlite;
using System.IO;

namespace bankapp
{
    public class SQLiteService
    {
        private readonly string _dbPath;

        public SQLiteService()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Combine(folderPath, "app.db");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                string createUserTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Password TEXT NOT NULL,
                        Balance REAL NOT NULL
                    );";

                using (var command = new SqliteCommand(createUserTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                AddSampleUser();
            }
        }

        private void AddSampleUser()
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                string insertUser = @"
                    INSERT OR IGNORE INTO Users (Username, Password, Balance) 
                    VALUES ('user', 'password', 1000.00);";

                using (var command = new SqliteCommand(insertUser, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                string queryUser = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Password = @password;";

                using (var command = new SqliteCommand(queryUser, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public (string Username, decimal Balance) GetUser()
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                string queryUser = "SELECT Username, Balance FROM Users LIMIT 1;";

                using (var command = new SqliteCommand(queryUser, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (reader.GetString(0), reader.GetDecimal(1));
                    }
                }
            }
            return ("Unknown", 0);
        }

        public void UpdateUserName(string newUsername)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                string updateUsername = "UPDATE Users SET Username = @username;";

                using (var command = new SqliteCommand(updateUsername, connection))
                {
                    command.Parameters.AddWithValue("@username", newUsername);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateBalance(decimal newBalance)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                string updateBalance = "UPDATE Users SET Balance = @balance;";

                using (var command = new SqliteCommand(updateBalance, connection))
                {
                    command.Parameters.AddWithValue("@balance", newBalance);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
