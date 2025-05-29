using Cash.DB;

namespace Services
{
    public class CurrencyService
    {
        private readonly DatabaseManager _db;

        public CurrencyService(DatabaseManager db)
        {
            _db = db;
        }

        public List<CurrencyRate> GetAllRates() => _db.GetAllCurrencyRates();

        public CurrencyRate GetRate(string from, string to) => _db.GetCurrencyRate(from, to);

        public void UpdateRate(string from, string to, decimal rate)
        {
            _db.UpsertCurrencyRate(from, to, rate);
        }
    }
}