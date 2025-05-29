namespace Cash.DB
{
    public class Card
    {
        public int CardId { get; set; }
        public int AccountId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string Cvv { get; set; }
        public bool IsBlocked { get; set; }
        public string PinHash { get; set; }

    }
}