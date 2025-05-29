using Cash.DB;

namespace Services
{
    public class AccountService
    {
        private readonly DatabaseManager _db;

        public AccountService(DatabaseManager db)
        {
            _db = db;
        }

        public void CreateAccount(int userId, string currency)
        {
            _db.CreateAccount(userId, currency);
        }

        public List<Account> GetUserAccounts(int userId) => _db.GetAccountsByUserId(userId);

        public Account GetAccountByNumber(string accountNumber) => _db.GetAccountByNumber(accountNumber);
    }
}