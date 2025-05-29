namespace Cash.DB
{
    public class LoginHistoryEntry
    {
        public int LoginId { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsSuccess { get; set; }

    }
}