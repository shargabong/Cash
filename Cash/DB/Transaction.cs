using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cash.DB
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        [Column(TypeName = "decimal(15, 2)")]
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}