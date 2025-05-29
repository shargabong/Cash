using System.ComponentModel.DataAnnotations.Schema;

namespace Cash.DB
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        [Column(TypeName = "decimal(15, 2)")]
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}