using System.Security.Cryptography;
using System.Text;
using Npgsql;

namespace Cash.DB
{
    public class DatabaseManager
    {
        private readonly string _connectionString = "Host=localhost;Database=ATM;Username=postgres;Password=123";

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public User GetUserByLogin(string login)
        {
            User user = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE login = @login;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                                Login = reader.GetString(reader.GetOrdinal("login")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString(reader.GetOrdinal("full_name")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                IsBlocked = reader.GetBoolean(reader.GetOrdinal("is_blocked")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                            };
                        }
                    }
                }
            }
            return user;
        }

        public bool CreateUser(string login, string password, string fullName, string role = "client")
        {
            string passwordHash = HashPassword(password);
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO users (login, password_hash, full_name, role)
                    VALUES (@login, @password_hash, @full_name, @role)
                    ON CONFLICT (login) DO NOTHING;
                ";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password_hash", passwordHash);
                    cmd.Parameters.AddWithValue("@full_name", fullName);
                    cmd.Parameters.AddWithValue("@role", role);
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (PostgresException ex) when (ex.SqlState == "23505")
                    {
                        Console.WriteLine("Ошибка: Пользователь с таким логином уже существует.");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка создания пользователя: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public List<Account> GetAccountsByUserId(int userId)
        {
            var accounts = new List<Account>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM accounts WHERE user_id = @user_id ORDER BY created_at;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                AccountId = reader.GetInt32(reader.GetOrdinal("account_id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                                AccountNumber = reader.GetString(reader.GetOrdinal("account_number")),
                                Balance = reader.GetDecimal(reader.GetOrdinal("balance")),
                                Currency = reader.GetString(reader.GetOrdinal("currency")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        public bool UpdateAccountBalance(int accountId, decimal newBalance)
        {
            if (newBalance < 0)
            {
                Console.WriteLine("Ошибка: Баланс не может быть отрицательным.");
                return false;
            }
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE accounts SET balance = @balance WHERE account_id = @account_id;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@balance", newBalance);
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка обновления баланса: {ex.Message}");
                        return false;
                    }
                }
            }
        }
        public int CreateTransaction(int accountId, decimal amount, string transactionType, string description = "")
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO transactions (account_id, amount, transaction_type, description)
                    VALUES (@account_id, @amount, @transaction_type, @description);";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@transaction_type", transactionType);
                    cmd.Parameters.AddWithValue("@description", (object)description ?? DBNull.Value);
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0 ? 1 : 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка создания транзакции: {ex.Message}");
                        return 0;
                    }
                }
            }
        }

        internal void CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        internal void CreateAccount(int userId, string currency)
        {
            throw new NotImplementedException();
        }

        internal Account GetAccountByNumber(string accountNumber)
        {
            throw new NotImplementedException();
        }

        internal void CreateCard(int accountId, string expiry, string cvv, string pinHash)
        {
            throw new NotImplementedException();
        }

        internal Card GetCardByAccountId(int accountId)
        {
            throw new NotImplementedException();
        }

        internal void SetCardBlockStatus(int cardId, bool v)
        {
            throw new NotImplementedException();
        }

        internal List<CurrencyRate> GetAllCurrencyRates()
        {
            throw new NotImplementedException();
        }

        internal CurrencyRate GetCurrencyRate(string from, string to)
        {
            throw new NotImplementedException();
        }

        internal void UpsertCurrencyRate(string from, string to, decimal rate)
        {
            throw new NotImplementedException();
        }

        internal bool Deposit(int accountId, decimal amount, string description)
        {
            throw new NotImplementedException();
        }

        internal bool Withdraw(int accountId, decimal amount, string description)
        {
            throw new NotImplementedException();
        }

        internal List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            throw new NotImplementedException();
        }

        internal bool TransferFunds(int fromAccountId, int toAccountId, decimal amount)
        {
            throw new NotImplementedException();
        }

        internal List<Transfer> GetTransfersByAccountId(int accountId)
        {
            throw new NotImplementedException();
        }

        internal void RecordLoginAttempt(string login, bool v)
        {
            throw new NotImplementedException();
        }

        internal void SetUserBlockStatus(int userId, bool v)
        {
            throw new NotImplementedException();
        }

        internal List<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}