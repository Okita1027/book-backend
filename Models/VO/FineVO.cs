namespace DEMO_CRUD.Models.VO
{
    public class FineVO
    {
        public int LoadId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
