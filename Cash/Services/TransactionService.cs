using Cash.DB;

namespace Services
{
    public class TransactionService
    {
        private readonly DatabaseManager _db;

        public TransactionService(DatabaseManager db)
        {
            _db = db;
        }

        public bool Deposit(int accountId, decimal amount, string description)
        {
            return _db.Deposit(accountId, amount, description);
        }

        public bool Withdraw(int accountId, decimal amount, string description)
        {
            return _db.Withdraw(accountId, amount, description);
        }

        public List<Transaction> GetTransactions(int accountId) => _db.GetTransactionsByAccountId(accountId);
    }
}