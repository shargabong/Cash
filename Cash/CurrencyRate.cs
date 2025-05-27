using System.ComponentModel.DataAnnotations.Schema;

namespace BankingAppCSharp.Models
{
    public class CurrencyRate
    {
        public int RateId { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal Rate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}