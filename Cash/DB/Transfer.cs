using System.ComponentModel.DataAnnotations.Schema;

namespace Cash.DB
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal Commission { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }

    }
}