using Cash.DB;

namespace Services
{
    public class TransferService
    {
        private readonly DatabaseManager _db;

        public TransferService(DatabaseManager db)
        {
            _db = db;
        }

        public bool Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            return _db.TransferFunds(fromAccountId, toAccountId, amount);
        }

        public List<Transfer> GetTransfers(int accountId) => _db.GetTransfersByAccountId(accountId);
    }
}