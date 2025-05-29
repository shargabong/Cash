using Cash.DB;

namespace Services
{
    public class CardService
    {
        private readonly DatabaseManager _db;

        public CardService(DatabaseManager db)
        {
            _db = db;
        }

        public void CreateCard(int accountId, string expiry, string cvv, string pin)
        {
            string pinHash = Utils.Hash(pin);
            _db.CreateCard(accountId, expiry, cvv, pinHash);
        }

        public Card GetCardByAccountId(int accountId) => _db.GetCardByAccountId(accountId);

        public void BlockCard(int cardId) => _db.SetCardBlockStatus(cardId, true);

        public void UnblockCard(int cardId) => _db.SetCardBlockStatus(cardId, false);
    }
}